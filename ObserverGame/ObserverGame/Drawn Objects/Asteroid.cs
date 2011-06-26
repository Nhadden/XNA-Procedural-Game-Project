#region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
#endregion

namespace ObserverGame
{
    #region AsteroidEvent
    public class AsteroidEventArgs : EventArgs
    {
        public AsteroidEventArgs(int scoreAwarded)
        {
            Score = scoreAwarded;
        }

        private int score;

        public int Score
        {
            get { return score; }
            set { score = value; }
        }
    }

    public delegate void AsteroidEventHandler(AsteroidEventArgs e);   
    #endregion

    public class Asteroid :WorldObject
    {
        #region Fields
        public event AsteroidEventHandler OnAsteroidDeath;
        #endregion

        #region Constructor

        public Asteroid(ObserverGame game, Vector3 spawnPoint, float velocity, int health) : base(game)
        {
            Random randomGenerator = new Random();

            var_Model = game.Content.Load<Model>(GetModel(health));

            var_Axis_Of_Rotation = Vector3.Normalize(new Vector3((float)randomGenerator.NextDouble(),
                                                                    (float)randomGenerator.NextDouble(),
                                                                    (float)randomGenerator.NextDouble()));
            var_Position = spawnPoint;

            var_Bounding_Region = new BoundingSphere(var_Position, 200.0f);

            var_Health = health;

            var_Velocity = velocity;

            Visible = false;

            var_HasBeenInCamera = false;

            var_DrawableObjects.Add(this);

            game.Components.Add(this);
        }

        private string GetModel(int health)
        {
            if (health <= 5) 
            {
                return "Model\\asteroidEasy";
            }
            else if (health <= 10) 
            { 
                return "Model\\asteroidNormal";
            }
            else if (health <= 15) 
            { 
                return "Model\\asteroidHard";
            }
            else 
            { 
                return "Model\\asteroidHardest";
            }
        }
        
        #endregion

        #region Methods
        public override void Update(GameTime gameTime)
        {
            CheckIsInCamera();
            MoveSelf();
            RotateSelf();
            UpdateBoundingSpherePositions();
            AttemptSelfDispose();
        }

        void MoveSelf()
        {
            var_Position.Z += var_Velocity;
        }

        void RotateSelf()
        {
            if (Visible)
            {
                var_Up = Vector3.Transform(var_Up, Matrix.CreateFromAxisAngle(var_Axis_Of_Rotation, 0.01f));
                var_Forward = Vector3.Transform(var_Forward, Matrix.CreateFromAxisAngle(var_Axis_Of_Rotation, 0.01f));
                var_Right = Vector3.Transform(var_Right, Matrix.CreateFromAxisAngle(var_Axis_Of_Rotation, 0.01f));
            }
        }

        internal override void DestroySelf()
        {
            var_Health -= 5;

            if (var_Health <= 0)
            {
                AsteroidEventArgs tempArgs = new AsteroidEventArgs(10);
                if (!(OnAsteroidDeath == null)) { OnAsteroidDeath(tempArgs); }

                DefaultDestructAction();
            }
        } 
        #endregion

    }//end class
}
