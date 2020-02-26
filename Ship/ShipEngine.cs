using System;
using System.Threading.Tasks;
using Figgle;
using Newtonsoft.Json;
using ShipModel;

namespace Ship
{
    class ShipEngine
    {
        private readonly IBattleships battleship;

        private IFiringStrategy firingStrategy;

        private readonly IFiringStrategy pingKiller;
       
        public ShipEngine(IBattleships battleship, IFiringStrategy firingStrategy)
        {
            this.battleship = battleship;

            this.pingKiller = firingStrategy;
            this.firingStrategy = pingKiller;
        }

        public void Run()
        {
            ShipModel.Ship carrier = new ShipModel.Ship("Graf Zeppelin", ShipModel.ShipType.AircraftCarrier, 0, 0, Direction.South);
            ShipModel.Ship destroyer = new ShipModel.Ship("Bismark", ShipModel.ShipType.Destroyer, 3, 0, Direction.South);
            ShipModel.Ship frigate1 = new ShipModel.Ship("Augsburg", ShipModel.ShipType.Frigate, 6, 0, Direction.West);
            ShipModel.Ship frigate2 = new ShipModel.Ship("Lubeck", ShipModel.ShipType.Frigate, 3, 5, Direction.West);
            ShipModel.Ship submarine = new ShipModel.Ship("U-96", ShipModel.ShipType.Submarine, 7, 7, Direction.South);

            battleship.PlaceShip(JsonConvert.SerializeObject(carrier));
            battleship.PlaceShip(JsonConvert.SerializeObject(destroyer));
            battleship.PlaceShip(JsonConvert.SerializeObject(frigate1));
            battleship.PlaceShip(JsonConvert.SerializeObject(frigate2));
            battleship.PlaceShip(JsonConvert.SerializeObject(submarine));

            bool hunting = false;

            var target = firingStrategy.GetOptimumTarget(battleship.StatusAPI().StateTeamA.TrackingGrid, null);

            bool hit = battleship.Shoot(target.PositionX, target.PositionY);

            var status = battleship.StatusAPI();

            Console.WriteLine(status.ToString());

            // Switch to hunter killer
            if (hit)
            {
                firingStrategy = new HunterKillerFiringStrategy(status.StateTeamA.TrackingGrid.Squares[target.PositionX, target.PositionY]);
                hunting = true;
            }

            //Main engine loop.
            Task t = Task.Run(async () =>
            {
                while (true)
                {
                    target = firingStrategy.GetOptimumTarget(battleship.StatusAPI().StateTeamA.TrackingGrid, hunting == true ? status.StateTeamA.TrackingGrid.Squares[target.PositionX, target.PositionY] : null);

                    if (target.PositionX == -1 && !hunting)
                    {
                        
                        Console.WriteLine("Firing solutions exhausted");
                        break;
                    }
                    else if (target.PositionX == -1)
                    {
                        firingStrategy = pingKiller;
                        hunting = false;
                        continue;
                    }

                    // Take a Shot - Read API documentation at XXXXX
                    bool hit = battleship.Shoot(target.PositionX, target.PositionY);

                    if (hit && !hunting)
                    {
                        firingStrategy = new HunterKillerFiringStrategy(status.StateTeamA.TrackingGrid.Squares[target.PositionX, target.PositionY]);
                        hunting = true;
                    }
                    else
                    {
                        if (hunting && target.PositionX == -1)
                        {
                            firingStrategy = pingKiller;
                            hunting = false;
                        }
                    }

                    status = battleship.StatusAPI();

                    Console.WriteLine(status.ToString());

                    if (status.StateTeamA.Score == 17)
                    {
                        Console.WriteLine("All enemy vessels destroyed, a glorious celebration awaits");
                        break;
                    }

                    Console.WriteLine("{0} - Ship standing by for orders...", DateTime.Now.ToString());
                    await Task.Delay(500);
                }
            });

            t.Wait();

            Console.WriteLine("Game finished");
        }
    }
}
