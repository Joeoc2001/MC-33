using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MC_33
{
    internal class ArrayGrid : IGrid
    {
        private readonly float[,,] _data;

        public Vector3 Origin { get; private set; }
        public Vector3 Offset { get; private set; }

        /// <summary>
        /// Creates a new immutable grid of sampled points
        /// </summary>
        /// <param name="data">The results of a function evaluated at a grid of regularly spaced points.
        /// Should be given in zyx format for faster indexing</param>
        /// <param name="r0">The coordinates of the first grid point</param>
        /// <param name="d">The distance between adjacent points in each dimension</param>
        public ArrayGrid(float[,,] data, Vector3 r0, Vector3 d)
        {
            this._data = data;
            Origin = r0;
            Offset = d;
        }

        public float this[int x, int y, int z]
        {
            get => _data[x, y, z];
        }

        public int SizeX
        {
            get => _data.GetLength(0) - 1;
        }
        public int SizeY
        {
            get => _data.GetLength(1) - 1;
        }
        public int SizeZ
        {
            get => _data.GetLength(2) - 1;
        }
    }
}
