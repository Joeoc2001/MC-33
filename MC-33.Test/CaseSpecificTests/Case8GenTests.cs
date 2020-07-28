using MC_33;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MC_33_Tests.CaseSpecificTests
{
    class Case8GenTests
    {
        [Test]
        public void Case8_0123_GeneratesCorrectSurface()
        {
            // ARRANGE
            float[,,] cells = new float[,,] {
                { {  1, -1 }, {  1, -1 } },
                { {  1, -1 }, {  1, -1 } }
            };

            // ACT
            List<Surface> expected = SpecificTests.GenAllTriangulations(new List<Vector3>() {
                new Vector3(0, 0, 0.5f),
                new Vector3(1, 0, 0.5f),
                new Vector3(1, 1, 0.5f),
                new Vector3(0, 1, 0.5f),
            });
            bool areEqual = SpecificTests.TestSpecific(cells, expected);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Case8_4567_GeneratesCorrectSurface()
        {
            // ARRANGE
            float[,,] cells = new float[,,] {
                { { -1,  1 }, { -1,  1 } },
                { { -1,  1 }, { -1,  1 } }
            };

            // ACT
            List<Surface> expected = SpecificTests.GenAllTriangulations(new List<Vector3>() {
                new Vector3(0, 1, 0.5f),
                new Vector3(1, 1, 0.5f),
                new Vector3(1, 0, 0.5f),
                new Vector3(0, 0, 0.5f),
            });
            bool areEqual = SpecificTests.TestSpecific(cells, expected);

            // ASSERT
            Assert.IsTrue(areEqual);
        }
    }
}
