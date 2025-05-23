using MoreLevelTypesInPlus.LevelTypes_Constructor;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MoreLevelTypesInPlus.LevelTypes_SpecialManagers
{
    public class LevelTypeManager_Castle : LevelTypeManager_Base
    {
        public override void OnBeginPlaying()
        {
            base.OnBeginPlaying();
            Singleton<CoreGameManager>.Instance.GetHud(0).UpdateInventorySize(Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.maxItem + 2);
            Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.SetItem(LevelType_Castle.instance.classKey, 5);
            Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.maxItem++;
            Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.LockSlot(5, true);
            keys = 0;

            foreach (Cell cell in Singleton<BaseGameManager>.Instance.ec.cells)
            {
                if (cell.hasLight && cell.room.type == RoomType.Room)
                {
                    cell.lightStrength /= 3;
                    Singleton<BaseGameManager>.Instance.ec.RegenerateLight(cell);
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.selectedItem = 5;
                Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.UpdateSelect();
            }

            Singleton<CoreGameManager>.Instance.GetHud(0).SetItemSelect(Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.selectedItem, Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.items[Singleton<CoreGameManager>.Instance.GetPlayer(0).itm.selectedItem].nameKey.Replace("#$%#", keys.ToString()));

            //#$%#
        }

        public int keys;
    }
}
