using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace MC_33
{
    /// <summary>
    /// The structure Grid contains a function double[,,] evaluated at a grid of regularly
    /// spaced points. 
    /// r0 is the coordinates of the first grid point.
    /// d is the distance between adjacent points in each dimension
    /// (can be different for each dimension)
    /// </summary>
    public abstract class Grid
    {
        public Vector3 Origin { get; private set; }
        public Vector3 Offset { get; private set; }

        public Grid(Vector3 r0, Vector3 d)
        {
            Origin = r0;
            Offset = d;
        }

        public abstract float this[int x, int y, int z]
        {
            get;
        }

        public abstract int SizeX
        {
            get;
        }

        public abstract int SizeY
        {
            get;
        }

        public abstract int SizeZ
        {
            get;
        }

        public Surface GenerateSurface(float isovalue)
        {
            return MarchingCubes.CalculateSurface(this, isovalue);
        }
    }
}
