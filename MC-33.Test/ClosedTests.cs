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
        private void TestClosed(int a, int b, int c, int d, int e, int f, int g, int h)
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

        [Test]
        public void ClosedTests_3x3GridClosed_101([Range(-1, 1)] int a, [Range(-1, 1)] int b, [Range(-1, 1)] int c, [Range(-1, 1)] int d,
            [Range(-1, 1)] int e, [Range(-1, 1)] int f, [Range(-1, 1)] int g, [Range(-1, 1)] int h)
        {
            TestClosed(a, b, c, d, e, f, g, h);
        }

        [Test]
        public void ClosedTests_3x3GridClosed_102([Values(-1, 0, 2)] int a, [Values(-1, 0, 2)] int b, [Values(-1, 0, 2)] int c, [Values(-1, 0, 2)] int d,
            [Values(-1, 0, 2)] int e, [Values(-1, 0, 2)] int f, [Values(-1, 0, 2)] int g, [Values(-1, 0, 2)] int h)
        {
            TestClosed(a, b, c, d, e, f, g, h);
        }

        [Test]
        public void ClosedTests_3x3GridClosed_201([Values(-2, 0, 1)] int a, [Values(-2, 0, 1)] int b, [Values(-2, 0, 1)] int c, [Values(-2, 0, 1)] int d,
            [Values(-2, 0, 1)] int e, [Values(-2, 0, 1)] int f, [Values(-2, 0, 1)] int g, [Values(-2, 0, 1)] int h)
        {
            TestClosed(a, b, c, d, e, f, g, h);
        }
    }
}
