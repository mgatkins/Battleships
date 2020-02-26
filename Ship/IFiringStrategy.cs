using System;
using ShipModel;

namespace Ship
{
    public interface IFiringStrategy
    {
        public GridSquare GetOptimumTarget(TrackingGrid grid, GridSquare lastShot);
    }
}
