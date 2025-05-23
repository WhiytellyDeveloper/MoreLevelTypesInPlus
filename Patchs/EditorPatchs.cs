using BaldiLevelEditor;
using HarmonyLib;
using MTM101BaldAPI;

namespace MoreLevelTypesInPlus.Patchs
{
    [HarmonyPatch]
    [ConditionalPatchMod("mtm101.rulerp.baldiplus.leveleditor")]
    internal class EditorLevelPatch
    {
        [HarmonyPatch(typeof(PlusLevelEditor), "Initialize")]
        [HarmonyPostfix]
        static void InitializeStuff(PlusLevelEditor __instance)
        {
            __instance.toolCats.Find(x => x.name == "halls").tools.AddRange([new FloorTool("MuseumPillarRoom")]);
            __instance.toolCats.Find(x => x.name == "objects").tools.Insert(0, new ObjectTool("MuseumPillar"));
        }

        [HarmonyPatch(typeof(EditorLevel), "InitializeDefaultTextures")]
        [HarmonyPostfix]
        private static void AddRoomTexs(EditorLevel __instance)
        {
            __instance.defaultTextures.Add("MuseumPillarRoom", new("BlueCarpet", "FacultyWall", "Ceiling"));
        }
    }
}
