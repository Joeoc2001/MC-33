using MC_33;
using NUnit.Framework;
using System.Numerics;

namespace MC_33_Tests.CaseSpecificTests
{
    public class Case1GenTests
    {
        private ArrayGrid GenerateSingle(float valueSingle, float valueOther, int cellVertex)
        {
            float[,,] values = new float[2, 2, 2];
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        values[k, j, i] = valueOther;
                    }
                }
            }

            int x = cellVertex & 0x01;
            int y = (cellVertex & 0x02) >> 1;
            int z = (cellVertex & 0x04) >> 2;
            values[z, y, x] = valueSingle;

            return new ArrayGrid(values, Vector3.Zero, Vector3.One);
        }

        private void SingleNodeCellTest(float valueSingle, float valueOther, int cellVertex, float iso)
        {
            // ARRANGE
            ArrayGrid grid = GenerateSingle(valueSingle, valueOther, cellVertex);

            // ACT
            ListSurface s = new ListSurface();
            MarchingCubes.MarchIntoSurface(grid, iso, s);
            int[] triangles = s.GetTriangles();
            Vector3[] vertices = s.GetVertices();

            // ASSERT
            // Should have one triangle with three vertices
            Assert.That(triangles, Has.Length.EqualTo(3)); 
            Assert.That(vertices, Has.Length.EqualTo(3));
        }

        [Test]
        public void GridGenerateSurface_SingleValue1_OtherValueMinus1_Iso0_GeneratesSingleTriangle([Range(0, 7)] int i)
        {
            SingleNodeCellTest(1, -1, i, 0);
        }

        [Test]
        public void GridGenerateSurface_SingleValue2_OtherValueMinus1_Iso0_GeneratesSingleTriangle([Range(0, 7)] int i)
        {
            SingleNodeCellTest(2, -1, i, 0);
        }

        [Test]
        public void GridGenerateSurface_SingleValue2_OtherValueMinus1_Iso1_GeneratesSingleTriangle([Range(0, 7)] int i)
        {
            SingleNodeCellTest(2, -1, i, 1);
        }

        [Test]
        public void GridGenerateSurface_SingleValueMinus1_OtherValue1_Iso0_GeneratesSingleTriangle([Range(0, 7)] int i)
        {
            SingleNodeCellTest(-1, 1, i, 0);
        }

        [Test]
        public void GridGenerateSurface_SingleValueMinus2_OtherValue1_Iso0_GeneratesSingleTriangle([Range(0, 7)] int i)
        {
            SingleNodeCellTest(-2, 1, i, 0);
        }

        [Test]
        public void GridGenerateSurface_SingleValueMinus2_OtherValue1_IsoMinus1_GeneratesSingleTriangle([Range(0, 7)] int i)
        {
            SingleNodeCellTest(-2, 1, i, -1);
        }

        [Test]
        public void GridGenerateSurface_SingleValue1_OtherValueMinus1_Iso0_GeneratesCorrectTriangle()
        {
            // ARRANGE
            ArrayGrid grid = GenerateSingle(1, -1, 0);
            ListSurface expected = new ListSurface();
            expected.AddVertex(new Vector3(0.5f, 0, 0));
            expected.AddVertex(new Vector3(0, 0.5f, 0));
            expected.AddVertex(new Vector3(0, 0, 0.5f));
            expected.AddTriangle(0, 1, 2);

            // ACT
            ListSurface s = new ListSurface();
            MarchingCubes.MarchIntoSurface(grid, 0, s);
            bool areEqual = ListSurface.AreSurfacesEquivalent(expected, s);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void GridGenerateSurface_SingleValueMinus1_OtherValue1_Iso0_GeneratesCorrectTriangle()
        {
            // ARRANGE
            ArrayGrid grid = GenerateSingle(-1, 1, 0);
            ListSurface expected = new ListSurface();
            expected.AddVertex(new Vector3(0.5f, 0, 0));
            expected.AddVertex(new Vector3(0, 0, 0.5f));
            expected.AddVertex(new Vector3(0, 0.5f, 0));
            expected.AddTriangle(0, 1, 2);

            // ACT
            ListSurface s = new ListSurface();
            MarchingCubes.MarchIntoSurface(grid, 0, s);
            bool areEqual = ListSurface.AreSurfacesEquivalent(expected, s);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void GridGenerateSurface_SingleValueMinus3_OtherValue1_Iso0_GeneratesCorrectTriangle()
        {
            // ARRANGE
            ArrayGrid grid = GenerateSingle(-3, 1, 0);
            ListSurface expected = new ListSurface();
            expected.AddVertex(new Vector3(0.75f, 0, 0));
            expected.AddVertex(new Vector3(0, 0, 0.75f));
            expected.AddVertex(new Vector3(0, 0.75f, 0));
            expected.AddTriangle(0, 1, 2);

            // ACT
            ListSurface s = new ListSurface();
            MarchingCubes.MarchIntoSurface(grid, 0, s);
            bool areEqual = ListSurface.AreSurfacesEquivalent(expected, s);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void GridGenerateSurface_SingleValue3_OtherValueMinus1_Iso0_GeneratesCorrectTriangle()
        {
            // ARRANGE
            ArrayGrid grid = GenerateSingle(3, -1, 0);
            ListSurface expected = new ListSurface();
            expected.AddVertex(new Vector3(0.75f, 0, 0));
            expected.AddVertex(new Vector3(0, 0.75f, 0));
            expected.AddVertex(new Vector3(0, 0, 0.75f));
            expected.AddTriangle(0, 1, 2);

            // ACT
            ListSurface s = new ListSurface();
            MarchingCubes.MarchIntoSurface(grid, 0, s);
            bool areEqual = ListSurface.AreSurfacesEquivalent(expected, s);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void GridGenerateSurface_SingleValue1_OtherValueMinus3_Iso0_GeneratesCorrectTriangle()
        {
            // ARRANGE
            ArrayGrid grid = GenerateSingle(1, -3, 0);
            ListSurface expected = new ListSurface();
            expected.AddVertex(new Vector3(0.25f, 0, 0));
            expected.AddVertex(new Vector3(0, 0.25f, 0));
            expected.AddVertex(new Vector3(0, 0, 0.25f));
            expected.AddTriangle(0, 1, 2);

            // ACT
            ListSurface s = new ListSurface();
            MarchingCubes.MarchIntoSurface(grid, 0, s);
            bool areEqual = ListSurface.AreSurfacesEquivalent(expected, s);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void GridGenerateSurface_SingleValueMinus1_OtherValue3_Iso0_GeneratesCorrectTriangle()
        {
            // ARRANGE
            ArrayGrid grid = GenerateSingle(-1, 3, 0);
            ListSurface expected = new ListSurface();
            expected.AddVertex(new Vector3(0.25f, 0, 0));
            expected.AddVertex(new Vector3(0, 0, 0.25f));
            expected.AddVertex(new Vector3(0, 0.25f, 0));
            expected.AddTriangle(0, 1, 2);

            // ACT
            ListSurface s = new ListSurface();
            MarchingCubes.MarchIntoSurface(grid, 0, s);
            bool areEqual = ListSurface.AreSurfacesEquivalent(expected, s);

            // ASSERT
            Assert.IsTrue(areEqual);
        }
    }
}