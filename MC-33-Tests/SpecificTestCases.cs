using MC_33;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MC_33_Tests
{
    class SpecificTestCases
    {
        /// <summary>
        /// Tests a specific example of a cell. Assumes that delta in the grid is 1, origin is 0 and isovalue is 0
        /// </summary>
        /// <param name="cells">The grid to march from</param>
        /// <param name="expected">The surface that is expected to be generated</param>
        private bool TestSpecific(float[,,] cells, Surface expected)
        {
            Grid grid = new Grid(cells, Vector3.Zero, Vector3.One);
            Surface s = grid.GenerateSurface(0);
            return Surface.AreSurfaceShapesEqual(expected, s);
        }

        [Test]
        public void GridGenerateSurface_TwoAdjacentInternal_GeneratesCorrectSurface()
        {
            // ARRANGE
            float[,,] cells = new float[,,] {
                { {  1,  1 }, { -1, -1 } },
                { { -1, -1 }, { -1, -1 } }
            };

            // Two valid surfaces
            Surface expected1 = new Surface();
            expected1.AddVertex(new Vector3(0, 0.5f, 0));
            expected1.AddVertex(new Vector3(0, 0, 0.5f));
            expected1.AddVertex(new Vector3(1, 0.5f, 0));
            expected1.AddVertex(new Vector3(1, 0, 0.5f));
            expected1.AddTriangle(0, 1, 2);
            expected1.AddTriangle(2, 1, 3);

            Surface expected2 = new Surface();
            expected2.AddVertex(new Vector3(0, 0.5f, 0));
            expected2.AddVertex(new Vector3(0, 0, 0.5f));
            expected2.AddVertex(new Vector3(1, 0.5f, 0));
            expected2.AddVertex(new Vector3(1, 0, 0.5f));
            expected2.AddTriangle(0, 1, 3);
            expected2.AddTriangle(3, 2, 0);

            // ACT & ASSERT
            bool areEqual = TestSpecific(cells, expected1) || TestSpecific(cells, expected2);
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void GridGenerateSurface_TwoAdjacentExternal_GeneratesCorrectSurface()
        {
            // ARRANGE
            float[,,] cells = new float[,,] {
                { { -1, -1 }, { 1, 1 } },
                { { 1, 1 }, { 1, 1 } }
            };

            // Two valid surfaces
            Surface expected1 = new Surface();
            expected1.AddVertex(new Vector3(0, 0.5f, 0));
            expected1.AddVertex(new Vector3(0, 0, 0.5f));
            expected1.AddVertex(new Vector3(1, 0.5f, 0));
            expected1.AddVertex(new Vector3(1, 0, 0.5f));
            expected1.AddTriangle(0, 2, 1);
            expected1.AddTriangle(2, 3, 1);

            Surface expected2 = new Surface();
            expected2.AddVertex(new Vector3(0, 0.5f, 0));
            expected2.AddVertex(new Vector3(0, 0, 0.5f));
            expected2.AddVertex(new Vector3(1, 0.5f, 0));
            expected2.AddVertex(new Vector3(1, 0, 0.5f));
            expected2.AddTriangle(0, 3, 1);
            expected2.AddTriangle(3, 0, 2);

            // ACT & ASSERT
            bool areEqual = TestSpecific(cells, expected1) || TestSpecific(cells, expected2);
            Assert.IsTrue(areEqual);
        }
    }
}
