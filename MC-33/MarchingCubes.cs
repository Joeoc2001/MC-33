using System;
using System.Numerics;

namespace MC_33
{
	public static class MarchingCubes
	{
		private static int ToInt(bool b) { return b ? 1 : 0; }

		private static int SignBit(float i)
		{
			return i < 0 ? 1 : 0;
		}

		/******************************************************************
		Vertices:           Faces:
			3 __________2        ___________
		   /|          /|      /|          /|
		  / |         / |     / |   2     / |
		7/__________6/  |    /  |     4  /  |
		|   |       |   |   |¯¯¯¯¯¯¯¯¯¯¯| 1 |     z
		|   0_______|___1   | 3 |_______|___|     |
		|  /        |  /    |  /  5     |  /      |____y
		| /         | /     | /     0   | /      /
		4/__________5/      |/__________|/      x


		This function return a vector with all six test face results (face[6]). Each
		result value is 1 if the positive face vertices are joined, -1 if the negative
		vertices are joined, and 0 (unchanged) if the test must no be applied. The
		return value of this function is the the sum of all six results.*/
		private static int FaceTestAll(out int[] face, int ind, int sw, float[] v)
		{
			face = new int[6];
			if ((ind & 0x80) != 0)//vertex 0
			{
				face[0] = ((ind & 0xCC) == 0x84 ? (v[0] * v[5] < v[1] * v[4] ? -sw : sw) : 0);//0x84 = 10000100, vertices 0 and 5
				face[3] = ((ind & 0x99) == 0x81 ? (v[0] * v[7] < v[3] * v[4] ? -sw : sw) : 0);//0x81 = 10000001, vertices 0 and 7
				face[4] = ((ind & 0xF0) == 0xA0 ? (v[0] * v[2] < v[1] * v[3] ? -sw : sw) : 0);//0xA0 = 10100000, vertices 0 and 2
			}
			else
			{
				face[0] = ((ind & 0xCC) == 0x48 ? (v[0] * v[5] < v[1] * v[4] ? sw : -sw) : 0);//0x48 = 01001000, vertices 1 and 4
				face[3] = ((ind & 0x99) == 0x18 ? (v[0] * v[7] < v[3] * v[4] ? sw : -sw) : 0);//0x18 = 00011000, vertices 3 and 4
				face[4] = ((ind & 0xF0) == 0x50 ? (v[0] * v[2] < v[1] * v[3] ? sw : -sw) : 0);//0x50 = 01010000, vertices 1 and 3
			}
			if ((ind & 0x02) != 0)//vertex 6
			{
				face[1] = ((ind & 0x66) == 0x42 ? (v[1] * v[6] < v[2] * v[5] ? -sw : sw) : 0);//0x42 = 01000010, vertices 1 and 6
				face[2] = ((ind & 0x33) == 0x12 ? (v[3] * v[6] < v[2] * v[7] ? -sw : sw) : 0);//0x12 = 00010010, vertices 3 and 6
				face[5] = ((ind & 0x0F) == 0x0A ? (v[4] * v[6] < v[5] * v[7] ? -sw : sw) : 0);//0x0A = 00001010, vertices 4 and 6
			}
			else
			{
				face[1] = ((ind & 0x66) == 0x24 ? (v[1] * v[6] < v[2] * v[5] ? sw : -sw) : 0);//0x24 = 00100100, vertices 2 and 5
				face[2] = ((ind & 0x33) == 0x21 ? (v[3] * v[6] < v[2] * v[7] ? sw : -sw) : 0);//0x21 = 00100001, vertices 2 and 7
				face[5] = ((ind & 0x0F) == 0x05 ? (v[4] * v[6] < v[5] * v[7] ? sw : -sw) : 0);//0x05 = 00000101, vertices 5 and 7
			}
			return face[0] + face[1] + face[2] + face[3] + face[4] + face[5];
		}

		/* Faster function for the face test, the test is applied to only one face
		(int face). This function is only used for the cases 3 and 6 of MC33*/
		private static int FaceTestOne(int face, float[] v)
		{
			switch (face)
			{
				case 0:
					return (v[0] * v[5] < v[1] * v[4] ? 0x48 : 0x84);
				case 1:
					return (v[1] * v[6] < v[2] * v[5] ? 0x24 : 0x42);
				case 2:
					return (v[3] * v[6] < v[2] * v[7] ? 0x21 : 0x12);
				case 3:
					return (v[0] * v[7] < v[3] * v[4] ? 0x18 : 0x81);
				case 4:
					return (v[0] * v[2] < v[1] * v[3] ? 0x50 : 0xA0);
				case 5:
					return (v[4] * v[6] < v[5] * v[7] ? 0x05 : 0x0A);
				default:
					throw new ArgumentOutOfRangeException("Face index must be 0 to 5");
			}
		}

