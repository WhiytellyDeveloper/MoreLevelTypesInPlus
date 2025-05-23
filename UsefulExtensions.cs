using MTM101BaldAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoreLevelTypesInPlus
{
    public static class UsefulExtensions
    {
        public static LevelType GetLevelType(this ExtendLevelType elt) => EnumExtensions.ExtendEnum<LevelType>(elt.ToString());
    }
}
