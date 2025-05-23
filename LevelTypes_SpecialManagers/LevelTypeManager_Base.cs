using System;
using System.Collections.Generic;
using System.Text;

namespace MoreLevelTypesInPlus.LevelTypes_SpecialManagers
{
    public class LevelTypeManager_Base : Singleton<LevelTypeManager_Base>
    {
        public virtual void OnInitialize()
        {

        }

        public virtual void OnBeginPlaying()
        {

        }

        public virtual void ActivityCompleted(bool correct, Activity act)
        {

        }
    }
}
