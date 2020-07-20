using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MC_33
{
    public class Surface
    {
		private readonly List<int> triangles = new List<int>();
		private readonly List<Vector3> vertices = new List<Vector3>();
		private readonly List<Vector3> normals = new List<Vector3>();

		public int AddVertex(Vector3 pos, Vector3 norm)
		{
			int c = vertices.Count;

			vertices.Add(pos);
			normals.Add(norm);

			return c;
		}

		public void AddTriangle(int p1, int p2, int p3)
		{
			triangles.Add(p1);
			triangles.Add(p2);
			triangles.Add(p3);
		}

		public int[] GetTriangles()
		{
			return triangles.ToArray();
		}

		public Vector3[] GetVertices()
		{
			return vertices.ToArray();
		}

		public Vector3[] GetNormals()
		{
			return normals.ToArray();
		}
	}
}