		/******************************************************************
		Interior test function. If the test is positive, the function returns a value
		different fom 0. The integer i must be 0 to test if the vertices 0 and 6 are
		joined. 1 for vertices 1 and 7, 2 for vertices 2 and 4, and 3 for 3 and 5.
		For case 13, the integer flagtplane must be 1, and the function returns 2 if
		one of the vertices 0, 1, 2 or 3 is joined to the center point of the cube
		(case 13.5.2), returns 1 if one of the vertices 4, 5, 6 or 7 is joined to the
		center point of the cube (case 13.5.2 too), and it returns 0 if the vertices
		are no joined (case 13.5.1)*/
		private static int InteriorTest(int i, int flagtplane, float[] v)
		{
			//Signs of cube vertices were changed to use signbit function in calc_isosurface
			//A0 = -v[0], B0 = -v[1], C0 = -v[2], D0 = -v[3]
			//A1 = -v[4], B1 = -v[5], C1 = -v[6], D1 = -v[7]
			//But the function still works
			float At = v[4] - v[0], Bt = v[5] - v[1],
						Ct = v[6] - v[2], Dt = v[7] - v[3];
			float t = At * Ct - Bt * Dt;//the "a" value.
			if ((i & 0x01) != 0)//false for i = 0 and 2, and true for i = 1 and 3
			{
				if (t <= 0)
				{
					return 0;
				}
			}
			else
			{
				if (t >= 0)
				{
					return 0;
				}
			}
			t = 0.5f * (v[3] * Bt + v[1] * Dt - v[2] * At - v[0] * Ct) / t;//t = -b/2a
			if (t <= 0 || t >= 1.0f)
			{
				return 0;
			}

			At = v[0] + At * t;
			Bt = v[1] + Bt * t;
			Ct = v[2] + Ct * t;
			Dt = v[3] + Dt * t;
			if ((i & 0x01) != 0)
			{
				if (At * Ct < Bt * Dt && SignBit(Bt) == SignBit(Dt))
				{
					return ToInt(SignBit(Bt) == SignBit(v[i])) + flagtplane;
				}
			}
			else
			{
				if (At * Ct > Bt * Dt && SignBit(At) == SignBit(Ct))
				{
					return ToInt(SignBit(At) == SignBit(v[i])) + flagtplane;
				}
			}
			return 0;
		}

		private static int StorePoint(IGrid grid, ISurface s, float x, float y, float z)
		{
			Vector3 pos = new Vector3(x, y, z);
			pos *= grid.Offset;
			pos += grid.Origin;

			return s.AddVertex(pos);
		}

