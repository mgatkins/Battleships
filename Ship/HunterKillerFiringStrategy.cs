using System;
using ShipModel;

namespace Ship
{
    public class HunterKillerFiringStrategy : IFiringStrategy
    {
        private GridSquare initialHitLocation;

        private Direction direction = Direction.South;

        private string hitDirection = null;

        private bool moreDirections = true;
        private bool startingHunt = true;

        public HunterKillerFiringStrategy(GridSquare initialHitLocation)
        {
            // Use the last hitLocatio to start shooting around the area
            this.initialHitLocation = initialHitLocation;
        }

        public GridSquare GetOptimumTarget(TrackingGrid grid, GridSquare lastShot)
        {
            Console.WriteLine("HunterKiller looking for target");

            GridSquare potentialTarget = LookForTarget(grid, lastShot);

            return potentialTarget;
        }

        private GridSquare LookForTarget(TrackingGrid grid, GridSquare lastShot)
        {
            GridSquare target = new GridSquare();

            // Last shot was hit - keep going in same direction
            if (lastShot != null)
            {
                bool changeDirection = false;
                bool offBoard = false;

                while (true)
                {
                    if (startingHunt)
                    {
                        if (!grid.Squares[lastShot.PositionX, lastShot.PositionY-1].Marked
                            && !grid.Squares[lastShot.PositionX, lastShot.PositionY - 1].HitPeg && lastShot.PositionY > 0)
                        {
                            direction = Direction.South;
                        }
                        else if (!grid.Squares[lastShot.PositionX, lastShot.PositionY + 1].Marked
                            && !grid.Squares[lastShot.PositionX, lastShot.PositionY + 1].HitPeg && lastShot.PositionY < 10)
                        {
                            direction = Direction.North;
                        }
                        else if (!grid.Squares[lastShot.PositionX-1, lastShot.PositionY].Marked
                            && !grid.Squares[lastShot.PositionX-1, lastShot.PositionY].HitPeg && lastShot.PositionX > 0)
                        {
                            direction = Direction.West;
                        }
                        else if (!grid.Squares[lastShot.PositionX+1, lastShot.PositionY].Marked
                            && !grid.Squares[lastShot.PositionX+1, lastShot.PositionY].HitPeg && lastShot.PositionX < 10)
                        {
                            direction = Direction.East;
                        }
                    }

                    if (lastShot.HitPeg && !startingHunt)
                    {
                        // Hit last shot keep going
                        hitDirection = direction.ToString();

                        // Force swap if we are at the edge of the board
                        if ((direction == Direction.South && lastShot.PositionY == 0) ||
                            (direction == Direction.North && lastShot.PositionY == 9) ||
                            (direction == Direction.East && lastShot.PositionX == 9) ||
                            (direction == Direction.West && lastShot.PositionX == 0))
                        {
                            offBoard = true;

                            if (moreDirections)
                            {
                                changeDirection = true;
                            }
                            else
                            {
                                target.PositionX = -1;
                                break;
                            }
                        }
                        else
                        {
                            if (moreDirections)
                            {
                                changeDirection = true;
                            }
                        }
                    }

                    if (!lastShot.HitPeg || changeDirection)
                    {
                        if (moreDirections)
                        {
                            // If we hit something lastTime - switch direction
                            if (hitDirection != null)
                            {
                                // Don't change anymore
                                moreDirections = false;

                                direction = (Direction)Enum.Parse(typeof(Direction), hitDirection);
                            }

                            if (direction == Direction.South)
                            {
                                direction = Direction.North;
                            }
                            else if (direction == Direction.North)
                            {
                                direction = Direction.East;
                            }
                            else if (direction == Direction.East)
                            {
                                direction = Direction.West;
                            }
                            else if (direction == Direction.West)
                            {
                                direction = Direction.East;
                            }
                        }
                    }

                    target = searchGridForTarget(lastShot.HitPeg == true && !offBoard ? lastShot : initialHitLocation);

                    if (!moreDirections)
                    {
                        if (target.PositionX == -1)
                        {
                            // No more targets
                            break;
                        }
                        else
                        {
                            // If we've already fired here - no more targets
                            if (grid.Squares[target.PositionX, target.PositionY].Marked || grid.Squares[target.PositionX, target.PositionY].HitPeg)
                            {
                                target.PositionX = -1;

                                break;
                            }
                        }
                    }
                    
                    if (target.PositionX != -1)
                    {
                        if (!grid.Squares[target.PositionX, target.PositionY].Marked && !grid.Squares[target.PositionX, target.PositionY].HitPeg)
                        {
                            break;
                        }
                    }
                }

                startingHunt = false;
            }

            return (target);
        }

        private GridSquare searchGridForTarget(GridSquare searchLocation)
        {
            GridSquare target = new GridSquare();

            target.PositionX = -1;

            if (direction == Direction.South)
            {
                if (searchLocation.PositionY > 0)
                {
                    target.PositionX = searchLocation.PositionX;
                    target.PositionY = searchLocation.PositionY-1;
                }
            }

            if (direction == Direction.North)
            {
                if (searchLocation.PositionY < 9)
                {
                    target.PositionX = searchLocation.PositionX;
                    target.PositionY = searchLocation.PositionY+1;
                }
            }

            if (direction == Direction.East)
            {
                if (searchLocation.PositionX < 9)
                {
                    target.PositionX = searchLocation.PositionX+1;
                    target.PositionY = searchLocation.PositionY;
                }
            }
            if (direction == Direction.West)
            {
                if (searchLocation.PositionX > 0)
                {
                    target.PositionX = searchLocation.PositionX-1;
                    target.PositionY = searchLocation.PositionY;
                }
            }

            return target;
        }
    }
}
