#region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using Nuclex.Input;
#endregion

namespace ObserverGame
{
    #region Movement Event

    public class MovementEventArgs : EventArgs 
    {
        public MovementEventArgs(int i) 
        {
            Movement = i;
        }

        private int movement;

        public int Movement
        {
            get { return movement; }
            set { movement = value; }
        }

        
    }//end class

    public delegate void InputEventHandler(MovementEventArgs e);

    #endregion

    #region Button Event

    public class ButtonEventArgs : EventArgs 
    { 
        public ButtonEventArgs(Buttons button)
        {
            Button = button;
        }

        private Buttons button;

        public Buttons Button
        {
            get { return button; }
            set { button = value; }
        }

        
    }//end class

    public delegate void ButtonEventHandler(ButtonEventArgs e);

    #endregion

    public class InputHandler : GameComponent
    {

        #region Fields
        public InputManager var_Input;

        public event InputEventHandler OnDpadInput;
        public event ButtonEventHandler OnButtonInput; 
        #endregion

        #region Constuctor
        public InputHandler(ObserverGame game)
            : base(game)
        {
            var_Input = new InputManager();
            game.Components.Add(var_Input);
            game.Components.Add(this);
        } 
        #endregion

        #region Methods
        public override void Update(GameTime gameTime) 
        {
            GetDXGamepadInput();

            GetKeyboardInput();
        }

        void GetKeyboardInput()
        {
            KeyboardState keys = new KeyboardState();

            keys = Keyboard.GetState();

            if (keys.IsKeyDown(Keys.W))
            {
                PublishMovementEvent(3);
            }
            else if (keys.IsKeyDown(Keys.S))
            {
                PublishMovementEvent(2);
            }
            if (keys.IsKeyDown(Keys.A))
            {
                PublishMovementEvent(0);
            }
            else if (keys.IsKeyDown(Keys.D))
            {
                PublishMovementEvent(1);
            }
            if (keys.IsKeyDown(Keys.Space)) { PublishButtonEvent(Buttons.A); }
            if (keys.IsKeyDown(Keys.LeftControl)) { PublishButtonEvent(Buttons.B); }
            if (keys.IsKeyDown(Keys.Up)) { PublishButtonEvent(Buttons.X); }
        }

        void GetDXGamepadInput()
        {
            GamePadState state = var_Input.GetGamePad(Nuclex.Input.ExtendedPlayerIndex.Five).GetState();

            GetDpadInput(state);

            GetButtonInput(state);
        }

        void GetDpadInput(GamePadState state)
        {
            if (state.ThumbSticks.Left.X < -0.2)//left
            {
                PublishMovementEvent(0);
            }
            else if (state.ThumbSticks.Left.X > 0.2)//right
            {
                PublishMovementEvent(1);
            }

            if (state.ThumbSticks.Left.Y > 0.2)//down
            {
                PublishMovementEvent(2);
            }
            else if (state.ThumbSticks.Left.Y < -0.2)//up
            {
                PublishMovementEvent(3);
            }
        }

        void GetButtonInput(GamePadState state)
        {
            if (state.Buttons.A == ButtonState.Pressed) { PublishButtonEvent(Buttons.A); }
            if (state.Buttons.B == ButtonState.Pressed) { PublishButtonEvent(Buttons.B); }
            if (state.Buttons.X == ButtonState.Pressed) { PublishButtonEvent(Buttons.X); }
            if (state.Buttons.Y == ButtonState.Pressed) { PublishButtonEvent(Buttons.Y); }
        }

        void PublishMovementEvent(int movement)
        {
            if (!(OnDpadInput == null))
            {
                MovementEventArgs tempArgs = new MovementEventArgs(movement);
                OnDpadInput(tempArgs);
            }
        }

        void PublishButtonEvent(Buttons button)
        {
            if (!(OnButtonInput == null))
            {
                ButtonEventArgs tempArgs = new ButtonEventArgs(button);
                OnButtonInput(tempArgs);
            }
        } 
        #endregion
    }//end class
}
