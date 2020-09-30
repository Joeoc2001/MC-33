using System;
using System.Collections.Generic;
using System.Numerics;

namespace MC_33
{
    public interface ISurface
    {
        int AddVertex(Vector3 pos);

        void AddTriangle(int p1, int p2, int p3);
    }
}
