using MC_33;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MC_33_Tests.CaseSpecificTests
{
    class Case5GenTests
    {
        [Test]
        public void Case5_013_GeneratesCorrectSurface()
        {
            // ARRANGE
            float[,,] cells = new float[,,] {
                { {  1, -1 }, { -1, -1 } },
                { {  1, -1 }, {  1, -1 } }
            };

            // Two valid surfaces
            Surface expected1 = new ListSurface();
            expected1.AddVertex(new Vector3(0, 0.5f, 0));
            expected1.AddVertex(new Vector3(0.5f, 1, 0));
            expected1.AddVertex(new Vector3(0, 0, 0.5f));
            expected1.AddVertex(new Vector3(1, 1, 0.5f));
            expected1.AddVertex(new Vector3(1, 0, 0.5f));
            expected1.AddTriangle(2, 1, 0);
            expected1.AddTriangle(2, 3, 1);
            expected1.AddTriangle(4, 3, 2);

            // ACT & ASSERT
            bool areEqual = SpecificTests.TestSpecific(cells, expected1);
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Case5_24567_GeneratesCorrectSurface()
        {
            // ARRANGE
            float[,,] cells = new float[,,] {
                { { -1,  1 }, {  1,  1 } },
                { { -1,  1 }, { -1,  1 } }
            };

            // Two valid surfaces
            Surface expected1 = new ListSurface();
            expected1.AddVertex(new Vector3(0, 0.5f, 0));
            expected1.AddVertex(new Vector3(0.5f, 1, 0));
            expected1.AddVertex(new Vector3(0, 0, 0.5f));
            expected1.AddVertex(new Vector3(1, 1, 0.5f));
            expected1.AddVertex(new Vector3(1, 0, 0.5f));
            expected1.AddTriangle(2, 0, 1);
            expected1.AddTriangle(2, 1, 3);
            expected1.AddTriangle(4, 2, 3);

            // ACT & ASSERT
            bool areEqual = SpecificTests.TestSpecific(cells, expected1);
            Assert.IsTrue(areEqual);
        }
    }
}
