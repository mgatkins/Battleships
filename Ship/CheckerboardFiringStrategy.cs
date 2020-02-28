using System;
using ShipModel;

namespace Ship
{
    public class CheckerboardFiringStrategy : IFiringStrategy
    {
        private GridSquare lastShot;

        public CheckerboardFiringStrategy()
        {
        }

        public GridSquare GetOptimumTarget(TrackingGrid grid, GridSquare lastShotInGame)
        {
            Console.WriteLine("PingKiller looking for target");

            GridSquare potentialTarget = null;

            while (true)
            {
                if (lastShot == null)
                {
                    lastShot = new GridSquare
                    {
                        PositionX = 0,
                        PositionY = 0
                    };

                    potentialTarget = lastShot;
                }
                else
                {
                    potentialTarget = new GridSquare();

                    bool evenRow = lastShot.PositionY % 2 == 0;

                    if ((evenRow && lastShot.PositionX < 7) || (!evenRow && lastShot.PositionX < 8))
                    {
                        potentialTarget.PositionY = lastShot.PositionY;
                        potentialTarget.PositionX = lastShot.PositionX + 2;
                    }
                    else
                    {
                        potentialTarget.PositionY = lastShot.PositionY + 1;

                        if (potentialTarget.PositionY > 9)
                        {
                            potentialTarget.PositionX = -1;
                         
                        }

                        if (potentialTarget.PositionX != -1)
                        {
                            evenRow = !evenRow;

                            // Even rows
                            if (evenRow)
                            {
                                potentialTarget.PositionX = 0;
                            }
                            else
                            {
                                // Odd rows
                                potentialTarget.PositionX = 1;
                            }
                        }
                    }

                    lastShot = potentialTarget;
                }
                
                if (potentialTarget.PositionX != -1)
                {
                    if (!grid.Squares[potentialTarget.PositionX, potentialTarget.PositionY].HitPeg && !grid.Squares[potentialTarget.PositionX, potentialTarget.PositionY].Marked)
                    {
                        break;
                    }
                    else
                    {
                        lastShot = potentialTarget;
                    }
                }
                else
                {
                    break;
                }
            }

            return potentialTarget;
        }
    }
}
  