		private static ushort[] GetCasePoints(int i, float[] cell, int mc33Case, int subCase, int lowestCaseBit, bool reverseTriangles)
		{
			switch (mc33Case)//find the MC33 case
			{
				case 1://********************************************
					return LookUpTable.Case_1[subCase];
				case 2://********************************************
					return LookUpTable.Case_2[subCase];
				case 3://********************************************
					if (((reverseTriangles ? i : i ^ 0xFF) & FaceTestOne(subCase >> 1, cell)) != 0)
					{
						return LookUpTable.Case_3_2[subCase];
					}
					else
					{
						return LookUpTable.Case_3_1[subCase];
					}
				case 4://********************************************
					if (InteriorTest(subCase, 0, cell) != 0)
					{
						return LookUpTable.Case_4_2[subCase];
					}
					else
					{
						return LookUpTable.Case_4_1[subCase];
					}
				case 5://********************************************
					return LookUpTable.Case_5[subCase];
				case 6://********************************************
					if (((reverseTriangles ? i : i ^ 0xFF) & FaceTestOne(subCase % 6, cell)) != 0)
					{
						return LookUpTable.Case_6_2[subCase];
					}
					else if (InteriorTest(subCase / 6, 0, cell) != 0)
					{
						return LookUpTable.Case_6_1_2[subCase];
					}
					else
					{
						return LookUpTable.Case_6_1_1[subCase];
					}
				case 7://********************************************
					switch (FaceTestAll(out int[] faces, i, (reverseTriangles ? 1 : -1), cell))
					{
						case -3:
							return LookUpTable.Case_7_1[subCase];
						case -1:
							if (faces[4] + faces[5] == 1)
							{
								return LookUpTable.Case_7_2_1[subCase];
							}
							else
							{
								return (faces[(33825 >> (subCase << 1)) & 3] == 1 ? LookUpTable.Case_7_2_3 : LookUpTable.Case_7_2_2)[subCase];
							}
						case 1:
							if (faces[4] + faces[5] == -1)
							{
								return LookUpTable.Case_7_3_3[subCase];
							}
							else
							{
								return (faces[(33825 >> (subCase << 1)) & 3] == 1 ? LookUpTable.Case_7_3_2 : LookUpTable.Case_7_3_1)[subCase];
							}
						case 3:
							if (InteriorTest(subCase >> 1, 0, cell) != 0)
							{
								return LookUpTable.Case_7_4_2[subCase];
							}
							else
							{
								return LookUpTable.Case_7_4_1[subCase];
							}
						default:
							throw new ArgumentOutOfRangeException("Case 7 test failed");
					}
				case 8://********************************************
					return LookUpTable.Case_8[subCase];
				case 9://********************************************
					return LookUpTable.Case_9[subCase];
				case 10://********************************************
					switch (FaceTestAll(out faces, i, (!reverseTriangles ? -1 : 1), cell))
					{
						case -2:
							if (subCase != 0 ? InteriorTest(0, 0, cell) != 0 || InteriorTest(lowestCaseBit == 0 ? 3 : 1, 0, cell) != 0 : InteriorTest(0, 0, cell) != 0)
							{
								return LookUpTable.Case_10_1_2_1[subCase];
							}
							else
							{
								return LookUpTable.Case_10_1_1_1[subCase];
							}
						case 2:
							if (subCase != 0 ? InteriorTest(2, 0, cell) != 0 || InteriorTest(lowestCaseBit == 0 ? 1 : 3, 0, cell) != 0 : InteriorTest(1, 0, cell) != 0)
							{
								return LookUpTable.Case_10_1_2_2[subCase];
							}
							else
							{
								return LookUpTable.Case_10_1_1_2[subCase];
							}
						case 0:
							return (faces[4 >> (subCase << 1)] == 1 ? LookUpTable.Case_10_2_2 : LookUpTable.Case_10_2_1)[subCase];
						default:
							throw new ArgumentOutOfRangeException("Case 10 test failed");
					}
				case 11://********************************************
					return LookUpTable.Case_11[subCase];
				case 12://********************************************
					switch (FaceTestAll(out faces, i, (reverseTriangles ? 1 : -1), cell))
					{
						case -2:
							if (InteriorTest(LookUpTable._12_test_index[0, subCase], 0, cell) != 0)
							{
								return LookUpTable.Case_12_1_2_1[subCase];
							}
							else
							{
								return LookUpTable.Case_12_1_1_1[subCase];
							}
						case 2:
							if (InteriorTest(LookUpTable._12_test_index[1, subCase], 0, cell) != 0)
							{
								return LookUpTable.Case_12_1_2_2[subCase];
							}
							else
							{
								return LookUpTable.Case_12_1_1_2[subCase];
							}
						case 0:
							if (faces[LookUpTable._12_test_index[2, subCase]] == 1)
							{
								return LookUpTable.Case_12_2_2[subCase];
							}
							else
							{
								return LookUpTable.Case_12_2_1[subCase];
							}
						default:
							throw new ArgumentOutOfRangeException("Case 12 test failed");
					}
				case 13://********************************************
					int caseCode = FaceTestAll(out faces, i, (reverseTriangles ? 1 : -1), cell);
					switch (Math.Abs(caseCode))
					{
						case 6:
							return LookUpTable.Case_13_1[(caseCode > 0 ? 1 : 0)];
						case 4:
							caseCode >>= 2;
							i = 0;
							while (faces[i] != -caseCode)
							{
								++i;
							}

							return LookUpTable.Case_13_2[(3 * caseCode + 3 + i)];
						case 2:
							caseCode = (((((((ToInt(faces[0] < 0) << 1) | ToInt(faces[1] < 0)) << 1) | ToInt(faces[2] < 0)) << 1) |
									ToInt(faces[3] < 0)) << 1) | ToInt(faces[4] < 0);
							return LookUpTable.Case_13_3[(25 - caseCode + ((ToInt(caseCode > 10) + ToInt(caseCode > 20)) << 1))];
						case 0:
							caseCode = (ToInt(faces[1] < 0) << 1) | ToInt(faces[5] < 0);
							if (faces[0] * faces[1] * faces[5] == 1)
							{
								return LookUpTable.Case_13_4[caseCode];
							}
							else
							{
								i = InteriorTest(caseCode, 1, cell);
								if (i != 0)
								{
									return LookUpTable.Case_13_5_2[(caseCode | ((i & 1) << 2))];
								}
								else
								{
									return LookUpTable.Case_13_5_1[caseCode];
								}
							}
						default:
							throw new ArgumentOutOfRangeException("Case 13 test failed");
					}
				case 14:
					return LookUpTable.Case_14[subCase];
				default:
					throw new ArgumentOutOfRangeException("MC case code out of range");
			}
		}

