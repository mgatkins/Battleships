using System;
using System.Text;

namespace ShipModel
{
    [Serializable]
    public class MasterGameState
    {
        public PlayerState StateTeamA { get; set; }
        public PlayerState StateTeamB { get; set; }

        public MasterGameState()
        {
            StateTeamA = new PlayerState();
            StateTeamB = new PlayerState();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Game State: ");
            builder.AppendLine("Shots / Score ").Append(StateTeamB.ShotsYouHaveFired).Append(" / ").Append(StateTeamB.Score);

            builder.AppendLine();

            for (int y = 9; y >= 0; y--)
            {
                for (int x = 0; x < 10; x++)
                {
                    if (StateTeamB.TrackingGrid.Squares[x, y].HitPeg)
                    {
                        builder.Append("X ");
                    }
                    else if (StateTeamB.TrackingGrid.Squares[x, y].Marked)
                    {
                        builder.Append("0 ");
                    }
                    else
                    {
                        builder.Append("  ");
                    }
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }
    }

    public class PlayerState
    {
        public Nullable<bool> WinStatus { get; set; }
        public int Score { get; set; }
        public int ShotsYouHaveFired { get; set; }
        public int TotalHitsAgainstYou { get; set; }
        public PlacementGrid PlacementGrid { get; set; }
        public TrackingGrid TrackingGrid { get; set; }
        public Fleet Fleet { get; set; }
        //public int ShootThreashold { get; set; }

        public PlayerState()
        {
            PlacementGrid = new PlacementGrid();
            TrackingGrid = new TrackingGrid();
            Fleet = new Fleet();
            Score = 0;
            ShotsYouHaveFired = 0;
            TotalHitsAgainstYou = 0;
            WinStatus = null;
            //ShootThreashold = 0;
        }
    }
}
