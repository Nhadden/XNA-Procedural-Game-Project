#region Using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace ObserverGame
{
    public class UI : DrawableGameComponent
    {
        #region Fields
        int var_Current_Score;
        int var_Current_Time_Remaining;
        int var_Current_Health;
        int var_Current_Bombs;

        SpriteFont var_Game_Font;
        Texture2D var_UI_Texture;
        SpriteBatch var_SpriteBatch;
        #endregion

        #region Constructor
        public UI(ObserverGame game, SpriteBatch spriteBatchIn, int initialHealth, int initialBombs)
            : base(game)
        {
            var_SpriteBatch = spriteBatchIn;
            var_Current_Health = initialHealth;
            var_Current_Bombs = initialBombs;

            var_Game_Font = game.Content.Load<SpriteFont>("MotorwerkOblique");
            var_UI_Texture = game.Content.Load<Texture2D>("UiOverlay");

            game.Components.Add(this);
        } 
        #endregion

        #region Methods
        public override void Draw(GameTime gameTime)
        {
            if (var_Current_Health > 0)
            {
                DrawUIElements();
            } 
            else if(var_Current_Score > 10000) 
            {
                DrawGameoverWin();
            }
            else
            {
                DrawGameoverLose();
            }
            ResetFor3D();

            base.Draw(gameTime);
        }

        void DrawUIElements()
        {
            string var_Temp_Time = "Time: " + FormatTicksToSeconds(var_Current_Time_Remaining);
            string var_Temp_Health = "Health: " + var_Current_Health + "\nBombs: " + var_Current_Bombs;
            string var_Temp_Score = "Score: " + var_Current_Score;

            var_SpriteBatch.Begin();

            var_SpriteBatch.Draw(var_UI_Texture, Vector2.Zero, Color.White);

            var_SpriteBatch.DrawString(var_Game_Font, var_Temp_Time, CenterHorizontally(var_Temp_Time), Color.Gold);

            var_SpriteBatch.DrawString(var_Game_Font, var_Temp_Health, new Vector2(0, GraphicsDevice.Viewport.Height - var_Game_Font.MeasureString(var_Temp_Health).Y), Color.Firebrick);

            var_SpriteBatch.DrawString(var_Game_Font, var_Temp_Score, new Vector2(0, 0), Color.Ivory);

            var_SpriteBatch.End();
        }

        void DrawGameoverLose()
        {
            string output = "Game Over\nYou Scored " + var_Current_Score + " points";

            var_SpriteBatch.Begin();

            var_SpriteBatch.DrawString(var_Game_Font, output, Vector2.Add(CenterHorizontally(output), CenterVertically(output)), Color.Ivory);

            var_SpriteBatch.End();
        }

        void DrawGameoverWin() 
        {
            string output = "You Won! " + var_Current_Score + " points";

            var_SpriteBatch.Begin();

            var_SpriteBatch.DrawString(var_Game_Font, output, Vector2.Add(CenterHorizontally(output), CenterVertically(output)), Color.Ivory);

            var_SpriteBatch.End();
        }

        Vector2 CenterHorizontally(string output)
        {
            return new Vector2(GraphicsDevice.Viewport.Width / 2 - var_Game_Font.MeasureString(output).X / 2, 0);
        }

        Vector2 CenterVertically(string output)
        {
            return new Vector2(0, GraphicsDevice.Viewport.Height / 2 - var_Game_Font.MeasureString(output).Y / 2);
        }

        void ResetFor3D()
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        }

        double FormatTicksToSeconds(int time)
        {
            double x = time / 60;

            return Math.Round(x, 0);
        } 
        #endregion

        #region Event Handlers
        public void UpdateStats(StatEventArgs e)
        {
            var_Current_Health = e.CurrentHealth;
            var_Current_Bombs = e.CurrentBombs;
        }

        public void UpdateStatsDirector(DirectorStatEventArgs e)
        {
            var_Current_Score = e.Score;
            var_Current_Time_Remaining = e.TimeRemaining;
        } 
        #endregion

    }
}
