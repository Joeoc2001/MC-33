using System;
using System.Numerics;

namespace MC_33
{
	internal static class MarchingCubes
	{
		private static int ToInt(bool b) { return b ? 1 : 0; }

		private static int SignBit(float i)
		{
			return Math.Sign(i) < 0 ? 1 : 0;
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
		static int FaceTestAll(int[] face, int ind, int sw, float[] v)
		{
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
				face[5] = ((ind & 0) == 0x0A ? (v[4] * v[6] < v[5] * v[7] ? -sw : sw) : 0);//0x0A = 00001010, vertices 4 and 6
			}
			else
			{
				face[1] = ((ind & 0x66) == 0x24 ? (v[1] * v[6] < v[2] * v[5] ? sw : -sw) : 0);//0x24 = 00100100, vertices 2 and 5
				face[2] = ((ind & 0x33) == 0x21 ? (v[3] * v[6] < v[2] * v[7] ? sw : -sw) : 0);//0x21 = 00100001, vertices 2 and 7
				face[5] = ((ind & 0) == 0x05 ? (v[4] * v[6] < v[5] * v[7] ? sw : -sw) : 0);//0x05 = 00000101, vertices 5 and 7
			}
			return face[0] + face[1] + face[2] + face[3] + face[4] + face[5];
		}

		/* Faster function for the face test, the test is applied to only one face
		(int face). This function is only used for the cases 3 and 6 of MC33*/
		static int FaceTestOne(int face, float[] v)
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
			}
			return 0;
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
		static int InteriorTest(int i, int flagtplane, float[] v)
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

		static int StorePoint(Grid grid, Surface s, float[] r)
		{
			return StorePoint(grid, s, r[0], r[1], r[2]);
		}

