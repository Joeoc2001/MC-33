using MC_33;
using NUnit.Framework;
using System.Numerics;

namespace MC_33_Tests.CaseSpecificTests
{
    public class Case0GenTests
    {
        private Grid GenerateUniform(float value, int width)
        {
            float[,,] values = new float[width, width, width];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    for (int k = 0; k < width; k++)
                    {
                        values[i, j, k] = value;
                    }
                }
            }

            return new Grid(values, Vector3.Zero, Vector3.One);
        }

        private void UniformTest(float value, int width, float iso)
        {
            // ARRANGE
            Grid grid = GenerateUniform(value, width);

            // ACT
            Surface s = grid.GenerateSurface(iso);
            int[] triangles = s.GetTriangles();
            Vector3[] vertices = s.GetVertices();

            // ASSERT
            Assert.That(triangles, Has.Length.EqualTo(0));
            Assert.That(vertices, Has.Length.EqualTo(0));
        }

        [Test]
        public void GridGenerateSurface_1x1_Value1_Iso0_GenerateNothing()
        {
            UniformTest(1, 1, 0);
        }

        [Test]
        public void GridGenerateSurface_1x1_Value0_Iso0_GenerateNothing()
        {
            UniformTest(0, 1, 0);
        }

        [Test]
        public void GridGenerateSurface_1x1_ValueMinus1_Iso0_GenerateNothing()
        {
            UniformTest(-1, 1, 0);
        }

        [Test]
        public void GridGenerateSurface_1x1_Value1_Iso1_GenerateNothing()
        {
            UniformTest(1, 1, 1);
        }

        [Test]
        public void GridGenerateSurface_1x1_Value0_Iso1_GenerateNothing()
        {
            UniformTest(0, 1, 1);
        }

        [Test]
        public void GridGenerateSurface_1x1_ValueMinus1_Iso1_GenerateNothing()
        {
            UniformTest(-1, 1, 1);
        }

        [Test]
        public void GridGenerateSurface_1x1_Value1_IsoMinus1_GenerateNothing()
        {
            UniformTest(1, 1, -1);
        }

        [Test]
        public void GridGenerateSurface_1x1_Value0_IsoMinus1_GenerateNothing()
        {
            UniformTest(0, 1, -1);
        }

        [Test]
        public void GridGenerateSurface_1x1_ValueMinus1_IsoMinus1_GenerateNothing()
        {
            UniformTest(-1, 1, -1);
        }

        [Test]
        public void GridGenerateSurface_Cell_Value1_Iso0_GenerateNothing()
        {
            UniformTest(1, 2, 0);
        }

        [Test]
        public void GridGenerateSurface_Cell_Value0_Iso0_GenerateNothing()
        {
            UniformTest(0, 2, 0);
        }

        [Test]
        public void GridGenerateSurface_Cell_ValueMinus1_Iso0_GenerateNothing()
        {
            UniformTest(-1, 2, 0);
        }

        [Test]
        public void GridGenerateSurface_Cell_Value1_Iso1_GenerateNothing()
        {
            UniformTest(1, 2, 1);
        }

        [Test]
        public void GridGenerateSurface_Cell_Value0_Iso1_GenerateNothing()
        {
            UniformTest(0, 2, 1);
        }

        [Test]
        public void GridGenerateSurface_Cell_ValueMinus1_Iso1_GenerateNothing()
        {
            UniformTest(-1, 2, 1);
        }

        [Test]
        public void GridGenerateSurface_Cell_Value1_IsoMinus1_GenerateNothing()
        {
            UniformTest(1, 2, -1);
        }

        [Test]
        public void GridGenerateSurface_Cell_Value0_IsoMinus1_GenerateNothing()
        {
            UniformTest(0, 2, -1);
        }

        [Test]
        public void GridGenerateSurface_Cell_ValueMinus1_IsoMinus1_GenerateNothing()
        {
            UniformTest(-1, 2, -1);
        }

        [Test]
        public void GridGenerateSurface_2x2_Value1_Iso0_GenerateNothing()
        {
            UniformTest(1, 3, 0);
        }

        [Test]
        public void GridGenerateSurface_2x2_Value0_Iso0_GenerateNothing()
        {
            UniformTest(0, 3, 0);
        }

        [Test]
        public void GridGenerateSurface_2x2_ValueMinus1_Iso0_GenerateNothing()
        {
            UniformTest(-1, 3, 0);
        }

        [Test]
        public void GridGenerateSurface_2x2_Value1_Iso1_GenerateNothing()
        {
            UniformTest(1, 3, 1);
        }

        [Test]
        public void GridGenerateSurface_2x2_Value0_Iso1_GenerateNothing()
        {
            UniformTest(0, 3, 1);
        }

        [Test]
        public void GridGenerateSurface_2x2_ValueMinus1_Iso1_GenerateNothing()
        {
            UniformTest(-1, 3, 1);
        }

        [Test]
        public void GridGenerateSurface_2x2_Value1_IsoMinus1_GenerateNothing()
        {
            UniformTest(1, 3, -1);
        }

        [Test]
        public void GridGenerateSurface_2x2_Value0_IsoMinus1_GenerateNothing()
        {
            UniformTest(0, 3, -1);
        }

        [Test]
        public void GridGenerateSurface_2x2_ValueMinus1_IsoMinus1_GenerateNothing()
        {
            UniformTest(-1, 3, -1);
        }
    }
}