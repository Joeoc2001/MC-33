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

            // Two valid surfaces
            Surface expected1 = new ListSurface();
            expected1.AddVertex(new Vector3(0, 0, 0.5f));
            expected1.AddVertex(new Vector3(1, 0, 0.5f));
            expected1.AddVertex(new Vector3(1, 1, 0.5f));
            expected1.AddVertex(new Vector3(0, 1, 0.5f));
            expected1.AddTriangle(0, 1, 2);
            expected1.AddTriangle(0, 2, 3);

            Surface expected2 = new ListSurface();
            expected2.AddVertex(new Vector3(0, 1, 0.5f));
            expected2.AddVertex(new Vector3(0, 0, 0.5f));
            expected2.AddVertex(new Vector3(1, 0, 0.5f));
            expected2.AddVertex(new Vector3(1, 1, 0.5f));
            expected2.AddTriangle(0, 1, 2);
            expected2.AddTriangle(0, 2, 3);

            // ACT & ASSERT
            bool areEqual = SpecificTests.TestSpecific(cells, expected1) || SpecificTests.TestSpecific(cells, expected2);
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

            // Two valid surfaces
            Surface expected1 = new ListSurface();
            expected1.AddVertex(new Vector3(0, 0, 0.5f));
            expected1.AddVertex(new Vector3(1, 0, 0.5f));
            expected1.AddVertex(new Vector3(1, 1, 0.5f));
            expected1.AddVertex(new Vector3(0, 1, 0.5f));
            expected1.AddTriangle(0, 2, 1);
            expected1.AddTriangle(0, 3, 2);

            Surface expected2 = new ListSurface();
            expected2.AddVertex(new Vector3(0, 1, 0.5f));
            expected2.AddVertex(new Vector3(0, 0, 0.5f));
            expected2.AddVertex(new Vector3(1, 0, 0.5f));
            expected2.AddVertex(new Vector3(1, 1, 0.5f));
            expected2.AddTriangle(0, 2, 1);
            expected2.AddTriangle(0, 3, 2);

            // ACT & ASSERT
            bool areEqual = SpecificTests.TestSpecific(cells, expected1) || SpecificTests.TestSpecific(cells, expected2);
            Assert.IsTrue(areEqual);
        }
    }
}
