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
    public class Bomb : WorldObject
    {
        #region Fields
        internal const int CONST_EXPLOSION_RADIUS = 2500;
        internal int var_Explosion_Expansion_Countdown;
        #endregion

        #region Constructor
        public Bomb(ObserverGame game, Vector3 spawnPoint) : base(game)
        {
            var_Model = game.Content.Load<Model>("Model\\Bomb");

            var_Position = spawnPoint;

            var_Bounding_Region = new BoundingSphere(var_Position, 40);

            var_Explosion_Expansion_Countdown = 60;

            var_DrawableObjects.Add(this);
            game.Components.Add(this);
        } 
        #endregion

        #region Methods
        public override void Update(GameTime gameTime)
        {
            ExplodeOrMove();

            UpdateBoundingSpherePositions();

            CheckForCollisions();

            var_Explosion_Expansion_Countdown--;
        }

        void ExplodeOrMove()
        {
            if (var_Explosion_Expansion_Countdown > 0)
            {
                MoveSelf();
            }
            else
            {
                ExpandExplosion();
            }
        }

        void ExpandExplosion()
        {
            if (var_Bounding_Region.Radius < CONST_EXPLOSION_RADIUS)
            {
                var_Bounding_Region.Radius += var_Bounding_Region.Radius;
            }
            else
            {
                this.DestroySelf();
            }
        }

        void MoveSelf()
        {
            this.var_Position.Z -= 100;
        }

        internal override void DestroySelf()
        {
            DefaultDestructAction();
        } 
        #endregion

    }//end class
}

