using System;
using ShipModel;

namespace Ship
{
    public interface IBattleships
    {
        public bool Shoot(int x, int y);

        public void PlaceShip(String payload);

        public MasterGameState StatusAPI();
    }
}
