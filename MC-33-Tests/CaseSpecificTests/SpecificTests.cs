using MC_33;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MC_33_Tests.CaseSpecificTests
{
    static class SpecificTests
    {
        /// <summary>
        /// Tests a specific example of a cell. Assumes that delta in the grid is 1, origin is 0 and isovalue is 0
        /// </summary>
        /// <param name="cells">The grid to march from</param>
        /// <param name="expected">The surface that is expected to be generated</param>
        public static bool TestSpecific(float[,,] cells, Surface expected)
        {
            Grid grid = new Grid(cells, Vector3.Zero, Vector3.One);
            Surface s = grid.GenerateSurface(0);
            return Surface.AreSurfacesEquivalent(expected, s);
        }
    }
}
