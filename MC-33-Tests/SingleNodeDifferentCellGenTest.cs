using MC_33;
using NUnit.Framework;
using System.Numerics;

namespace MC_33_Tests
{
    public class SingleNodeDifferentCellGenTest
    {
        private Grid GenerateSingle(float valueSingle, float valueOther, int cellVertex)
        {
            float[,,] values = new float[2, 2, 2];
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        values[i, j, k] = valueOther;
                    }
                }
            }

            int x = cellVertex & 0x01;
            int y = (cellVertex & 0x02) >> 1;
            int z = (cellVertex & 0x04) >> 2;
            values[z, y, x] = valueSingle;

            return new Grid(values, Vector3.Zero, Vector3.One);
        }

        private void SingleNodeCellTest(float valueSingle, float valueOther, int cellVertex, float iso)
        {
            // ARRANGE
            Grid grid = GenerateSingle(valueSingle, valueOther, cellVertex);

            // ACT
            Surface s = grid.GenerateSurface(iso);
            int[] triangles = s.GetTriangles();
            Vector3[] vertices = s.GetVertices();
            Vector3[] normals = s.GetNormals();

            // ASSERT
            // Should have one triangle with three vertices
            Assert.That(triangles, Has.Length.EqualTo(3)); 
            Assert.That(vertices, Has.Length.EqualTo(3));
            Assert.That(normals, Has.Length.EqualTo(3));
        }

        [Test]
        public void GridGenerateSurface_SingleValue1_OtherValueMinus1_Iso0_GeneratesSingleTriangle()
        {
            for (int i = 0; i < 8; i++)
            {
                SingleNodeCellTest(1, -1, i, 0);
            }
        }

        [Test]
        public void GridGenerateSurface_SingleValue2_OtherValueMinus1_Iso0_GeneratesSingleTriangle()
        {
            for (int i = 0; i < 8; i++)
            {
                SingleNodeCellTest(2, -1, i, 0);
            }
        }

        [Test]
        public void GridGenerateSurface_SingleValue2_OtherValueMinus1_Iso1_GeneratesSingleTriangle()
        {
            for (int i = 0; i < 8; i++)
            {
                SingleNodeCellTest(2, -1, i, 1);
            }
        }

        [Test]
        public void GridGenerateSurface_SingleValueMinus1_OtherValue1_Iso0_GeneratesSingleTriangle()
        {
            for (int i = 0; i < 8; i++)
            {
                SingleNodeCellTest(-1, 1, i, 0);
            }
        }

        [Test]
        public void GridGenerateSurface_SingleValueMinus2_OtherValue1_Iso0_GeneratesSingleTriangle()
        {
            for (int i = 0; i < 8; i++)
            {
                SingleNodeCellTest(-2, 1, i, 0);
            }
        }

        [Test]
        public void GridGenerateSurface_SingleValueMinus2_OtherValue1_IsoMinus1_GeneratesSingleTriangle()
        {
            for (int i = 0; i < 8; i++)
            {
                SingleNodeCellTest(-2, 1, i, -1);
            }
        }

        [Test]
        public void GridGenerateSurface_SingleValue1_OtherValueMinus1_Iso0_GeneratesCorrectTriangle()
        {
            Vector3 norm = new Vector3(1, 0, 0);

            // ARRANGE
            Grid grid = GenerateSingle(1, -1, 0);
            Surface expected = new Surface();
            expected.AddVertex(new Vector3(0, 0.5f, 0), norm);
            expected.AddVertex(new Vector3(0.5f, 0, 0), norm);
            expected.AddVertex(new Vector3(0, 0, 0.5f), norm);
            expected.AddTriangle(0, 1, 2);

            // ACT
            Surface s = grid.GenerateSurface(0);
            bool areEqual = Surface.AreSurfaceShapesEqual(expected, s);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void GridGenerateSurface_SingleValue1_OtherValueMinus1_Iso0_DoesntGenerateIncorrectTriangle()
        {
            Vector3 norm = new Vector3(1, 0, 0);

            // ARRANGE
            Grid grid = GenerateSingle(1, -1, 0);
            Surface expected = new Surface();
            expected.AddVertex(new Vector3(0.5f, 0, 0), norm);
            expected.AddVertex(new Vector3(0, 0.5f, 0), norm);
            expected.AddVertex(new Vector3(0, 0, 0.5f), norm);
            expected.AddTriangle(0, 1, 2);

            // ACT
            Surface s = grid.GenerateSurface(0);
            bool areEqual = Surface.AreSurfaceShapesEqual(expected, s);

            // ASSERT
            Assert.IsFalse(areEqual);
        }
    }
}