using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MC_33
{
    internal class ArrayGrid : Grid
    {
        private readonly float[,,] _data;

        /// <summary>
        /// Creates a new immutable grid of sampled points
        /// </summary>
        /// <param name="data">The results of a function evaluated at a grid of regularly spaced points.
        /// Should be given in zyx format for faster indexing</param>
        /// <param name="r0">The coordinates of the first grid point</param>
        /// <param name="d">The distance between adjacent points in each dimension</param>
        public ArrayGrid(float[,,] data, Vector3 r0, Vector3 d)
            : base(r0, d)
        {
            this._data = data;
        }

        public override float this[int x, int y, int z]
        {
            get => _data[x, y, z];
        }

        public override int SizeX
        {
            get => _data.GetLength(0) - 1;
        }
        public override int SizeY
        {
            get => _data.GetLength(1) - 1;
        }
        public override int SizeZ
        {
            get => _data.GetLength(2) - 1;
        }
    }
}
