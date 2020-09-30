using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MC_33
{
    public class ListSurface : ISurface
    {
        private static readonly double _epsilon = 0.000000001; // Used for comparing vectors in equality function

        private readonly List<int> _triangles = new List<int>();
        private readonly List<Vector3> _vertices = new List<Vector3>();

        public int AddVertex(Vector3 pos)
        {
            int c = _vertices.Count;

            _vertices.Add(pos);

            return c;
        }

        public void AddTriangle(int p1, int p2, int p3)
        {
            _triangles.Add(p1);
            _triangles.Add(p2);
            _triangles.Add(p3);
        }

        public int[] GetTriangles()
        {
            return _triangles.ToArray();
        }

        public Vector3[] GetVertices()
        {
            return _vertices.ToArray();
        }

        public int GetVertexCount()
        {
            return _vertices.Count;
        }

        public int AddVertex(float x, float y, float z)
        {
            return AddVertex(new Vector3(x, y, z));
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
                    diff |= !t1[i].EqualsApproximately(t2[j], _epsilon);
                }
                if (!diff)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool AreSurfacesEquivalent(ListSurface surface1, ListSurface surface2)
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

        public bool IsEquivalentTo(ListSurface s)
        {
            return AreSurfacesEquivalent(this, s);
        }

        private class Edge
        {
            public readonly int v1;
            public readonly int v2;

            public Edge(int v1, int v2)
            {
                if (v1 < v2)
                {
                    this.v1 = v1;
                    this.v2 = v2;
                }
                else
                {
                    this.v2 = v1;
                    this.v1 = v2;
                }
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as Edge);
            }

            public bool Equals(Edge edge)
            {
                if (edge == null)
                {
                    return false;
                }

                return v1 == edge.v1 && v2 == edge.v2;
            }

            public override int GetHashCode()
            {
                return 31 * v1 ^ v2;
            }
        }

        public bool IsClosed()
        {
            /**
             * A surface is closed iff every edge is adjacent to exactly two triangles
             */

            int[] triangles = GetTriangles();

            // Populate a record of all of the triangles attached to each edge
            Dictionary<Edge, List<int>> trianglesByEdge = new Dictionary<Edge, List<int>>();
            for (int iTriangle = 0; iTriangle < triangles.Length / 3; iTriangle++)
            {
                Edge[] edges = new Edge[]
                {
                    new Edge(triangles[iTriangle * 3 + 0], triangles[iTriangle * 3 + 1]),
                    new Edge(triangles[iTriangle * 3 + 1], triangles[iTriangle * 3 + 2]),
                    new Edge(triangles[iTriangle * 3 + 2], triangles[iTriangle * 3 + 0])
                };
                foreach (Edge edge in edges)
                {
                    if (!trianglesByEdge.TryGetValue(edge, out List<int> edgeTriangles))
                    {
                        edgeTriangles = new List<int>();
                        trianglesByEdge.Add(edge, edgeTriangles);
                    }
                    edgeTriangles.Add(iTriangle);
                }
            }

            foreach (List<int> edgeTriangles in trianglesByEdge.Values)
            {
                if (edgeTriangles.Count != 2)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
