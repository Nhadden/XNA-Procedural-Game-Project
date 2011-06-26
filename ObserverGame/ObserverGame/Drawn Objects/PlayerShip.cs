#region Using Statements
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Audio;
#endregion

namespace ObserverGame
{
    #region Ship Events
    public class ShipEventArgs : EventArgs
    {
        public ShipEventArgs(Vector3 position)
        {
            Position = position;
        }

        private Vector3 position;
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
    }

    public delegate void ShipEventHandler(ShipEventArgs e);

    public class StatEventArgs : EventArgs 
    {
        public StatEventArgs(int moved, int laserShot, int laserHit, int healthLost, int currentHealth, int currentBombs) 
        {
            Moved = moved;
            LaserShot = laserShot;
            LaserHit = laserHit;
            HealthLost = healthLost;
            CurrentHealth = currentHealth;
            CurrentBombs = currentBombs;
        }

        private int moved;
        public int Moved
        {
            get { return moved; }
            set { moved = value; }
        }

        private int laserShot;
        public int LaserShot
        {
            get { return laserShot; }
            set { laserShot = value; }
        }

        private int laserHit;
        public int LaserHit
        {
            get { return laserHit; }
            set { laserHit = value; }
        }

        private int healthLost;
        public int HealthLost
        {
            get { return healthLost; }
            set { healthLost = value; }
        }

        private int currentHealth;

        public int CurrentHealth
        {
            get { return currentHealth; }
            set { currentHealth = value; }
        }

        private int currentBombs;

        public int CurrentBombs
        {
            get { return currentBombs; }
            set { currentBombs = value; }
        }
    }

    public delegate void StatEventHandler(StatEventArgs e);

    #endregion

    public class PlayerShip : WorldObject
    {
        #region Fields
        public event ShipEventHandler OnShipMovement;
        public event StatEventHandler OnUpdate;

        internal const int   CONST_MAX_VELOCITY = 40;
        internal const int   CONST_MIN_VELOCITY = 1;
        internal const int   CONST_COOLDOWN_TIME = 20;
        internal const int   CONST_STARTING_BOMBS = 3;
        internal const int   CONST_STARTING_HITPOINTS = 10;
        internal const float CONST_DECELERATION_RATE = 0.25f;
        internal const float CONST_ACCELERATION_RATE = 0.50f;

        internal BoundingCircle var_Bounding_Circle; 

        internal int var_Cooldown_Timer;
        internal int var_Bomb_Count;

        private GameCamera camera;

        internal bool var_Accellerate;

        internal Vector3 var_Previous_Position;

        internal SoundEffect sfxSingleLaserShot;
        internal SoundEffect sfxBreak;
        internal SoundEffect sfxBoost;
        internal SoundEffect sfxHitObstacle;
        internal SoundEffect sfxBombFired;

        internal List<BoundingSphere> var_Ship_Bounding_Regions;
        #endregion

        #region Constuctor
        public PlayerShip(ObserverGame game, Vector3 spawnPoint, GameCamera cameraIn) : base(game)
        {
            var_Model = game.Content.Load<Model>("Model\\StarFighter");

            sfxBoost = game.Content.Load<SoundEffect>("SFX\\boost");
            sfxBreak = game.Content.Load<SoundEffect>("SFX\\brake");
            sfxHitObstacle = game.Content.Load<SoundEffect>("SFX\\arwingHitObstacle");
            sfxSingleLaserShot = game.Content.Load<SoundEffect>("SFX\\arwingSingleLaserOneShot");
            sfxBombFired = game.Content.Load<SoundEffect>("SFX\\bombFireAndExplode");

            var_Position = spawnPoint;
            var_Previous_Position = var_Position;
            var_Velocity = 0;
            var_Axis_Of_Rotation = Vector3.Zero;
            var_Cooldown_Timer = 0;
            var_Bomb_Count = CONST_STARTING_BOMBS;
            var_Health = CONST_STARTING_HITPOINTS;
            var_Accellerate = false;

            var_Bounding_Circle = new BoundingCircle(3500);

            //var_Bounding_Region = new BoundingSphere(var_Position, 250);

            //var_Bounding_Region = new BoundingSphere(var_Position , 30);

            var_Bounding_Region = new BoundingSphere(new Vector3(var_Position.X, var_Position.Y, var_Position.Z - 560), 50);

            var_Ship_Bounding_Regions = new List<BoundingSphere>();
            //var_Ship_Bounding_Regions.Add(new BoundingSphere(var_Position, 30));

            camera = cameraIn;

            var_DrawableObjects.Add(this);

            game.Components.Add(this);
        } 
        #endregion

        #region Methods
        public override void Update(GameTime gameTime)
        {
            var_Cooldown_Timer--;

            MoveShip();

            AdjustVelocity();

            UpdateBoundingSpherePositions();

            CheckForCollisions();

            PublishShipEvent();
        }

