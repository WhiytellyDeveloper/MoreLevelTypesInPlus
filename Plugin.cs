using BepInEx;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Registers;
using MTM101BaldAPI.SaveSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MoreLevelTypesInPlus
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("mtm101.rulerp.bbplus.baldidevapi", MTM101BaldiDevAPI.VersionNumber)]
    [BepInProcess("BALDI.exe")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin __instance { get; private set; }
        public static AssetManager assetMan = new AssetManager();

        private void Awake()
        {
            Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            __instance = this;
            harmony.PatchAllConditionals();

            LoadingEvents.RegisterOnAssetsLoaded(Info, PostLoading(), true);

            ModdedSaveGame.AddSaveHandler(Info);
            GeneratorManagement.Register(this, GenerationModType.Preparation, TestCreateNewLevelTypes);
        }
        private IEnumerator PostLoading()
        {
            yield return 1;
            yield return "Doing More Level Types!";
        }

        public void TestCreateNewLevelTypes(string levelName, int levelId, SceneObject scene)
        {
            if (scene.randomizedLevelObject.Length == 0)
                return;

            CustomLevelObject level = scene.GetCustomLevelObjects().Where(x => x.type == LevelType.Laboratory).FirstOrDefault();
            CustomLevelObject customLevel = level.MakeClone();
            customLevel.name = name.Replace("(Clone)", "").Replace("Laboratory", "Castle");
            List<StructureWithParameters> structures = customLevel.forcedStructures.ToList();
            structures.RemoveAll(x => x.prefab is Structure_TeleporterRoom);
            customLevel.forcedStructures = structures.ToArray();

            customLevel = Castle_LevelType(customLevel);

            scene.randomizedLevelObject = scene.randomizedLevelObject.AddToArray(new WeightedLevelObject { selection = customLevel, weight = 999999 });
        }

        public CustomLevelObject Castle_LevelType(CustomLevelObject baseLevel)
        {
            baseLevel.minPlots += 8;
            baseLevel.maxPlots += 8;
            baseLevel.minSize -= new IntVector2(7, 7);
            baseLevel.maxSize -= new IntVector2(7, 7);
            baseLevel.hallWallTexs = [new WeightedTexture2D { selection = AssetLoader.TextureFromMod(this, "CastleWall.png"), weight = 100 }];
            baseLevel.hallFloorTexs = [new WeightedTexture2D { selection = AssetLoader.TextureFromMod(this, "CastleFloor.png"), weight = 100 }];
            baseLevel.hallCeilingTexs = [new WeightedTexture2D { selection = AssetLoader.TextureFromMod(this, "CastleCeiling.png"), weight = 100 }];
            baseLevel.standardLightStrength = 6;
            baseLevel.maxLightDistance = 10;

            //Adicionar jaulas e espinhos coloridos como estrutura base
            //támbem adicionar a sala com suas funções princiapis
            //adicionar lamparinas
            return baseLevel;
        }
    }

    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "whiytellydeveloper.plugin.plusmod.moreleveltypesinbbplus";
        public const string PLUGIN_NAME = "More Level Types!";
        public const string PLUGIN_VERSION = "1.0";
    }
}
