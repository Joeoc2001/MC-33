using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MC_33
{
    internal class ListSurface : Surface
    {
        private readonly List<int> _triangles = new List<int>();
        private readonly List<Vector3> _vertices = new List<Vector3>();

        public override int AddVertex(Vector3 pos)
        {
            int c = _vertices.Count;

            _vertices.Add(pos);

            return c;
        }

        public override void AddTriangle(int p1, int p2, int p3)
        {
            _triangles.Add(p1);
            _triangles.Add(p2);
            _triangles.Add(p3);
        }

        public override int[] GetTriangles()
        {
            return _triangles.ToArray();
        }

        public override Vector3[] GetVertices()
        {
            return _vertices.ToArray();
        }

        public override int GetVertexCount()
        {
            return _vertices.Count;
        }
    }
}
