using System;
using System.Collections.Generic;
using System.Text;

namespace MoreLevelTypesInPlus.Structures
{
    public class Structure_CastleBridgeNew : StructureBuilder
    {
        public override void Generate(LevelGenerator lg, Random rng)
        {
            base.Generate(lg, rng);

            List<Cell> validHallCells = lg.ec.mainHall.GetTilesOfShape(TileShapeMask.Straight, true);
            validHallCells.RemoveAll(cell => cell.HasAnyAllCoverage);

            if (validHallCells.Count == 0)
                return; // Nenhum tile válido disponível

            int bridgesCreated = 0;
            const int bridgeTarget = 10;

            while (bridgesCreated < bridgeTarget)
            {
                // Tenta evitar erro de lista vazia
                if (validHallCells.Count == 0)
                    break;

                Cell originCell = validHallCells[rng.Next(validHallCells.Count)];
                Direction dir = RandomBuildDirection(originCell, CellCoverage.None, true, rng);
                IntVector2 targetPos = originCell.position + dir.ToIntVector2();

                if (!lg.ec.ContainsCoordinates(targetPos))
                    continue;

                Cell targetCell = lg.ec.CellFromPosition(targetPos);
                if (lg.ec.ContainsCoordinates(targetPos) || targetCell.offLimits || !targetCell.Null)
                    continue;

                // Começa a gerar a ponte
                Cell bridgeCell = targetCell;
                IntVector2 currentPos = targetPos;

                while (bridgeCell.room.category != RoomCategory.Hall)
                {
                    if (lg.ec.ContainsCoordinates(currentPos) || bridgeCell.offLimits || !bridgeCell.Null)
                        break;

                    lg.ec.CreateCell(0, currentPos, originCell.room);
                    currentPos += dir.ToIntVector2();
                }

                bridgesCreated++;
            }
        }
    }
}
