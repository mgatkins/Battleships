using System;
using System.Collections.Generic;

namespace Ship
{
    public interface IFleetDeployer
    {
        public List<ShipModel.Ship> getOrders();
    }
}
