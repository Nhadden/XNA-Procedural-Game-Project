#region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
#endregion

namespace ObserverGame
{
    public class SegmentGenerator : GameComponent
    {
        #region Fields
        const int CONST_SPACE_BETWEEN_OBJECTS = 400;
        const int CONST_RING_INTERVAL = 10;
        const int CONST_ASTEROID_SPEED_MULTIPLIER = 2;
        const int CONST_ASTEROID_DURABILITY_MULTIPLIER = 5;

        float   var_Asteroid_Speed,
                var_Asteroid_Durability,
                var_Asteroid_Field_Density,
                var_Asteroid_Lethality,
                var_Previous_Player_Z_Position;

        int var_Frames_Generated,
            var_Total_Frames,
            var_Next_Ring;

        Random var_Randomiser;
        BoundingCircle var_BoundingCircle;
        GameCamera var_Camera;
        Vector3 var_Player_Position;
        NewDirector var_Director;
        #endregion

        #region Mutators

            public float Var_Asteroid_Lethality
            {
                set { var_Asteroid_Lethality = DifficultyLimiter(value); }
            }

            public float Var_Asteroid_Field_Density
            {
                set { var_Asteroid_Field_Density = DifficultyLimiter(value); }
            }

            public float Var_Asteroid_Durability
            {
                set { var_Asteroid_Durability = DifficultyLimiter(value); }
            }

            public float Var_Asteroid_Speed
            {
                set { var_Asteroid_Speed = DifficultyLimiter(value); }
            }

        #endregion

        #region Constructor
        public SegmentGenerator(ObserverGame game, ChallengeLevel initialDifficulty, GameCamera cameraIn, NewDirector directorIn) : base(game)
        {
            var_Asteroid_Speed = (float)initialDifficulty / 4;
            var_Asteroid_Durability = (float)initialDifficulty / 4;
            var_Asteroid_Field_Density = (float)initialDifficulty / 4;
            var_Asteroid_Lethality = (float)initialDifficulty / 4;

            var_Randomiser = new Random();

            var_Player_Position = Vector3.Zero;
            var_BoundingCircle = new BoundingCircle(3500);
            var_Camera = cameraIn;

            var_Total_Frames = 20;
            var_Frames_Generated = 0;
            var_Next_Ring = 0;
            var_Director = directorIn;

            //DEBUG_SetupFile();

            game.Components.Add(this);
        } 
        #endregion

        #region Methods
        public override void Update(GameTime gameTime)
        {
            CheckSpawnNewFrame();
            GenerateFrames();
        }

        private void CheckSpawnNewFrame()
        {
            if (var_Player_Position.Z <= var_Previous_Player_Z_Position - CONST_SPACE_BETWEEN_OBJECTS)
            {
                var_Total_Frames++;
                var_Previous_Player_Z_Position -= CONST_SPACE_BETWEEN_OBJECTS;

                //DEBUG_publishFields();
            }
        }

        private void GenerateFrames()
        {
            while (var_Frames_Generated != var_Total_Frames)
            {
                GenerateNextFrame(new Vector3(-4000, -4000, -(CONST_SPACE_BETWEEN_OBJECTS * var_Total_Frames)), 4000, 4000);
                var_Frames_Generated++;
            }
        }

        private void GenerateNextFrame(Vector3 startPoint, float endPointX, float endPointY)
        {
            while (startPoint.Y <= endPointY)
            {
                GenerateRow(startPoint, endPointX);

                startPoint.Y += CONST_SPACE_BETWEEN_OBJECTS;
            }
            var_Next_Ring--;
        }

        private void GenerateRow(Vector3 startPoint, float endPointX)
        {
            while (startPoint.X <= endPointX)
            {
                if (var_BoundingCircle.Intersects(startPoint))
                {
                    if ((var_Randomiser.Next(0, 101) <= var_Asteroid_Field_Density * 2))
                    {
                        startPoint = CreateElementAt(startPoint);
                    }
                }
                startPoint.X += CONST_SPACE_BETWEEN_OBJECTS;
            } 
        }

        private Vector3 CreateElementAt(Vector3 startPoint)
        {
            if ((var_Randomiser.Next(0, 101) <= 50) && (var_Next_Ring < 0))
            {
                Ring temp = new Ring(ObserverGame.var_Game, startPoint);
                var_Camera.frustrumEvent += temp.UpdateFrustrum;
                temp.OnRingHit += var_Director.UpdateStats;

                var_Next_Ring = 10;
            }
            else
            {
                Asteroid temp = new Asteroid(ObserverGame.var_Game, GenerateNoise(startPoint), var_Asteroid_Speed * CONST_ASTEROID_SPEED_MULTIPLIER, (int)(var_Asteroid_Durability * CONST_ASTEROID_DURABILITY_MULTIPLIER));
                var_Camera.frustrumEvent += temp.UpdateFrustrum;
                temp.OnAsteroidDeath += var_Director.UpdateStats;
            }
            return startPoint;
        }

        private Vector3 GenerateNoise(Vector3 point)
        {
            Vector3 noisyPoint;

            noisyPoint.X = point.X + var_Randomiser.Next(-100, 100);
            noisyPoint.Y = point.Y + var_Randomiser.Next(-100, 100);
            noisyPoint.Z = point.Z + var_Randomiser.Next(-100, 100);

            return noisyPoint;
        }

        private float DifficultyLimiter(float numberIn)
        {
            if (numberIn > 6.0)
            {
                return 6.0f;
            }
            else if (numberIn < 0.0001f)
            {
                return 0.25f;
            }
            else
            {
                return numberIn;
            }
        }

        float IncreaseStatByPercentage(float stat, float percentage)
        {
            if ((percentage != 0) && (percentage != float.NaN))
            {
                float x = percentage / 100;

                float y = stat * x;

                return y;
            }
            else 
            {
                return stat;
            }
        } 
        #endregion

        #region Event Handlers
        public void UpdateStatsDirector(DirectionEventArgs e)
        {
            Var_Asteroid_Speed          = IncreaseStatByPercentage(var_Asteroid_Speed, e.Var_Asteroid_Speed_Adjustment);
            Var_Asteroid_Field_Density  = IncreaseStatByPercentage(var_Asteroid_Field_Density, e.Var_Asteroid_Field_Density_Adjustment);
            Var_Asteroid_Durability     = IncreaseStatByPercentage(var_Asteroid_Durability, e.Var_Asteroid_Durability_Adjustment);
            Var_Asteroid_Lethality      = IncreaseStatByPercentage(var_Asteroid_Lethality, e.Var_Asteroid_Lethality_Adjustment);
        }

        public void PlayerMoved(ShipEventArgs e)
        {
            var_Player_Position = e.Position;
        }

        public void TunnelUpdate(TunnelEventArgs e)
        {
            var_BoundingCircle = e.Circle;
        } 



        #endregion

        #region DebugCode

        private void DEBUG_SetupFile() 
        {
            string initial = "Test File for Observer Game";
            System.IO.File.WriteAllText(@"D:\WriteLines.txt", initial);
        }

        private void DEBUG_publishFields()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"D:\WriteLines.txt", true))
            {
                file.WriteLine("{0},{1},{2},{3},{4},{5},{6}",
                var_Asteroid_Speed,
                var_Asteroid_Durability,
                var_Asteroid_Field_Density,
                var_Asteroid_Lethality,
                var_Previous_Player_Z_Position,
                var_Frames_Generated,
                var_Total_Frames);
            }  
        }

        #endregion
        

    }
}
