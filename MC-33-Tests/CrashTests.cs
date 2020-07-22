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
        private const int r = 1;

        [Test]
        public void CrashTests_NoBasicCell_Crashes([Range(-r, r)] int a, [Range(-r, r)] int b, [Range(-r, r)] int c, [Range(-r, r)] int d,
            [Range(-r, r)] int e, [Range(-r, r)] int f, [Range(-r, r)] int g, [Range(-r, r)] int h)
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
