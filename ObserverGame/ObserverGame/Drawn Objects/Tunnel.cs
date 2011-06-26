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
    #region Tunnel Events
    public class TunnelEventArgs : EventArgs
    {
        public TunnelEventArgs(BoundingCircle circle)
        {
            Circle = circle;
        }

        private BoundingCircle circle;

        public BoundingCircle Circle
        {
            get { return circle; }
            set { circle = value; }
        }
        
    }//end class
    

    public delegate void CircleEventHandler(TunnelEventArgs e);

    #endregion

    public class Tunnel : WorldObject
    {
        #region Fields
        private BoundingCircle var_Tunnel_Wall;

        public event CircleEventHandler tunnelEvent; 
        #endregion

        #region Constuctor
        public Tunnel(ObserverGame game)
            : base(game)
        {
            var_Model = game.Content.Load<Model>("Model\\Tunnel");
            var_Position = new Vector3(0, 0, -8000);
            var_Bounding_Region = new BoundingSphere(var_Position, 1);
            var_Tunnel_Wall = new BoundingCircle(3500);
            var_DrawableObjects.Add(this);
            game.Components.Add(this);
        } 
        #endregion

        #region Methods
        public override void Update(GameTime gameTime)
        {
            PublishCircle();
        }

        private void PublishCircle()
        {
            if (!(tunnelEvent == null))
            {
                TunnelEventArgs tempArgs = new TunnelEventArgs(var_Tunnel_Wall);
                tunnelEvent(tempArgs);
            }
        }

        internal override void DestroySelf()
        {
            
        }
        #endregion

        #region Event Handler
        public void PlayerMoved(ShipEventArgs e)
        {
            var_Position.Z = e.Position.Z - 5000;
        } 
        #endregion

    }//end class
}
