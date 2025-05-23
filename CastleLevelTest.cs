
using MoreLevelTypesInPlus.LevelTypes_Constructor;
using MTM101BaldAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoreLevelTypesInPlus
{
    /*
    public class CastleLevelTest : LevelTypedGenerator
    {
        public override void ApplyChanges(string levelName, int levelId, CustomLevelObject obj)
        {
            List<StructureWithParameters> structures = obj.forcedStructures.ToList();
            structures.RemoveAll(x => x.prefab is Structure_TeleporterRoom);
            obj.forcedStructures = structures.ToArray();
            LevelType_Castle.instance.OverrideFloor(obj, levelId);
        }

        public override int GetWeight(int defaultWeight)
        {
            return defaultWeight * 22;
        }

        public override bool ShouldGenerate(string levelName, int levelId, SceneObject sceneObject)
        {
            return true;
        }

        public override LevelType myLevelType => EnumExtensions.GetFromExtendedName<LevelType>("Castle");
        public override string levelObjectName => "Castle";
        public override LevelType levelTypeToBaseOff => LevelType.Laboratory;
    }
    */
}
