using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace MC_33
{

    /// <summary>
    /// The structure Grid contains a function double[,,] evaluated at a grid of regularly
    /// spaced points. 
    /// r0 is the coordinates of the first grid point.
    /// d is the distance between adjacent points in each dimension
    /// (can be different for each dimension)
    /// </summary>
    public class Grid
    {
        private readonly float[,,] data;
        private readonly Vector3 r0;
        private readonly Vector3 d;

        /// <summary>
        /// Creates a new immutable grid of sampled points
        /// </summary>
        /// <param name="data">The results of a function evaluated at a grid of regularly spaced points.
        /// Should be given in zyx format</param>
        /// <param name="r0">The coordinates of the first grid point</param>
        /// <param name="d">The distance between adjacent points in each dimension</param>
        public Grid(float[,,] data, Vector3 r0, Vector3 d)
        {
            this.data = data;
            this.r0 = r0;
            this.d = d;
        }

        public float this[int z, int y, int x]
        {
            get => data[z, y, x];
        }

        public Vector3 GetPosition(int z, int y, int x)
        {
            return r0 + (d * new Vector3(z, y, x));
        }

        public int SizeX
        {
            get => data.GetLength(0);
        }
        public int SizeY
        {
            get => data.GetLength(1);
        }
        public int SizeZ
        {
            get => data.GetLength(2);
        }

        public Surface GenerateSurface()
        {
            return GenerateSurface(0);
        }

        public Surface GenerateSurface(float isovalue)
        {
            return MarchingCubes.calc_isosurface(this, isovalue);
        }
    }
}
