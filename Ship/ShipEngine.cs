using System;
using System.Collections.Generic;
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

        private readonly IFleetDeployer fleetDeployer;
       
        public ShipEngine(IBattleships battleship, IFiringStrategy firingStrategy, IFleetDeployer fleetDeployer)
        {
            this.battleship = battleship;

            this.pingKiller = firingStrategy;
            this.firingStrategy = pingKiller;

            this.fleetDeployer = fleetDeployer;
        }

        public void Run()
        {
            bool hunting = false;

            List<ShipModel.Ship> ships = fleetDeployer.getOrders();

            foreach (ShipModel.Ship ship in ships)
            {
                battleship.PlaceShip(JsonConvert.SerializeObject(ship));
            }

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
