using System;
using Newtonsoft.Json;
using ShipModel;

namespace Ship
{
    public class BattleshipsMockImpl : IBattleships
    {
        private PlacementGrid ships = new PlacementGrid();

        private MasterGameState mockState = new MasterGameState();

        public BattleshipsMockImpl()
        {
        }

        private void SetShip(ShipModel.Ship ship)
        {
            ships.SetShipLocation(ship, ship.PositionX, ship.PositionY, ship.Facing);
        }

        public void PlaceShip(string payload)
        {
            Console.WriteLine("Received orders to place ship {0}", payload);
            ShipModel.Ship ship = JsonConvert.DeserializeObject<ShipModel.Ship>(payload);
            SetShip(ship);
        }

        public bool Shoot(int x, int y)
        {
            Console.WriteLine("Firing {0},{1}", x, y);
            bool hit = ships.Squares[x, y].CurrentOccupier != null;

            mockState.StateTeamB.ShotsYouHaveFired++;

            GridSquare square = new GridSquare
            {
                PositionX = x,
                PositionY = y,
                Marked = true,
                HitPeg = hit
            };

            mockState.StateTeamB.Score += Convert.ToInt16(hit);

            mockState.StateTeamB.TrackingGrid.Squares[x, y] = square;
            return hit;
        }

        public MasterGameState StatusAPI()
        {
            return mockState;
        }
    }
}
