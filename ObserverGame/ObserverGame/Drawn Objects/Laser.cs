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
    #region Laser Event
    public class LaserEventArgs : EventArgs
    {
        public LaserEventArgs(bool hit)
        {
            Hit = hit;
        }

        private bool hit;

        public bool Hit
        {
            get { return hit; }
            set { hit = value; }
        }
    }

    public delegate void LaserEventHandler(LaserEventArgs e);
    #endregion

    public class Laser : WorldObject
    {

        #region Fields
        public event LaserEventHandler OnLaserHit; 
        #endregion

        #region Constructor
        public Laser(ObserverGame game, Vector3 spawnPoint) : base(game)
        {
            var_Model = game.Content.Load<Model>("Model\\laser");

            var_Position = spawnPoint;

            var_Bounding_Region = new BoundingSphere(var_Position, 20);

            Visible = false;
            var_HasBeenInCamera = false;

            var_DrawableObjects.Add(this);
            game.Components.Add(this);
        } 
        #endregion

        #region Methods
        public override void Update(GameTime gameTime)
        {
            CheckIsInCamera();
            MoveSelf();
            UpdateBoundingSpherePositions();
            CheckForCollisions();
            AttemptSelfDispose();
        }

        void MoveSelf()
        {
            this.var_Position.Z -= 100;
        }

        internal override void DestroySelf()
        {
            LaserEventArgs tempArgs = new LaserEventArgs(true);
            OnLaserHit(tempArgs);

            DefaultDestructAction();
        } 
        #endregion
    }//end class
}
