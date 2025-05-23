using HarmonyLib;
using MoreLevelTypesInPlus.LevelTypes_SpecialManagers;
using MoreLevelTypesInPlus.ModItems;
using MoreLevelTypesInPlus.Structures;
using MoreLevelTypesInPlus.Structures.Objects;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.Registers;
using PixelInternalAPI.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace MoreLevelTypesInPlus.LevelTypes_Constructor
{
    public class LevelType_Castle : LevelType_Base
    {
        public override LevelType_Base PreloadAssets()
        {
            base.PreloadAssets();

            wallTex = AssetLoader.TextureFromMod(Plugin.__instance, Path.Combine(["Castle", "CastleWall.png"]));
            florTex = AssetLoader.TextureFromMod(Plugin.__instance, Path.Combine(["Castle", "CastleFloor.png"]));
            ceilTex = AssetLoader.TextureFromMod(Plugin.__instance, Path.Combine(["Castle", "CastleCeiling.png"]));
            keyLarge = AssetLoader.SpriteFromMod(Plugin.__instance, Vector2.one / 2, 50, [Path.Combine(["Castle", "CastleKey_IconLarge.png"])]);
            keySmall = AssetLoader.SpriteFromMod(Plugin.__instance, Vector2.one / 2, 1, [Path.Combine(["Castle", "CastleKey_IconSmall.png"])]);
            octopusLockTex = AssetLoader.TextureFromMod(Plugin.__instance, Path.Combine(["Castle", "CastleLocker.png"]));
            originalLightBase = SceneObjectMetaStorage.Instance.Find(x => x.title == "F4").value.randomizedLevelObject.FirstOrDefault(x => x.selection.type == LevelType.Laboratory).selection.hallLights[0].selection.gameObject;
            lightSprite = AssetLoader.SpriteFromMod(Plugin.__instance, Vector2.one / 2, 28, [Path.Combine(["Castle", "NewCastleLamp.png"])]);
            spikeSprite = AssetLoader.SpriteFromMod(Plugin.__instance, Vector2.one / 2, 16, [Path.Combine(["Castle", "spikesprite.png"])]);
            spikeHoleSprite = AssetLoader.SpriteFromMod(Plugin.__instance, Vector2.one / 2, 16, [Path.Combine(["Castle", "SpikeHole.png"])]);
            spikesUp = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromMod(Plugin.__instance, Path.Combine(["Castle", "SpikesUp.wav"])), "*ShiwsH*", SoundType.Effect, Color.white);
            spikesDown = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromMod(Plugin.__instance, Path.Combine(["Castle", "SpikesDown.wav"])), "*Shiw*", SoundType.Effect, Color.white);
            spikeUpMapIcon = AssetLoader.SpriteFromMod(Plugin.__instance, Vector2.one / 2, 16, [Path.Combine(["Castle", "Icon_Spike_Up.png"])]);
            spikeDownMapIcon = AssetLoader.SpriteFromMod(Plugin.__instance, Vector2.one / 2, 16, [Path.Combine(["Castle", "Icon_Spike_Down.png"])]);
            bridgeWall = AssetLoader.TextureFromMod(Plugin.__instance, Path.Combine(["Castle", "CastleBridge_Wall.png"]));
            bridgeFlor = AssetLoader.TextureFromMod(Plugin.__instance, Path.Combine(["Castle", "CastleBridge_Floor.png"]));
            bridgeCeil = AssetLoader.TextureFromMod(Plugin.__instance, Path.Combine(["Castle", "CastleBridge_Ceiling.png"]));
            bridgeHangWall = AssetLoader.TextureFromMod(Plugin.__instance, Path.Combine(["Castle", "CastleBridge_HangingWall.png"]));
            bridgeSkybox = AssetLoader.TextureFromMod(Plugin.__instance, Path.Combine(["Castle", "CastleBridge_Skybox.png"]));
            leverUp = AssetLoader.TextureFromMod(Plugin.__instance, Path.Combine(["Castle", "MedivalLever_Up_base.png"]));
            leverMiddle = AssetLoader.TextureFromMod(Plugin.__instance, Path.Combine(["Castle", "MedivalLever_Mid_base.png"]));
            leverDown = AssetLoader.TextureFromMod(Plugin.__instance, Path.Combine(["Castle", "MedivalLever_Down_base.png"]));
            leverUpLight = AssetLoader.TextureFromMod(Plugin.__instance, Path.Combine(["Castle", "MedivalLever_Up_Light.png"]));
            leverMiddleLight = AssetLoader.TextureFromMod(Plugin.__instance, Path.Combine(["Castle", "MedivalLever_Mid_Light.png"]));
            leverDownLight = AssetLoader.TextureFromMod(Plugin.__instance, Path.Combine(["Castle", "MedivalLever_Down_Light.png"]));
            bridgeDoorClosed = AssetLoader.TextureFromMod(Plugin.__instance, Path.Combine(["Castle", "CastleBridgeDoorClosed.png"]));
            bridgeDoorOpen = AssetLoader.TextureFromMod(Plugin.__instance, Path.Combine(["Castle", "CastleBridgeDoorOpen.png"]));
            ceilingGatebottom = AssetLoader.SpriteFromMod(Plugin.__instance, Vector2.one / 2, 24, Path.Combine(["Castle", "CelingCage_Bottom.png"]));
            gateTexture = AssetLoader.TextureFromMod(Plugin.__instance, Path.Combine("Castle", "CeilingGateTexture.png"));
            gateModelPrefab = AssetLoader.ModelFromMod(Plugin.__instance, Path.Combine("Castle", "CeilingCage.obj"));
            levelManager = typeof(LevelTypeManager_Castle);
            instance = this;

            return instance;
        }

        public override CustomLevelObject OverrideFloor(CustomLevelObject original, int floorNum)
        {
            original.type = type;

            original.minPlots += 4;
            original.maxPlots += 5;

            original.hallWallTexs = [new WeightedTexture2D { selection = wallTex }];
            original.hallFloorTexs = [new WeightedTexture2D { selection = florTex }];
            original.hallCeilingTexs = [new WeightedTexture2D { selection = ceilTex }];

            original.standardLightStrength = 4 + 2 * floorNum;
            original.maxLightDistance = 6 + 4 * floorNum;
            original.standardLightColor = new(255f/255f, 198f/266f, 175f/255f);
            original.hallLights = [new WeightedTransform { selection = lampLight.transform, weight = 100 }  ];

            original.skybox = Resources.FindObjectsOfTypeAll<Cubemap>().FirstOrDefault(x => x.name == "Cubemap_Twilight");

            original.maxItemValue += 275;

            StructureParameters spikesParamters = new StructureParameters { minMax = [new(6 + (floorNum) * 2, (5 + (floorNum) * 2) + 2)] };

            original.forcedStructures = original.forcedStructures.AddRangeToArray([new StructureWithParameters { prefab = structure_castleBridge, parameters = new StructureParameters { minMax = [new(5, 5)] } }]);
            original.forcedStructures = original.forcedStructures.AddRangeToArray([new StructureWithParameters { prefab = structure_lockedrooms, parameters = new() }]);
            original.forcedStructures = original.forcedStructures.AddRangeToArray([new StructureWithParameters { prefab = structure_spikes, parameters = spikesParamters }]);
            original.forcedStructures = original.forcedStructures.AddRangeToArray([new StructureWithParameters { prefab = structure_ceilingGate, parameters = spikesParamters }]);

            foreach (RoomGroup group in original.roomGroup)
            {
                if (group.name == "Faculty")
                {
                    group.minRooms += 7;
                    group.maxRooms += 9;
                }

                group.light = original.hallLights;
            }

            return original;

        }

        public ItemObject classKey => GetOrCreate("ClassKey", () =>
        {
            var itemScriptBase = ItemMetaStorage.Instance.FindByEnum(Items.SquareKey).value.item as ITM_Acceptable;

            ItemObject item = new ItemBuilder(Plugin.__instance.Info)
                .SetNameAndDescription("Flowery Octopus Key (#$%#)", "Flowery Octopus Key\nThey unlock locks in numerous places, from classrooms to even the mighty king's room!\nBesides looking like an octopus or a flower")
                .SetEnum("ClassKey")
                .SetSprites(keySmall, keyLarge)
                .SetShopPrice(0)
                .SetGeneratorCost(25)
                .SetItemComponent<ITM_FloweryOctopusKey>()
                .SetAsNotOverridable()
                .Build();

            ITM_FloweryOctopusKey itemScript = item.item as ITM_FloweryOctopusKey;
            itemScript.layerMask = itemScriptBase.layerMask;
            itemScript.audUse = itemScriptBase.audUse;

            return item;
        });

        public ItemObject classKeyPickup => GetOrCreate("ClassKeyPickup", () =>
        {
            ItemObject item = new ItemBuilder(Plugin.__instance.Info)
                .SetNameAndDescription("Flowery Octopus Key Pickup", "Flowery Octopus Key Pickup\nYou shouldn't be reading this lol")
                .SetEnum("ClassKeyPickup")
                .SetSprites(keySmall, keyLarge)
                .SetShopPrice(0)
                .SetGeneratorCost(25)
                .SetItemComponent<ITM_FloweryOctopusKeyPickup>()
                .SetAsInstantUse()
                .SetMeta(ItemFlags.InstantUse, [])
                .Build();

            return item;
        });

        public Structure_ClassroomsLockers structure_lockedrooms => GetOrCreate("StructureLockedRooms", () =>
        {
            Structure_ClassroomsLockers structure = new GameObject("Structure_Classrooms").AddComponent<Structure_ClassroomsLockers>();
            structure.classKeyPickup = classKeyPickup;
            structure.classKey = classKey;
            structure.gameLock = floweryOctopusLock;
            structure.roomCategories = [RoomCategory.Class, RoomCategory.Special];
            structure.gameObject.ConvertToPrefab(true);
            return structure;
        });


        public GameLock floweryOctopusLock => GetOrCreate("FloweryOctopusLock", () =>
        {
            var flLock = GameObject.Instantiate(Resources.FindObjectsOfTypeAll<GameLock>().LastOrDefault());
            flLock.GetComponentInChildren<MeshRenderer>().material.SetMainTexture(octopusLockTex);
            flLock.gameObject.ConvertToPrefab(true);
            return flLock;
        });

        public GameObject lampLight => GetOrCreate("LampLight", () =>
        {
            GameObject light = GameObject.Instantiate(originalLightBase);
            var sprite = light.GetComponentInChildren<SpriteRenderer>();
            sprite.sprite = lightSprite;
            sprite.transform.localPosition = new(0, 7.5f, 0);
            light.ConvertToPrefab(true);
            return light;
        });

        public Structure_Spikes structure_spikes => GetOrCreate("StructureSpikes", () =>
        {
            Structure_Spikes structure = new GameObject("Structure_Spikes").AddComponent<Structure_Spikes>();
            structure.spikes = spikes;
            structure.gameObject.ConvertToPrefab(true);
            return structure;
        });

        public Spikes spikes => GetOrCreate("Spikes", () =>
        {
            Spikes structure = new GameObject("Spikes").AddComponent<Spikes>();
            float[] xValues = { 3f, 1f, -1f, -3f };

            foreach (float x in xValues)
            {
                var spike = ObjectCreationExtensions.CreateSpriteBillboard(spikeSprite, false)
                    .AddSpriteHolder(out SpriteRenderer renderer, -0.8f);
                spike.transform.localPosition = new Vector3(x, -0.8f + 5f, 0);
                spike.transform.SetParent(structure.transform);

                var spikeHole = ObjectCreationExtensions.CreateSpriteBillboard(spikeHoleSprite, false)
                    .AddSpriteHolder(out SpriteRenderer holeRenderer, -4.999f);
                spikeHole.transform.localPosition = new Vector3(x, -4.999f + 5f, 5);
                spikeHole.transform.localRotation = Quaternion.Euler(90, 0, 0);
                spikeHole.transform.SetParent(structure.transform);

                structure._spikeTransforms.Add(spike.transform);
            }

            var collider = structure.gameObject.AddComponent<BoxCollider>();
            collider.size = new(10f, 10f, 1f);
            collider.isTrigger = false;
            collider.center = new(0f, 5f, 0f);
            structure._collider = collider;

            structure.audMan = structure.gameObject.CreatePropagatedAudioManager(30, 60);
            structure.audMan.pitchTimeScaleType = TimeScaleType.Environment;
            structure.spikesUp = spikesUp;
            structure.spikesDown = spikesDown;

            structure.upIconSprite = spikeUpMapIcon;
            structure.downIconSprite = spikeDownMapIcon;

            structure.gameObject.ConvertToPrefab(true);
            return structure;
        });

        public Structure_OLDCastleBridge structure_castleBridge => GetOrCreate("StructureCastleBridge", () =>
        {
            Structure_OLDCastleBridge structure = new GameObject("Structure_CastleBridge").AddComponent<Structure_OLDCastleBridge>();
            structure.wall = bridgeWall;
            structure.flor = bridgeFlor;
            structure.ceil = bridgeCeil;
            structure.hngw = bridgeHangWall;
            structure.valve = castleLever;
            structure.doorPrefab = bridgeDoor;
            structure.quad = Resources.FindObjectsOfTypeAll<MeshRenderer>().FirstOrDefault(x => x.name == "Quad");
            structure.skybox = bridgeSkybox;
            structure.gameObject.ConvertToPrefab(true);
            return structure;
        });

        public CastleBridgeLever castleLever => GetOrCreate("CastleBridgeLever", () =>
        {
            var customLever = GameObject.Instantiate<GameLever>(Resources.FindObjectsOfTypeAll<GameLever>().FirstOrDefault());

            var lever = customLever.gameObject.AddComponent<CastleBridgeLever>();
            lever.on = new(customLever.onMat);
            lever.off = new(customLever.offMat);
            lever.mid = new(customLever.onMat);
            lever.on.SetMainTexture(leverUp);
            lever.on.SetTexture("_ColorGuide", leverUpLight);
            lever.on.SetTexture("_LightGuide", leverUpLight);
            lever.mid.SetMainTexture(leverMiddle);
            lever.mid.SetTexture("_ColorGuide", leverMiddleLight);
            lever.mid.SetTexture("_LightGuide", leverMiddleLight);
            lever.off.SetMainTexture(leverDown);
            lever.off.SetTexture("_ColorGuide", leverDownLight);
            lever.off.SetTexture("_LightGuide", leverDownLight);
            lever.audMan = customLever.audMan;
            lever.renderer = customLever.meshRenderer;
            lever.up = customLever.audOn;
            lever.middle = customLever.audOff;
            lever.down = customLever.audOff;

            GameObject.Destroy(lever.GetComponent<GameLever>());
            lever.gameObject.ConvertToPrefab(true);

            return lever;
        });

        public SwingDoor bridgeDoor => GetOrCreate("BridgeDoor", () =>
        {
            var door = GameObject.Instantiate<SwingDoor>(Resources.FindObjectsOfTypeAll<SwingDoor>().FirstOrDefault(x => x.name == "Door_Swinging"));

            Material matClosed = new(door.overlayShut[0]);
            matClosed.SetMainTexture(bridgeDoorClosed);

            Material matOpen = new(door.overlayOpen[0]);
            matOpen.SetMainTexture(bridgeDoorOpen);

            Material matMask = new(door.mask[0]);
            matMask.SetMaskTexture(bridgeCeil);

            door.overlayShut = [matClosed, matClosed];
            door.overlayOpen = [matOpen, matOpen];
            door.mask = [matMask, matMask];

            door.gameObject.ConvertToPrefab(true);

            return door;
        });

        public Structure_JailGates structure_ceilingGate => GetOrCreate("StructureCeilingGate", () =>
        {
            var structure = new GameObject("Structure_CeilingGates").AddComponent<Structure_JailGates>();

            structure.gate = gate;

            structure.gameObject.ConvertToPrefab(true);

            return structure;
        });

        public CeilingGates gate => GetOrCreate("CeilingGate", () =>
        {
            var gate = new GameObject("CeilingGate").AddComponent<CeilingGates>();

            var model = GameObject.Instantiate<GameObject>(gateModel, gate.transform);
            model.transform.localPosition = new(0, 1, -12);

            var sprite = ObjectCreationExtensions.CreateSpriteBillboard(ceilingGatebottom, false).AddSpriteHolder(out SpriteRenderer renderer, 0f);
            sprite.transform.localPosition = new(0, -4.999f, 0);
            sprite.transform.localRotation = Quaternion.Euler(90, 0, 0);
            sprite.transform.SetParent(gate.transform);

            gate.gameObject.AddComponent<BoxCollider>().size = new(1, 100, 1);
            gate.gameObject.GetComponent<BoxCollider>().isTrigger = true;

            gate.model = model.transform;
            gate.gameObject.ConvertToPrefab(true);
            return gate;
        });

        public GameObject gateModel => GetOrCreate("GateModel", () =>
        {
            Material cageMat = new Material(Resources.FindObjectsOfTypeAll<Material>().FirstOrDefault(x => x.name == "Locker_Red"));
            cageMat.SetMainTexture(gateTexture);
            var ceilingCageModel = gateModelPrefab;

            foreach (MeshRenderer renderer in ceilingCageModel.GetComponentsInChildren<MeshRenderer>())
            {
                var a = renderer.gameObject.AddComponent<MeshCollider>();
                a.sharedMesh = renderer.GetComponent<MeshFilter>().mesh;
                a.isTrigger = false;
                renderer.material = cageMat;
            }

            ceilingCageModel.transform.localScale = Vector3.one * 50;
            return ceilingCageModel;
        });

        public override LevelType type => ExtendLevelType.Castle.GetLevelType();
        public override int weight => 0;

        public Texture2D wallTex, florTex, ceilTex;

        public Texture2D octopusLockTex;
        public Sprite keyLarge, keySmall;

        public GameObject originalLightBase;
        public Sprite lightSprite;

        public Sprite spikeSprite;
        public Sprite spikeHoleSprite;
        public SoundObject spikesUp;
        public SoundObject spikesDown;
        public Sprite spikeUpMapIcon, spikeDownMapIcon;

        public Texture2D bridgeWall, bridgeFlor, bridgeCeil, bridgeHangWall, bridgeSkybox;
        public Texture2D leverUp, leverMiddle, leverDown;
        public Texture2D leverUpLight, leverMiddleLight, leverDownLight;

        public Texture2D bridgeDoorClosed, bridgeDoorOpen;

        public Texture2D gateTexture;
        public Sprite ceilingGatebottom;
        public GameObject gateModelPrefab;

        public static LevelType_Castle instance;
    }
}
