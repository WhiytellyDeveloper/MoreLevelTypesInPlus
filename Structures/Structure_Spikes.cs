using MoreLevelTypesInPlus.Structures.Objects;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Structure_Spikes : StructureBuilder
{
    public Spikes spikes;

    public override void Generate(LevelGenerator lg, System.Random rng)
    {
        base.Generate(lg, rng);

        List<IntVector2> roomPositions = new();
        foreach (RoomController room in lg.ec.rooms)
            roomPositions.Add(room.position);

        HashSet<int> blockedRows = new();
        HashSet<int> blockedCols = new();
        List<Spikes> placedSpikes = new();
        List<IntVector2> placedPositions = new();

        int attempts = 0;
        int placed = 0;

        int minDistanceFromOthers = 5;
        int minDistanceFromRoom = 5;
        int maxDistanceFromRoom = 7;

        int toPlace = rng.Next(parameters.minMax[0].x, parameters.minMax[0].z);

        var allCells = lg.ec.mainHall.GetTilesOfShape(TileShapeMask.Straight, true);
        allCells.FilterOutCellsThatDontFitCoverage(CellCoverage.Up, CellCoverageType.Hard, false);
        allCells.ControlledShuffle<Cell>(rng);

        while (placed < toPlace && attempts < allCells.Count)
        {
            Cell cell = allCells[attempts++];
            IntVector2 gridPos = cell.position;
            Vector3 position = cell.CenterWorldPosition;

            Direction dir = RandomBuildDirection(cell, CellCoverage.None, false, rng);
            bool horizontal = dir == Direction.West || dir == Direction.East;

            bool blockedLine = horizontal ? blockedRows.Contains(gridPos.z) : blockedCols.Contains(gridPos.x);
            bool closeToRoom = false;

            // Verifica se há uma sala nas redondezas entre 5 e 7 tiles
            foreach (var roomPos in roomPositions)
            {
                int dist = Mathf.Abs(gridPos.x - roomPos.x) + Mathf.Abs(gridPos.z - roomPos.z);
                if (dist >= minDistanceFromRoom && dist <= maxDistanceFromRoom)
                {
                    closeToRoom = true;
                    break;
                }
            }

            // Verifica distância dos spikes já colocados
            bool farFromOthers = true;
            foreach (var pos in placedPositions)
            {
                int dist = Mathf.Abs(gridPos.x - pos.x) + Mathf.Abs(gridPos.z - pos.z);
                if (dist < minDistanceFromOthers)
                {
                    farFromOthers = false;
                    break;
                }
            }

            // Se a linha está bloqueada, só permite quebrar a regra se estiver longe dos outros e perto de uma sala
            if (blockedLine && !(farFromOthers && closeToRoom))
                continue;

            Spikes cg = GameObject.Instantiate<Spikes>(spikes);
            lg.ec.SetupDoor(cg, cell, dir);
            cg.Initialize();

            if (!lg.ec.CheckPath(cg.aTile, cg.bTile, PathType.Nav) && !lg.ec.CheckPath(cg.bTile, cg.aTile, PathType.Nav))
            {
                cg.UnInitialize();
                Destroy(cg.gameObject);
                continue;
            }


            if (horizontal)
                blockedRows.Add(gridPos.z);
            else
                blockedCols.Add(gridPos.x);

            placedPositions.Add(gridPos);
            placedSpikes.Add(cg);
            placed++;
        }

        Spikes.PairAllSpikes(placedSpikes, ec);

        Debug.Log(placed);
        Finished();
    }
}
