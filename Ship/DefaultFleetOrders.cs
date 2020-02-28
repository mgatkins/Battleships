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

            ships.Add(new ShipModel.Ship("Graf Zeppelin", ShipModel.ShipType.AircraftCarrier, 5, 5, Direction.North, 1));
            ships.Add(new ShipModel.Ship("Bismark", ShipModel.ShipType.Destroyer, 0, 9, Direction.North,1 ));
            ships.Add(new ShipModel.Ship("Bismark 2", ShipModel.ShipType.Destroyer, 8, 3, Direction.South, 2));
            ships.Add(new ShipModel.Ship("Augsburg", ShipModel.ShipType.Frigate, 9, 7, Direction.North, 1));
            ships.Add(new ShipModel.Ship("Lubeck", ShipModel.ShipType.Frigate, 3, 3, Direction.East,2));
            ships.Add(new ShipModel.Ship("Lubeck 2", ShipModel.ShipType.Frigate, 7, 9, Direction.East, 3));
            ships.Add(new ShipModel.Ship("U-96", ShipModel.ShipType.Submarine, 1, 1, Direction.North,1));

            return ships;
        }
    }
}
