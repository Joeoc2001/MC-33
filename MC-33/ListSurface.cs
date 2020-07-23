using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MC_33
{
    public class ListSurface : Surface
    {
        private readonly List<int> triangles = new List<int>();
        private readonly List<Vector3> vertices = new List<Vector3>();

        public override int AddVertex(Vector3 pos)
        {
            int c = vertices.Count;

            vertices.Add(pos);

            return c;
        }

        public override void AddTriangle(int p1, int p2, int p3)
        {
            triangles.Add(p1);
            triangles.Add(p2);
            triangles.Add(p3);
        }

        public override int[] GetTriangles()
        {
            return triangles.ToArray();
        }

        public override Vector3[] GetVertices()
        {
            return vertices.ToArray();
        }

        public override int GetVertexCount()
        {
            return vertices.Count;
        }
    }
}
