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
    public interface IGrid
    {
        Vector3 Origin { get; }
        Vector3 Offset { get; }

        float this[int x, int y, int z]
        {
            get;
        }

        int SizeX
        {
            get;
        }

        int SizeY
        {
            get;
        }

        int SizeZ
        {
            get;
        }
    }
}