		private static int GetPointIndex(IGrid grid, ISurface surface, int caseCode, ref int[] pointIndices, int x, int y, int z, float[] cell,
			int[] oldLayer, int[] newLayer, int[,] oldY, int[,] newY, int[,] oldX, int[,] newX)
		{
			float t;
			int pointIndex;

			if (pointIndices[caseCode] >= 0)
			{
				return pointIndices[caseCode];
			}

			switch (caseCode)
			{
				case 0:
					if (z != 0 || x != 0)
					{
						return oldY[y, x];
					}

					if (cell[0] == 0)
					{
						pointIndex = StorePoint(grid, surface, 0, y, 0);

						if (SignBit(cell[3]) != 0)
						{
							pointIndices[3] = pointIndex;
						}

						if (SignBit(cell[4]) != 0)
						{
							pointIndices[8] = pointIndex;
						}

						return pointIndex;
					}

					if (cell[1] == 0)
					{
						pointIndex = StorePoint(grid, surface, 0, y + 1, 0);

						if (SignBit(cell[2]) != 0)
						{
							newLayer[0] = pointIndex;
							pointIndices[1] = pointIndex;
						}

						if (SignBit(cell[5]) != 0)
						{
							oldX[y + 1, 0] = pointIndex;
							pointIndices[9] = pointIndex;
						}

						return pointIndex;
					}

					t = cell[0] / (cell[0] - cell[1]);
					return StorePoint(grid, surface, 0, y + t, 0);

				case 1:
					if (x != 0)
					{
						return newLayer[x];
					}

					if (cell[1] == 0)
					{
						pointIndex = StorePoint(grid, surface, 0, y + 1, z);

						newLayer[0] = pointIndex;

						if (SignBit(cell[0]) != 0)
						{
							pointIndices[0] = pointIndex;
						}

						if (SignBit(cell[5]) != 0)
						{
							pointIndices[9] = pointIndex;
							if (z == 0)
							{
								oldX[y + 1, 0] = pointIndices[9];
							}
						}

						return pointIndex;
					}

					if (cell[2] == 0)
					{
						pointIndex = StorePoint(grid, surface, 0, y + 1, z + 1);

						newLayer[0] = pointIndex;

						if (SignBit(cell[3]) != 0)
						{
							newY[y, 0] = pointIndices[2] = pointIndex;
						}

						if (SignBit(cell[6]) != 0)
						{
							newX[y + 1, 0] = pointIndices[10] = pointIndex;
						}

						return pointIndex;
					}

					t = cell[1] / (cell[1] - cell[2]);
					pointIndex = StorePoint(grid, surface, 0, y + 1, z + t);
					newLayer[0] = pointIndex;
					return pointIndex;
				case 2:
					if (x != 0)
					{
						return newY[y, x];
					}

					if (cell[3] == 0)
					{
						pointIndex = StorePoint(grid, surface, 0, y, z + 1);
						newY[y, 0] = pointIndex;
						if (SignBit(cell[0]) != 0)
						{
							pointIndices[3] = pointIndex;
						}

						if (SignBit(cell[7]) != 0)
						{
							pointIndices[11] = pointIndex;
							if (y == 0)
							{
								newX[0, 0] = pointIndices[11];
							}
						}
						return pointIndex;
					}

					if (cell[2] == 0)
					{
						pointIndex = StorePoint(grid, surface, 0, y + 1, z + 1);
						newY[y, 0] = pointIndex;
						if (SignBit(cell[1]) != 0)
						{
							newLayer[0] = pointIndices[1] = pointIndex;
						}

						if (SignBit(cell[6]) != 0)
						{
							newX[y + 1, 0] = pointIndices[10] = pointIndex;
						}
						return pointIndex;
					}

					t = cell[3] / (cell[3] - cell[2]);
					pointIndex = StorePoint(grid, surface, 0, y + t, z + 1);
					newY[y, 0] = pointIndex;
					return pointIndex;
				case 3:
					if (y != 0 || x != 0)
					{
						return oldLayer[x];
					}
					if (cell[0] == 0)
					{
						pointIndex = StorePoint(grid, surface, 0, 0, z);
						if (SignBit(cell[1]) != 0)
						{
							pointIndices[0] = pointIndex;
						}

						if (SignBit(cell[4]) != 0)
						{
							pointIndices[8] = pointIndex;
						}
						return pointIndex;
					}
					if (cell[3] == 0)
					{
						pointIndex = StorePoint(grid, surface, 0, 0, z + 1);
						if (SignBit(cell[2]) != 0)
						{
							newY[0, 0] = pointIndices[2] = pointIndex;
						}

						if (SignBit(cell[7]) != 0)
						{
							newX[0, 0] = pointIndices[11] = pointIndex;
						}
						return pointIndex;
					}
					t = cell[0] / (cell[0] - cell[3]);
					pointIndex = StorePoint(grid, surface, 0, 0, z + t);
					return pointIndex;
				case 4:
					if (z != 0)
					{
						return oldY[y, x + 1];
					}
					if (cell[4] == 0)
					{
						pointIndex = StorePoint(grid, surface, x + 1, y, 0);
						oldY[y, x + 1] = pointIndex;
						if (SignBit(cell[7]) != 0)
						{
							pointIndices[7] = pointIndex;
						}

						if (SignBit(cell[0]) != 0)
						{
							pointIndices[8] = pointIndex;
						}

						if (y == 0)
						{
							oldLayer[x + 1] = pointIndices[7];
						}
						return pointIndex;
					}
					if (cell[5] == 0)
					{
						pointIndex = StorePoint(grid, surface, x + 1, y + 1, 0);
						oldY[y, x + 1] = pointIndex;
						if (SignBit(cell[6]) != 0)
						{
							newLayer[x + 1] = pointIndices[5] = pointIndex;
						}

						if (SignBit(cell[1]) != 0)
						{
							oldX[y + 1, x] = pointIndices[9] = pointIndex;
						}
						return pointIndex;
					}
					t = cell[4] / (cell[4] - cell[5]);
					pointIndex = StorePoint(grid, surface, x + 1, y + t, 0);
					oldY[y, x + 1] = pointIndex;
					return pointIndex;
				case 5:
					if (cell[5] == 0)
					{
						if (SignBit(cell[4]) != 0)
						{
							if (z != 0)
							{
								newLayer[x + 1] = pointIndex = pointIndices[4] = oldY[y, x + 1];
								if (SignBit(cell[1]) != 0)
								{
									pointIndices[9] = pointIndex;
								}
								return pointIndex;
							}

							newLayer[x + 1] = pointIndex = oldY[y, x + 1] = pointIndices[4] = StorePoint(grid, surface, x + 1, y + 1, 0);
							if (SignBit(cell[1]) != 0)
							{
								oldX[y + 1, x] = pointIndices[9] = pointIndex;
							}
							return pointIndex;
						}

						if (SignBit(cell[1]) != 0)
						{
							if (z != 0)
							{
								newLayer[x + 1] = pointIndex = pointIndices[9] = oldX[y + 1, x];
								return pointIndex;
							}
							newLayer[x + 1] = pointIndex = oldX[y + 1, x] = pointIndices[9] = StorePoint(grid, surface, x + 1, y + 1, 0);
							return pointIndex;
						}

						newLayer[x + 1] = pointIndex = StorePoint(grid, surface, x + 1, y + 1, z);
						return pointIndex;
					}
					if (cell[6] == 0)
					{
						newLayer[x + 1] = pointIndex = StorePoint(grid, surface, x + 1, y + 1, z + 1);
						if (SignBit(cell[2]) != 0)
						{
							newX[y + 1, x] = pointIndices[10] = pointIndex;
						}

						if (SignBit(cell[7]) != 0)
						{
							newY[y, x + 1] = pointIndices[6] = pointIndex;
						}

						return pointIndex;
					}
					t = cell[5] / (cell[5] - cell[6]);
					newLayer[x + 1] = pointIndex = StorePoint(grid, surface, x + 1, y + 1, z + t);
					return pointIndex;
				case 6:
					if (cell[7] == 0)
					{
						if (SignBit(cell[3]) != 0)
						{
							if (y != 0)
							{
								newY[y, x + 1] = pointIndex = pointIndices[11] = newX[y, x];
								if (SignBit(cell[4]) != 0)
								{
									pointIndices[7] = pointIndex;
								}
								return pointIndex;
							}
							newY[y, x + 1] = pointIndex = newX[0, x] = pointIndices[11] = StorePoint(grid, surface, x + 1, 0, z + 1);
							if (SignBit(cell[4]) != 0)
							{
								oldLayer[x + 1] = pointIndices[7] = pointIndex;
							}
							return pointIndex;
						}
						if (SignBit(cell[4]) != 0)
						{
							if (y != 0)
							{
								newY[y, x + 1] = pointIndex = pointIndices[7] = oldLayer[x + 1];
								return pointIndex;
							}

							newY[y, x + 1] = pointIndex = oldLayer[x + 1] = pointIndices[7] = StorePoint(grid, surface, x + 1, 0, z + 1);
							return pointIndex;
						}

						newY[y, x + 1] = pointIndex = StorePoint(grid, surface, x + 1, y, z + 1);
						return pointIndex;
					}

					if (cell[6] == 0)
					{
						newY[y, x + 1] = pointIndex = StorePoint(grid, surface, x + 1, y + 1, z + 1);
						if (SignBit(cell[5]) != 0)
						{
							newLayer[x + 1] = pointIndices[5] = pointIndex;
						}

						if (SignBit(cell[2]) != 0)
						{
							newX[y + 1, x] = pointIndices[10] = pointIndex;
						}

						return pointIndex;
					}
					t = cell[7] / (cell[7] - cell[6]);
					newY[y, x + 1] = pointIndex = StorePoint(grid, surface, x + 1, y + t, z + 1);

					return pointIndex;
				case 7:
					if (y != 0)
					{
						return oldLayer[x + 1];
					}
					if (cell[4] == 0)
					{
						oldLayer[x + 1] = pointIndex = StorePoint(grid, surface, x + 1, 0, z);
						if (SignBit(cell[0]) != 0)
						{
							pointIndices[8] = pointIndex;
						}

						if (SignBit(cell[5]) != 0)
						{
							pointIndices[4] = pointIndex;
							if (z == 0)
							{
								oldY[0, x + 1] = pointIndices[4];
							}
						}

						return pointIndex;
					}
					if (cell[7] == 0)
					{
						oldLayer[x + 1] = pointIndex = StorePoint(grid, surface, x + 1, 0, z + 1);
						if (SignBit(cell[6]) != 0)
						{
							newY[0, x + 1] = pointIndices[6] = pointIndex;
						}

						if (SignBit(cell[3]) != 0)
						{
							newX[0, x] = pointIndices[11] = pointIndex;
						}

						return pointIndex;
					}
					t = cell[4] / (cell[4] - cell[7]);
					oldLayer[x + 1] = pointIndex = StorePoint(grid, surface, x + 1, 0, z + t);
					return pointIndex;
				case 8:
					if (z != 0 || y != 0)
					{
						return oldX[y, x];
					}
					if (cell[0] == 0)
					{
						pointIndex = StorePoint(grid, surface, x, 0, 0);
						if (SignBit(cell[1]) != 0)
						{
							pointIndices[0] = pointIndex;
						}

						if (SignBit(cell[3]) != 0)
						{
							pointIndices[3] = pointIndex;
						}

						return pointIndex;
					}
					if (cell[4] == 0)
					{
						pointIndex = StorePoint(grid, surface, x + 1, 0, 0);
						if (SignBit(cell[5]) != 0)
						{
							oldY[0, x + 1] = pointIndices[4] = pointIndex;
						}

						if (SignBit(cell[7]) != 0)
						{
							oldLayer[x + 1] = pointIndices[7] = pointIndex;
						}
						return pointIndex;
					}
					t = cell[0] / (cell[0] - cell[4]);
					pointIndex = StorePoint(grid, surface, x + t, 0, 0);
					return pointIndex;
				case 9:
					if (z != 0)
					{
						return oldX[y + 1, x];
					}
					if (cell[1] == 0)
					{
						oldX[y + 1, x] = pointIndex = StorePoint(grid, surface, x, y + 1, 0);
						if (SignBit(cell[2]) != 0)
						{
							pointIndices[1] = pointIndex;
							if (x == 0)
							{
								newLayer[0] = pointIndices[1];
							}
						}
						if (SignBit(cell[0]) != 0)
						{
							pointIndices[0] = pointIndex;
						}
						return pointIndex;
					}
					if (cell[5] == 0)
					{
						oldX[y + 1, x] = pointIndex = StorePoint(grid, surface, x + 1, y + 1, 0);
						if (SignBit(cell[6]) != 0)
						{
							newLayer[x + 1] = pointIndices[5] = pointIndex;
						}

						if (SignBit(cell[4]) != 0)
						{
							oldY[y, x + 1] = pointIndices[4] = pointIndex;
						}
						return pointIndex;
					}
					t = cell[1] / (cell[1] - cell[5]);
					oldX[y + 1, x] = pointIndex = StorePoint(grid, surface, x + t, y + 1, 0);
					return pointIndex;
				case 10:
					if (cell[2] == 0)
					{
						if (SignBit(cell[1]) != 0)
						{
							if (x != 0)
							{
								newX[y + 1, x] = pointIndex = pointIndices[1] = newLayer[x];
								if (SignBit(cell[3]) != 0)
								{
									pointIndices[2] = pointIndex;
								}
								return pointIndex;
							}
							newX[y + 1, 0] = pointIndex = newLayer[0] = pointIndices[1] = StorePoint(grid, surface, 0, y + 1, z + 1);
							if (SignBit(cell[3]) != 0)
							{
								newY[y, 0] = pointIndices[2] = pointIndex;
							}
							return pointIndex;
						}
						if (SignBit(cell[3]) != 0)
						{
							if (x != 0)
							{
								newX[y + 1, x] = pointIndex = pointIndices[2] = newY[y, x];
								return pointIndex;
							}
							newX[y + 1, 0] = pointIndex = newY[y, 0] = pointIndices[2] = StorePoint(grid, surface, 0, y + 1, z + 1);
							return pointIndex;
						}
						newX[y + 1, x] = pointIndex = StorePoint(grid, surface, x, y + 1, z + 1);
						return pointIndex;
					}
					if (cell[6] == 0)
					{
						newX[y + 1, x] = pointIndex = StorePoint(grid, surface, x + 1, y + 1, z + 1);
						if (SignBit(cell[5]) != 0)
						{
							newLayer[x + 1] = pointIndices[5] = pointIndex;
						}

						if (SignBit(cell[7]) != 0)
						{
							newY[y, x + 1] = pointIndices[6] = pointIndex;
						}
						return pointIndex;
					}
					t = cell[2] / (cell[2] - cell[6]);
					newX[y + 1, x] = pointIndex = StorePoint(grid, surface, x + t, y + 1, z + 1);
					return pointIndex;
				case 11:
					if (y != 0)
					{
						return newX[y, x];
					}
					if (cell[3] == 0)
					{
						newX[0, x] = pointIndex = StorePoint(grid, surface, x, 0, z + 1);
						if (SignBit(cell[0]) != 0)
						{
							pointIndices[3] = pointIndex;
						}

						if (SignBit(cell[2]) != 0)
						{
							pointIndices[2] = pointIndex;
							if (x == 0)
							{
								newY[0, 0] = pointIndices[2];
							}
						}

						return pointIndex;
					}
					if (cell[7] == 0)
					{
						newX[0, x] = pointIndex = StorePoint(grid, surface, x + 1, 0, z + 1);
						if (SignBit(cell[4]) != 0)
						{
							oldLayer[x + 1] = pointIndices[7] = pointIndex;
						}

						if (SignBit(cell[6]) != 0)
						{
							newY[0, x + 1] = pointIndices[6] = pointIndex;
						}
						return pointIndex;
					}
					t = cell[3] / (cell[3] - cell[7]);
					newX[0, x] = pointIndex = StorePoint(grid, surface, x + t, 0, z + 1);
					return pointIndex;
				case 12:
					return StorePoint(grid, surface, x + 0.5f, y + 0.5f, z + 0.5f);
				default:
					throw new ArgumentOutOfRangeException("Point case can only be 0-12");
			}
		}

