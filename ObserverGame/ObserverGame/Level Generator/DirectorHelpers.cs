using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObserverGame
{
    public class DirectorHelpers
    {
        public static float getNewSpeed(PlayerStatistics previous, PlayerStatistics current)//Working 17/04 bug was ship not sending laser fired events.
        {
            float fired = (float)current.Var_Shots_Taken;
            float hit = (float)current.Var_Shots_Hit;

            float previousFired = (float)previous.Var_Shots_Taken;
            float previousHit = (float)previous.Var_Shots_Taken;

            if ((fired == 0) || (hit == 0) || (previousFired == 0) || (previousHit == 0)) 
            {
                return 0;
            }

            float difference = CalculateMagnitudeOfDifference(CalculateAccuracy(fired,hit),CalculateAccuracy(previousFired,previousHit));

            return difference;
        }

        public static float getNewDurability(PlayerStatistics previous, PlayerStatistics current)
        {
            float score = current.Var_Score;
            float time = current.Var_Sample_Time;

            float previousScore = previous.Var_Score;
            float previousTime = previous.Var_Sample_Time;

            if ((score == 0) || (previousScore == 0))
            {
                return 0;
            }

            float var_Temp_Previous_Score_Per_Second = CalculateAmountPerSecond(previousScore, previousTime);
            float var_Temp_Current_Score_Per_Second = CalculateAmountPerSecond(score, time);

            return CalculateMagnitudeOfDifference(var_Temp_Current_Score_Per_Second, var_Temp_Previous_Score_Per_Second);
        }

        public static float getNewFieldDensity(PlayerStatistics previous, PlayerStatistics current)
        {
            int var_Temp_Previous_Movement = previous.Var_Player_Movement;
            int var_Temp_Current_Movement = current.Var_Player_Movement;

            float movement = current.Var_Player_Movement;
            float previousMovement = previous.Var_Player_Movement;
            
            if ((movement == 0) || (previousMovement == 0))
            {
                return 0;
            }

            return CalculateMagnitudeOfDifference(movement, previousMovement);
        }

        public static float getNewLethality(PlayerStatistics previous, PlayerStatistics current)
        {
            float damage = current.Var_Damage_Taken;
            float previousDamage = previous.Var_Damage_Taken;

            if ((damage == 0) || (previousDamage == 0))
            {
                return 0;
            }

            return CalculateMagnitudeOfDifference(damage, previousDamage);
        }

        private static float CalculateAccuracy(float shotsfired, float shotshit)
        {
            if (shotsfired > 0)
            {
                float shotsHit = shotshit;
                float shotsTaken = shotsfired;

                float accuracy = shotsHit / shotsTaken * 100;

                return accuracy;
            }
            else
            {
                return 50;
            }
        }

        private static float CalculateMagnitudeOfDifference(float a, float b)
        {
            float x = a / b;

            return x * 100;
        }

        private static float CalculateAmountPerSecond(float stat, float time)
        {
            if (stat != 0)
            {
                return stat / time;
            }
            else
            {
                return 0;
            }
        }
    }
}
