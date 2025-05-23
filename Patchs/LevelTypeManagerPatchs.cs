using HarmonyLib;
using MoreLevelTypesInPlus.LevelTypes_Constructor;
using MoreLevelTypesInPlus.LevelTypes_SpecialManagers;
using MTM101BaldAPI;
using MTM101BaldAPI.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreLevelTypesInPlus.Patchs
{
    [HarmonyPatch]
    internal class LevelTypeApplyPatch
    {
        [HarmonyPatch(typeof(BaseGameManager), nameof(BaseGameManager.Initialize)), HarmonyPrefix]
        public static void Prefix_Applay(BaseGameManager __instance)
        {
            if (__instance.gameObject.GetComponent<LevelType_Base>() != null)
                GameObject.Destroy(__instance.gameObject.GetComponent<LevelTypeManager_Base>());

            if (Singleton<BaseGameManager>.Instance.levelObject == null)
                return;

            System.Type managerType = GetManagerFromActualLevelType();

            if (managerType != null)
            {
                var component = __instance.gameObject.AddComponent(managerType);
                component.ReflectionSetVariable("destroyOnLoad", true);
                SceneManager.MoveGameObjectToScene(__instance.gameObject, SceneManager.GetActiveScene());
            }

        }

        [HarmonyPatch(typeof(BaseGameManager), nameof(BaseGameManager.BeginPlay)), HarmonyPostfix]
        public static void Prefix_BeginPlaying()
        {
            if (Singleton<BaseGameManager>.Instance.levelObject == null)
                return;

            System.Type managerType = GetManagerFromActualLevelType();

            if (managerType != null) Singleton<LevelTypeManager_Base>.Instance.OnBeginPlaying();
        }

        [HarmonyPatch(typeof(BaseGameManager), nameof(BaseGameManager.ActivityCompleted)), HarmonyPostfix]
        public static void Prefix_ActivityCompleted(bool correct, Activity activity)
        {
            if (Singleton<BaseGameManager>.Instance.levelObject == null)
                return;

            System.Type managerType = GetManagerFromActualLevelType();

            if (managerType != null) Singleton<LevelTypeManager_Base>.Instance.ActivityCompleted(correct, activity);
        }

        public static System.Type GetManagerFromActualLevelType()
        {
            var levelType = Singleton<BaseGameManager>.Instance.levelObject.type;

          // if (levelType == ExtendLevelType.Castle.GetLevelType())
                //return LevelType_Castle.levelManager;

            //if (levelType == ExtendLevelType.Museum.GetLevelType())
                return LevelType_Museum.levelManager;

           // return null;
        }

    }
}
