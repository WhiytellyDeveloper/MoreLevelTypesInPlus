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
using MoreLevelTypesInPlus.LevelTypes_Constructor;

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

            LoadingEvents.RegisterOnAssetsLoaded(Info, PreLoading(), false);

            ModdedSaveGame.AddSaveHandler(Info);
            GeneratorManagement.Register(this, GenerationModType.Preparation, TestCreateNewLevelTypes);
        }
        private IEnumerator PreLoading()
        {
            yield return 1;
            yield return "Doing More Level Types!";

            levels.Add(new LevelType_Castle().PreloadAssets());
            levels.Add(new LevelType_Museum().PreloadAssets());



        }

        public void TestCreateNewLevelTypes(string levelName, int levelId, SceneObject scene)
        {
            if (scene.randomizedLevelObject.Length == 0)
                return;

            foreach (var level in levels)
            {
                CustomLevelObject labLevel = scene.GetCustomLevelObjects().Where(x => x.type == LevelType.Laboratory).FirstOrDefault();
                CustomLevelObject customLevel = labLevel.MakeClone();
                customLevel.name = customLevel.name.Replace("(Clone)", "").Replace("Laboratory", level.type.ToStringExtended());
                List<StructureWithParameters> structures = customLevel.forcedStructures.ToList();
                structures.RemoveAll(x => x.prefab is Structure_TeleporterRoom);
                customLevel.forcedStructures = structures.ToArray();

                customLevel = level.OverrideFloor(customLevel, levelId);

                scene.randomizedLevelObject = scene.randomizedLevelObject.AddToArray(new WeightedLevelObject { selection = customLevel, weight = level.weight });
            }
        }

        public List<LevelType_Base> levels = [];
    }

    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "whiytellydeveloper.plugin.plusmod.moreleveltypesinbbplus";
        public const string PLUGIN_NAME = "More Level Types!";
        public const string PLUGIN_VERSION = "1.0";
    }
}
