using System;

namespace Ship
{
    class Program
    {
        static void Main(string[] args)
        {
            IBattleships battleships;

            IFiringStrategy firingStrategy = new CheckerboardFiringStrategy();
            
            if (Environment.GetEnvironmentVariable("debug") == null)
            {
                battleships = new BattleshipsImpl();
            }
            else
            {
                battleships = new BattleshipsMockImpl();
            }

            ShipEngine e = new ShipEngine(battleships, firingStrategy);
            e.Run();
        }
    }
}
