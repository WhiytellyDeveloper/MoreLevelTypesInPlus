using MoreLevelTypesInPlus.Structures.Functions;
using MoreLevelTypesInPlus.Structures.Objects;
using MTM101BaldAPI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MoreLevelTypesInPlus.Structures
{
    public class Structure_OLDCastleBridge : StructureBuilder
    {
        public Texture2D wall, flor, ceil, hngw, skybox;
        public MeshRenderer quad;
        public CastleBridgeLever valve;
        public SwingDoor doorPrefab;
        public GameObject lightHanger;

        public override void Generate(LevelGenerator lg, System.Random rng)
        {
            base.Generate(lg, rng);

            int targetBridges = rng.Next(parameters.minMax[0].x, parameters.minMax[0].z);
            int attempts = int.MaxValue;
            int bridgesBuilt = 0;

            HashSet<IntVector2> usedPositions = new();

            while (attempts-- > 0 && bridgesBuilt < targetBridges)
            {
                var tilesOfShape = ec.mainHall.GetTilesOfShape(TileShapeMask.Straight, true);
                tilesOfShape.FilterOutCellsThatDontFitCoverage(CellCoverage.Center, CellCoverageType.All, true);

                if (tilesOfShape.Count == 0) continue;

                tilesOfShape.ControlledShuffle(rng);

                Cell bestStart = null;
                Direction bestDir = Direction.Null;
                List<IntVector2> bestPath = new();
                RoomController targetRoom = null;

                foreach (var startCell in tilesOfShape)
                {
                    foreach (var dir in Directions.All())
                    {
                        var pos = startCell.position;
                        var path = new List<IntVector2>();

                        for (int i = 0; i < 22; i++)
                        {
                            pos += dir.ToIntVector2();
                            if (IsNearUsed(pos, usedPositions)) break;

                            var next = lg.ec.CellFromPosition(pos);
                            if (!next.Null)
                            {
                                if (next.room.category == RoomCategory.Hall && !next.HasAnyHardCoverage && path.Count >= 5)
                                {
                                    if (path.Count > bestPath.Count)
                                    {
                                        bestStart = startCell;
                                        bestDir = dir;
                                        bestPath = new(path);
                                        targetRoom = next.room;
                                    }
                                }
                                break;
                            }
                            path.Add(pos);
                        }
                    }
                }

                if (bestPath.Count > 0)
                {
                    var rooms = new List<RoomController> { null };
                    lg.CreateArea(rooms, RoomType.Hall, RoomCategory.Null, 0, new(0, 0), new(0, 0), false);
                    var bridgeRoom = rooms[0];
                    SetupBridgeRoom(bridgeRoom, bridgesBuilt);
                    var bridgeFunc = bridgeRoom.functions.GetComponent<RoomFunction_CastleBridge>();

                    var parent = new GameObject($"BoxColliders_{bridgesBuilt}");

                    foreach (var pos in bestPath)
                    {
                        lg.ec.CreateCell(15, pos, bridgeRoom);
                        var cell = lg.ec.CellFromPosition(pos);
                        lg.ec.ConnectCells(pos, bestDir);
                        lg.ec.ConnectCells(pos, bestDir.GetOpposite());
                        cell.HardCoverEntirely();

                        foreach (var dir in cell.AllWallDirections)
                        {
                            for (int i = -5; i < 5; i++)
                            {
                                var wall = Instantiate(quad, bridgeRoom.objectObject.transform);
                                wall.transform.SetPositionAndRotation(
                                    cell.CenterWorldPosition + dir.ToVector3() * 5.001f + Vector3.up * (i * 10),
                                    dir.ToRotation()
                                );
                                wall.transform.localScale = Vector3.one * 10;
                                wall.material = Resources.FindObjectsOfTypeAll<Material>().First(x => x.name == "Skybox");
                                wall.material.SetMainTexture(bridgeRoom.wallTex);
                            }
                        }

                        var box = parent.AddComponent<BoxCollider>();
                        box.size = new Vector3(10, 100, 10);
                        box.center = cell.TileTransform.localPosition;
                        box.isTrigger = true;
                        box.transform.SetParent(bridgeRoom.transform);
                        bridgeFunc.colliders.Add(box);

                        MarkUsedAround(pos, usedPositions);
                    }

                    MarkUsedAround(bestStart.position, usedPositions);
                    if (bridgeFunc.colliders.Count > 0)
                        bridgeFunc.colliders.RemoveAt(0);

                    bridgeFunc.pointA = lg.ec.CellFromPosition(bestPath[0]);
                    bridgeFunc.pointB = lg.ec.CellFromPosition(bestPath[bestPath.Count - 1]);

                    Color lightColor = new(255f / 255f, 172f / 266f, 106f / 255f);
                    lg.ec.GenerateLight(bridgeFunc.pointA, lightColor, 5);
                    lg.ec.GenerateLight(bridgeFunc.pointB, lightColor, 5);

                    var button = GameButton.BuildInArea(ec, bestStart.position, 1, bridgeFunc.gameObject, valve, rng);
                    var button2 = GameButton.BuildInArea(ec, bridgeFunc.pointB.position, 2, bridgeFunc.gameObject, valve, rng);

                    foreach (var lever in new[] { button, button2 })
                    {
                        var leverComponent = lever.GetComponent<CastleBridgeLever>();
                        leverComponent.off.SetColor("_TextureColor", new Color(0f, 0f, 1f, 0f));
                        leverComponent.mid.SetColor("_TextureColor", new Color(0f, 0f, 1f, 0f));
                        leverComponent.on.SetColor("_TextureColor", new Color(0f, 0f, 1f, 0f));
                    }

                    SetupBridgeEnd(bridgeFunc.pointA, lg, bridgeRoom, hngw, lightHanger, ref bridgeFunc.dirA);
                    SetupBridgeEnd(bridgeFunc.pointB, lg, bridgeRoom, hngw, lightHanger, ref bridgeFunc.dirB);

                    bridgesBuilt++;
                    Debug.Log($"Castle Bridge #{bridgesBuilt} built with {bestPath.Count} cells.");
                }
                else
                {
                    Debug.LogWarning("Castle Bridge generation failed this attempt.");
                }
            }

            Debug.Log($"Castle Bridges Generated: {bridgesBuilt}/{targetBridges}");
            Finished();
        }

        private void SetupBridgeRoom(RoomController room, int index)
        {
            room.wallTex = wall;
            room.florTex = flor;
            room.ceilTex = ceil;
            room.GenerateTextureAtlas();
            room.color = new Color(86f / 255f, 34f / 255f, 32f / 255f);
            room.name = $"CastleBridge_{index}";

            var functionObject = new GameObject($"CastleBridge_{index} RoomFunctionContainer");
            functionObject.transform.SetParent(room.transform);

            var container = functionObject.AddComponent<RoomFunctionContainer>();
            container.functions = new List<RoomFunction>();
            room.functions = container;

            var bridgeFunc = functionObject.AddComponent<RoomFunction_CastleBridge>();
            container.AddFunction(bridgeFunc);
            bridgeFunc.Initialize(room);
        }

        private void SetupBridgeEnd(Cell point, LevelGenerator lg, RoomController bridgeRoom, Texture hngw, GameObject lightHanger, ref Direction outDir)
        {
            foreach (Direction dir in Directions.All())
            {
                var neighbor = lg.ec.CellFromPosition(point.position + dir.ToIntVector2());
                if (!point.HasWallInDirection(dir) && neighbor.room == lg.ec.mainHall)
                {
                    var hangingWall = Instantiate(quad, point.ObjectBase.transform);
                    hangingWall.transform.localScale = Vector3.one * 10;
                    hangingWall.transform.localRotation = dir.ToRotation();
                    hangingWall.transform.position = point.CenterWorldPosition + dir.ToVector3() * 5 + Vector3.up * 10;
                    hangingWall.material.SetMainTexture(hngw);
                    outDir = dir;

                    /*
                    if (lightHanger != null)
                    {
                        var hanger = Instantiate(lightHanger, point.ObjectBase.transform);
                        hanger.transform.position = point.CenterWorldPosition + Vector3.up * 6;
                        hanger.transform.rotation = dir.GetOpposite().ToRotation();
                    }
                    */

                    PlaceDoor(point.position, dir);
                }
                else if (!point.HasWallInDirection(dir) && neighbor.room == bridgeRoom)
                {
                    var fakeWall = Instantiate(quad, point.ObjectBase.transform);
                    fakeWall.transform.localScale = Vector3.one * 10;
                    fakeWall.transform.localRotation = dir.GetOpposite().ToRotation();
                    fakeWall.transform.position = point.CenterWorldPosition + dir.ToVector3() * 5;
                    fakeWall.material.SetMainTexture(bridgeRoom.florTex);
                    bridgeRoom.functions.GetComponent<RoomFunction_CastleBridge>().fakeWalls.Add(fakeWall);
                }
            }
        }

        public void PlaceDoor(IntVector2 position, Direction direction)
        {
            IntVector2 offset = direction.ToIntVector2();
            Cell cell = ec.CellFromPosition(position);
            Cell neighbor = ec.cells[position.x + offset.x, position.z + offset.z];

            Door door = Object.Instantiate(doorPrefab, cell.ObjectBase);
            door.transform.rotation = direction.ToRotation();
            door.ec = ec;
            door.position = position;
            door.bOffset = offset;
            door.direction = direction;
            door.Initialize();

            var rendererContainer = door.GetComponent<RendererContainer>();
            if (rendererContainer != null)
                cell.renderers.AddRange(rendererContainer.renderers);

            cell.HardCoverWall(direction, true);
        }

        private bool IsNearUsed(IntVector2 pos, HashSet<IntVector2> used)
        {
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                    if (used.Contains(new IntVector2(pos.x + dx, pos.z + dy)))
                        return true;
            return false;
        }

        private void MarkUsedAround(IntVector2 pos, HashSet<IntVector2> used)
        {
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                    used.Add(new IntVector2(pos.x + dx, pos.z + dy));
        }
    }
}