		/******************************************************************
		This function find the MC33 case (using the index i, and the face and interior
		tests). The correct triangle pattern is obtained from the arrays contained in
		the MC33_LookUpTable.h file. The necessary vertices (intersection points) are
		also calculated here (using trilinear interpolation).
			   _____2_____
			 /|          /|
		   11 |<-3     10 |
		   /____6_____ /  1     z
		  |   |       |   |     |
		  |   |_____0_|___|     |____y
		  7  /        5  /     /
		  | 8         | 9     x
		  |/____4_____|/

		*/

		private static void FindCase(IGrid grid, ISurface surface, int x, int y, int z, int i, float[] cell,
			int[] oldLayer, int[] newLayer, int[,] oldY, int[,] newY, int[,] oldX, int[,] newX)
		{
			int vertexIndex;
			bool reverseTriangles = (i & 0x80) > 0;

			int caseCode = LookUpTable.Case_Index[!reverseTriangles ? i : i ^ 0xFF];
			int mc33Case = caseCode >> 8;
			int subCase = caseCode & 0x7F;
			int lowestCaseBit = caseCode & 0x01;

			if ((caseCode & 0x0080) != 0)
			{
				reverseTriangles = !reverseTriangles;
			}

			ushort[] casePoints = GetCasePoints(i, cell, mc33Case, subCase, lowestCaseBit, reverseTriangles);

			int[] pointIndices = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
			foreach (ushort caseItem in casePoints)
			{
				int[] triangle = new int[3];
				for (vertexIndex = 0; vertexIndex < 3; ++vertexIndex)
				{
					caseCode = (caseItem >> (4 * vertexIndex)) & 0x0F;

					int pointIndex = GetPointIndex(grid, surface, caseCode, ref pointIndices, x, y, z, cell, oldLayer, newLayer, oldY, newY, oldX, newX);
					pointIndices[caseCode] = pointIndex;
					triangle[vertexIndex] = pointIndex;
				}
				if (triangle[0] != triangle[1] && triangle[0] != triangle[2] && triangle[1] != triangle[2])//to avoid zero area triangles
				{
					if (reverseTriangles)//The order of triangle vertices is reversed if m is not zero
					{
						triangle[2] = triangle[0]; 
						triangle[0] = pointIndices[caseCode];
					}
					surface.AddTriangle(triangle[0], triangle[1], triangle[2]);
				}
			}
		}

