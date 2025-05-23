using MoreLevelTypesInPlus.Structures.Objects;
using System;
using UnityEngine;

namespace MoreLevelTypesInPlus.Structures
{
    public class Structure_JailGates : StructureBuilder
    {
        public override void Generate(LevelGenerator lg, System.Random rng)
        {
            base.Generate(lg, rng);

            for (int i = 0; i < 10; i++)
            {
                var cg = Instantiate<CeilingGates>(gate);
                cg.Initialize();

                var cells = lg.ec.mainHall.GetTilesOfShape(TileShapeMask.Corner | TileShapeMask.Straight | TileShapeMask.End, true);
                cells.FilterOutCellsThatDontFitCoverage(CellCoverage.Up, CellCoverageType.Hard, false);
                cells.ControlledShuffle<Cell>(rng);
                cg.transform.localPosition = cells[0].CenterWorldPosition;
                cells[0].HardCoverEntirely();
                Debug.Log(cells[0].lightColor.ToString());
            }
        }

        public CeilingGates gate;
    }
}
