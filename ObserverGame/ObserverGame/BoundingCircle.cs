#region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Xna.Framework;
#endregion

namespace ObserverGame
{
    public class BoundingCircle
    {

        #region Fields
        private double var_Radius;
        private Vector3 var_Position;
        #endregion

        #region Constructors
        public BoundingCircle(double radius)
        {
            this.var_Radius = radius;
        }

        public BoundingCircle(double radius, Vector3 spawnPoint)
        {
            this.var_Radius = radius;
            this.var_Position = spawnPoint;
        } 
        #endregion

        #region Methods
        /// <summary>
        /// adapted from the code found via this link http://stackoverflow.com/questions/481144/how-do-you-test-if-a-point-is-inside-a-circle/481151#481151
        /// </summary>
        /// <param name="positionTested">A Vector3 Value to be tested.</param>
        /// <returns></returns>
        public bool Intersects(Vector3 positionTested)
        {
            double length = Math.Sqrt(Math.Pow(var_Position.X - positionTested.X, 2) + Math.Pow(var_Position.Y - positionTested.Y, 2));

            return length <= this.var_Radius;
        } 
        #endregion

    }//end class
}
