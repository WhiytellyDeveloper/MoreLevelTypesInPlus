using MoreLevelTypesInPlus.Structures.Objects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MoreLevelTypesInPlus.Structures.Functions
{
    public class RoomFunction_CastleBridge : RoomFunction, IButtonReceiver
    {
        private void Start()
        {
            foreach (Cell cell in room.cells)
            {
                if (!positions.ContainsKey(cell))
                    positions.Add(cell, cell.TileTransform.localPosition);
            }

            foreach (MeshRenderer fakeWall in fakeWalls)
                fakeWall.gameObject.SetActive(true);

            foreach (CastleBridgeLever lever in levers)
                lever.Toggle(2);

            ButtonPressed(false);
        }


        public override void Initialize(RoomController room)
        {
            base.Initialize(room);
            //There's no point in having anything here, since nothing here works, so it's here to be an idiot. But it's super useful.
        }


        public void ButtonPressed(bool val)
        {
            /*
            foreach (CastleBridgeLever lever in levers)
            {
                if (CastleBridgeLever.last != null)
                {
                    lever.Toggle(CastleBridgeLever.last.value);
                }
            }

            if (CastleBridgeLever.last.value == 2)
            {
                room.ec.CloseCell(pointA.position, dirA.GetOpposite());
                room.ec.CloseCell(pointB.position, dirB.GetOpposite());

                foreach (Cell cell in room.cells)
                {
                    if (cell == pointA || cell == pointB)
                        continue;

                    if (!positions.ContainsKey(cell))
                        positions.Add(cell, cell.TileTransform.localPosition);

                    cell.SetShape(0, TileShapeMask.Open);
                    cell.HideTile(true);
                    cell.TileTransform.localPosition = pointA.TileTransform.localPosition;

                }

                foreach (MeshRenderer fakeWall in fakeWalls)
                    fakeWall.gameObject.SetActive(true);
            }
            else if (CastleBridgeLever.last.value == 0)
            {
                room.ec.ConnectCells(pointA.position, dirA.GetOpposite());
                room.ec.ConnectCells(pointB.position, dirB.GetOpposite());

                foreach (Cell cell in room.cells)
                {
                    if (cell == pointA || cell == pointB)
                        continue;

                    cell.SetShape(pointA.constBin, TileShapeMask.Straight);
                    cell.HideTile(false);
                    cell.TileTransform.localPosition = positions[cell];

                    foreach (MeshRenderer fakeWall in fakeWalls)
                        fakeWall.gameObject.SetActive(false);
                }
            }
                        */
        }

        public void ConnectButton(GameButtonBase button)
        {
            levers.Add(button as CastleBridgeLever);
        }

        private void Update()
        {
            for (int x = 0; x < room.ec.map.tiles.GetLength(0); x++)
            {
                for (int y = 0; y < room.ec.map.tiles.GetLength(1); y++)
                {
                    MapTile tile = room.ec.map.tiles[x, y];
                    IntVector2 pos = new(x, y);

                    bool isRoomCell = room.cells.Any(cell => cell.position == pos);

                    if (tile.Found && pos != pointA.position && pos != pointB.position && isRoomCell)
                    {
                        tile.spriteRenderer.enabled = room.ec.CellFromPosition(x, y).TileTransform.localPosition != pointA.TileTransform.localPosition;
                    }
                    else if (tile.Found)
                    {
                        tile.SpriteRenderer.sprite = room.ec.map.mapTileSprite[room.ec.CellFromPosition(x, y).ConstBin];
                    }
                }
            }
        }

        public List<BoxCollider> colliders = [];
        public List<MeshRenderer> fakeWalls = [];
        public Cell pointA, pointB;
        public Direction dirA, dirB;

        private Dictionary<Cell, Vector3> positions = [];
        private List<CastleBridgeLever> levers = [];
    }
}