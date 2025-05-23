using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoreLevelTypesInPlus.Functions
{
    public class FullOpenAreaFunction : RoomFunction
    {
        public override void Build(LevelBuilder builder, Random rng)
        {
            base.Build(builder, rng);

            foreach (Cell cell in room.cells)
            {
                foreach (Direction dir in Directions.All())
                {
                    if (room.ec.ContainsCoordinates(cell.position + dir.ToIntVector2()) 
                        && room.ec.CellFromPosition(cell.position + dir.ToIntVector2()).room.category == RoomCategory.Hall
                        && room.eventSafeCells.Contains(cell.position))
                    room.ec.ConnectCells(cell.position, dir);
                }
            }
        }

        public override void OnGenerationFinished()
        {
            base.OnGenerationFinished();

            foreach (Door door in room.doors)
            {
                door.UnInitialize();
                door.GetComponent<StandardDoor>().aMapTile.spriteRenderer.enabled = false;
                door.GetComponent<StandardDoor>().bMapTile.spriteRenderer.enabled = false;
                Destroy(door.gameObject);
            }
        }
    }
}
