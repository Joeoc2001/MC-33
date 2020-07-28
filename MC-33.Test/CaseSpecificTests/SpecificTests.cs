using MC_33;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace MC_33_Tests.CaseSpecificTests
{
    static class SpecificTests
    {
        public static bool TestSpecific(float[,,] cells, Surface expected)
        {
            return TestSpecific(cells, new List<Surface>() { expected });
        }

        /// <summary>
        /// Tests a specific example of a cell. Assumes that delta in the grid is 1, origin is 0 and isovalue is 0
        /// </summary>
        public static bool TestSpecific(float[,,] cells, IEnumerable<Surface> expecteds)
        {
            ArrayGrid grid = new ArrayGrid(cells, Vector3.Zero, Vector3.One);
            Surface s = grid.GenerateSurface(0);

            foreach (Surface e in expecteds)
            {
                if (Surface.AreSurfacesEquivalent(e, s))
                {
                    return true;
                }
            }
            return false;
        }

        private static void JoinInto(Surface into, Surface from)
        {
            int offset = into.GetVertexCount();

            foreach (Vector3 vertex in from.GetVertices())
            {
                into.AddVertex(vertex);
            }

            int[] triangles = from.GetTriangles();
            for (int i = 0; i < triangles.Length; i += 3)
            {
                into.AddTriangle(triangles[i] + offset,
                    triangles[i + 1] + offset,
                    triangles[i + 2] + offset);
            }
        }

        private static List<List<Tuple<int, int>>> GenAllTriangulationPatterns(int n)
        {
            // http://cgm.cs.mcgill.ca/~athens/cs507/Projects/2002/AjitRajwade/

            if (n < 3)
            {
                throw new ArgumentOutOfRangeException("A polygon must have at least 3 vertices");
            }

            if (n == 3)
            {
                return new List<List<Tuple<int, int>>>() { 
                    new List<Tuple<int, int>>()
                    {
                        new Tuple<int, int>(0, 1),
                        new Tuple<int, int>(1, 2),
                        new Tuple<int, int>(2, 0)
                    }
                };
            }

            List<List<Tuple<int, int>>> last = GenAllTriangulationPatterns(n - 1);
            List<List<Tuple<int, int>>> current = new List<List<Tuple<int, int>>>();

            foreach (List<Tuple<int, int>> lastPermutation in last)
            {
                // Find all vertices that connect to vertex n - 1
                List<int> connections = new List<int>();
                List<Tuple<int, int>> secure = new List<Tuple<int, int>>(); // The edges that will always be present
                foreach (Tuple<int, int> edge in lastPermutation)
                {
                    if (edge.Item1 == n - 2)
                    {
                        connections.Add(edge.Item2);
                    }
                    else if (edge.Item2 == n - 2)
                    {
                        connections.Add(edge.Item1);
                    }
                    else
                    {
                        secure.Add(edge);
                    }
                }

                // Split & loop
                foreach (int i in connections)
                {
                    List<Tuple<int, int>> currentPermutation = new List<Tuple<int, int>>(secure);

                    foreach (int j in connections)
                    {
                        if (j <= i)
                        {
                            currentPermutation.Add(new Tuple<int, int>(j, n - 1));
                        }
                        if (j >= i)
                        {
                            currentPermutation.Add(new Tuple<int, int>(j, n - 2));
                        }
                    }

                    currentPermutation.Add(new Tuple<int, int>(n - 2, n - 1));

                    current.Add(currentPermutation);
                }
            }

            return current;
        }

        private static List<Tuple<int, int, int>> GenTrianglesFromEdges(List<Tuple<int, int>> edges)
        {
            List<Tuple<int, int, int>> triangles = new List<Tuple<int, int, int>>();

            for (int i = 0; i < edges.Count; i++)
            {
                Tuple<int, int> edge1 = edges[i];
                for (int iFlip = 0; iFlip < 2; iFlip++)
                {
                    int[] vs = new int[3];

                    if (iFlip == 0) // Swap first edge direction next time
                    {
                        vs[0] = edge1.Item1;
                        vs[1] = edge1.Item2;
                    }
                    else
                    {
                        vs[0] = edge1.Item2;
                        vs[1] = edge1.Item1;
                    }

                    for (int j = i + 1; j < edges.Count; j++)
                    {
                        Tuple<int, int> edge2 = edges[j];

                        if (edge2.Item1 == vs[1])
                        {
                            vs[2] = edge2.Item2;
                        }
                        else if (edge2.Item2 == vs[1])
                        {
                            vs[2] = edge2.Item1;
                        }
                        else
                        {
                            continue;
                        }

                        for (int k = j + 1; k < edges.Count; k++)
                        {
                            Tuple<int, int> edge3 = edges[k];
                            if ((edge3.Item1 != vs[2] || edge3.Item2 != vs[0])
                                && (edge3.Item2 != vs[2] || edge3.Item1 != vs[0]))
                            {
                                continue;
                            }

                            int[] vsSorted = new int[3];
                            Array.Copy(vs, 0, vsSorted, 0, 3);
                            Array.Sort(vsSorted);

                            triangles.Add(new Tuple<int, int, int>(vsSorted[0], vsSorted[1], vsSorted[2]));
                        }
                    }
                }
            }

            return triangles;
        }

        private static Surface GenSurfaceFromEdges(List<Vector3> vertices, List<Tuple<int, int>> edges)
        {
            Surface surface = new ListSurface();

            foreach (Vector3 vertex in vertices)
            {
                surface.AddVertex(vertex);
            }

            List<Tuple<int, int, int>> triangles = GenTrianglesFromEdges(edges);
            foreach(Tuple<int, int, int> triangle in triangles)
            {
                surface.AddTriangle(triangle.Item1, triangle.Item2, triangle.Item3);
            }

            return surface;
        }

        public static List<Surface> GenAllTriangulations(List<Vector3> polygon)
        {
            List<List<Tuple<int, int>>> perms = GenAllTriangulationPatterns(polygon.Count);

            List<Surface> surfaces = new List<Surface>();
            foreach (List<Tuple<int, int>> edgeSet in perms)
            {
                surfaces.Add(GenSurfaceFromEdges(polygon, edgeSet));
            }

            return surfaces;
        }

        public static List<Surface> AddAllTriangulations(IEnumerable<Surface> surfaces, List<Vector3> polygon)
        {
            List<Surface> triangulations = GenAllTriangulations(polygon);

            List<Surface> newSurfaces = new List<Surface>();
            foreach (Surface oldSurface in surfaces)
            {
                int offset = oldSurface.GetVertexCount();
                foreach (Surface triangulation in triangulations)
                {
                    Surface newSurface = new ListSurface();
                    JoinInto(newSurface, oldSurface);
                    JoinInto(newSurface, triangulation);
                    newSurfaces.Add(newSurface);
                }
            }

            return newSurfaces;
        }
    }
}
