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
        [Test]
        public void CrashTests_NoBasicCell_Crashes_101([Range(-1, 1)] int a, [Range(-1, 1)] int b, [Range(-1, 1)] int c, [Range(-1, 1)] int d,
            [Range(-1, 1)] int e, [Range(-1, 1)] int f, [Range(-1, 1)] int g, [Range(-1, 1)] int h)
        {
            // ARRANGE
            float[,,] cells = new float[,,] {
                { { a, b }, { c, d } },
                { { e, f }, { g, h } }
            };

            Grid grid = new Grid(cells, Vector3.Zero, Vector3.One);

            // ACT & ASSERT
            Assert.DoesNotThrow(() => grid.GenerateSurface(0));
        }
    }
}
