﻿using MC_33;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MC_33_Tests
{
    internal class SurfaceTests
    {
        [Test]
        public void Surface_TrianglesEqual_TrueForSameTriangles()
        {
            // ARRANGE
            Vector3[] t1 = new Vector3[]
            {
                new Vector3 ( 0, 0, 0 ),
                new Vector3 ( 1, 0, 0 ),
                new Vector3 ( 0, 1, 0 ),
            };
            Vector3[] t2 = new Vector3[]
            {
                new Vector3 ( 0, 0, 0 ),
                new Vector3 ( 1, 0, 0 ),
                new Vector3 ( 0, 1, 0 ),
            };

            // ACT
            bool areEqual = ListSurface.AreTrianglesEqual(t1, t2);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Surface_TrianglesEqual_TrueForSameWindings1()
        {
            // ARRANGE
            Vector3[] t1 = new Vector3[]
            {
                new Vector3 ( 0, 0, 0 ),
                new Vector3 ( 1, 0, 0 ),
                new Vector3 ( 0, 1, 0 ),
            };
            Vector3[] t2 = new Vector3[]
            {
                new Vector3 ( 1, 0, 0 ),
                new Vector3 ( 0, 1, 0 ),
                new Vector3 ( 0, 0, 0 ),
            };

            // ACT
            bool areEqual = ListSurface.AreTrianglesEqual(t1, t2);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Surface_TrianglesEqual_TrueForSameWindings2()
        {
            // ARRANGE
            Vector3[] t1 = new Vector3[]
            {
                new Vector3 ( 0, 0, 0 ),
                new Vector3 ( 1, 0, 0 ),
                new Vector3 ( 0, 1, 0 ),
            };
            Vector3[] t2 = new Vector3[]
            {
                new Vector3 ( 0, 1, 0 ),
                new Vector3 ( 0, 0, 0 ),
                new Vector3 ( 1, 0, 0 ),
            };

            // ACT
            bool areEqual = ListSurface.AreTrianglesEqual(t1, t2);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Surface_TrianglesEqual_FalseForOppositeWindings1()
        {
            // ARRANGE
            Vector3[] t1 = new Vector3[]
            {
                new Vector3 ( 0, 0, 0 ),
                new Vector3 ( 1, 0, 0 ),
                new Vector3 ( 0, 1, 0 ),
            };
            Vector3[] t2 = new Vector3[]
            {
                new Vector3 ( 0, 0, 0 ),
                new Vector3 ( 0, 1, 0 ),
                new Vector3 ( 1, 0, 0 ),
            };

            // ACT
            bool areEqual = ListSurface.AreTrianglesEqual(t1, t2);

            // ASSERT
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Surface_TrianglesEqual_FalseForOppositeWindings2()
        {
            // ARRANGE
            Vector3[] t1 = new Vector3[]
            {
                new Vector3 ( 0, 0, 0 ),
                new Vector3 ( 1, 0, 0 ),
                new Vector3 ( 0, 1, 0 ),
            };
            Vector3[] t2 = new Vector3[]
            {
                new Vector3 ( 0, 1, 0 ),
                new Vector3 ( 1, 0, 0 ),
                new Vector3 ( 0, 0, 0 ),
            };

            // ACT
            bool areEqual = ListSurface.AreTrianglesEqual(t1, t2);

            // ASSERT
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Surface_TrianglesEqual_FalseForOppositeWindings3()
        {
            // ARRANGE
            Vector3[] t1 = new Vector3[]
            {
                new Vector3 ( 0, 0, 0 ),
                new Vector3 ( 1, 0, 0 ),
                new Vector3 ( 0, 1, 0 ),
            };
            Vector3[] t2 = new Vector3[]
            {
                new Vector3 ( 1, 0, 0 ),
                new Vector3 ( 0, 0, 0 ),
                new Vector3 ( 0, 1, 0 ),
            };

            // ACT
            bool areEqual = ListSurface.AreTrianglesEqual(t1, t2);

            // ASSERT
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Surface_Equal_TrueForSameTriangles()
        {
            // ARRANGE
            ListSurface s1 = new ListSurface();
            s1.AddVertex(new Vector3(0, 0, 0));
            s1.AddVertex(new Vector3(1, 0, 0));
            s1.AddVertex(new Vector3(0, 1, 0));
            s1.AddTriangle(0, 1, 2);
            ListSurface s2 = new ListSurface();
            s2.AddVertex(new Vector3(0, 0, 0));
            s2.AddVertex(new Vector3(1, 0, 0));
            s2.AddVertex(new Vector3(0, 1, 0));
            s2.AddTriangle(0, 1, 2);

            // ACT
            bool areEqual = ListSurface.AreSurfacesEquivalent(s1, s2);

            // ASSERT
            Assert.IsTrue(areEqual);
        }


        [Test]
        public void Surface_Equal_TrueForSameWindingsVertex1()
        {
            // ARRANGE
            ListSurface s1 = new ListSurface();
            s1.AddVertex(new Vector3(0, 0, 0));
            s1.AddVertex(new Vector3(1, 0, 0));
            s1.AddVertex(new Vector3(0, 1, 0));
            s1.AddTriangle(0, 1, 2);
            ListSurface s2 = new ListSurface();
            s2.AddVertex(new Vector3(1, 0, 0));
            s2.AddVertex(new Vector3(0, 1, 0));
            s2.AddVertex(new Vector3(0, 0, 0));
            s2.AddTriangle(0, 1, 2);

            // ACT
            bool areEqual = ListSurface.AreSurfacesEquivalent(s1, s2);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Surface_Equal_TrueForSameWindingsVertex2()
        {
            // ARRANGE
            ListSurface s1 = new ListSurface();
            s1.AddVertex(new Vector3(0, 0, 0));
            s1.AddVertex(new Vector3(1, 0, 0));
            s1.AddVertex(new Vector3(0, 1, 0));
            s1.AddTriangle(0, 1, 2);
            ListSurface s2 = new ListSurface();
            s2.AddVertex(new Vector3(0, 1, 0));
            s2.AddVertex(new Vector3(0, 0, 0));
            s2.AddVertex(new Vector3(1, 0, 0));
            s2.AddTriangle(0, 1, 2);

            // ACT
            bool areEqual = ListSurface.AreSurfacesEquivalent(s1, s2);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Surface_Equal_TrueForSameWindingsTriangle1()
        {
            // ARRANGE
            ListSurface s1 = new ListSurface();
            s1.AddVertex(new Vector3(0, 0, 0));
            s1.AddVertex(new Vector3(1, 0, 0));
            s1.AddVertex(new Vector3(0, 1, 0));
            s1.AddTriangle(0, 1, 2);
            ListSurface s2 = new ListSurface();
            s2.AddVertex(new Vector3(0, 0, 0));
            s2.AddVertex(new Vector3(1, 0, 0));
            s2.AddVertex(new Vector3(0, 1, 0));
            s2.AddTriangle(1, 2, 0);

            // ACT
            bool areEqual = ListSurface.AreSurfacesEquivalent(s1, s2);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Surface_Equal_TrueForSameWindingsTriangle2()
        {
            // ARRANGE
            ListSurface s1 = new ListSurface();
            s1.AddVertex(new Vector3(0, 0, 0));
            s1.AddVertex(new Vector3(1, 0, 0));
            s1.AddVertex(new Vector3(0, 1, 0));
            s1.AddTriangle(0, 1, 2);
            ListSurface s2 = new ListSurface();
            s2.AddVertex(new Vector3(0, 0, 0));
            s2.AddVertex(new Vector3(1, 0, 0));
            s2.AddVertex(new Vector3(0, 1, 0));
            s2.AddTriangle(2, 0, 1);

            // ACT
            bool areEqual = ListSurface.AreSurfacesEquivalent(s1, s2);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Surface_Equal_FalseForDifferentWindingsVertex1()
        {
            // ARRANGE
            ListSurface s1 = new ListSurface();
            s1.AddVertex(new Vector3(0, 0, 0));
            s1.AddVertex(new Vector3(1, 0, 0));
            s1.AddVertex(new Vector3(0, 1, 0));
            s1.AddTriangle(0, 1, 2);
            ListSurface s2 = new ListSurface();
            s2.AddVertex(new Vector3(0, 0, 0));
            s2.AddVertex(new Vector3(0, 1, 0));
            s2.AddVertex(new Vector3(1, 0, 0));
            s2.AddTriangle(0, 1, 2);

            // ACT
            bool areEqual = ListSurface.AreSurfacesEquivalent(s1, s2);

            // ASSERT
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Surface_Equal_FalseForDifferentWindingsVertex2()
        {
            // ARRANGE
            ListSurface s1 = new ListSurface();
            s1.AddVertex(new Vector3(0, 0, 0));
            s1.AddVertex(new Vector3(1, 0, 0));
            s1.AddVertex(new Vector3(0, 1, 0));
            s1.AddTriangle(0, 1, 2);
            ListSurface s2 = new ListSurface();
            s2.AddVertex(new Vector3(0, 1, 0));
            s2.AddVertex(new Vector3(1, 0, 0));
            s2.AddVertex(new Vector3(0, 0, 0));
            s2.AddTriangle(0, 1, 2);

            // ACT
            bool areEqual = ListSurface.AreSurfacesEquivalent(s1, s2);

            // ASSERT
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Surface_Equal_FalseForDifferentWindingsVertex3()
        {
            // ARRANGE
            ListSurface s1 = new ListSurface();
            s1.AddVertex(new Vector3(0, 0, 0));
            s1.AddVertex(new Vector3(1, 0, 0));
            s1.AddVertex(new Vector3(0, 1, 0));
            s1.AddTriangle(0, 1, 2);
            ListSurface s2 = new ListSurface();
            s2.AddVertex(new Vector3(1, 0, 0));
            s2.AddVertex(new Vector3(0, 0, 0));
            s2.AddVertex(new Vector3(0, 1, 0));
            s2.AddTriangle(0, 1, 2);

            // ACT
            bool areEqual = ListSurface.AreSurfacesEquivalent(s1, s2);

            // ASSERT
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Surface_Equal_FalseForDifferentWindingsTriangle1()
        {
            // ARRANGE
            ListSurface s1 = new ListSurface();
            s1.AddVertex(new Vector3(0, 0, 0));
            s1.AddVertex(new Vector3(1, 0, 0));
            s1.AddVertex(new Vector3(0, 1, 0));
            s1.AddTriangle(0, 1, 2);
            ListSurface s2 = new ListSurface();
            s2.AddVertex(new Vector3(0, 0, 0));
            s2.AddVertex(new Vector3(1, 0, 0));
            s2.AddVertex(new Vector3(0, 1, 0));
            s2.AddTriangle(0, 2, 1);

            // ACT
            bool areEqual = ListSurface.AreSurfacesEquivalent(s1, s2);

            // ASSERT
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Surface_Equal_FalseForDifferentWindingsTriangle2()
        {
            // ARRANGE
            ListSurface s1 = new ListSurface();
            s1.AddVertex(new Vector3(0, 0, 0));
            s1.AddVertex(new Vector3(1, 0, 0));
            s1.AddVertex(new Vector3(0, 1, 0));
            s1.AddTriangle(0, 1, 2);
            ListSurface s2 = new ListSurface();
            s2.AddVertex(new Vector3(0, 0, 0));
            s2.AddVertex(new Vector3(1, 0, 0));
            s2.AddVertex(new Vector3(0, 1, 0));
            s2.AddTriangle(2, 1, 0);

            // ACT
            bool areEqual = ListSurface.AreSurfacesEquivalent(s1, s2);

            // ASSERT
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Surface_Equal_FalseForDifferentWindingsTriangle3()
        {
            // ARRANGE
            ListSurface s1 = new ListSurface();
            s1.AddVertex(new Vector3(0, 0, 0));
            s1.AddVertex(new Vector3(1, 0, 0));
            s1.AddVertex(new Vector3(0, 1, 0));
            s1.AddTriangle(0, 1, 2);
            ListSurface s2 = new ListSurface();
            s2.AddVertex(new Vector3(0, 0, 0));
            s2.AddVertex(new Vector3(1, 0, 0));
            s2.AddVertex(new Vector3(0, 1, 0));
            s2.AddTriangle(1, 0, 2);

            // ACT
            bool areEqual = ListSurface.AreSurfacesEquivalent(s1, s2);

            // ASSERT
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Surface_Equal_True_ForTwoTriangles_Same()
        {
            // ARRANGE
            ListSurface s1 = new ListSurface();
            s1.AddVertex(new Vector3(0, 0, 0));
            s1.AddVertex(new Vector3(1, 0, 0));
            s1.AddVertex(new Vector3(0, 1, 0));
            s1.AddVertex(new Vector3(0, 0, 0));
            s1.AddVertex(new Vector3(1, 0, 0));
            s1.AddVertex(new Vector3(1, 1, 0));
            s1.AddTriangle(0, 1, 2);
            s1.AddTriangle(3, 4, 5);
            ListSurface s2 = new ListSurface();
            s2.AddVertex(new Vector3(0, 0, 0));
            s2.AddVertex(new Vector3(1, 0, 0));
            s2.AddVertex(new Vector3(0, 1, 0));
            s2.AddVertex(new Vector3(0, 0, 0));
            s2.AddVertex(new Vector3(1, 0, 0));
            s2.AddVertex(new Vector3(1, 1, 0));
            s2.AddTriangle(0, 1, 2);
            s2.AddTriangle(3, 4, 5);

            // ACT
            bool areEqual = ListSurface.AreSurfacesEquivalent(s1, s2);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Surface_Equal_True_ForTwoTriangles_FlippedTriangles()
        {
            // ARRANGE
            ListSurface s1 = new ListSurface();
            s1.AddVertex(new Vector3(0, 0, 0));
            s1.AddVertex(new Vector3(1, 0, 0));
            s1.AddVertex(new Vector3(0, 1, 0));
            s1.AddVertex(new Vector3(0, 0, 0));
            s1.AddVertex(new Vector3(1, 0, 0));
            s1.AddVertex(new Vector3(1, 1, 0));
            s1.AddTriangle(3, 4, 5);
            s1.AddTriangle(0, 1, 2);
            ListSurface s2 = new ListSurface();
            s2.AddVertex(new Vector3(0, 0, 0));
            s2.AddVertex(new Vector3(1, 0, 0));
            s2.AddVertex(new Vector3(0, 1, 0));
            s2.AddVertex(new Vector3(0, 0, 0));
            s2.AddVertex(new Vector3(1, 0, 0));
            s2.AddVertex(new Vector3(1, 1, 0));
            s2.AddTriangle(0, 1, 2);
            s2.AddTriangle(3, 4, 5);

            // ACT
            bool areEqual = ListSurface.AreSurfacesEquivalent(s1, s2);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Surface_Equal_True_ForTwoTriangles_FlippedVertices()
        {
            // ARRANGE
            ListSurface s1 = new ListSurface();
            s1.AddVertex(new Vector3(0, 0, 0));
            s1.AddVertex(new Vector3(1, 0, 0));
            s1.AddVertex(new Vector3(1, 1, 0));
            s1.AddVertex(new Vector3(0, 0, 0));
            s1.AddVertex(new Vector3(1, 0, 0));
            s1.AddVertex(new Vector3(0, 1, 0));
            s1.AddTriangle(0, 1, 2);
            s1.AddTriangle(3, 4, 5);
            ListSurface s2 = new ListSurface();
            s2.AddVertex(new Vector3(0, 0, 0));
            s2.AddVertex(new Vector3(1, 0, 0));
            s2.AddVertex(new Vector3(0, 1, 0));
            s2.AddVertex(new Vector3(0, 0, 0));
            s2.AddVertex(new Vector3(1, 0, 0));
            s2.AddVertex(new Vector3(1, 1, 0));
            s2.AddTriangle(0, 1, 2);
            s2.AddTriangle(3, 4, 5);

            // ACT
            bool areEqual = ListSurface.AreSurfacesEquivalent(s1, s2);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Surface_Equal_False_ForOneVsTwoTriangles()
        {
            // ARRANGE
            ListSurface s1 = new ListSurface();
            s1.AddVertex(new Vector3(0, 0, 0));
            s1.AddVertex(new Vector3(1, 0, 0));
            s1.AddVertex(new Vector3(1, 1, 0));
            s1.AddTriangle(0, 1, 2);
            ListSurface s2 = new ListSurface();
            s2.AddVertex(new Vector3(0, 0, 0));
            s2.AddVertex(new Vector3(1, 0, 0));
            s2.AddVertex(new Vector3(0, 1, 0));
            s2.AddVertex(new Vector3(0, 0, 0));
            s2.AddVertex(new Vector3(1, 0, 0));
            s2.AddVertex(new Vector3(1, 1, 0));
            s2.AddTriangle(0, 1, 2);
            s2.AddTriangle(3, 4, 5);

            // ACT
            bool areEqual = ListSurface.AreSurfacesEquivalent(s1, s2);

            // ASSERT
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Surface_Equal_True_ForTwoTriangles_SharedVertices()
        {
            // ARRANGE
            ListSurface s1 = new ListSurface();
            s1.AddVertex(new Vector3(0, 0, 0));
            s1.AddVertex(new Vector3(1, 0, 0));
            s1.AddVertex(new Vector3(0, 1, 0));
            s1.AddVertex(new Vector3(1, 1, 0));
            s1.AddTriangle(0, 1, 2);
            s1.AddTriangle(0, 1, 3);
            ListSurface s2 = new ListSurface();
            s2.AddVertex(new Vector3(0, 0, 0));
            s2.AddVertex(new Vector3(1, 0, 0));
            s2.AddVertex(new Vector3(0, 1, 0));
            s2.AddVertex(new Vector3(0, 0, 0));
            s2.AddVertex(new Vector3(1, 0, 0));
            s2.AddVertex(new Vector3(1, 1, 0));
            s2.AddTriangle(0, 1, 2);
            s2.AddTriangle(3, 4, 5);

            // ACT
            bool areEqual = ListSurface.AreSurfacesEquivalent(s1, s2);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Surface_Equal_True_ForTwoTriangles_SharedVertices2()
        {
            // ARRANGE
            ListSurface s1 = new ListSurface();
            s1.AddVertex(new Vector3(0, 0, 0));
            s1.AddVertex(new Vector3(1, 0, 0));
            s1.AddVertex(new Vector3(0, 1, 0));
            s1.AddVertex(new Vector3(1, 1, 0));
            s1.AddTriangle(0, 1, 2);
            s1.AddTriangle(0, 1, 3);
            ListSurface s2 = new ListSurface();
            s2.AddVertex(new Vector3(0, 0, 0));
            s2.AddVertex(new Vector3(1, 0, 0));
            s2.AddVertex(new Vector3(0, 1, 0));
            s2.AddVertex(new Vector3(1, 0, 0));
            s2.AddVertex(new Vector3(1, 1, 0));
            s2.AddTriangle(0, 1, 2);
            s2.AddTriangle(0, 3, 4);

            // ACT
            bool areEqual = ListSurface.AreSurfacesEquivalent(s1, s2);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Surface_IsClosed_True_ForEmptySurface()
        {
            // ARRANGE
            ListSurface s = new ListSurface();

            // ACT
            bool isClosed = s.IsClosed();

            // ASSERT
            Assert.IsTrue(isClosed);
        }

        [Test]
        public void Surface_IsClosed_False_ForSingleTriangle()
        {
            // ARRANGE
            ListSurface s = new ListSurface();
            s.AddVertex(0, 0, 0);
            s.AddVertex(0, 1, 0);
            s.AddVertex(1, 0, 0);
            s.AddTriangle(0, 1, 2);

            // ACT
            bool isClosed = s.IsClosed();

            // ASSERT
            Assert.IsFalse(isClosed);
        }

        [Test]
        public void Surface_IsClosed_True_ForBackToBackTriangles()
        {
            // ARRANGE
            ListSurface s = new ListSurface();
            s.AddVertex(0, 0, 0);
            s.AddVertex(0, 1, 0);
            s.AddVertex(1, 0, 0);
            s.AddTriangle(0, 1, 2);
            s.AddTriangle(0, 2, 1);

            // ACT
            bool isClosed = s.IsClosed();

            // ASSERT
            Assert.IsTrue(isClosed);
        }
    }
}
