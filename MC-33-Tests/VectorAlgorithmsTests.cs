using MC_33;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MC_33_Tests
{
    public class VectorAlgorithmsTests
    {
        [Test]
        public void TestingFunctions_VectorsApproxEqual_TrueForAllFromMinus10To10()
        {
            for (int i = -10; i <= 10; i++)
            {
                for (int j = -10; j <= 10; j++)
                {
                    for (int k = -10; k <= 10; k++)
                    {
                        // ARRANGE
                        Vector3 v1 = new Vector3(i, j, k);
                        Vector3 v2 = new Vector3(i, j, k);

                        // ACT
                        bool areEqual = v1.EqualsApproximately(v2, 0.00000001);

                        // ASSERT
                        Assert.IsTrue(areEqual);
                    }
                }
            }
        }

        [Test]
        public void TestingFunctions_VectorsApproxEqual_FalseForAllFromMinus10To10IfNotOne()
        {
            Vector3 v1 = new Vector3(1, 1, 1);

            for (int i = -10; i <= 10; i++)
            {
                for (int j = -10; j <= 10; j++)
                {
                    for (int k = -10; k <= 10; k++)
                    {
                        if (i == 1 && j == 1 && k == 1)
                        {
                            continue;
                        }

                        // ARRANGE
                        Vector3 v2 = new Vector3(i, j, k);

                        // ACT
                        bool areEqual = v1.EqualsApproximately(v2, 0.00000001);

                        // ASSERT
                        Assert.IsFalse(areEqual);
                    }
                }
            }
        }
    }
}
