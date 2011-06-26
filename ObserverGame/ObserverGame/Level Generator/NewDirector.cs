#region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
#endregion

namespace ObserverGame
{
    public enum ChallengeLevel { VeryEasy = 2, Easy = 3, Normal = 4, Hard = 5, VeryHard = 6 };

    #region Direction Event



    public class DirectionEventArgs : EventArgs
    {
        public DirectionEventArgs(float speed, float durability,float fieldDensity, float lethality)
        {
            Var_Asteroid_Speed_Adjustment = speed;
            Var_Asteroid_Durability_Adjustment = durability;
            Var_Asteroid_Field_Density_Adjustment = fieldDensity;
            Var_Asteroid_Lethality_Adjustment = lethality;
        }

        private float var_Asteroid_Speed_Adjustment;
        private float var_Asteroid_Durability_Adjustment;
        private float var_Asteroid_Field_Density_Adjustment;
        private float var_Asteroid_Lethality_Adjustment;

        public float Var_Asteroid_Speed_Adjustment
        {
            get { return var_Asteroid_Speed_Adjustment; }
            set { var_Asteroid_Speed_Adjustment = value; }
        }

        public float Var_Asteroid_Durability_Adjustment
        {
            get { return var_Asteroid_Durability_Adjustment; }
            set { var_Asteroid_Durability_Adjustment = value; }
        }

        public float Var_Asteroid_Field_Density_Adjustment
        {
            get { return var_Asteroid_Field_Density_Adjustment; }
            set { var_Asteroid_Field_Density_Adjustment = value; }
        }
        
        public float Var_Asteroid_Lethality_Adjustment
        {
            get { return var_Asteroid_Lethality_Adjustment; }
            set { var_Asteroid_Lethality_Adjustment = value; }
        }
        
    }//end class

    public delegate void DirectionEventHandler(DirectionEventArgs e);

    public class DirectorStatEventArgs : EventArgs 
    {
        public DirectorStatEventArgs(int score, int timeRemaining)
            {
                Score = score;
                TimeRemaining = timeRemaining;
            }

        private int score;

        public int Score
        {
            get { return score; }
            set { score = value; }
        }
        private int timeRemaining;

        public int TimeRemaining
        {
            get { return timeRemaining; }
            set { timeRemaining = value; }
        }
    }

    public delegate void DirectorStatEventHandler(DirectorStatEventArgs e);

    #endregion

    public class NewDirector : GameComponent
    {
        #region Fields
        public event DirectionEventHandler OnNewDirections;
        public event DirectorStatEventHandler OnNewStats;

        const int TIME_BETWEEN_SAMPLES = 600;
        static int DIFFICULTY_DIVISOR = 4;
        int var_Game_Time_Remaining;
        int var_Last_Sample;
        int var_Current_Score;

        PlayerStatistics var_Current_Player_Details;
        List<PlayerStatistics> var_Previous_Player_Details;
        ChallengeLevel var_Initial_Difficulty;
        GameTime var_CurrentTime;
        #endregion

        #region Constructor
        public NewDirector(Game game, ChallengeLevel initial_Difficulty)
            : base(game)
        {
            var_Last_Sample = TIME_BETWEEN_SAMPLES;

            var_Previous_Player_Details = new List<PlayerStatistics>();

            var_Current_Player_Details = new PlayerStatistics();

            var_Initial_Difficulty = initial_Difficulty;

            var_Game_Time_Remaining = (100 * (10 - ((int)initial_Difficulty / DIFFICULTY_DIVISOR)));
        
            game.Components.Add(this);
        } 

        #endregion

        #region Methods
        public override void Update(GameTime gameTime)
        {
            if (var_Last_Sample <= 0)
            {
                var_CurrentTime = gameTime;

                MakeSegmentGeneratorModifiers();

                AppendAndClearCurrentStatistics();

                var_Last_Sample = TIME_BETWEEN_SAMPLES;
            }
            else
            {
                var_Last_Sample--;
            }

            var_Game_Time_Remaining--;

            var_Current_Player_Details.Var_Sample_Time = gameTime.TotalGameTime.Seconds;

            PublishDirectorStats();
        }

        private void MakeSegmentGeneratorModifiers()
        {
            if (var_Previous_Player_Details.Count > 1)
            {
                float var_Temp_Asteroid_Speed = 0.0f;
                float var_Temp_Asteroid_Durability = 0.0f;
                float var_Temp_Asteroid_Field_Density = 0.0f;
                float var_Temp_Asteroid_Lethality = 0.0f;

                var_Temp_Asteroid_Speed         = DirectorHelpers.getNewSpeed(var_Previous_Player_Details.Last(),var_Current_Player_Details);
                var_Temp_Asteroid_Durability    = DirectorHelpers.getNewDurability(var_Previous_Player_Details.Last(), var_Current_Player_Details);
                var_Temp_Asteroid_Field_Density = DirectorHelpers.getNewFieldDensity(var_Previous_Player_Details.Last(), var_Current_Player_Details);
                var_Temp_Asteroid_Lethality     = DirectorHelpers.getNewLethality(var_Previous_Player_Details.Last(), var_Current_Player_Details);

                Console.WriteLine("\nSpeed Adjustment: {0}%\nDurability Adjustment: {1}%\nDensity Adjustment: {2}%\nLethality Adjustment: {3}%", var_Temp_Asteroid_Speed, var_Temp_Asteroid_Durability, var_Temp_Asteroid_Field_Density, var_Temp_Asteroid_Lethality);

                PublishGeneratorDirections(var_Temp_Asteroid_Speed, var_Temp_Asteroid_Durability, var_Temp_Asteroid_Field_Density, var_Temp_Asteroid_Lethality);
            }
            else
            {
                var_Previous_Player_Details.Add(var_Current_Player_Details);
            }
        }

        private void AppendAndClearCurrentStatistics()
        {
            var_Previous_Player_Details.Add(var_Current_Player_Details);
            var_Current_Player_Details = new PlayerStatistics();
        }

        private void PublishGeneratorDirections(float var_Temp_Asteroid_Speed, float var_Temp_Asteroid_Durability, float var_Temp_Asteroid_Field_Density, float var_Temp_Asteroid_Lethality)
        {
            if (!(OnNewDirections == null))
            {
                DirectionEventArgs temp = new DirectionEventArgs(var_Temp_Asteroid_Speed, var_Temp_Asteroid_Durability, var_Temp_Asteroid_Field_Density, var_Temp_Asteroid_Lethality);
                OnNewDirections(temp);
            }
        }       

        private void PublishDirectorStats()
        {
            if (!(OnNewStats == null))
            {
                DirectorStatEventArgs tempArgs = new DirectorStatEventArgs(var_Current_Score, var_Game_Time_Remaining);
                OnNewStats(tempArgs);
            }
        }

        #endregion

        #region Event Handlers
        public void UpdateStats(StatEventArgs e)
        {
            var_Current_Player_Details.Var_Shots_Hit += e.LaserHit;
            var_Current_Player_Details.Var_Shots_Taken += e.LaserShot;
            var_Current_Player_Details.Var_Player_Movement += e.Moved;
            var_Current_Player_Details.Var_Damage_Taken += e.HealthLost;
        }

        public void UpdateStats(AsteroidEventArgs e)
        {
            var_Current_Player_Details.Var_Score = e.Score;
            var_Current_Score += e.Score;
        }

        public void UpdateStats(RingEventArgs e)
        {
            var_Current_Player_Details.Var_Score += 100;
            var_Current_Score += 100;
            var_Game_Time_Remaining += e.Time;
        } 
        #endregion
    }//end class
}
