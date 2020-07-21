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
            bool areEqual = Surface.AreTrianglesEqual(t1, t2);

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
            bool areEqual = Surface.AreTrianglesEqual(t1, t2);

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
            bool areEqual = Surface.AreTrianglesEqual(t1, t2);

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
            bool areEqual = Surface.AreTrianglesEqual(t1, t2);

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
            bool areEqual = Surface.AreTrianglesEqual(t1, t2);

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
            bool areEqual = Surface.AreTrianglesEqual(t1, t2);

            // ASSERT
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Surface_Equal_TrueForSameTriangles()
        {
            Vector3 norm = new Vector3(1, 0, 0);

            // ARRANGE
            Surface s1 = new Surface();
            s1.AddVertex(new Vector3(0, 0, 0), norm);
            s1.AddVertex(new Vector3(1, 0, 0), norm);
            s1.AddVertex(new Vector3(0, 1, 0), norm);
            s1.AddTriangle(0, 1, 2);
            Surface s2 = new Surface();
            s2.AddVertex(new Vector3(0, 0, 0), norm);
            s2.AddVertex(new Vector3(1, 0, 0), norm);
            s2.AddVertex(new Vector3(0, 1, 0), norm);
            s2.AddTriangle(0, 1, 2);

            // ACT
            bool areEqual = Surface.AreSurfaceShapesEqual(s1, s2);

            // ASSERT
            Assert.IsTrue(areEqual);
        }


        [Test]
        public void Surface_Equal_TrueForSameWindingsVertex1()
        {
            Vector3 norm = new Vector3(1, 0, 0);

            // ARRANGE
            Surface s1 = new Surface();
            s1.AddVertex(new Vector3(0, 0, 0), norm);
            s1.AddVertex(new Vector3(1, 0, 0), norm);
            s1.AddVertex(new Vector3(0, 1, 0), norm);
            s1.AddTriangle(0, 1, 2);
            Surface s2 = new Surface();
            s2.AddVertex(new Vector3(1, 0, 0), norm);
            s2.AddVertex(new Vector3(0, 1, 0), norm);
            s2.AddVertex(new Vector3(0, 0, 0), norm);
            s2.AddTriangle(0, 1, 2);

            // ACT
            bool areEqual = Surface.AreSurfaceShapesEqual(s1, s2);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Surface_Equal_TrueForSameWindingsVertex2()
        {
            Vector3 norm = new Vector3(1, 0, 0);

            // ARRANGE
            Surface s1 = new Surface();
            s1.AddVertex(new Vector3(0, 0, 0), norm);
            s1.AddVertex(new Vector3(1, 0, 0), norm);
            s1.AddVertex(new Vector3(0, 1, 0), norm);
            s1.AddTriangle(0, 1, 2);
            Surface s2 = new Surface();
            s2.AddVertex(new Vector3(0, 1, 0), norm);
            s2.AddVertex(new Vector3(0, 0, 0), norm);
            s2.AddVertex(new Vector3(1, 0, 0), norm);
            s2.AddTriangle(0, 1, 2);

            // ACT
            bool areEqual = Surface.AreSurfaceShapesEqual(s1, s2);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Surface_Equal_TrueForSameWindingsTriangle1()
        {
            Vector3 norm = new Vector3(1, 0, 0);

            // ARRANGE
            Surface s1 = new Surface();
            s1.AddVertex(new Vector3(0, 0, 0), norm);
            s1.AddVertex(new Vector3(1, 0, 0), norm);
            s1.AddVertex(new Vector3(0, 1, 0), norm);
            s1.AddTriangle(0, 1, 2);
            Surface s2 = new Surface();
            s2.AddVertex(new Vector3(0, 0, 0), norm);
            s2.AddVertex(new Vector3(1, 0, 0), norm);
            s2.AddVertex(new Vector3(0, 1, 0), norm);
            s2.AddTriangle(1, 2, 0);

            // ACT
            bool areEqual = Surface.AreSurfaceShapesEqual(s1, s2);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Surface_Equal_TrueForSameWindingsTriangle2()
        {
            Vector3 norm = new Vector3(1, 0, 0);

            // ARRANGE
            Surface s1 = new Surface();
            s1.AddVertex(new Vector3(0, 0, 0), norm);
            s1.AddVertex(new Vector3(1, 0, 0), norm);
            s1.AddVertex(new Vector3(0, 1, 0), norm);
            s1.AddTriangle(0, 1, 2);
            Surface s2 = new Surface();
            s2.AddVertex(new Vector3(0, 0, 0), norm);
            s2.AddVertex(new Vector3(1, 0, 0), norm);
            s2.AddVertex(new Vector3(0, 1, 0), norm);
            s2.AddTriangle(2, 0, 1);

            // ACT
            bool areEqual = Surface.AreSurfaceShapesEqual(s1, s2);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Surface_Equal_FalseForDifferentWindingsVertex1()
        {
            Vector3 norm = new Vector3(1, 0, 0);

            // ARRANGE
            Surface s1 = new Surface();
            s1.AddVertex(new Vector3(0, 0, 0), norm);
            s1.AddVertex(new Vector3(1, 0, 0), norm);
            s1.AddVertex(new Vector3(0, 1, 0), norm);
            s1.AddTriangle(0, 1, 2);
            Surface s2 = new Surface();
            s2.AddVertex(new Vector3(0, 0, 0), norm);
            s2.AddVertex(new Vector3(0, 1, 0), norm);
            s2.AddVertex(new Vector3(1, 0, 0), norm);
            s2.AddTriangle(0, 1, 2);

            // ACT
            bool areEqual = Surface.AreSurfaceShapesEqual(s1, s2);

            // ASSERT
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Surface_Equal_FalseForDifferentWindingsVertex2()
        {
            Vector3 norm = new Vector3(1, 0, 0);

            // ARRANGE
            Surface s1 = new Surface();
            s1.AddVertex(new Vector3(0, 0, 0), norm);
            s1.AddVertex(new Vector3(1, 0, 0), norm);
            s1.AddVertex(new Vector3(0, 1, 0), norm);
            s1.AddTriangle(0, 1, 2);
            Surface s2 = new Surface();
            s2.AddVertex(new Vector3(0, 1, 0), norm);
            s2.AddVertex(new Vector3(1, 0, 0), norm);
            s2.AddVertex(new Vector3(0, 0, 0), norm);
            s2.AddTriangle(0, 1, 2);

            // ACT
            bool areEqual = Surface.AreSurfaceShapesEqual(s1, s2);

            // ASSERT
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Surface_Equal_FalseForDifferentWindingsVertex3()
        {
            Vector3 norm = new Vector3(1, 0, 0);

            // ARRANGE
            Surface s1 = new Surface();
            s1.AddVertex(new Vector3(0, 0, 0), norm);
            s1.AddVertex(new Vector3(1, 0, 0), norm);
            s1.AddVertex(new Vector3(0, 1, 0), norm);
            s1.AddTriangle(0, 1, 2);
            Surface s2 = new Surface();
            s2.AddVertex(new Vector3(1, 0, 0), norm);
            s2.AddVertex(new Vector3(0, 0, 0), norm);
            s2.AddVertex(new Vector3(0, 1, 0), norm);
            s2.AddTriangle(0, 1, 2);

            // ACT
            bool areEqual = Surface.AreSurfaceShapesEqual(s1, s2);

            // ASSERT
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Surface_Equal_FalseForDifferentWindingsTriangle1()
        {
            Vector3 norm = new Vector3(1, 0, 0);

            // ARRANGE
            Surface s1 = new Surface();
            s1.AddVertex(new Vector3(0, 0, 0), norm);
            s1.AddVertex(new Vector3(1, 0, 0), norm);
            s1.AddVertex(new Vector3(0, 1, 0), norm);
            s1.AddTriangle(0, 1, 2);
            Surface s2 = new Surface();
            s2.AddVertex(new Vector3(0, 0, 0), norm);
            s2.AddVertex(new Vector3(1, 0, 0), norm);
            s2.AddVertex(new Vector3(0, 1, 0), norm);
            s2.AddTriangle(0, 2, 1);

            // ACT
            bool areEqual = Surface.AreSurfaceShapesEqual(s1, s2);

            // ASSERT
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Surface_Equal_FalseForDifferentWindingsTriangle2()
        {
            Vector3 norm = new Vector3(1, 0, 0);

            // ARRANGE
            Surface s1 = new Surface();
            s1.AddVertex(new Vector3(0, 0, 0), norm);
            s1.AddVertex(new Vector3(1, 0, 0), norm);
            s1.AddVertex(new Vector3(0, 1, 0), norm);
            s1.AddTriangle(0, 1, 2);
            Surface s2 = new Surface();
            s2.AddVertex(new Vector3(0, 0, 0), norm);
            s2.AddVertex(new Vector3(1, 0, 0), norm);
            s2.AddVertex(new Vector3(0, 1, 0), norm);
            s2.AddTriangle(2, 1, 0);

            // ACT
            bool areEqual = Surface.AreSurfaceShapesEqual(s1, s2);

            // ASSERT
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Surface_Equal_FalseForDifferentWindingsTriangle3()
        {
            Vector3 norm = new Vector3(1, 0, 0);

            // ARRANGE
            Surface s1 = new Surface();
            s1.AddVertex(new Vector3(0, 0, 0), norm);
            s1.AddVertex(new Vector3(1, 0, 0), norm);
            s1.AddVertex(new Vector3(0, 1, 0), norm);
            s1.AddTriangle(0, 1, 2);
            Surface s2 = new Surface();
            s2.AddVertex(new Vector3(0, 0, 0), norm);
            s2.AddVertex(new Vector3(1, 0, 0), norm);
            s2.AddVertex(new Vector3(0, 1, 0), norm);
            s2.AddTriangle(1, 0, 2);

            // ACT
            bool areEqual = Surface.AreSurfaceShapesEqual(s1, s2);

            // ASSERT
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Surface_Equal_True_ForTwoTriangles_Same()
        {
            Vector3 norm = new Vector3(1, 0, 0);

            // ARRANGE
            Surface s1 = new Surface();
            s1.AddVertex(new Vector3(0, 0, 0), norm);
            s1.AddVertex(new Vector3(1, 0, 0), norm);
            s1.AddVertex(new Vector3(0, 1, 0), norm);
            s1.AddVertex(new Vector3(0, 0, 0), norm);
            s1.AddVertex(new Vector3(1, 0, 0), norm);
            s1.AddVertex(new Vector3(1, 1, 0), norm);
            s1.AddTriangle(0, 1, 2);
            s1.AddTriangle(3, 4, 5);
            Surface s2 = new Surface();
            s2.AddVertex(new Vector3(0, 0, 0), norm);
            s2.AddVertex(new Vector3(1, 0, 0), norm);
            s2.AddVertex(new Vector3(0, 1, 0), norm);
            s2.AddVertex(new Vector3(0, 0, 0), norm);
            s2.AddVertex(new Vector3(1, 0, 0), norm);
            s2.AddVertex(new Vector3(1, 1, 0), norm);
            s2.AddTriangle(0, 1, 2);
            s2.AddTriangle(3, 4, 5);

            // ACT
            bool areEqual = Surface.AreSurfaceShapesEqual(s1, s2);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Surface_Equal_True_ForTwoTriangles_FlippedTriangles()
        {
            Vector3 norm = new Vector3(1, 0, 0);

            // ARRANGE
            Surface s1 = new Surface();
            s1.AddVertex(new Vector3(0, 0, 0), norm);
            s1.AddVertex(new Vector3(1, 0, 0), norm);
            s1.AddVertex(new Vector3(0, 1, 0), norm);
            s1.AddVertex(new Vector3(0, 0, 0), norm);
            s1.AddVertex(new Vector3(1, 0, 0), norm);
            s1.AddVertex(new Vector3(1, 1, 0), norm);
            s1.AddTriangle(3, 4, 5);
            s1.AddTriangle(0, 1, 2);
            Surface s2 = new Surface();
            s2.AddVertex(new Vector3(0, 0, 0), norm);
            s2.AddVertex(new Vector3(1, 0, 0), norm);
            s2.AddVertex(new Vector3(0, 1, 0), norm);
            s2.AddVertex(new Vector3(0, 0, 0), norm);
            s2.AddVertex(new Vector3(1, 0, 0), norm);
            s2.AddVertex(new Vector3(1, 1, 0), norm);
            s2.AddTriangle(0, 1, 2);
            s2.AddTriangle(3, 4, 5);

            // ACT
            bool areEqual = Surface.AreSurfaceShapesEqual(s1, s2);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Surface_Equal_True_ForTwoTriangles_FlippedVertices()
        {
            Vector3 norm = new Vector3(1, 0, 0);

            // ARRANGE
            Surface s1 = new Surface();
            s1.AddVertex(new Vector3(0, 0, 0), norm);
            s1.AddVertex(new Vector3(1, 0, 0), norm);
            s1.AddVertex(new Vector3(1, 1, 0), norm);
            s1.AddVertex(new Vector3(0, 0, 0), norm);
            s1.AddVertex(new Vector3(1, 0, 0), norm);
            s1.AddVertex(new Vector3(0, 1, 0), norm);
            s1.AddTriangle(0, 1, 2);
            s1.AddTriangle(3, 4, 5);
            Surface s2 = new Surface();
            s2.AddVertex(new Vector3(0, 0, 0), norm);
            s2.AddVertex(new Vector3(1, 0, 0), norm);
            s2.AddVertex(new Vector3(0, 1, 0), norm);
            s2.AddVertex(new Vector3(0, 0, 0), norm);
            s2.AddVertex(new Vector3(1, 0, 0), norm);
            s2.AddVertex(new Vector3(1, 1, 0), norm);
            s2.AddTriangle(0, 1, 2);
            s2.AddTriangle(3, 4, 5);

            // ACT
            bool areEqual = Surface.AreSurfaceShapesEqual(s1, s2);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Surface_Equal_False_ForOneVsTwoTriangles()
        {
            Vector3 norm = new Vector3(1, 0, 0);

            // ARRANGE
            Surface s1 = new Surface();
            s1.AddVertex(new Vector3(0, 0, 0), norm);
            s1.AddVertex(new Vector3(1, 0, 0), norm);
            s1.AddVertex(new Vector3(1, 1, 0), norm);
            s1.AddTriangle(0, 1, 2);
            Surface s2 = new Surface();
            s2.AddVertex(new Vector3(0, 0, 0), norm);
            s2.AddVertex(new Vector3(1, 0, 0), norm);
            s2.AddVertex(new Vector3(0, 1, 0), norm);
            s2.AddVertex(new Vector3(0, 0, 0), norm);
            s2.AddVertex(new Vector3(1, 0, 0), norm);
            s2.AddVertex(new Vector3(1, 1, 0), norm);
            s2.AddTriangle(0, 1, 2);
            s2.AddTriangle(3, 4, 5);

            // ACT
            bool areEqual = Surface.AreSurfaceShapesEqual(s1, s2);

            // ASSERT
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Surface_Equal_True_ForTwoTriangles_SharedVertices()
        {
            Vector3 norm = new Vector3(1, 0, 0);

            // ARRANGE
            Surface s1 = new Surface();
            s1.AddVertex(new Vector3(0, 0, 0), norm);
            s1.AddVertex(new Vector3(1, 0, 0), norm);
            s1.AddVertex(new Vector3(0, 1, 0), norm);
            s1.AddVertex(new Vector3(1, 1, 0), norm);
            s1.AddTriangle(0, 1, 2);
            s1.AddTriangle(0, 1, 3);
            Surface s2 = new Surface();
            s2.AddVertex(new Vector3(0, 0, 0), norm);
            s2.AddVertex(new Vector3(1, 0, 0), norm);
            s2.AddVertex(new Vector3(0, 1, 0), norm);
            s2.AddVertex(new Vector3(0, 0, 0), norm);
            s2.AddVertex(new Vector3(1, 0, 0), norm);
            s2.AddVertex(new Vector3(1, 1, 0), norm);
            s2.AddTriangle(0, 1, 2);
            s2.AddTriangle(3, 4, 5);

            // ACT
            bool areEqual = Surface.AreSurfaceShapesEqual(s1, s2);

            // ASSERT
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Surface_Equal_True_ForTwoTriangles_SharedVertices2()
        {
            Vector3 norm = new Vector3(1, 0, 0);

            // ARRANGE
            Surface s1 = new Surface();
            s1.AddVertex(new Vector3(0, 0, 0), norm);
            s1.AddVertex(new Vector3(1, 0, 0), norm);
            s1.AddVertex(new Vector3(0, 1, 0), norm);
            s1.AddVertex(new Vector3(1, 1, 0), norm);
            s1.AddTriangle(0, 1, 2);
            s1.AddTriangle(0, 1, 3);
            Surface s2 = new Surface();
            s2.AddVertex(new Vector3(0, 0, 0), norm);
            s2.AddVertex(new Vector3(1, 0, 0), norm);
            s2.AddVertex(new Vector3(0, 1, 0), norm);
            s2.AddVertex(new Vector3(1, 0, 0), norm);
            s2.AddVertex(new Vector3(1, 1, 0), norm);
            s2.AddTriangle(0, 1, 2);
            s2.AddTriangle(0, 3, 4);

            // ACT
            bool areEqual = Surface.AreSurfaceShapesEqual(s1, s2);

            // ASSERT
            Assert.IsTrue(areEqual);
        }
    }
}