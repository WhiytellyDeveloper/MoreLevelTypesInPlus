using MoreLevelTypesInPlus.LevelTypes_SpecialManagers;
using MTM101BaldAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoreLevelTypesInPlus.LevelTypes_Constructor
{
    public class LevelType_Base
    {
        public virtual LevelType_Base PreloadAssets()
        {
            return this;
        }

        public virtual CustomLevelObject OverrideFloor(CustomLevelObject original, int floorNum) => null;

        public virtual T GetOrCreate<T>(string key, Func<T> creator)
        {
            if (_createdAssets.TryGetValue(key, out object obj) && obj is T typedObj)
                return typedObj;

            T newObj = creator();
            _createdAssets[key] = newObj;
            return newObj;
        }


        public virtual LevelType type => LevelType.Schoolhouse;
        public virtual int weight => 100;
        public readonly Dictionary<string, object> _createdAssets = new();
        public static System.Type levelManager = typeof(LevelTypeManager_Base);
    }
}
