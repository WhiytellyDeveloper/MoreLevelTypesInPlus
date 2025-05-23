using HarmonyLib;
using UnityEngine;

namespace MoreLevelTypesInPlus
{
    [HarmonyPatch(typeof(Principal_StateBase), nameof(Principal_SubState.OnRoomExit))]
    internal class ExamplePrivateFuncPatch
    {
        static bool Prefix()
        {
            return false;
        }
    }
}
