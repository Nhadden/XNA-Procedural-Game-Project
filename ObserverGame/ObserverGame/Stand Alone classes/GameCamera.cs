#region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
#endregion

namespace ObserverGame
{
    #region Frustrum Event
    public class FrustrumEventArgs : EventArgs
    {
        public FrustrumEventArgs(BoundingFrustum var_Viewable_AreaIn)
        {
            Var_Viewable_Area = var_Viewable_AreaIn;
        }

        private BoundingFrustum var_Viewable_Area;

        public BoundingFrustum Var_Viewable_Area
        {
            get { return var_Viewable_Area; }
            set { var_Viewable_Area = value; }
        }
    }//end class

    public delegate void FrustrumUpdateEventHandler(FrustrumEventArgs e);
    #endregion

    public class GameCamera : GameComponent
    {
        #region Fields
        private Vector3 CONST_CAMERA_POSITION = new Vector3(0, -30, -400);
        private Matrix var_Camera_Details;
        private Matrix var_Projection_Details;
        private BoundingFrustum var_Viewable_Area;
        private Vector3 var_Camera_Target;
        private Vector3 var_Camera_Position;
        private Vector3 var_Up;
        private float var_Field_Of_View;
        private float var_Aspect_Ratio;
        private float var_Near_Plane;
        private float var_Far_Plane;
        public event FrustrumUpdateEventHandler frustrumEvent; 
        #endregion

        #region Mutators
        public Matrix CameraDetails { get { return var_Camera_Details; } }
        public Matrix ProjectionDetails { get { return var_Projection_Details; } }
        public BoundingFrustum ViewableArea { get { return var_Viewable_Area; } } 
        #endregion

        #region Constructor
        public GameCamera(ObserverGame game)
            : base(game)
        {
            var_Aspect_Ratio = game.GraphicsDevice.Viewport.AspectRatio;
            var_Near_Plane = 10.0f;
            var_Far_Plane = 8500.0f;
            var_Field_Of_View = 65.0f;
            var_Up = Vector3.Up;

            UpdateCamera();

            game.Components.Add(this);
        } 
        #endregion

        #region Methods
        public override void Update (GameTime gameTime)
        {
            UpdateCamera();
            PublishFrustrum();
        }

        private void UpdateCamera()
        {
            var_Camera_Details = Matrix.CreateLookAt(var_Camera_Position - CONST_CAMERA_POSITION, var_Camera_Target, var_Up);
            var_Projection_Details = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(var_Field_Of_View), var_Aspect_Ratio, var_Near_Plane, var_Far_Plane);
            var_Viewable_Area = new BoundingFrustum(var_Camera_Details * var_Projection_Details);
        }

        private void PublishFrustrum()
        {
            FrustrumEventArgs tempArgs = new FrustrumEventArgs(var_Viewable_Area);

            if (!(frustrumEvent == null)) 
            { 
                frustrumEvent(tempArgs); 
            }
        } 
        #endregion

        #region Event Handlers
        public void PlayerMoved(ShipEventArgs e)
        {
            var_Camera_Position = e.Position;
            var_Camera_Target = e.Position;
        }  
        #endregion
    }//end class
}
