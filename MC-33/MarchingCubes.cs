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
		static int face_tests(int[] face, int ind, int sw, float[] v)
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
		static int face_test1(int face, float[] v)
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
		static int interior_test(int i, int flagtplane, float[] v)
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
				if (t <= 0.0f)
				{
					return 0;
				}
			}
			else
			{
				if (t >= 0.0f)
				{
					return 0;
				}
			}
			t = 0.5f * (v[3] * Bt + v[1] * Dt - v[2] * At - v[0] * Ct) / t;//t = -b/2a
			if (t <= 0.0f || t >= 1.0f)
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

		/******************************************************************
		Assign memory for the vertex r[3], normal n[3]. The return value is the new
		vertex label.
		*/
		static int store_point_normal(Grid grid, Surface s, float[] r)
		{

			Vector3 pos = new Vector3(r[0], r[1], r[2]);
			pos *= grid.Offset;
			pos += grid.Origin;

			return s.AddVertex(pos);
		}

		/******************************************************************
		Store the triangle, an array of three vertex indices (integers).
		*/
		static void store_triangle(Surface s, int[] t)
		{
			s.AddTriangle(t[0], t[1], t[2]);
		}

		/******************************************************************
		Auxiliary function that calculates the normal if a vertex
		of the cube lies on the isosurface.
		*/
		static int surfint(Grid grid, Surface s, int x, int y, int z, float[] r, float[] n)
		{
			r[0] = x; r[1] = y; r[2] = z;
			if (x == 0)
			{
				n[0] = grid[0, y, z] - grid[1, y, z];
			}
			else if (x == grid.SizeX)
			{
				n[0] = grid[x - 1, y, z] - grid[x, y, z];
			}
			else
			{
				n[0] = 0.5f * (grid[x - 1, y, z] - grid[x + 1, y, z]);
			}

			if (y == 0)
			{
				n[1] = grid[x, 0, z] - grid[x, 1, z];
			}
			else if (y == grid.SizeY)
			{
				n[1] = grid[x, y - 1, z] - grid[x, y, z];
			}
			else
			{
				n[1] = 0.5f * (grid[x, y - 1, z] - grid[x, y + 1, z]);
			}

			if (z == 0)
			{
				n[2] = grid[x, y, 0] - grid[x, y, 1];
			}
			else if (z == grid.SizeZ)
			{
				n[2] = grid[x, y, z - 1] - grid[x, y, z];
			}
			else
			{
				n[2] = 0.5f * (grid[x, y, z - 1] - grid[x, y, z + 1]);
			}

			return store_point_normal(grid, s, r);
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

		The temporary matrices: _Ox, _Oy, _Nx and _Ny, and vectors: _OL and _NL are filled
		and used here.*/

		static void find_case(Grid grid, Surface s, int x, int y, int z, int i, float[] v,
			int[] _OL, int[] _NL, int[,] _Oy, int[,] _Ny, int[,] _Ox, int[,] _Nx)
		{
			ushort[] pcase = new ushort[0];
			float t;
			float[] r = new float[3];
			float[] n = new float[3];
			int k;
			int[] f = new int[6];//for the face tests
			int[] p = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
			int m = i & 0x80;
			int c = LookUpTable.Case_Index[m == 0 ? i : i ^ 0xFF];
			switch (c >> 8)//find the MC33 case
			{
				case 1://********************************************
					if ((c & 0x0080) != 0)
					{
						m ^= 0x80;
					}

					pcase = LookUpTable.Case_1[(c & 0x7F)];
					break;
				case 2://********************************************
					if ((c & 0x0080) != 0)
					{
						m ^= 0x80;
					}

					pcase = LookUpTable.Case_2[(c & 0x7F)];
					break;
				case 3://********************************************
					if ((c & 0x0080) != 0)
					{
						m ^= 0x80;
					}

					if (((m != 0 ? i : i ^ 0xFF) & face_test1((c & 0x7F) >> 1, v)) != 0)
					{
						pcase = LookUpTable.Case_3_2[(c & 0x7F)];
					}
					else
					{
						pcase = LookUpTable.Case_3_1[(c & 0x7F)];
					}

					break;
				case 4://********************************************
					if ((c & 0x0080) != 0)
					{
						m ^= 0x80;
					}

					if (interior_test((c & 0x7F), 0, v) != 0)
					{
						pcase = LookUpTable.Case_4_2[(c & 0x7F)];
					}
					else
					{
						pcase = LookUpTable.Case_4_1[(c & 0x7F)];
					}

					break;
				case 5://********************************************
					if ((c & 0x0080) != 0)
					{
						m ^= 0x80;
					}

					pcase = LookUpTable.Case_5[(c & 0x7F)];
					break;
				case 6://********************************************
					if ((c & 0x0080) != 0)
					{
						m ^= 0x80;
					}

					if (((m != 0 ? i : i ^ 0xFF) & face_test1((c & 0x7F) % 6, v)) != 0)
					{
						pcase = LookUpTable.Case_6_2[(c & 0x7F)];
					}
					else if (interior_test((c & 0x7F) / 6, 0, v) != 0)
					{
						pcase = LookUpTable.Case_6_1_2[(c & 0x7F)];
					}
					else
					{
						pcase = LookUpTable.Case_6_1_1[(c & 0x7F)];
					}

					break;
				case 7://********************************************
					if ((c & 0x0080) != 0)
					{
						m ^= 0x80;
					}

					switch (face_tests(f, i, (m != 0 ? 1 : -1), v))
					{
						case -3:
							pcase = LookUpTable.Case_7_1[(c & 0x7F)];
							break;
						case -1:
							if (f[4] + f[5] == 1)
							{
								pcase = LookUpTable.Case_7_2_1[(c & 0x7F)];
							}
							else
							{
								pcase = (f[(33825 >> ((c & 0x7F) << 1)) & 3] == 1 ? LookUpTable.Case_7_2_3 : LookUpTable.Case_7_2_2)[(c & 0x7F)];
							}

							break;
						case 1:
							if (f[4] + f[5] == -1)
							{
								pcase = LookUpTable.Case_7_3_3[(c & 0x7F)];
							}
							else
							{
								pcase = (f[(33825 >> ((c & 0x7F) << 1)) & 3] == 1 ? LookUpTable.Case_7_3_2 : LookUpTable.Case_7_3_1)[(c & 0x7F)];
							}

							break;
						case 3:
							if (interior_test((c & 0x7F) >> 1, 0, v) != 0)
							{
								pcase = LookUpTable.Case_7_4_2[(c & 0x7F)];
							}
							else
							{
								pcase = LookUpTable.Case_7_4_1[(c & 0x7F)];
							}

							break;
					}
					break;
				case 8://********************************************
					pcase = LookUpTable.Case_8[(c & 0x7F)];
					break;
				case 9://********************************************
					pcase = LookUpTable.Case_9[(c & 0x7F)];
					break;
				case 10://********************************************
					switch (face_tests(f, i, (m == 0 ? -1 : 1), v))
					{
						case -2:
							if ((c & 0x7F) != 0 ? interior_test(0, 0, v) != 0 || interior_test((c & 0x01) == 0 ? 3 : 1, 0, v) != 0 : interior_test(0, 0, v) != 0)
							{
								pcase = LookUpTable.Case_10_1_2_1[(c & 0x7F)];
							}
							else
							{
								pcase = LookUpTable.Case_10_1_1_1[(c & 0x7F)];
							}

							break;
						case 2:
							if ((c & 0x7F) != 0 ? interior_test(2, 0, v) != 0 || interior_test((c & 0x01) != 0 ? 3 : 1, 0, v) != 0 : interior_test(1, 0, v) != 0)
							{
								pcase = LookUpTable.Case_10_1_2_2[(c & 0x7F)];
							}
							else
							{
								pcase = LookUpTable.Case_10_1_1_2[(c & 0x7F)];
							}

							break;
						case 0:
							pcase = (f[4 >> ((c & 0x7F) << 1)] == 1 ? LookUpTable.Case_10_2_2 : LookUpTable.Case_10_2_1)[(c & 0x7F)];
							break;
					}
					break;
				case 11://********************************************
					pcase = LookUpTable.Case_11[(c & 0x7F)];
					break;
				case 12://********************************************
					switch (face_tests(f, i, (m != 0 ? 1 : -1), v))
					{
						case -2:
							if (interior_test(LookUpTable._12_test_index[0, c & 0x7F], 0, v) != 0)
							{
								pcase = LookUpTable.Case_12_1_2_1[(c & 0x7F)];
							}
							else
							{
								pcase = LookUpTable.Case_12_1_1_1[(c & 0x7F)];
							}

							break;
						case 2:
							if (interior_test(LookUpTable._12_test_index[1, c & 0x7F], 0, v) != 0)
							{
								pcase = LookUpTable.Case_12_1_2_2[(c & 0x7F)];
							}
							else
							{
								pcase = LookUpTable.Case_12_1_1_2[(c & 0x7F)];
							}

							break;
						case 0:
							pcase = (f[LookUpTable._12_test_index[2, c & 0x7F]] == 1 ? LookUpTable.Case_12_2_2 : LookUpTable.Case_12_2_1)[(c & 0x7F)];
							break;
					}
					break;
				case 13://********************************************
					c = face_tests(f, i, (m != 0 ? 1 : -1), v);
					switch (Math.Abs(c))
					{
						case 6:
							pcase = LookUpTable.Case_13_1[(c > 0 ? 1 : 0)];
							break;
						case 4:
							c >>= 2;
							i = 0;
							while (f[i] != -c)
							{
								++i;
							}

							pcase = LookUpTable.Case_13_2[(3 * c + 3 + i)];
							i = 1;
							break;
						case 2:
							c = (((((((ToInt(f[0] < 0) << 1) | ToInt(f[1] < 0)) << 1) | ToInt(f[2] < 0)) << 1) |
									ToInt(f[3] < 0)) << 1) | ToInt(f[4] < 0);
							pcase = LookUpTable.Case_13_3[(25 - c + ((ToInt(c > 10) + ToInt(c > 20)) << 1))];
							break;
						case 0:
							c = (ToInt(f[1] < 0) << 1) | ToInt(f[5] < 0);
							if (f[0] * f[1] * f[5] == 1)
							{
								pcase = LookUpTable.Case_13_4[c];
							}
							else
							{
								i = interior_test(c, 1, v);
								if (i != 0)
								{
									pcase = LookUpTable.Case_13_5_2[(c | ((i & 1) << 2))];
								}
								else
								{
									pcase = LookUpTable.Case_13_5_1[c];
									i = 1;
								}
							}
							break;
					}
					break;
				case 14:
					pcase = LookUpTable.Case_14[(c & 0x7F)];
					break;
			}
			foreach (ushort caseItem in pcase)
			{
				ushort j = caseItem;
				for (k = 0; k < 3; ++k)
				{
					c = j & 0x0F;
					j >>= 4;
					if (p[c] < 0)
					{
						switch (c)//the vertices r[3] and normals n[3] are calculated here
						{
							case 0:
								if (z != 0 || x != 0)
								{
									p[0] = _Oy[y, x];
								}
								else
								{
									if (v[0] == 0.0f)
									{
										p[0] = surfint(grid, s, 0, y, 0, r, n);
										if (SignBit(v[3]) != 0)
										{
											p[3] = p[0];
										}

										if (SignBit(v[4]) != 0)
										{
											p[8] = p[0];
										}
									}
									else if (v[1] == 0.0f)
									{
										p[0] = surfint(grid, s, 0, y + 1, 0, r, n);
										if (SignBit(v[2]) != 0)
										{
											_NL[0] = p[1] = p[0];
										}

										if (SignBit(v[5]) != 0)
										{
											_Ox[y + 1, 0] = p[9] = p[0];
										}
									}
									else
									{
										t = v[0] / (v[0] - v[1]);
										r[0] = r[2] = 0.0f;
										r[1] = y + t;
										n[0] = (v[4] - v[0]) * (1.0f - t) + (v[5] - v[1]) * t;
										n[1] = v[1] - v[0];
										n[2] = (v[3] - v[0]) * (1.0f - t) + (v[2] - v[1]) * t;
										p[0] = store_point_normal(grid, s, r);
									}
								}
								break;
							case 1:
								if (x != 0)
								{
									p[1] = _NL[x];
								}
								else
								{
									if (v[1] == 0.0f)
									{
										_NL[0] = p[1] = surfint(grid, s, 0, y + 1, z, r, n);
										if (SignBit(v[0]) != 0)
										{
											p[0] = p[1];
										}

										if (SignBit(v[5]) != 0)
										{
											p[9] = p[1];
											if (z == 0)
											{
												_Ox[y + 1, 0] = p[9];
											}
										}
									}
									else if (v[2] == 0.0f)
									{
										_NL[0] = p[1] = surfint(grid, s, 0, y + 1, z + 1, r, n);
										if (SignBit(v[3]) != 0)
										{
											_Ny[y, 0] = p[2] = p[1];
										}

										if (SignBit(v[6]) != 0)
										{
											_Nx[y + 1, 0] = p[10] = p[1];
										}
									}
									else
									{
										t = v[1] / (v[1] - v[2]);
										r[0] = 0.0f; r[1] = y + 1;
										r[2] = z + t;
										n[0] = (v[5] - v[1]) * (1.0f - t) + (v[6] - v[2]) * t;
										n[1] = (y + 1 < grid.SizeY ? 0.5f * ((grid[0, y, z] - grid[0, y + 2, z]) * (1.0f - t)
													+ (grid[0, y, z + 1] - grid[0, y + 2, z + 1]) * t) :
													(v[1] - v[0]) * (1.0f - t) + (v[2] - v[3]) * t);
										n[2] = v[2] - v[1];
										_NL[0] = p[1] = store_point_normal(grid, s, r);
									}
								}
								break;
							case 2:
								if (x != 0)
								{
									p[2] = _Ny[y, x];
								}
								else
								{
									if (v[3] == 0.0f)
									{
										_Ny[y, 0] = p[2] = surfint(grid, s, 0, y, z + 1, r, n);
										if (SignBit(v[0]) != 0)
										{
											p[3] = p[2];
										}

										if (SignBit(v[7]) != 0)
										{
											p[11] = p[2];
											if (y == 0)
											{
												_Nx[0, 0] = p[11];
											}
										}
									}
									else if (v[2] == 0.0f)
									{
										_Ny[y, 0] = p[2] = surfint(grid, s, 0, y + 1, z + 1, r, n);
										if (SignBit(v[1]) != 0)
										{
											_NL[0] = p[1] = p[2];
										}

										if (SignBit(v[6]) != 0)
										{
											_Nx[y + 1, 0] = p[10] = p[2];
										}
									}
									else
									{
										t = v[3] / (v[3] - v[2]);
										r[0] = 0.0f; r[2] = z + 1;
										r[1] = y + t;
										n[0] = (v[7] - v[3]) * (1.0f - t) + (v[6] - v[2]) * t;
										n[1] = v[2] - v[3];
										n[2] = (z + 1 < grid.SizeZ ? 0.5f * ((grid[0, y, z] - grid[0, y, z + 2]) * (1.0f - t)
													+ (grid[0, y + 1, z] - grid[0, y + 1, z + 2]) * t) :
													(v[3] - v[0]) * (1.0f - t) + (v[2] - v[1]) * t);
										_Ny[y, 0] = p[2] = store_point_normal(grid, s, r);
									}
								}
								break;
							case 3:
								if (y != 0 || x != 0)
								{
									p[3] = _OL[x];
								}
								else
								{
									if (v[0] == 0.0f)
									{
										p[3] = surfint(grid, s, 0, 0, z, r, n);
										if (SignBit(v[1]) != 0)
										{
											p[0] = p[3];
										}

										if (SignBit(v[4]) != 0)
										{
											p[8] = p[3];
										}
									}
									else if (v[3] == 0.0f)
									{
										p[3] = surfint(grid, s, 0, 0, z + 1, r, n);
										if (SignBit(v[2]) != 0)
										{
											_Ny[0, 0] = p[2] = p[3];
										}

										if (SignBit(v[7]) != 0)
										{
											_Nx[0, 0] = p[11] = p[3];
										}
									}
									else
									{
										t = v[0] / (v[0] - v[3]);
										r[0] = r[1] = 0.0f;
										r[2] = z + t;
										n[0] = (v[4] - v[0]) * (1.0f - t) + (v[7] - v[3]) * t;
										n[1] = (v[1] - v[0]) * (1.0f - t) + (v[2] - v[3]) * t;
										n[2] = v[3] - v[0];
										p[3] = store_point_normal(grid, s, r);
									}
								}
								break;
							case 4:
								if (z != 0)
								{
									p[4] = _Oy[y, x + 1];
								}
								else
								{
									if (v[4] == 0.0f)
									{
										_Oy[y, x + 1] = p[4] = surfint(grid, s, x + 1, y, 0, r, n);
										if (SignBit(v[7]) != 0)
										{
											p[7] = p[4];
										}

										if (SignBit(v[0]) != 0)
										{
											p[8] = p[4];
										}

										if (y == 0)
										{
											_OL[x + 1] = p[7];
										}
									}
									else if (v[5] == 0.0f)
									{
										_Oy[y, x + 1] = p[4] = surfint(grid, s, x + 1, y + 1, 0, r, n);
										if (SignBit(v[6]) != 0)
										{
											_NL[x + 1] = p[5] = p[4];
										}

										if (SignBit(v[1]) != 0)
										{
											_Ox[y + 1, x] = p[9] = p[4];
										}
									}
									else
									{
										t = v[4] / (v[4] - v[5]);
										r[0] = x + 1; r[2] = 0.0f;
										r[1] = y + t;
										n[0] = (x + 1 < grid.SizeX ? 0.5f * ((grid[x, y, 0] - grid[x + 2, y, 0]) * (1.0f - t)
													+ (grid[x, y + 1, 0] - grid[x + 2, y + 1, 0]) * t) :
													(v[4] - v[0]) * (1.0f - t) + (v[5] - v[1]) * t);
										n[1] = v[5] - v[4];
										n[2] = (v[7] - v[4]) * (1.0f - t) + (v[6] - v[5]) * t;
										_Oy[y, x + 1] = p[4] = store_point_normal(grid, s, r);
									}
								}
								break;
							case 5:
								if (v[5] == 0.0f)
								{
									if (SignBit(v[4]) != 0)
									{
										if (z != 0)
										{
											_NL[x + 1] = p[5] = p[4] = _Oy[y, x + 1];
											if (SignBit(v[1]) != 0)
											{
												p[9] = p[5];
											}
										}
										else
										{
											_NL[x + 1] = p[5] = _Oy[y, x + 1] = p[4] = surfint(grid, s, x + 1, y + 1, 0, r, n);
											if (SignBit(v[1]) != 0)
											{
												_Ox[y + 1, x] = p[9] = p[5];
											}
										}
									}
									else if (SignBit(v[1]) != 0)
									{
										if (z != 0)
										{
											_NL[x + 1] = p[5] = p[9] = _Ox[y + 1, x];
										}
										else
										{
											_NL[x + 1] = p[5] = _Ox[y + 1, x] = p[9] = surfint(grid, s, x + 1, y + 1, 0, r, n);
										}
									}
									else
									{
										_NL[x + 1] = p[5] = surfint(grid, s, x + 1, y + 1, z, r, n);
									}
								}
								else if (v[6] == 0.0f)
								{
									_NL[x + 1] = p[5] = surfint(grid, s, x + 1, y + 1, z + 1, r, n);
									if (SignBit(v[2]) != 0)
									{
										_Nx[y + 1, x] = p[10] = p[5];
									}

									if (SignBit(v[7]) != 0)
									{
										_Ny[y, x + 1] = p[6] = p[5];
									}
								}
								else
								{
									t = v[5] / (v[5] - v[6]);
									r[0] = x + 1; r[1] = y + 1;
									r[2] = z + t;
									n[0] = (x + 1 < grid.SizeX ? 0.5f * ((grid[x, y + 1, z] - grid[x + 2, y + 1, z]) * (1.0f - t)
												+ (grid[x, y + 1, z + 1] - grid[x + 2, y + 1, z + 1]) * t) :
												(v[5] - v[1]) * (1.0f - t) + (v[6] - v[2]) * t);
									n[1] = (y + 1 < grid.SizeY ? 0.5f * ((grid[x + 1, y, z] - grid[x + 1, y + 2, z]) * (1.0f - t)
												+ (grid[x + 1, y, z + 1] - grid[x + 1, y + 2, z + 1]) * t) :
												(v[5] - v[4]) * (1.0f - t) + (v[6] - v[7]) * t);
									n[2] = v[6] - v[5];
									_NL[x + 1] = p[5] = store_point_normal(grid, s, r);
								}
								break;
							case 6:
								if (v[7] == 0.0f)
								{
									if (SignBit(v[3]) != 0)
									{
										if (y != 0)
										{
											_Ny[y, x + 1] = p[6] = p[11] = _Nx[y, x];
											if (SignBit(v[4]) != 0)
											{
												p[7] = p[6];
											}
										}
										else
										{
											_Ny[y, x + 1] = p[6] = _Nx[0, x] = p[11] = surfint(grid, s, x + 1, 0, z + 1, r, n);
											if (SignBit(v[4]) != 0)
											{
												_OL[x + 1] = p[7] = p[6];
											}
										}
									}
									else if (SignBit(v[4]) != 0)
									{
										if (y != 0)
										{
											_Ny[y, x + 1] = p[6] = p[7] = _OL[x + 1];
										}
										else
										{
											_Ny[y, x + 1] = p[6] = _OL[x + 1] = p[7] = surfint(grid, s, x + 1, 0, z + 1, r, n);
										}
									}
									else
									{
										_Ny[y, x + 1] = p[6] = surfint(grid, s, x + 1, y, z + 1, r, n);
									}
								}
								else if (v[6] == 0.0f)
								{
									_Ny[y, x + 1] = p[6] = surfint(grid, s, x + 1, y + 1, z + 1, r, n);
									if (SignBit(v[5]) != 0)
									{
										_NL[x + 1] = p[5] = p[6];
									}

									if (SignBit(v[2]) != 0)
									{
										_Nx[y + 1, x] = p[10] = p[6];
									}
								}
								else
								{
									t = v[7] / (v[7] - v[6]);
									r[0] = x + 1; r[2] = z + 1;
									r[1] = y + t;
									n[0] = (x + 1 < grid.SizeX ? 0.5f * ((grid[x, y, z + 1] - grid[x + 2, y, z + 1]) * (1.0f - t)
												+ (grid[x, y + 1, z + 1] - grid[x + 2, y + 1, z + 1]) * t) :
												(v[7] - v[3]) * (1.0f - t) + (v[6] - v[2]) * t);
									n[1] = v[6] - v[7];
									n[2] = (z + 1 < grid.SizeZ ? 0.5f * ((grid[x + 1, y, z] - grid[x + 1, y, z + 2]) * (1.0f - t)
													+ (grid[x + 1, y + 1, z] - grid[x + 1, y + 1, z + 2]) * t) :
												(v[7] - v[4]) * (1.0f - t) + (v[6] - v[5]) * t);
									_Ny[y, x + 1] = p[6] = store_point_normal(grid, s, r);
								}
								break;
							case 7:
								if (y != 0)
								{
									p[7] = _OL[x + 1];
								}
								else
								{
									if (v[4] == 0.0f)
									{
										_OL[x + 1] = p[7] = surfint(grid, s, x + 1, 0, z, r, n);
										if (SignBit(v[0]) != 0)
										{
											p[8] = p[7];
										}

										if (SignBit(v[5]) != 0)
										{
											p[4] = p[7];
											if (z == 0)
											{
												_Oy[0, x + 1] = p[4];
											}
										}
									}
									else if (v[7] == 0.0f)
									{
										_OL[x + 1] = p[7] = surfint(grid, s, x + 1, 0, z + 1, r, n);
										if (SignBit(v[6]) != 0)
										{
											_Ny[0, x + 1] = p[6] = p[7];
										}

										if (SignBit(v[3]) != 0)
										{
											_Nx[0, x] = p[11] = p[7];
										}
									}
									else
									{
										t = v[4] / (v[4] - v[7]);
										r[0] = x + 1; r[1] = 0.0f;
										r[2] = z + t;
										n[0] = (x + 1 < grid.SizeX ? 0.5f * ((grid[x, 0, z] - grid[x + 2, 0, z]) * (1.0f - t)
													+ (grid[x, 0, z + 1] - grid[x + 2, 0, z + 1]) * t) :
													(v[4] - v[0]) * (1.0f - t) + (v[7] - v[3]) * t);
										n[1] = (v[5] - v[4]) * (1.0f - t) + (v[6] - v[7]) * t;
										n[2] = v[7] - v[4];
										_OL[x + 1] = p[7] = store_point_normal(grid, s, r);
									}
								}
								break;
							case 8:
								if (z != 0 || y != 0)
								{
									p[8] = _Ox[y, x];
								}
								else
								{
									if (v[0] == 0.0f)
									{
										p[8] = surfint(grid, s, x, 0, 0, r, n);
										if (SignBit(v[1]) != 0)
										{
											p[0] = p[8];
										}

										if (SignBit(v[3]) != 0)
										{
											p[3] = p[8];
										}
									}
									else if (v[4] == 0.0f)
									{
										p[8] = surfint(grid, s, x + 1, 0, 0, r, n);
										if (SignBit(v[5]) != 0)
										{
											_Oy[0, x + 1] = p[4] = p[8];
										}

										if (SignBit(v[7]) != 0)
										{
											_OL[x + 1] = p[7] = p[8];
										}
									}
									else
									{
										t = v[0] / (v[0] - v[4]);
										r[1] = r[2] = 0.0f;
										r[0] = x + t;
										n[0] = v[4] - v[0];
										n[1] = (v[1] - v[0]) * (1.0f - t) + (v[5] - v[4]) * t;
										n[2] = (v[3] - v[0]) * (1.0f - t) + (v[7] - v[4]) * t;
										p[8] = store_point_normal(grid, s, r);
									}
								}
								break;
							case 9:
								if (z != 0)
								{
									p[9] = _Ox[y + 1, x];
								}
								else
								{
									if (v[1] == 0.0f)
									{
										_Ox[y + 1, x] = p[9] = surfint(grid, s, x, y + 1, 0, r, n);
										if (SignBit(v[2]) != 0)
										{
											p[1] = p[9];
											if (x == 0)
											{
												_NL[0] = p[1];
											}
										}
										if (SignBit(v[0]) != 0)
										{
											p[0] = p[9];
										}
									}
									else if (v[5] == 0.0f)
									{
										_Ox[y + 1, x] = p[9] = surfint(grid, s, x + 1, y + 1, 0, r, n);
										if (SignBit(v[6]) != 0)
										{
											_NL[x + 1] = p[5] = p[9];
										}

										if (SignBit(v[4]) != 0)
										{
											_Oy[y, x + 1] = p[4] = p[9];
										}
									}
									else
									{
										t = v[1] / (v[1] - v[5]);
										r[1] = y + 1; r[2] = 0.0f;
										r[0] = x + t;
										n[0] = v[5] - v[1];
										n[1] = (y + 1 < grid.SizeY ? 0.5f * ((grid[x, y, 0] - grid[x, y + 2, 0]) * (1.0f - t)
													+ (grid[x + 1, y, 0] - grid[x + 1, y + 2, 0]) * t) :
													(v[1] - v[0]) * (1.0f - t) + (v[5] - v[4]) * t);
										n[2] = (v[2] - v[1]) * (1.0f - t) + (v[6] - v[5]) * t;
										_Ox[y + 1, x] = p[9] = store_point_normal(grid, s, r);
									}
								}
								break;
							case 10:
								if (v[2] == 0.0f)
								{
									if (SignBit(v[1]) != 0)
									{
										if (x != 0)
										{
											_Nx[y + 1, x] = p[10] = p[1] = _NL[x];
											if (SignBit(v[3]) != 0)
											{
												p[2] = p[10];
											}
										}
										else
										{
											_Nx[y + 1, 0] = p[10] = _NL[0] = p[1] = surfint(grid, s, 0, y + 1, z + 1, r, n);
											if (SignBit(v[3]) != 0)
											{
												_Ny[y, 0] = p[2] = p[10];
											}
										}
									}
									else if (SignBit(v[3]) != 0)
									{
										if (x != 0)
										{
											_Nx[y + 1, x] = p[10] = p[2] = _Ny[y, x];
										}
										else
										{
											_Nx[y + 1, 0] = p[10] = _Ny[y, 0] = p[2] = surfint(grid, s, 0, y + 1, z + 1, r, n);
										}
									}
									else
									{
										_Nx[y + 1, x] = p[10] = surfint(grid, s, x, y + 1, z + 1, r, n);
									}
								}
								else if (v[6] == 0.0f)
								{
									_Nx[y + 1, x] = p[10] = surfint(grid, s, x + 1, y + 1, z + 1, r, n);
									if (SignBit(v[5]) != 0)
									{
										_NL[x + 1] = p[5] = p[10];
									}

									if (SignBit(v[7]) != 0)
									{
										_Ny[y, x + 1] = p[6] = p[10];
									}
								}
								else
								{
									t = v[2] / (v[2] - v[6]);
									r[1] = y + 1; r[2] = z + 1;
									r[0] = x + t;
									n[0] = v[6] - v[2];
									n[1] = (y + 1 < grid.SizeY ? 0.5f * ((grid[x, y, z + 1] - grid[x, y + 2, z + 1]) * (1.0f - t)
												+ (grid[x + 1, y, z + 1] - grid[x + 1, y + 2, z + 1]) * t) :
												(v[2] - v[3]) * (1.0f - t) + (v[6] - v[7]) * t);
									n[2] = (z + 1 < grid.SizeZ ? 0.5f * ((grid[x, y + 1, z] - grid[x, y + 1, z + 2]) * (1.0f - t)
												+ (grid[x + 1, y + 1, z] - grid[x + 1, y + 1, z + 2]) * t) :
												(v[2] - v[1]) * (1.0f - t) + (v[6] - v[5]) * t);
									_Nx[y + 1, x] = p[10] = store_point_normal(grid, s, r);
								}
								break;
							case 11:
								if (y != 0)
								{
									p[11] = _Nx[y, x];
								}
								else
								{
									if (v[3] == 0.0f)
									{
										_Nx[0, x] = p[11] = surfint(grid, s, x, 0, z + 1, r, n);
										if (SignBit(v[0]) != 0)
										{
											p[3] = p[11];
										}

										if (SignBit(v[2]) != 0)
										{
											p[2] = p[11];
											if (x == 0)
											{
												_Ny[0, 0] = p[2];
											}
										}
									}
									else if (v[7] == 0.0f)
									{
										_Nx[0, x] = p[11] = surfint(grid, s, x + 1, 0, z + 1, r, n);
										if (SignBit(v[4]) != 0)
										{
											_OL[x + 1] = p[7] = p[11];
										}

										if (SignBit(v[6]) != 0)
										{
											_Ny[0, x + 1] = p[6] = p[11];
										}
									}
									else
									{
										t = v[3] / (v[3] - v[7]);
										r[1] = 0.0f; r[2] = z + 1;
										r[0] = x + t;
										n[0] = v[7] - v[3];
										n[1] = (v[2] - v[3]) * (1.0f - t) + (v[6] - v[7]) * t;
										n[2] = (z + 1 < grid.SizeZ ? 0.5f * ((grid[x, 0, z] - grid[x, 0, z + 2]) * (1.0f - t)
													+ (grid[x + 1, 0, z] - grid[x + 1, 0, z + 2]) * t) :
													(v[3] - v[0]) * (1.0f - t) + (v[7] - v[4]) * t);
										_Nx[0, x] = p[11] = store_point_normal(grid, s, r);
									}
								}
								break;
							case 12:
								r[0] = x + 0.5f; r[1] = y + 0.5f; r[2] = z + 0.5f;
								n[0] = v[4] + v[5] + v[6] + v[7] - v[0] - v[1] - v[2] - v[3];
								n[1] = v[1] + v[2] + v[5] + v[6] - v[0] - v[3] - v[4] - v[7];
								n[2] = v[2] + v[3] + v[6] + v[7] - v[0] - v[1] - v[4] - v[5];
								p[12] = store_point_normal(grid, s, r);
								break;
						}
					}
					f[k] = p[c];//now f contains the vertex indices of the triangle
				}
				if (f[0] != f[1] && f[0] != f[2] && f[1] != f[2])//to avoid zero area triangles
				{
					if (m != 0)//The order of triangle vertices is reversed if m is not zero
					{ f[2] = f[0]; f[0] = p[c]; }
					store_triangle(s, f);
				}
			}
		}

		public static Surface calc_isosurface(Grid grid, float iso)
		{
			int x, y, z;
			int nx = grid.SizeX;
			int ny = grid.SizeY;
			int nz = grid.SizeZ;
			float[] vs = new float[8];
			int[] _OL = new int[nx + 1];
			int[] _NL = new int[nx + 1];
			int[,] _Oy = new int[ny, nx + 1];
			int[,] _Ny = new int[ny, nx + 1];
			int[,] _Ox = new int[ny + 1, nx];
			int[,] _Nx = new int[ny + 1, nx];
			Surface surface = new ListSurface();
			for (z = 0; z < nz; z++)
			{
				for (y = 0; y < ny; y++)
				{
					vs[4] = iso - grid[0, y, z];
					vs[5] = iso - grid[0, y + 1, z];
					vs[6] = iso - grid[0, y + 1, z + 1];
					vs[7] = iso - grid[0, y, z + 1];
					//the eight least significant bits of i correspond to vertex indices. (x...x01234567)
					//If the bit is 1 then the vertex value is greater than zero.
					int i = (((((SignBit(vs[4]) << 1) | SignBit(vs[5])) << 1) | SignBit(vs[6])) << 1) | SignBit(vs[7]);
					for (x = 0; x < nx; x++)
					{
						vs[0] = vs[4];
						vs[1] = vs[5];
						vs[2] = vs[6];
						vs[3] = vs[7];

						vs[4] = iso - grid[x + 1, y, z];
						vs[5] = iso - grid[x + 1, y + 1, z];
						vs[6] = iso - grid[x + 1, y + 1, z + 1];
						vs[7] = iso - grid[x + 1, y, z + 1];
						i = ((((((((i & 0x0F) << 1) | SignBit(vs[4])) << 1) | SignBit(vs[5])) << 1) | SignBit(vs[6])) << 1) | SignBit(vs[7]);
						if (i != 0 && i != 0xFF)//i is different from 0 and 0xFF
						{
							find_case(grid, surface, x, y, z, i, vs, _OL, _NL, _Oy, _Ny, _Ox, _Nx);
						}
					}

					int[] _Ltmp = _OL;
					_OL = _NL;
					_NL = _Ltmp;
				}
				int[,] _tmp = _Ox;
				_Ox = _Nx;
				_Nx = _tmp;

				_tmp = _Oy;
				_Oy = _Ny;
				_Ny = _tmp;
			}
			return surface;
		}
	}
}
