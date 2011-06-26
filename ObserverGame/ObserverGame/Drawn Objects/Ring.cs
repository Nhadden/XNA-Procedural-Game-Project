#region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Audio;
#endregion

namespace ObserverGame
{
    #region RingEvent
    public class RingEventArgs : EventArgs
    {
        public RingEventArgs(int timeAwarded)
        {
            Time = timeAwarded;
        }

        private int time;

        public int Time
        {
            get { return time; }
            set { time = value; }
        }
    }

    public delegate void RingEventHandler(RingEventArgs e);
    #endregion

    public class Ring : WorldObject
    {
        #region Fields
        internal SoundEffect sfx_Ring_Collission;
        public event RingEventHandler OnRingHit; 
        #endregion

        #region Constuctor
        public Ring(ObserverGame game, Vector3 spawnPoint)
            : base(game)
        {
            var_Model = game.Content.Load<Model>("Model\\ringGold");
            var_Position = spawnPoint;
            var_Bounding_Region = new BoundingSphere(var_Position, 200.0f);
            sfx_Ring_Collission = game.Content.Load<SoundEffect>("SFX\\start");

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

            AttemptSelfDispose();
        }

        internal override void DestroySelf()
        {
            if (!(OnRingHit == null))
            {
                RingEventArgs tempArgs = new RingEventArgs(150);
                OnRingHit(tempArgs);
            }
            sfx_Ring_Collission.Play();
            DefaultDestructAction();
        } 
        #endregion
    }//end class
}
