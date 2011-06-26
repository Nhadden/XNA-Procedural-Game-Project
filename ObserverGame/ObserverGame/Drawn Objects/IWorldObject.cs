using System;
namespace ObserverGame.Drawn_Objects
{
    interface IWorldObject
    {
        void Draw(global::Microsoft.Xna.Framework.GameTime gameTime);

        void Initialize();

        void Update(global::Microsoft.Xna.Framework.GameTime gameTime);

        void UpdateFrustrum(global::ObserverGame.FrustrumEventArgs e);
    }
}
