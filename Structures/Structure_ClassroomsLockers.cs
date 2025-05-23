using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MoreLevelTypesInPlus.Structures.Functions;
using UnityEngine;

namespace MoreLevelTypesInPlus
{
    public class Structure_ClassroomsLockers : StructureBuilder
    {
        public override void Generate(LevelGenerator lg, System.Random rng)
        {
            base.Generate(lg, rng);

            foreach (RoomController room in lg.Ec.rooms)
            {
                if (room.category != RoomCategory.Class)
                    continue;

                var func = room.functions.gameObject.AddComponent<Classroom_LockedRoomFunction>();
                room.functions.AddFunction(func);
                func.Initialize(room);

                foreach (Door door in room.doors)
                {
                    GameLock actualLock = Instantiate<GameLock>(gameLock, room.functionObject.transform);
                    actualLock.requiredKey = classKey.itemType;
                    func.locks.Add(actualLock);

                    if (door.aTile.room == room)
                    {
                        actualLock.transform.position = door.aTile.CenterWorldPosition;
                        actualLock.transform.rotation = door.direction.ToRotation();
                        actualLock.Initialize(func, ec, door.aTile.position, door.direction);
                    }
                    if (door.bTile.room == room)
                    {
                        actualLock.transform.position = door.bTile.CenterWorldPosition;
                        actualLock.transform.rotation = door.direction.GetOpposite().ToRotation();
                        actualLock.Initialize(func, ec, door.bTile.position, door.direction.GetOpposite());
                    }
                }
            }
        }

        public override void OnGenerationFinished(LevelBuilder lb)
        {
            base.OnGenerationFinished(lb);

            foreach (RoomController room in ec.rooms)
            {
                if (room.category != RoomCategory.Class)
                    continue;

                DijkstraMap dijkstraMap = new DijkstraMap(ec, PathType.Const, int.MaxValue, Array.Empty<Transform>());
                dijkstraMap.Calculate(int.MaxValue, false, [room.cells[0].position]);

                List<WeightedRoomController> list = new List<WeightedRoomController>();
                foreach (RoomController roomController in ec.rooms)
                {
                    if (roomController.category != RoomCategory.Class && roomController != room && dijkstraMap.Value(roomController.cells[0].position) > 0 && roomController.itemSpawnPoints.Count > 0)
                    {
                        list.Add(new WeightedRoomController());
                        list[list.Count - 1].selection = roomController;

                        if (roomController.currentItemValue == 0)
                            list[list.Count - 1].weight = 999999;
                        else
                            list[list.Count - 1].weight = 5;
                    }
                }
                RoomController roomController2 = null;
                while (roomController2 == null && list.Count > 0)
                {
                    int index = WeightedSelection<RoomController>.ControlledRandomIndexList(WeightedRoomController.Convert(list), new(lb.seed));
                    RoomController selection = list[index].selection;
                    int num;
                    if (room.ec.RoomDependencyIsValid(room, selection, out num))
                    {
                        roomController2 = selection;
                    }
                    else
                    {
                        list.RemoveAt(index);
                    }
                }
                if (roomController2 != null)
                {
                    room.ec.AddRoomDependency(room, roomController2);
                    lb.PlaceItemInRoom(classKeyPickup, roomController2, new(lb.seed), true);
                    Debug.Log(roomController2.name);
                }
            }
        }


        public List<RoomCategory> roomCategories = [];
        public ItemObject classKeyPickup, classKey;
        public GameLock gameLock;
    }
}
