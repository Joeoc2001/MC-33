using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MC_33
{
    internal static class VectorAlgorithms
    {
        /// <summary>
        /// Checks if two vectors are approximately equal, within some bound epsilon
        /// </summary>
        /// <param name="v1">The first vector</param>
        /// <param name="v2">The second vector</param>
        /// <param name="epsilon">The bound to compare to</param>
        /// <returns>True if the squared magnitude of the difference between the two vectors is less than epsilon</returns>
        public static bool EqualsApproximately(this Vector3 v1, Vector3 v2, double epsilon)
        {
            return (v1 - v2).LengthSquared() < epsilon;
        }
    }
}
