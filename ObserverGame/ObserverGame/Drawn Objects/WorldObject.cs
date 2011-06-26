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
    public abstract class WorldObject : DrawableGameComponent
    {

        #region Fields
        internal static List<WorldObject> var_DrawableObjects = new List<WorldObject>();

        public static List<WorldObject> DrawableObjects
        {
          get { return WorldObject.var_DrawableObjects; }
        }

        internal float          var_Velocity;
        internal int            var_Health;
        internal BoundingSphere var_Bounding_Region;
        internal Model          var_Model;
        internal Vector3        var_Position,
                                var_Axis_Of_Rotation,
                                var_Forward,
                                var_Up,
                                var_Right;

        internal BoundingFrustum var_Viewable_Area;

        internal bool var_HasBeenInCamera;
        #endregion

        public override void  Initialize()
        {
            var_Forward = Vector3.Forward;
            var_Up      = Vector3.Up;
            var_Right   = Vector3.Right;

 	        base.Initialize();
        }

        public WorldObject(Game game) : base(game)
        {

        }

        #region Methods
        public override void Update(GameTime gameTime) { }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        internal void CheckIsInCamera()
        {
            if (this.var_Bounding_Region.Intersects(var_Viewable_Area))
            {
                this.Visible = true;
            }
            else
            {
                this.Visible = false;
            }
        }

        internal void UpdateBoundingSpherePositions()
        {
            this.var_Bounding_Region.Center = this.var_Position;
        }

        internal void CheckForCollisions()
        {
            for (int i = var_DrawableObjects.Count - 1; i >= 0; i--)
            {
                if (var_Bounding_Region.Intersects(var_DrawableObjects[i].var_Bounding_Region))
                {
                    CollisionResolver.ResolveCollision(this, var_DrawableObjects[i]);
                }
            }
        }

        internal void AttemptSelfDispose()
        {
            if (Visible)
            {
                var_HasBeenInCamera = true;
            }

            if (var_HasBeenInCamera && !Visible)
            {
                DefaultDestructAction();
            }
        }

        internal void OnCollision() { }

        internal abstract void DestroySelf();

        internal void DefaultDestructAction()
        {
            var_DrawableObjects.Remove(this);
            Game.Components.Remove(this);
            Dispose();
        }

        public void UpdateFrustrum(FrustrumEventArgs e)
        {
            var_Viewable_Area = e.Var_Viewable_Area;
        } 
        #endregion

    }//end class
}