		private static int GetLayerIntoVertices(ref float[] vertices, float iso, IGrid grid, int x, int y, int z, int oldI)
		{
			vertices[0] = vertices[4];
			vertices[1] = vertices[5];
			vertices[2] = vertices[6];
			vertices[3] = vertices[7];

			vertices[4] = iso - grid[x, y, z];
			vertices[5] = iso - grid[x, y + 1, z];
			vertices[6] = iso - grid[x, y + 1, z + 1];
			vertices[7] = iso - grid[x, y, z + 1];

			return ((((((((oldI & 0x0F) << 1) | SignBit(vertices[4])) << 1) | SignBit(vertices[5])) << 1) | SignBit(vertices[6])) << 1) | SignBit(vertices[7]);
		}

		public static void MarchIntoSurface(IGrid grid, float iso, ISurface surface)
		{
			int nx = grid.SizeX;
			int ny = grid.SizeY;
			int nz = grid.SizeZ;
			float[] vertices = new float[8];
			int[] oldLayer = new int[nx + 1];
			int[] newLayer = new int[nx + 1];
			int[,] oldY = new int[ny, nx + 1];
			int[,] newY = new int[ny, nx + 1];
			int[,] oldX = new int[ny + 1, nx];
			int[,] newX = new int[ny + 1, nx];
			for (int z = 0; z < nz; z++)
			{
				for (int y = 0; y < ny; y++)
				{
					//the eight least significant bits of i correspond to vertex indices. (x...x01234567)
					//If the bit is 1 then the vertex value is greater than zero.
					int i = GetLayerIntoVertices(ref vertices, iso, grid, 0, y, z, 0);
					for (int x = 0; x < nx; x++)
					{
						i = GetLayerIntoVertices(ref vertices, iso, grid, x + 1, y, z, i);
						if (i != 0 && i != 0xFF)//i is different from 0 and 0xFF
						{
							FindCase(grid, surface, x, y, z, i, vertices, oldLayer, newLayer, oldY, newY, oldX, newX);
						}
					}

					int[] _Ltmp = oldLayer;
					oldLayer = newLayer;
					newLayer = _Ltmp;
				}
				int[,] _tmp = oldX;
				oldX = newX;
				newX = _tmp;

				_tmp = oldY;
				oldY = newY;
				newY = _tmp;
			}
		}
	}
}