        void CheckCollisionX()
        {
            foreach (WorldObject o in var_DrawableObjects)
            {
                if (o.Visible)
                {
                    if (o.var_Bounding_Region.Intersects(var_Bounding_Region))
                    {
                        foreach (BoundingSphere b in var_Ship_Bounding_Regions)
                        {
                            if (b.Intersects(o.var_Bounding_Region))
                            {
                                //collision has occured with outer sphere
                            }
                        }
                    }
                }
            }
        }

        void UpdateBoundingSphereX()
        {
            base.UpdateBoundingSpherePositions();
        }

        void PublishShipEvent()
        {
            ShipEventArgs tempShipArgs = new ShipEventArgs(var_Position);
            OnShipMovement(tempShipArgs);
            PublishStateEvent(0, 0, 0, 0);
        }

        void MoveShip()
        {
            if (var_Velocity > CONST_MIN_VELOCITY)
            {
                this.var_Position.Z -= var_Velocity;
            }
            else
            {
                this.var_Position.Z -= CONST_MIN_VELOCITY;
            }
        }

        bool WithinBoundry()
        {
            return var_Bounding_Circle.Intersects(var_Position);
        }

        void UndoMovement()
        {
            var_Position.X = var_Previous_Position.X;
            var_Position.Y = var_Previous_Position.Y;
        }

        #region Input Methods

        void MoveLeft()
        {
            var_Position.X -= CONST_MIN_VELOCITY + var_Velocity;
        }

        void MoveRight()
        {
            var_Position.X += CONST_MIN_VELOCITY + var_Velocity;
        }

        void MoveUp()
        {
            var_Position.Y += CONST_MIN_VELOCITY + var_Velocity;
        }

        void MoveDown()
        {
            var_Position.Y -= CONST_MIN_VELOCITY + var_Velocity;
        }

        void ShootLaser()
        {
            Laser temp = new Laser(ObserverGame.var_Game, var_Position);
            sfxSingleLaserShot.Play();
            camera.frustrumEvent += temp.UpdateFrustrum;
            temp.OnLaserHit += this.LaserHandler;
            var_Cooldown_Timer = CONST_COOLDOWN_TIME;
            PublishStateEvent(0, 1, 0, 0);
        }

        void ShootBomb()
        {
            Bomb temp = new Bomb(ObserverGame.var_Game, var_Position);
            sfxBombFired.Play();
            var_Bomb_Count--;
            var_Cooldown_Timer = CONST_COOLDOWN_TIME;
        }

        void AdjustVelocity()
        {
            if (var_Accellerate)
            {
                if (var_Velocity <= CONST_MAX_VELOCITY)
                {
                    var_Velocity += CONST_ACCELERATION_RATE;
                }
            }
            else
            {
                if (var_Velocity >= CONST_MIN_VELOCITY)
                {
                    var_Velocity -= CONST_DECELERATION_RATE;
                }
            }

            var_Accellerate = false;
        }

        #endregion

        public void DecreaseHealth(int var_Amount_Lost)
        {
            var_Health -= var_Amount_Lost;

            PublishStateEvent(0, 0, 0, var_Amount_Lost);
        }

        void PublishStateEvent(int moved, int laserShot, int laserHit, int healthLost)
        {
            if (!(OnUpdate == null))
            {
                StatEventArgs tempArgs = new StatEventArgs(moved, laserShot, laserHit, healthLost, var_Health, var_Bomb_Count);
                OnUpdate(tempArgs);
            }
        }

        internal override void DestroySelf()
        {
            DecreaseHealth(1);
            sfxHitObstacle.Play();

            if (var_Health <= 0)
            {
                DefaultDestructAction();
            }
        } 
        #endregion

        #region Event Handlers
        public void Move(MovementEventArgs e)
        {
            var_Previous_Position = var_Position;

            if (e.Movement == 0)
            { MoveLeft(); }
            if (e.Movement == 1)
            { MoveRight(); }
            if (e.Movement == 2)
            { MoveDown(); }
            if (e.Movement == 3)
            { MoveUp(); }

            if (!WithinBoundry())
            {
                UndoMovement();
            }

            PublishStateEvent(1, 0, 0, 0);
        }

        public void Button(ButtonEventArgs e)
        {
            if (e.Button == Buttons.A)
            {
                if (var_Cooldown_Timer <= 0)
                {
                    ShootLaser();
                }
            }

            if (e.Button == Buttons.B)
            {
                if ((var_Cooldown_Timer <= 0) && (var_Bomb_Count > 0))
                {
                    ShootBomb();
                }
            }

            if (e.Button == Buttons.X)
            {
                var_Accellerate = true;
            }

            if (e.Button == Buttons.Y) { }
        }

        public void LaserHandler(LaserEventArgs e)
        {
            PublishStateEvent(0, 0, 1, 0);
        }

        public void TunnelUpdate(TunnelEventArgs e)
        {
            var_Bounding_Circle = e.Circle;
        } 
        #endregion
    }//end class
}
