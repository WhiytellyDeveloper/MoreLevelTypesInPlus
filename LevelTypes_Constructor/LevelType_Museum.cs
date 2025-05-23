using BaldiLevelEditor;
using EditorCustomRooms;
using HarmonyLib;
using MoreLevelTypesInPlus.Functions;
using MoreLevelTypesInPlus.LevelTypes_SpecialManagers;
using MoreLevelTypesInPlus.Objects;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.Registers;
using PixelInternalAPI.Extensions;
using PlusLevelFormat;
using PlusLevelLoader;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MoreLevelTypesInPlus.LevelTypes_Constructor
{
    public class LevelType_Museum : LevelType_Base
    {
        public override LevelType_Base PreloadAssets()
        {
            base.PreloadAssets();

            wall = AssetLoader.TextureFromMod(Plugin.__instance, ["Museum", "MuseumWall.png"]);
            floor = AssetLoader.TextureFromMod(Plugin.__instance, ["Museum", "MuseumCarpet.png"]);
            ceiling = AssetLoader.TextureFromMod(Plugin.__instance, ["Museum", "MuseumCeiling.png"]);
            fakeNotebookLarge = AssetLoader.SpriteFromMod(Plugin.__instance, Vector2.one / 2, 50f, ["Museum", "NotebookArtifact_IconLarge.png"]);
            fakeNotebookSmall = AssetLoader.SpriteFromMod(Plugin.__instance, Vector2.one / 2, 1f, ["Museum", "NotebookArtifact_IconSmall.png"]);
            pedestalSprite = AssetLoader.SpriteFromMod(Plugin.__instance, new(1, 0), 30, ["Museum", "MuseumPedestal.png"]);
            Texture2D tex = pedestalSprite.texture;
            Rect rect = pedestalSprite.rect;
            Vector2 newPivot = new Vector2(64f / rect.width, 0f); // normalizado (0 a 1)

            pedestalSprite = Sprite.Create(tex, rect, newPivot, pedestalSprite.pixelsPerUnit);


            var door = Resources.FindObjectsOfTypeAll<StandardDoorMats>().LastOrDefault();
            PlusLevelLoaderPlugin.Instance.roomSettings.Add("MuseumPillarRoom", new(RoomCategory.Special, RoomType.Room, new(219f/255f, 81 /255f, 99f/255f), door));

            BaldiLevelEditorPlugin.editorObjects.Add(EditorObjectType.CreateFromGameObject<EditorPrefab, PrefabLocation>("MuseumPillar", pillar, Vector3.zero, false));
            PlusLevelLoaderPlugin.Instance.prefabAliases.Add("MuseumPillar", pillar);

            instance = this;
            levelManager = typeof(LevelTypeManager_Museum);

            return this;
        }

        public override CustomLevelObject OverrideFloor(CustomLevelObject original, int floorNum)
        {
            original.minPlots += 4;
            original.maxPlots += 4;

            original.hallWallTexs = [new WeightedTexture2D { selection = wall, weight = 100 }];
            original.hallFloorTexs = [new WeightedTexture2D { selection = floor, weight = 100 }];
            original.hallCeilingTexs = [new WeightedTexture2D { selection = ceiling, weight = 100 }];

            RoomGroup group = new RoomGroup
            {
                name = "PillarRooms",
                minRooms = 8,
                maxRooms = 8,
                potentialRooms = [new WeightedRoomAsset { selection = pillarRoom, weight = 100 }],
                stickToHallChance = 1,
                wallTexture = [original.hallWallTexs[0]],
                floorTexture = [original.hallWallTexs[0]],
                ceilingTexture = [original.hallWallTexs[0]],
                light = [original.hallLights[0]]
            };

            original.roomGroup = original.roomGroup.AddRangeToArray([group]);
            return original;
        }

        public ItemObject fakeNotebook => GetOrCreate("FakeNotebok", () =>
        {
            ItemObject item = new ItemBuilder(Plugin.__instance.Info)
            .SetNameAndDescription("Notebook artifact", "Notebook artifact\nPlace it on one of the museum's pillars, press a button and see the magic!")
            .SetEnum("NotebookArtifact")
            .SetSprites(fakeNotebookSmall, fakeNotebookLarge)
            .SetShopPrice(0)
            .SetGeneratorCost(0)
            .SetMeta(ItemFlags.NoUses, ["Useless"])
            .SetAsNotOverridable()
            .SetItemComponent<Item>()
            .Build();

            return item;
        });

        public RoomAsset pillarRoom => GetOrCreate("PillarRoom", () =>
        {
            var room = RoomFactory.CreateAssetsFromPath(Path.Combine([AssetLoader.GetModPath(Plugin.__instance), "Museum", "PlaceholderRoom.cbld"]), 100, false)[0];
            var function = new GameObject("PillarRoomFunctionContainer").AddComponent<RoomFunctionContainer>();
            function.functions = [];
            function.AddFunction(function.gameObject.AddComponent<FullOpenAreaFunction>());
            function.AddFunction(function.gameObject.AddComponent<CoverRoomFunction>());
            function.gameObject.GetComponent<CoverRoomFunction>().hardCover = true;
            function.gameObject.GetComponent<CoverRoomFunction>().coverage = CellCoverage.Center | CellCoverage.North | CellCoverage.East | CellCoverage.South | CellCoverage.West | CellCoverage.Up | CellCoverage.Down;
            function.gameObject.ConvertToPrefab(true);
            room.activity = new ActivityData { prefab = Resources.FindObjectsOfTypeAll<NoActivity>().FirstOrDefault() };
            room.hasActivity = true;
            room.roomFunctionContainer = function;
            room.potentialDoorPositions.Clear();
            room.requiredDoorPositions.Clear();
            room.forcedDoorPositions.Clear();

            return room;
        });

        public GameObject pillar => GetOrCreate("Pillar", () =>
        {
            var origin = Resources.FindObjectsOfTypeAll<Transform>().FirstOrDefault(x => x.name == "Decor_Pedestal").gameObject.DuplicatePrefab();
            origin.GetComponent<SpriteRenderer>().sprite = pedestalSprite;;
            origin.AddComponent<MuseumPedestal>();
            origin.ConvertToPrefab(true);

            return origin;
        });

        public override LevelType type => ExtendLevelType.Museum.GetLevelType();
        public override int weight => 999999;

        public Texture2D wall, floor, ceiling;
        
        public Sprite fakeNotebookLarge, fakeNotebookSmall;

        public Sprite pedestalSprite;

        public static LevelType_Museum instance;
    }
}
