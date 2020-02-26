using System;

namespace Ship
{
    class Program
    {
        static void Main(string[] args)
        {
            IBattleships battleships;

            IFiringStrategy firingStrategy = new CheckerboardFiringStrategy();

            IFleetDeployer fleetDeployer = new DefaultFleetOrders();
            
            if (Environment.GetEnvironmentVariable("debug") == null)
            {
                battleships = new BattleshipsImpl();
            }
            else
            {
                battleships = new BattleshipsMockImpl();
            }

            ShipEngine e = new ShipEngine(battleships, firingStrategy, fleetDeployer);
            e.Run();
        }
    }
}