		static int StorePoint(Grid grid, Surface s, float x, float y, float z)
		{
			Vector3 pos = new Vector3(x, y, z);
			pos *= grid.Offset;
			pos += grid.Origin;

			return s.AddVertex(pos);
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

		static void FindCase(Grid grid, Surface surface, int x, int y, int z, int i, float[] cell,
			int[] oldLayer, int[] newLayer, int[,] oldY, int[,] newY, int[,] oldX, int[,] newX)
		{
			ushort[] casePoints = new ushort[0];
			float t;
			float[] point = new float[3];
			int k;
			int[] faces = new int[6];//for the face tests
			int[] pointIndices = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
			int reverseTriangles = i & 0x80;
			int caseCode = LookUpTable.Case_Index[reverseTriangles == 0 ? i : i ^ 0xFF];
			switch (caseCode >> 8)//find the MC33 case
			{
				case 1://********************************************
					if ((caseCode & 0x0080) != 0)
					{
						reverseTriangles ^= 0x80;
					}

					casePoints = LookUpTable.Case_1[(caseCode & 0x7F)];
					break;
				case 2://********************************************
					if ((caseCode & 0x0080) != 0)
					{
						reverseTriangles ^= 0x80;
					}

					casePoints = LookUpTable.Case_2[(caseCode & 0x7F)];
					break;
				case 3://********************************************
					if ((caseCode & 0x0080) != 0)
					{
						reverseTriangles ^= 0x80;
					}

					if (((reverseTriangles != 0 ? i : i ^ 0xFF) & FaceTestOne((caseCode & 0x7F) >> 1, cell)) != 0)
					{
						casePoints = LookUpTable.Case_3_2[(caseCode & 0x7F)];
					}
					else
					{
						casePoints = LookUpTable.Case_3_1[(caseCode & 0x7F)];
					}

					break;
				case 4://********************************************
					if ((caseCode & 0x0080) != 0)
					{
						reverseTriangles ^= 0x80;
					}

					if (InteriorTest((caseCode & 0x7F), 0, cell) != 0)
					{
						casePoints = LookUpTable.Case_4_2[(caseCode & 0x7F)];
					}
					else
					{
						casePoints = LookUpTable.Case_4_1[(caseCode & 0x7F)];
					}

					break;
				case 5://********************************************
					if ((caseCode & 0x0080) != 0)
					{
						reverseTriangles ^= 0x80;
					}

					casePoints = LookUpTable.Case_5[(caseCode & 0x7F)];
					break;
				case 6://********************************************
					if ((caseCode & 0x0080) != 0)
					{
						reverseTriangles ^= 0x80;
					}

					if (((reverseTriangles != 0 ? i : i ^ 0xFF) & FaceTestOne((caseCode & 0x7F) % 6, cell)) != 0)
					{
						casePoints = LookUpTable.Case_6_2[(caseCode & 0x7F)];
					}
					else if (InteriorTest((caseCode & 0x7F) / 6, 0, cell) != 0)
					{
						casePoints = LookUpTable.Case_6_1_2[(caseCode & 0x7F)];
					}
					else
					{
						casePoints = LookUpTable.Case_6_1_1[(caseCode & 0x7F)];
					}

					break;
				case 7://********************************************
					if ((caseCode & 0x0080) != 0)
					{
						reverseTriangles ^= 0x80;
					}

					switch (FaceTestAll(faces, i, (reverseTriangles != 0 ? 1 : -1), cell))
					{
						case -3:
							casePoints = LookUpTable.Case_7_1[(caseCode & 0x7F)];
							break;
						case -1:
							if (faces[4] + faces[5] == 1)
							{
								casePoints = LookUpTable.Case_7_2_1[(caseCode & 0x7F)];
							}
							else
							{
								casePoints = (faces[(33825 >> ((caseCode & 0x7F) << 1)) & 3] == 1 ? LookUpTable.Case_7_2_3 : LookUpTable.Case_7_2_2)[(caseCode & 0x7F)];
							}

							break;
						case 1:
							if (faces[4] + faces[5] == -1)
							{
								casePoints = LookUpTable.Case_7_3_3[(caseCode & 0x7F)];
							}
							else
							{
								casePoints = (faces[(33825 >> ((caseCode & 0x7F) << 1)) & 3] == 1 ? LookUpTable.Case_7_3_2 : LookUpTable.Case_7_3_1)[(caseCode & 0x7F)];
							}

							break;
						case 3:
							if (InteriorTest((caseCode & 0x7F) >> 1, 0, cell) != 0)
							{
								casePoints = LookUpTable.Case_7_4_2[(caseCode & 0x7F)];
							}
							else
							{
								casePoints = LookUpTable.Case_7_4_1[(caseCode & 0x7F)];
							}

							break;
					}
					break;
				case 8://********************************************
					casePoints = LookUpTable.Case_8[(caseCode & 0x7F)];
					break;
				case 9://********************************************
					casePoints = LookUpTable.Case_9[(caseCode & 0x7F)];
					break;
				case 10://********************************************
					switch (FaceTestAll(faces, i, (reverseTriangles == 0 ? -1 : 1), cell))
					{
						case -2:
							if ((caseCode & 0x7F) != 0 ? InteriorTest(0, 0, cell) != 0 || InteriorTest((caseCode & 0x01) == 0 ? 3 : 1, 0, cell) != 0 : InteriorTest(0, 0, cell) != 0)
							{
								casePoints = LookUpTable.Case_10_1_2_1[(caseCode & 0x7F)];
							}
							else
							{
								casePoints = LookUpTable.Case_10_1_1_1[(caseCode & 0x7F)];
							}

							break;
						case 2:
							if ((caseCode & 0x7F) != 0 ? InteriorTest(2, 0, cell) != 0 || InteriorTest((caseCode & 0x01) != 0 ? 3 : 1, 0, cell) != 0 : InteriorTest(1, 0, cell) != 0)
							{
								casePoints = LookUpTable.Case_10_1_2_2[(caseCode & 0x7F)];
							}
							else
							{
								casePoints = LookUpTable.Case_10_1_1_2[(caseCode & 0x7F)];
							}

							break;
						case 0:
							casePoints = (faces[4 >> ((caseCode & 0x7F) << 1)] == 1 ? LookUpTable.Case_10_2_2 : LookUpTable.Case_10_2_1)[(caseCode & 0x7F)];
							break;
					}
					break;
				case 11://********************************************
					casePoints = LookUpTable.Case_11[(caseCode & 0x7F)];
					break;
				case 12://********************************************
					switch (FaceTestAll(faces, i, (reverseTriangles != 0 ? 1 : -1), cell))
					{
						case -2:
							if (InteriorTest(LookUpTable._12_test_index[0, caseCode & 0x7F], 0, cell) != 0)
							{
								casePoints = LookUpTable.Case_12_1_2_1[(caseCode & 0x7F)];
							}
							else
							{
								casePoints = LookUpTable.Case_12_1_1_1[(caseCode & 0x7F)];
							}

							break;
						case 2:
							if (InteriorTest(LookUpTable._12_test_index[1, caseCode & 0x7F], 0, cell) != 0)
							{
								casePoints = LookUpTable.Case_12_1_2_2[(caseCode & 0x7F)];
							}
							else
							{
								casePoints = LookUpTable.Case_12_1_1_2[(caseCode & 0x7F)];
							}

							break;
						case 0:
							casePoints = (faces[LookUpTable._12_test_index[2, caseCode & 0x7F]] == 1 ? LookUpTable.Case_12_2_2 : LookUpTable.Case_12_2_1)[(caseCode & 0x7F)];
							break;
					}
					break;
				case 13://********************************************
					caseCode = FaceTestAll(faces, i, (reverseTriangles != 0 ? 1 : -1), cell);
					switch (Math.Abs(caseCode))
					{
						case 6:
							casePoints = LookUpTable.Case_13_1[(caseCode > 0 ? 1 : 0)];
							break;
						case 4:
							caseCode >>= 2;
							i = 0;
							while (faces[i] != -caseCode)
							{
								++i;
							}

							casePoints = LookUpTable.Case_13_2[(3 * caseCode + 3 + i)];
							break;
						case 2:
							caseCode = (((((((ToInt(faces[0] < 0) << 1) | ToInt(faces[1] < 0)) << 1) | ToInt(faces[2] < 0)) << 1) |
									ToInt(faces[3] < 0)) << 1) | ToInt(faces[4] < 0);
							casePoints = LookUpTable.Case_13_3[(25 - caseCode + ((ToInt(caseCode > 10) + ToInt(caseCode > 20)) << 1))];
							break;
						case 0:
							caseCode = (ToInt(faces[1] < 0) << 1) | ToInt(faces[5] < 0);
							if (faces[0] * faces[1] * faces[5] == 1)
							{
								casePoints = LookUpTable.Case_13_4[caseCode];
							}
							else
							{
								i = InteriorTest(caseCode, 1, cell);
								if (i != 0)
								{
									casePoints = LookUpTable.Case_13_5_2[(caseCode | ((i & 1) << 2))];
								}
								else
								{
									casePoints = LookUpTable.Case_13_5_1[caseCode];
								}
							}
							break;
					}
					break;
				case 14:
					casePoints = LookUpTable.Case_14[(caseCode & 0x7F)];
					break;
			}
			foreach (ushort caseItem in casePoints)
			{
				ushort j = caseItem;
				for (k = 0; k < 3; ++k)
				{
					caseCode = j & 0;
					j >>= 4;
					if (pointIndices[caseCode] < 0)
					{
						switch (caseCode)//the vertices r[3] and normals n[3] are calculated here
						{
							case 0:
								if (z != 0 || x != 0)
								{
									pointIndices[0] = oldY[y, x];
								}
								else
								{
									if (cell[0] == 0)
									{
										pointIndices[0] = StorePoint(grid, surface, 0, y, 0);
										if (SignBit(cell[3]) != 0)
										{
											pointIndices[3] = pointIndices[0];
										}

										if (SignBit(cell[4]) != 0)
										{
											pointIndices[8] = pointIndices[0];
										}
									}
									else if (cell[1] == 0)
									{
										pointIndices[0] = StorePoint(grid, surface, 0, y + 1, 0);
										if (SignBit(cell[2]) != 0)
										{
											newLayer[0] = pointIndices[1] = pointIndices[0];
										}

										if (SignBit(cell[5]) != 0)
										{
											oldX[y + 1, 0] = pointIndices[9] = pointIndices[0];
										}
									}
									else
									{
										t = cell[0] / (cell[0] - cell[1]);
										point[0] = point[2] = 0;
										point[1] = y + t;
										pointIndices[0] = StorePoint(grid, surface, point);
									}
								}
								break;
							case 1:
								if (x != 0)
								{
									pointIndices[1] = newLayer[x];
								}
								else
								{
									if (cell[1] == 0)
									{
										newLayer[0] = pointIndices[1] = StorePoint(grid, surface, 0, y + 1, z);
										if (SignBit(cell[0]) != 0)
										{
											pointIndices[0] = pointIndices[1];
										}

										if (SignBit(cell[5]) != 0)
										{
											pointIndices[9] = pointIndices[1];
											if (z == 0)
											{
												oldX[y + 1, 0] = pointIndices[9];
											}
										}
									}
									else if (cell[2] == 0)
									{
										newLayer[0] = pointIndices[1] = StorePoint(grid, surface, 0, y + 1, z + 1);
										if (SignBit(cell[3]) != 0)
										{
											newY[y, 0] = pointIndices[2] = pointIndices[1];
										}

										if (SignBit(cell[6]) != 0)
										{
											newX[y + 1, 0] = pointIndices[10] = pointIndices[1];
										}
									}
									else
									{
										t = cell[1] / (cell[1] - cell[2]);
										point[0] = 0;
										point[1] = y + 1;
										point[2] = z + t;
										newLayer[0] = pointIndices[1] = StorePoint(grid, surface, point);
									}
								}
								break;
							case 2:
								if (x != 0)
								{
									pointIndices[2] = newY[y, x];
								}
								else
								{
									if (cell[3] == 0)
									{
										newY[y, 0] = pointIndices[2] = StorePoint(grid, surface, 0, y, z + 1);
										if (SignBit(cell[0]) != 0)
										{
											pointIndices[3] = pointIndices[2];
										}

										if (SignBit(cell[7]) != 0)
										{
											pointIndices[11] = pointIndices[2];
											if (y == 0)
											{
												newX[0, 0] = pointIndices[11];
											}
										}
									}
									else if (cell[2] == 0)
									{
										newY[y, 0] = pointIndices[2] = StorePoint(grid, surface, 0, y + 1, z + 1);
										if (SignBit(cell[1]) != 0)
										{
											newLayer[0] = pointIndices[1] = pointIndices[2];
										}

										if (SignBit(cell[6]) != 0)
										{
											newX[y + 1, 0] = pointIndices[10] = pointIndices[2];
										}
									}
									else
									{
										t = cell[3] / (cell[3] - cell[2]);
										point[0] = 0;
										point[2] = z + 1;
										point[1] = y + t;
										newY[y, 0] = pointIndices[2] = StorePoint(grid, surface, point);
									}
								}
								break;
							case 3:
								if (y != 0 || x != 0)
								{
									pointIndices[3] = oldLayer[x];
								}
								else
								{
									if (cell[0] == 0)
									{
										pointIndices[3] = StorePoint(grid, surface, 0, 0, z);
										if (SignBit(cell[1]) != 0)
										{
											pointIndices[0] = pointIndices[3];
										}

										if (SignBit(cell[4]) != 0)
										{
											pointIndices[8] = pointIndices[3];
										}
									}
									else if (cell[3] == 0)
									{
										pointIndices[3] = StorePoint(grid, surface, 0, 0, z + 1);
										if (SignBit(cell[2]) != 0)
										{
											newY[0, 0] = pointIndices[2] = pointIndices[3];
										}

										if (SignBit(cell[7]) != 0)
										{
											newX[0, 0] = pointIndices[11] = pointIndices[3];
										}
									}
									else
									{
										t = cell[0] / (cell[0] - cell[3]);
										point[0] = point[1] = 0;
										point[2] = z + t;
										pointIndices[3] = StorePoint(grid, surface, point);
									}
								}
								break;
							case 4:
								if (z != 0)
								{
									pointIndices[4] = oldY[y, x + 1];
								}
								else
								{
									if (cell[4] == 0)
									{
										oldY[y, x + 1] = pointIndices[4] = StorePoint(grid, surface, x + 1, y, 0);
										if (SignBit(cell[7]) != 0)
										{
											pointIndices[7] = pointIndices[4];
										}

										if (SignBit(cell[0]) != 0)
										{
											pointIndices[8] = pointIndices[4];
										}

										if (y == 0)
										{
											oldLayer[x + 1] = pointIndices[7];
										}
									}
									else if (cell[5] == 0)
									{
										oldY[y, x + 1] = pointIndices[4] = StorePoint(grid, surface, x + 1, y + 1, 0);
										if (SignBit(cell[6]) != 0)
										{
											newLayer[x + 1] = pointIndices[5] = pointIndices[4];
										}

										if (SignBit(cell[1]) != 0)
										{
											oldX[y + 1, x] = pointIndices[9] = pointIndices[4];
										}
									}
									else
									{
										t = cell[4] / (cell[4] - cell[5]);
										point[0] = x + 1;
										point[2] = 0;
										point[1] = y + t;
										oldY[y, x + 1] = pointIndices[4] = StorePoint(grid, surface, point);
									}
								}
								break;
							case 5:
								if (cell[5] == 0)
								{
									if (SignBit(cell[4]) != 0)
									{
										if (z != 0)
										{
											newLayer[x + 1] = pointIndices[5] = pointIndices[4] = oldY[y, x + 1];
											if (SignBit(cell[1]) != 0)
											{
												pointIndices[9] = pointIndices[5];
											}
										}
										else
										{
											newLayer[x + 1] = pointIndices[5] = oldY[y, x + 1] = pointIndices[4] = StorePoint(grid, surface, x + 1, y + 1, 0);
											if (SignBit(cell[1]) != 0)
											{
												oldX[y + 1, x] = pointIndices[9] = pointIndices[5];
											}
										}
									}
									else if (SignBit(cell[1]) != 0)
									{
										if (z != 0)
										{
											newLayer[x + 1] = pointIndices[5] = pointIndices[9] = oldX[y + 1, x];
										}
										else
										{
											newLayer[x + 1] = pointIndices[5] = oldX[y + 1, x] = pointIndices[9] = StorePoint(grid, surface, x + 1, y + 1, 0);
										}
									}
									else
									{
										newLayer[x + 1] = pointIndices[5] = StorePoint(grid, surface, x + 1, y + 1, z);
									}
								}
								else if (cell[6] == 0)
								{
									newLayer[x + 1] = pointIndices[5] = StorePoint(grid, surface, x + 1, y + 1, z + 1);
									if (SignBit(cell[2]) != 0)
									{
										newX[y + 1, x] = pointIndices[10] = pointIndices[5];
									}

									if (SignBit(cell[7]) != 0)
									{
										newY[y, x + 1] = pointIndices[6] = pointIndices[5];
									}
								}
								else
								{
									t = cell[5] / (cell[5] - cell[6]);
									point[0] = x + 1;
									point[1] = y + 1;
									point[2] = z + t;
									newLayer[x + 1] = pointIndices[5] = StorePoint(grid, surface, point);
								}
								break;
							case 6:
								if (cell[7] == 0)
								{
									if (SignBit(cell[3]) != 0)
									{
										if (y != 0)
										{
											newY[y, x + 1] = pointIndices[6] = pointIndices[11] = newX[y, x];
											if (SignBit(cell[4]) != 0)
											{
												pointIndices[7] = pointIndices[6];
											}
										}
										else
										{
											newY[y, x + 1] = pointIndices[6] = newX[0, x] = pointIndices[11] = StorePoint(grid, surface, x + 1, 0, z + 1);
											if (SignBit(cell[4]) != 0)
											{
												oldLayer[x + 1] = pointIndices[7] = pointIndices[6];
											}
										}
									}
									else if (SignBit(cell[4]) != 0)
									{
										if (y != 0)
										{
											newY[y, x + 1] = pointIndices[6] = pointIndices[7] = oldLayer[x + 1];
										}
										else
										{
											newY[y, x + 1] = pointIndices[6] = oldLayer[x + 1] = pointIndices[7] = StorePoint(grid, surface, x + 1, 0, z + 1);
										}
									}
									else
									{
										newY[y, x + 1] = pointIndices[6] = StorePoint(grid, surface, x + 1, y, z + 1);
									}
								}
								else if (cell[6] == 0)
								{
									newY[y, x + 1] = pointIndices[6] = StorePoint(grid, surface, x + 1, y + 1, z + 1);
									if (SignBit(cell[5]) != 0)
									{
										newLayer[x + 1] = pointIndices[5] = pointIndices[6];
									}

									if (SignBit(cell[2]) != 0)
									{
										newX[y + 1, x] = pointIndices[10] = pointIndices[6];
									}
								}
								else
								{
									t = cell[7] / (cell[7] - cell[6]);
									point[0] = x + 1;
									point[2] = z + 1;
									point[1] = y + t;
									newY[y, x + 1] = pointIndices[6] = StorePoint(grid, surface, point);
								}
								break;
							case 7:
								if (y != 0)
								{
									pointIndices[7] = oldLayer[x + 1];
								}
								else
								{
									if (cell[4] == 0)
									{
										oldLayer[x + 1] = pointIndices[7] = StorePoint(grid, surface, x + 1, 0, z);
										if (SignBit(cell[0]) != 0)
										{
											pointIndices[8] = pointIndices[7];
										}

										if (SignBit(cell[5]) != 0)
										{
											pointIndices[4] = pointIndices[7];
											if (z == 0)
											{
												oldY[0, x + 1] = pointIndices[4];
											}
										}
									}
									else if (cell[7] == 0)
									{
										oldLayer[x + 1] = pointIndices[7] = StorePoint(grid, surface, x + 1, 0, z + 1);
										if (SignBit(cell[6]) != 0)
										{
											newY[0, x + 1] = pointIndices[6] = pointIndices[7];
										}

										if (SignBit(cell[3]) != 0)
										{
											newX[0, x] = pointIndices[11] = pointIndices[7];
										}
									}
									else
									{
										t = cell[4] / (cell[4] - cell[7]);
										point[0] = x + 1;
										point[1] = 0;
										point[2] = z + t;
										oldLayer[x + 1] = pointIndices[7] = StorePoint(grid, surface, point);
									}
								}
								break;
							case 8:
								if (z != 0 || y != 0)
								{
									pointIndices[8] = oldX[y, x];
								}
								else
								{
									if (cell[0] == 0)
									{
										pointIndices[8] = StorePoint(grid, surface, x, 0, 0);
										if (SignBit(cell[1]) != 0)
										{
											pointIndices[0] = pointIndices[8];
										}

										if (SignBit(cell[3]) != 0)
										{
											pointIndices[3] = pointIndices[8];
										}
									}
									else if (cell[4] == 0)
									{
										pointIndices[8] = StorePoint(grid, surface, x + 1, 0, 0);
										if (SignBit(cell[5]) != 0)
										{
											oldY[0, x + 1] = pointIndices[4] = pointIndices[8];
										}

										if (SignBit(cell[7]) != 0)
										{
											oldLayer[x + 1] = pointIndices[7] = pointIndices[8];
										}
									}
									else
									{
										t = cell[0] / (cell[0] - cell[4]);
										point[1] = point[2] = 0;
										point[0] = x + t;
										pointIndices[8] = StorePoint(grid, surface, point);
									}
								}
								break;
							case 9:
								if (z != 0)
								{
									pointIndices[9] = oldX[y + 1, x];
								}
								else
								{
									if (cell[1] == 0)
									{
										oldX[y + 1, x] = pointIndices[9] = StorePoint(grid, surface, x, y + 1, 0);
										if (SignBit(cell[2]) != 0)
										{
											pointIndices[1] = pointIndices[9];
											if (x == 0)
											{
												newLayer[0] = pointIndices[1];
											}
										}
										if (SignBit(cell[0]) != 0)
										{
											pointIndices[0] = pointIndices[9];
										}
									}
									else if (cell[5] == 0)
									{
										oldX[y + 1, x] = pointIndices[9] = StorePoint(grid, surface, x + 1, y + 1, 0);
										if (SignBit(cell[6]) != 0)
										{
											newLayer[x + 1] = pointIndices[5] = pointIndices[9];
										}

										if (SignBit(cell[4]) != 0)
										{
											oldY[y, x + 1] = pointIndices[4] = pointIndices[9];
										}
									}
									else
									{
										t = cell[1] / (cell[1] - cell[5]);
										point[1] = y + 1;
										point[2] = 0;
										point[0] = x + t;
										oldX[y + 1, x] = pointIndices[9] = StorePoint(grid, surface, point);
									}
								}
								break;
							case 10:
								if (cell[2] == 0)
								{
									if (SignBit(cell[1]) != 0)
									{
										if (x != 0)
										{
											newX[y + 1, x] = pointIndices[10] = pointIndices[1] = newLayer[x];
											if (SignBit(cell[3]) != 0)
											{
												pointIndices[2] = pointIndices[10];
											}
										}
										else
										{
											newX[y + 1, 0] = pointIndices[10] = newLayer[0] = pointIndices[1] = StorePoint(grid, surface, 0, y + 1, z + 1);
											if (SignBit(cell[3]) != 0)
											{
												newY[y, 0] = pointIndices[2] = pointIndices[10];
											}
										}
									}
									else if (SignBit(cell[3]) != 0)
									{
										if (x != 0)
										{
											newX[y + 1, x] = pointIndices[10] = pointIndices[2] = newY[y, x];
										}
										else
										{
											newX[y + 1, 0] = pointIndices[10] = newY[y, 0] = pointIndices[2] = StorePoint(grid, surface, 0, y + 1, z + 1);
										}
									}
									else
									{
										newX[y + 1, x] = pointIndices[10] = StorePoint(grid, surface, x, y + 1, z + 1);
									}
								}
								else if (cell[6] == 0)
								{
									newX[y + 1, x] = pointIndices[10] = StorePoint(grid, surface, x + 1, y + 1, z + 1);
									if (SignBit(cell[5]) != 0)
									{
										newLayer[x + 1] = pointIndices[5] = pointIndices[10];
									}

									if (SignBit(cell[7]) != 0)
									{
										newY[y, x + 1] = pointIndices[6] = pointIndices[10];
									}
								}
								else
								{
									t = cell[2] / (cell[2] - cell[6]);
									point[1] = y + 1;
									point[2] = z + 1;
									point[0] = x + t;
									newX[y + 1, x] = pointIndices[10] = StorePoint(grid, surface, point);
								}
								break;
							case 11:
								if (y != 0)
								{
									pointIndices[11] = newX[y, x];
								}
								else
								{
									if (cell[3] == 0)
									{
										newX[0, x] = pointIndices[11] = StorePoint(grid, surface, x, 0, z + 1);
										if (SignBit(cell[0]) != 0)
										{
											pointIndices[3] = pointIndices[11];
										}

										if (SignBit(cell[2]) != 0)
										{
											pointIndices[2] = pointIndices[11];
											if (x == 0)
											{
												newY[0, 0] = pointIndices[2];
											}
										}
									}
									else if (cell[7] == 0)
									{
										newX[0, x] = pointIndices[11] = StorePoint(grid, surface, x + 1, 0, z + 1);
										if (SignBit(cell[4]) != 0)
										{
											oldLayer[x + 1] = pointIndices[7] = pointIndices[11];
										}

										if (SignBit(cell[6]) != 0)
										{
											newY[0, x + 1] = pointIndices[6] = pointIndices[11];
										}
									}
									else
									{
										t = cell[3] / (cell[3] - cell[7]);
										point[1] = 0;
										point[2] = z + 1;
										point[0] = x + t;
										newX[0, x] = pointIndices[11] = StorePoint(grid, surface, point);
									}
								}
								break;
							case 12:
								point[0] = x + 0.5f;
								point[1] = y + 0.5f;
								point[2] = z + 0.5f;
								pointIndices[12] = StorePoint(grid, surface, point);
								break;
						}
					}
					faces[k] = pointIndices[caseCode];//now f contains the vertex indices of the triangle
				}
				if (faces[0] != faces[1] && faces[0] != faces[2] && faces[1] != faces[2])//to avoid zero area triangles
				{
					if (reverseTriangles != 0)//The order of triangle vertices is reversed if m is not zero
					{
						faces[2] = faces[0]; 
						faces[0] = pointIndices[caseCode];
					}
					surface.AddTriangle(faces[0], faces[1], faces[2]);
				}
			}
		}

		private static int GetLayerIntoVertices(ref float[] vertices, float iso, Grid grid, int x, int y, int z, int oldI)
		{
			vertices[0] = vertices[4];
			vertices[1] = vertices[5];
			vertices[2] = vertices[6];
			vertices[3] = vertices[7];

			vertices[4] = iso - grid[x, y, z];
			vertices[5] = iso - grid[x, y + 1, z];
			vertices[6] = iso - grid[x, y + 1, z + 1];
			vertices[7] = iso - grid[x, y, z + 1];

			return ((((((((oldI & 0) << 1) | SignBit(vertices[4])) << 1) | SignBit(vertices[5])) << 1) | SignBit(vertices[6])) << 1) | SignBit(vertices[7]);
		}

		public static Surface CalculateSurface(Grid grid, float iso)
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
			Surface surface = new ListSurface();
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
			return surface;
		}
	}
}
