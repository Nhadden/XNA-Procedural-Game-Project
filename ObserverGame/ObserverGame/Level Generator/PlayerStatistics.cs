#region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
#endregion

namespace ObserverGame
{
    public class PlayerStatistics
    {
        #region Fields
        private int var_Damage_Taken;
        private int var_Score;
        private int var_Shots_Hit;
        private int var_Shots_Taken;
        private int var_Player_Movement;
        private int var_Sample_Time;
        #endregion

        #region Mutators
        public int Var_Damage_Taken
        {
            get { return var_Damage_Taken; }
            set { var_Damage_Taken = value; }
        }

        public int Var_Score
        {
            get { return var_Score; }
            set { var_Score = value; }
        }

        public int Var_Shots_Hit
        {
            get { return var_Shots_Hit; }
            set { var_Shots_Hit = value; }
        }

        public int Var_Shots_Taken
        {
            get { return var_Shots_Taken; }
            set { var_Shots_Taken = value; }
        }

        public int Var_Player_Movement
        {
            get { return var_Player_Movement; }
            set { var_Player_Movement = value; }
        }

        public int Var_Sample_Time
        {
            get { return var_Sample_Time; }
            set { var_Sample_Time = value; }
        }
        #endregion

        #region Constuctors
        public PlayerStatistics()
        {
            Var_Damage_Taken = 0;
            Var_Score = 0;
            Var_Shots_Hit = 0;
            Var_Shots_Taken = 0;
            Var_Player_Movement = 0;
            Var_Sample_Time = 0;
        }

        public PlayerStatistics(int damage_taken, int score, int shots_hit, int shots_taken, int player_movement, GameTime sample_Time)
        {
            Var_Damage_Taken = damage_taken;
            Var_Score = score;
            Var_Shots_Hit = shots_hit;
            Var_Shots_Taken = shots_taken;
            Var_Player_Movement = player_movement;
            Var_Sample_Time = sample_Time.TotalGameTime.Seconds;
        } 
        #endregion
    }//end class
}
