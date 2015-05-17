using System;

namespace MindHelper
{
    public class Randomizer
    {
        private static Random _Random;

        /// <summary>
        /// Class Constructor:
        /// </summary>
        static Randomizer()
        {
            _Random = new Random();
        }

        /// <summary>
        /// Gets a Random non negative Number from 0 - int.max.
        /// </summary>
        /// <returns>a Random number within specified Range.</returns>
        public static int GetRandomNumber()
        {
            return _Random.Next();
        }

        /// <summary>
        /// Gets a Random non negative Number from 0 - specified Max.
        /// </summary>
        /// <param name="Max">Max Number to be randomize.</param>
        /// <returns>a Random number within specified Range.</returns>
        public static int GetRandomNumber(int Max)
        {
            return _Random.Next(Max);
        }

        /// <summary>
        /// Gets a Random non negative Number from specified Min - specified Max.
        /// </summary>
        /// <param name="Min">Min Number to be randomize.</param>
        /// <param name="Max">>Max Number to be randomize.</param>
        /// <returns>a Random number within specified Range.</returns>
        public static int GetRandomNumber(int Min, int Max)
        {
            return _Random.Next(Min, Max);
        }

    }
}
