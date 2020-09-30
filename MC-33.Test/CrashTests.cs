using MC_33;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MC_33_Tests
{
    class CrashTests
    {
        private void TestDoesntCrash(int a, int b, int c, int d, int e, int f, int g, int h)
        {
            // ARRANGE
            float[,,] cells = new float[,,] {
                { { a, b }, { c, d } },
                { { e, f }, { g, h } }
            };

            ArrayGrid grid = new ArrayGrid(cells, Vector3.Zero, Vector3.One);
            ListSurface s = new ListSurface();

            // ACT & ASSERT
            Assert.DoesNotThrow(() => MarchingCubes.MarchIntoSurface(grid, 0, s));
        }

        [Test]
        public void CrashTests_NoBasicCell_Crashes_101([Range(-1, 1)] int a, [Range(-1, 1)] int b, [Range(-1, 1)] int c, [Range(-1, 1)] int d,
            [Range(-1, 1)] int e, [Range(-1, 1)] int f, [Range(-1, 1)] int g, [Range(-1, 1)] int h)
        {
            TestDoesntCrash(a, b, c, d, e, f, g, h);
        }

        [Test]
        public void CrashTests_NoBasicCell_Crashes_012([Range(0, 2)] int a, [Range(0, 2)] int b, [Range(0, 2)] int c, [Range(0, 2)] int d,
            [Range(0, 2)] int e, [Range(0, 2)] int f, [Range(0, 2)] int g, [Range(0, 2)] int h)
        {
            TestDoesntCrash(a, b, c, d, e, f, g, h);
        }

        [Test]
        public void CrashTests_NoBasicCell_Crashes_102([Values(-1, 0, 2)] int a, [Values(-1, 0, 2)] int b, [Values(-1, 0, 2)] int c, [Values(-1, 0, 2)] int d,
            [Values(-1, 0, 2)] int e, [Values(-1, 0, 2)] int f, [Values(-1, 0, 2)] int g, [Values(-1, 0, 2)] int h)
        {
            TestDoesntCrash(a, b, c, d, e, f, g, h);
        }
    }
}
