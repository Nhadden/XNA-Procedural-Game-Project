#region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.GamerServices;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Media;
    using Nuclex.Input;
#endregion

namespace ObserverGame
{
    public class ObserverGame : Microsoft.Xna.Framework.Game
    {
        #region Fields
        public static ObserverGame var_Game;

        GraphicsDeviceManager graphics;
        SpriteBatch var_SpriteBatch;
        PlayerShip var_PlayerShip;
        GameCamera var_GameCamera;
        NewDirector var_Director;
        SegmentGenerator var_SegmentGenerator;
        Tunnel var_Tunnel;
        UI var_UI;
        InputHandler var_InputHandler;
        #endregion

        #region Constructor
        public ObserverGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        } 
        #endregion

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            var_Game = this;

            var_InputHandler = new InputHandler(this);
            var_SpriteBatch = new SpriteBatch(GraphicsDevice);

            var_GameCamera = new GameCamera(this);
            var_PlayerShip = new PlayerShip(this, Vector3.Zero, var_GameCamera);
            var_Director = new NewDirector(this, ChallengeLevel.Normal);
            var_SegmentGenerator = new SegmentGenerator(var_Game, ChallengeLevel.Normal, var_GameCamera, var_Director );
            var_Tunnel = new Tunnel(this);
            var_UI = new UI(this, var_SpriteBatch, var_PlayerShip.var_Health, var_PlayerShip.var_Bomb_Count);

            AssignSubscribers();

            base.Initialize();
        }

        void AssignSubscribers()
        {
            var_Director.OnNewDirections += var_SegmentGenerator.UpdateStatsDirector;
            var_Director.OnNewStats += var_UI.UpdateStatsDirector;

            var_InputHandler.OnDpadInput += var_PlayerShip.Move;
            var_InputHandler.OnButtonInput += var_PlayerShip.Button;

            var_PlayerShip.OnShipMovement += var_GameCamera.PlayerMoved;
            var_PlayerShip.OnShipMovement += var_SegmentGenerator.PlayerMoved;
            var_PlayerShip.OnShipMovement += var_Tunnel.PlayerMoved;

            var_PlayerShip.OnUpdate += var_Director.UpdateStats;
            var_PlayerShip.OnUpdate += var_UI.UpdateStats;

            var_Tunnel.tunnelEvent += var_SegmentGenerator.TunnelUpdate;
            var_Tunnel.tunnelEvent += var_PlayerShip.TunnelUpdate;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            foreach (WorldObject Drawable in WorldObject.DrawableObjects)
            {
                Matrix[] transforms = new Matrix[Drawable.var_Model.Bones.Count];

                //BoundingSphereRenderer.Render(Drawable.var_Bounding_Region, GraphicsDevice, camera.CameraDetails, camera.ProjectionDetails, Color.Red);

                Drawable.var_Model.CopyAbsoluteBoneTransformsTo(transforms);

                foreach (ModelMesh mesh in Drawable.var_Model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateWorld(Drawable.var_Position, Drawable.var_Forward, Drawable.var_Up);

                        effect.View = var_GameCamera.CameraDetails;
                        effect.Projection = var_GameCamera.ProjectionDetails;
                    }

                    mesh.Draw();
                }
            }
            base.Draw(gameTime);
        }

    }//end class
}
