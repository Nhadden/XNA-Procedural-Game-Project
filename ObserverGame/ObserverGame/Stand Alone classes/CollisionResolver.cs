#region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
#endregion

namespace ObserverGame
{
    public static class CollisionResolver
    {
        public static void ResolveCollision(WorldObject objectTriggering, WorldObject objectHit) 
        {
            if (objectTriggering is PlayerShip) 
            {
                if (objectHit is Asteroid) 
                {
                    objectTriggering.DestroySelf();
                    objectHit.DefaultDestructAction(); ;
                }
                if (objectHit is Ring) 
                {   
                    objectHit.DestroySelf();  
                }
            }

            if (objectTriggering is Laser) 
            {
                if (objectHit is Asteroid) { objectHit.DestroySelf(); objectTriggering.DestroySelf(); }
            }

            if (objectTriggering is Bomb) 
            {
                if (objectHit is Asteroid) { objectHit.DestroySelf(); }
            }

        }
    }//end class
}
