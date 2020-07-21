using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MC_33
{
    public class Surface
    {
        private static readonly double EPSILON = 0.000000001; // Used for comparing vectors in equality function

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

        public static bool AreTrianglesEqual(Vector3[] t1, Vector3[] t2)
        {
            // Check if they are actually triangles
            if (t1.Length != 3 || t2.Length != 3)
            {
                throw new ArgumentException("Triangles must have exactly three vertices");
            }

            // Check each possible offset to begin the triangle
            for (int offset = 0; offset < 3; offset++)
            {
                bool diff = false;
                for (int i = 0; i < 3; i++)
                {
                    int j = (i + offset) % 3;
                    diff |= !t1[i].EqualsApproximately(t2[j], EPSILON);
                }
                if (!diff)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool AreSurfaceShapesEqual(Surface surface1, Surface surface2)
        {
            int[] triangles1 = surface1.GetTriangles();
            int[] triangles2 = surface2.GetTriangles();

            if (triangles1.Length != triangles2.Length)
            {
                return false;
            }

            // Check triangles are valid
            if (triangles1.Length % 3 != 0)
            {
                return false;
            }

            Vector3[] vertices1 = surface1.GetVertices();
            Vector3[] vertices2 = surface2.GetVertices();

            for (int tri1 = 0; tri1 < triangles1.Length / 3; tri1++)
            {
                // Linear search for matching triangle
                // TODO: Use Octrees to speed this up
                // O(n^2) is too slow
                bool found = false;
                for (int tri2 = 0; tri2 < triangles2.Length / 3; tri2++)
                {
                    // Gather triangles
                    Vector3[] triangle1 = new Vector3[3];
                    Vector3[] triangle2 = new Vector3[3];
                    for (int j = 0; j < 3; j++)
                    {
                        triangle1[j] = vertices1[triangles1[tri1 * 3 + j]];
                        triangle2[j] = vertices2[triangles2[tri2 * 3 + j]];
                    }

                    // Check if triangles match
                    if (AreTrianglesEqual(triangle1, triangle2))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
