using System;
using System.Collections.Generic;
using ShipModel;

namespace Ship
{
    public class DefaultFleetOrders : IFleetDeployer
    {
        public DefaultFleetOrders()
        {
        }

        public List<ShipModel.Ship> getOrders()
        {
            List<ShipModel.Ship> ships = new List<ShipModel.Ship>();

            ships.Add(new ShipModel.Ship("Graf Zeppelin", ShipModel.ShipType.AircraftCarrier, 0, 0, Direction.South));
            ships.Add(new ShipModel.Ship("Bismark", ShipModel.ShipType.Destroyer, 3, 0, Direction.South));
            ships.Add(new ShipModel.Ship("Augsburg", ShipModel.ShipType.Frigate, 6, 0, Direction.West));
            ships.Add(new ShipModel.Ship("Lubeck", ShipModel.ShipType.Frigate, 3, 5, Direction.West));
            ships.Add(new ShipModel.Ship("U-96", ShipModel.ShipType.Submarine, 7, 7, Direction.South));

            return ships;
        }
    }
}
