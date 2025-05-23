using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MoreLevelTypesInPlus.Structures.Functions
{
    public class Classroom_LockedRoomFunction : LockedRoomFunction
    {
        public override void Initialize(RoomController room)
        {

            this.room = room;
        }


        public override void AfterRoomValuesCalculated(LevelBuilder builder, System.Random rng)
        {
        }
    }
}
