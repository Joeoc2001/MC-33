using MC_33;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MC_33_Tests
{
    class ClosedTests
    {
        [Test]
        public void ClosedTests_NoBasicCell_Crashes_1012([Range(-1, 1)] int a, [Range(-1, 1)] int b, [Range(-1, 1)] int c, [Range(-1, 1)] int d,
            [Range(-1, 1)] int e, [Range(-1, 1)] int f, [Range(-1, 1)] int g, [Range(-1, 1)] int h)
        {
            // ARRANGE
            float[,,] cells = new float[,,] {
                { { -1, -1, -1, -1 }, { -1, -1, -1, -1 }, { -1, -1, -1, -1 }, { -1, -1, -1, -1 } },
                { { -1, -1, -1, -1 }, { -1,  a,  b, -1 }, { -1,  c,  d, -1 }, { -1, -1, -1, -1 } },
                { { -2, -2, -2, -2 }, { -2,  e,  f, -2 }, { -2,  g,  h, -2 }, { -2, -2, -2, -2 } },
                { { -2, -2, -2, -2 }, { -2, -2, -2, -2 }, { -2, -2, -2, -2 }, { -2, -2, -2, -2 } },
            };

            // ACT
            ArrayGrid grid = new ArrayGrid(cells, Vector3.Zero, Vector3.One);
            Surface s = grid.GenerateSurface(0);
            bool isClosed = s.IsClosed();

            // ASSERT
            Assert.IsTrue(isClosed);
        }
    }
}
