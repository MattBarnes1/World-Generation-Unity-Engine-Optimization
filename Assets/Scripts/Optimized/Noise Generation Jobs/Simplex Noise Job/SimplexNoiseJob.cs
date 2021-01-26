using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;

namespace NoiseJobs
{
    public struct SimplexNoise 
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 FastFloor(Double t)
        {
            return (t > 0 ? (Int32)t : (Int32)t - 1);
        }

        public static readonly Double F2 = 0.36602540378443864676372317075294;
        public static readonly Double G2 = 0.21132486540518711774542560974902;
        public static readonly Double F3 = 1.0 / 3.0;
        public static readonly Double G3 = 1.0 / 6.0;

        private static readonly Int32[,] Simplex = {
                                            {0, 1, 2, 3}, {0, 1, 3, 2}, {0, 0, 0, 0}, {0, 2, 3, 1},
                                            {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}, {1, 2, 3, 0},
                                            {0, 2, 1, 3}, {0, 0, 0, 0}, {0, 3, 1, 2}, {0, 3, 2, 1},
                                            {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}, {1, 3, 2, 0},
                                            {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0},
                                            {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0},
                                            {1, 2, 0, 3}, {0, 0, 0, 0}, {1, 3, 0, 2}, {0, 0, 0, 0},
                                            {0, 0, 0, 0}, {0, 0, 0, 0}, {2, 3, 0, 1}, {2, 3, 1, 0},
                                            {1, 0, 2, 3}, {1, 0, 3, 2}, {0, 0, 0, 0}, {0, 0, 0, 0},
                                            {0, 0, 0, 0}, {2, 0, 3, 1}, {0, 0, 0, 0}, {2, 1, 3, 0},
                                            {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0},
                                            {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0},
                                            {2, 0, 1, 3}, {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0},
                                            {3, 0, 1, 2}, {3, 0, 2, 1}, {0, 0, 0, 0}, {3, 1, 2, 0},
                                            {2, 1, 0, 3}, {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0},
                                            {3, 1, 0, 2}, {0, 0, 0, 0}, {3, 2, 0, 1}, {3, 2, 1, 0}
                                        };
        public Double Generate(Double x, Double y, Int32 seed, InterpolationDelegate interp)
        {
            Double s = (x + y) * F2;
            int i = FastFloor(x + s);
            int j = FastFloor(y + s);

            Double t = (i + j) * Noise.G2;
            Double X0 = i - t;
            Double Y0 = j - t;
            Double x0 = x - X0;
            Double y0 = y - Y0;

            int i1, j1;
            if (x0 > y0)
            {
                i1 = 1;
                j1 = 0;
            }
            else
            {
                i1 = 0;
                j1 = 1;
            }

            Double x1 = x0 - (Double)i1 + Noise.G2;
            Double y1 = y0 - (Double)j1 + Noise.G2;
            Double x2 = x0 - 1.0 + 2.0 * Noise.G2;
            Double y2 = y0 - 1.0 + 2.0 * Noise.G2;

            // Hash the triangle coordinates to index the gradient table
            uint h0 = Noise.HashCoordinates(i, j, (int)seed);
            uint h1 = Noise.HashCoordinates(i + i1, j + j1, (int)seed);
            uint h2 = Noise.HashCoordinates(i + 1, j + 1, (int)seed);

            // Now, index the tables
            Double[] g0 = { NoiseLookupTable.Gradient2D[h0, 0], NoiseLookupTable.Gradient2D[h0, 1] };
            Double[] g1 = { NoiseLookupTable.Gradient2D[h1, 0], NoiseLookupTable.Gradient2D[h1, 1] };
            Double[] g2 = { NoiseLookupTable.Gradient2D[h2, 0], NoiseLookupTable.Gradient2D[h2, 1] };

            Double n0, n1, n2;
            // Calculate the contributions from the 3 corners
            Double t0 = 0.5 - x0 * x0 - y0 * y0;
            if (t0 < 0) n0 = 0;
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * Noise.ArrayDot(g0, x0, y0);
            }

            Double t1 = 0.5 - x1 * x1 - y1 * y1;
            if (t1 < 0) n1 = 0;
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * Noise.ArrayDot(g1, x1, y1);
            }

            Double t2 = 0.5 - x2 * x2 - y2 * y2;
            if (t2 < 0) n2 = 0;
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * Noise.ArrayDot(g2, x2, y2);
            }

            // Add contributions together
            return (70.0 * (n0 + n1 + n2)) * 1.42188695 + 0.001054489;
        }

        public Double Generate(Double x, Double y, Double z, Int32 seed, InterpolationDelegate interp)
        {
            //static Double Noise.F3 = 1.0/3.0;
            //static Double Noise.G3 = 1.0/6.0;
            Double n0, n1, n2, n3;

            Double s = (x + y + z) * Noise.F3;
            int i = FastFloor(x + s);
            int j = FastFloor(y + s);
            int k = FastFloor(z + s);

            Double t = (i + j + k) * Noise.G3;
            Double X0 = i - t;
            Double Y0 = j - t;
            Double Z0 = k - t;

            Double x0 = x - X0;
            Double y0 = y - Y0;
            Double z0 = z - Z0;

            int i1, j1, k1;
            int i2, j2, k2;

            if (x0 >= y0)
            {
                if (y0 >= z0)
                {
                    i1 = 1;
                    j1 = 0;
                    k1 = 0;
                    i2 = 1;
                    j2 = 1;
                    k2 = 0;
                }
                else if (x0 >= z0)
                {
                    i1 = 1;
                    j1 = 0;
                    k1 = 0;
                    i2 = 1;
                    j2 = 0;
                    k2 = 1;
                }
                else
                {
                    i1 = 0;
                    j1 = 0;
                    k1 = 1;
                    i2 = 1;
                    j2 = 0;
                    k2 = 1;
                }
            }
            else
            {
                if (y0 < z0)
                {
                    i1 = 0;
                    j1 = 0;
                    k1 = 1;
                    i2 = 0;
                    j2 = 1;
                    k2 = 1;
                }
                else if (x0 < z0)
                {
                    i1 = 0;
                    j1 = 1;
                    k1 = 0;
                    i2 = 0;
                    j2 = 1;
                    k2 = 1;
                }
                else
                {
                    i1 = 0;
                    j1 = 1;
                    k1 = 0;
                    i2 = 1;
                    j2 = 1;
                    k2 = 0;
                }
            }

            var x1 = x0 - i1 + Noise.G3;
            var y1 = y0 - j1 + Noise.G3;
            var z1 = z0 - k1 + Noise.G3;
            var x2 = x0 - i2 + 2.0 * Noise.G3;
            var y2 = y0 - j2 + 2.0 * Noise.G3;
            var z2 = z0 - k2 + 2.0 * Noise.G3;
            var x3 = x0 - 1.0 + 3.0 * Noise.G3;
            var y3 = y0 - 1.0 + 3.0 * Noise.G3;
            var z3 = z0 - 1.0 + 3.0 * Noise.G3;

            var h0 = Noise.HashCoordinates(i, j, k, (int)seed);
            var h1 = Noise.HashCoordinates(i + i1, j + j1, k + k1, (int)seed);
            var h2 = Noise.HashCoordinates(i + i2, j + j2, k + k2, (int)seed);
            var h3 = Noise.HashCoordinates(i + 1, j + 1, k + 1, (int)seed);

            Double[] g0 = {
                              NoiseLookupTable.Gradient3D[h0, 0], NoiseLookupTable.Gradient3D[h0, 1],
                              NoiseLookupTable.Gradient3D[h0, 2]
                          };
            Double[] g1 = {
                              NoiseLookupTable.Gradient3D[h1, 0], NoiseLookupTable.Gradient3D[h1, 1],
                              NoiseLookupTable.Gradient3D[h1, 2]
                          };
            Double[] g2 = {
                              NoiseLookupTable.Gradient3D[h2, 0], NoiseLookupTable.Gradient3D[h2, 1],
                              NoiseLookupTable.Gradient3D[h2, 2]
                          };
            Double[] g3 = {
                              NoiseLookupTable.Gradient3D[h3, 0], NoiseLookupTable.Gradient3D[h3, 1],
                              NoiseLookupTable.Gradient3D[h3, 2]
                          };
            
            var t0 = 0.6 - x0 * x0 - y0 * y0 - z0 * z0;
            if (t0 < 0.0) n0 = 0.0;
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * Noise.ArrayDot(g0, x0, y0, z0);
            }

            var t1 = 0.6 - x1 * x1 - y1 * y1 - z1 * z1;
            if (t1 < 0.0) n1 = 0.0;
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * Noise.ArrayDot(g1, x1, y1, z1);
            }

            var t2 = 0.6 - x2 * x2 - y2 * y2 - z2 * z2;
            if (t2 < 0) n2 = 0.0;
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * Noise.ArrayDot(g2, x2, y2, z2);
            }

            var t3 = 0.6 - x3 * x3 - y3 * y3 - z3 * z3;
            if (t3 < 0) n3 = 0.0;
            else
            {
                t3 *= t3;
                n3 = t3 * t3 * Noise.ArrayDot(g3, x3, y3, z3);
            }

            return (32.0 * (n0 + n1 + n2 + n3)) * 1.25086885 + 0.0003194984;
        }

        public Double Generate(Double x, Double y, Double z, Double w, Int32 seed, InterpolationDelegate interp)
        {

            Double F4 = (Math.Sqrt(5.0) - 1.0) / 4.0;
            Double G4 = (5.0 - Math.Sqrt(5.0)) / 20.0;
            Double n0, n1, n2, n3, n4; // Noise contributions from the five corners
            // Skew the (x,y,z,w) space to determine which cell of 24 simplices we're in
            Double s = (x + y + z + w) * F4; // Factor for 4D skewing


            int i = FastFloor(x + s);
            int j = FastFloor(y + s);
            int k = FastFloor(z + s);
            int l = FastFloor(w + s);
            Double t = (i + j + k + l) * G4; // Factor for 4D unskewing
            Double X0 = i - t; // Unskew the cell origin back to (x,y,z,w) space
            Double Y0 = j - t;
            Double Z0 = k - t;
            Double W0 = l - t;
            Double x0 = x - X0; // The x,y,z,w distances from the cell origin
            Double y0 = y - Y0;
            Double z0 = z - Z0;
            Double w0 = w - W0;
            // For the 4D case, the simplex is a 4D shape I won't even try to describe.
            // To find out which of the 24 possible simplices we're in, we need to
            // determine the magnitude ordering of x0, y0, z0 and w0.
            // The method below is a good way of finding the ordering of x,y,z,w and
            // then find the correct traversal order for the simplex we’re in.
            // First, six pair-wise comparisons are performed between each possible pair
            // of the four coordinates, and the results are used to add up binary bits
            // for an integer index.
            int c1 = (x0 > y0) ? 32 : 0;
            int c2 = (x0 > z0) ? 16 : 0;
            int c3 = (y0 > z0) ? 8 : 0;
            int c4 = (x0 > w0) ? 4 : 0;
            int c5 = (y0 > w0) ? 2 : 0;
            int c6 = (z0 > w0) ? 1 : 0;
            int c = c1 + c2 + c3 + c4 + c5 + c6;
            int i1, j1, k1, l1; // The integer offsets for the second simplex corner
            int i2, j2, k2, l2; // The integer offsets for the third simplex corner
            int i3, j3, k3, l3; // The integer offsets for the fourth simplex corner
            // simplex[c] is a 4-vector with the numbers 0, 1, 2 and 3 in some order.
            // Many values of c will never occur, since e.g. x>y>z>w makes x<z, y<w and x<w
            // impossible. Only the 24 indices which have non-zero entries make any sense.
            // We use a thresholding to set the coordinates in turn from the largest magnitude.
            // The number 3 in the "simplex" array is at the position of the largest coordinate.
            i1 = Simplex[c, 0] >= 3 ? 1 : 0;
            j1 = Simplex[c, 1] >= 3 ? 1 : 0;
            k1 = Simplex[c, 2] >= 3 ? 1 : 0;
            l1 = Simplex[c, 3] >= 3 ? 1 : 0;
            // The number 2 in the "simplex" array is at the second largest coordinate.
            i2 = Simplex[c, 0] >= 2 ? 1 : 0;
            j2 = Simplex[c, 1] >= 2 ? 1 : 0;
            k2 = Simplex[c, 2] >= 2 ? 1 : 0;
            l2 = Simplex[c, 3] >= 2 ? 1 : 0;
            // The number 1 in the "simplex" array is at the second smallest coordinate.
            i3 = Simplex[c, 0] >= 1 ? 1 : 0;
            j3 = Simplex[c, 1] >= 1 ? 1 : 0;
            k3 = Simplex[c, 2] >= 1 ? 1 : 0;
            l3 = Simplex[c, 3] >= 1 ? 1 : 0;
            // The fifth corner has all coordinate offsets = 1, so no need to look that up.
            Double x1 = x0 - i1 + G4; // Offsets for second corner in (x,y,z,w) coords
            Double y1 = y0 - j1 + G4;
            Double z1 = z0 - k1 + G4;
            Double w1 = w0 - l1 + G4;
            Double x2 = x0 - i2 + 2.0 * G4; // Offsets for third corner in (x,y,z,w) coords
            Double y2 = y0 - j2 + 2.0 * G4;
            Double z2 = z0 - k2 + 2.0 * G4;
            Double w2 = w0 - l2 + 2.0 * G4;
            Double x3 = x0 - i3 + 3.0 * G4; // Offsets for fourth corner in (x,y,z,w) coords
            Double y3 = y0 - j3 + 3.0 * G4;
            Double z3 = z0 - k3 + 3.0 * G4;
            Double w3 = w0 - l3 + 3.0 * G4;
            Double x4 = x0 - 1.0 + 4.0 * G4; // Offsets for last corner in (x,y,z,w) coords
            Double y4 = y0 - 1.0 + 4.0 * G4;
            Double z4 = z0 - 1.0 + 4.0 * G4;
            Double w4 = w0 - 1.0 + 4.0 * G4;
            // Work out the hashed gradient indices of the five simplex corners
            uint h0 = Noise.HashCoordinates(i, j, k, l, (int)seed);
            uint h1 = Noise.HashCoordinates(i + i1, j + j1, k + k1, l + l1, (int)seed);
            uint h2 = Noise.HashCoordinates(i + i2, j + j2, k + k2, l + l2, (int)seed);
            uint h3 = Noise.HashCoordinates(i + i3, j + j3, k + k3, l + l3, (int)seed);
            uint h4 = Noise.HashCoordinates(i + 1, j + 1, k + 1, l + 1, (int)seed);

            Double[] g0 = {
                              NoiseLookupTable.Gradient4D[h0, 0], NoiseLookupTable.Gradient4D[h0, 1],
                              NoiseLookupTable.Gradient4D[h0, 2], NoiseLookupTable.Gradient4D[h0, 3]
                          };
            Double[] g1 = {
                              NoiseLookupTable.Gradient4D[h1, 0], NoiseLookupTable.Gradient4D[h1, 1],
                              NoiseLookupTable.Gradient4D[h1, 2], NoiseLookupTable.Gradient4D[h1, 3]
                          };
            Double[] g2 = {
                              NoiseLookupTable.Gradient4D[h2, 0], NoiseLookupTable.Gradient4D[h2, 1],
                              NoiseLookupTable.Gradient4D[h2, 2], NoiseLookupTable.Gradient4D[h2, 3]
                          };
            Double[] g3 = {
                              NoiseLookupTable.Gradient4D[h3, 0], NoiseLookupTable.Gradient4D[h3, 1],
                              NoiseLookupTable.Gradient4D[h3, 2], NoiseLookupTable.Gradient4D[h3, 3]
                          };
            Double[] g4 = {
                              NoiseLookupTable.Gradient4D[h4, 0], NoiseLookupTable.Gradient4D[h4, 1],
                              NoiseLookupTable.Gradient4D[h4, 2], NoiseLookupTable.Gradient4D[h4, 3]
                          };


            // Calculate the contribution from the five corners
            Double t0 = 0.6 - x0 * x0 - y0 * y0 - z0 * z0 - w0 * w0;
            if (t0 < 0) n0 = 0.0;
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * Noise.ArrayDot(g0, x0, y0, z0, w0);
            }
            Double t1 = 0.6 - x1 * x1 - y1 * y1 - z1 * z1 - w1 * w1;
            if (t1 < 0) n1 = 0.0;
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * Noise.ArrayDot(g1, x1, y1, z1, w1);
            }
            Double t2 = 0.6 - x2 * x2 - y2 * y2 - z2 * z2 - w2 * w2;
            if (t2 < 0) n2 = 0.0;
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * Noise.ArrayDot(g2, x2, y2, z2, w2);
            }
            Double t3 = 0.6 - x3 * x3 - y3 * y3 - z3 * z3 - w3 * w3;
            if (t3 < 0) n3 = 0.0;
            else
            {
                t3 *= t3;
                n3 = t3 * t3 * Noise.ArrayDot(g3, x3, y3, z3, w3);
            }
            Double t4 = 0.6 - x4 * x4 - y4 * y4 - z4 * z4 - w4 * w4;
            if (t4 < 0) n4 = 0.0;
            else
            {
                t4 *= t4;
                n4 = t4 * t4 * Noise.ArrayDot(g4, x4, y4, z4, w4);
            }
            // Sum up and scale the result to cover the range [-1,1]
            return 27.0 * (n0 + n1 + n2 + n3 + n4);
        }
       public Double Generate(Double x, Double y, Double z, Double w, Double u, Double v, Int32 seed, InterpolationDelegate interp)
        {
            // Skew
            var f4 = (Math.Sqrt(7.0) - 1.0) / 6.0;

            // Unskew
            var g4 = f4 / (1.0 + 6.0 * f4);

            var sideLength = Math.Sqrt(6.0) / (6.0 * f4 + 1.0);
            var a = Math.Sqrt((sideLength * sideLength) - ((sideLength / 2.0) * (sideLength / 2.0)));
            var cornerFace = Math.Sqrt(a * a + (a / 2.0) * (a / 2.0));

            var cornerFaceSqrd = cornerFace * cornerFace;

            var valueScaler = Math.Pow(5.0, -0.5);
            valueScaler *= Math.Pow(5.0, -3.5) * 100 + 13;

            var loc = new[] { x, y, z, w, u, v };
            var s = 0.00;
            for (var c = 0; c < 6; ++c)
                s += loc[c];
            s *= f4;

            var skewLoc = new[]{
                FastFloor(x + s), FastFloor(y + s), FastFloor(z + s),
                FastFloor(w + s), FastFloor(u + s), FastFloor(v + s)
            };
            var intLoc = new[]{
               FastFloor(x + s), FastFloor(y + s), FastFloor(z + s),
                FastFloor(w + s), FastFloor(u + s), FastFloor(v + s)
            };
            var unskew = 0.0;
            for (var c = 0; c < 6; ++c)
                unskew += skewLoc[c];
            unskew *= g4;

            var cellDist = new double[]
            {
                loc[0] - skewLoc[0] + unskew, loc[1] - skewLoc[1] + unskew,
                loc[2] - skewLoc[2] + unskew, loc[3] - skewLoc[3] + unskew,
                loc[4] - skewLoc[4] + unskew, loc[5] - skewLoc[5] + unskew
            };
            var distOrder = new[] { 0, 1, 2, 3, 4, 5 };
            Noise.SortBy6(cellDist, distOrder);

            var newDistOrder = new[]
            {
                -1, distOrder[0], distOrder[1], distOrder[2], distOrder[3], distOrder[4], distOrder[5]
            };

            var n = 0.00;
            var skewOffset = 0.00;

            for (var c = 0; c < 7; ++c)
            {
                var i = newDistOrder[c];
                if (i != -1)
                    intLoc[i] += 1;

                var uu = new Double[6];
                for (var d = 0; d < 6; ++d)
                {
                    uu[d] = cellDist[d] - (intLoc[d] - skewLoc[d]) + skewOffset;
                }

                var t = cornerFaceSqrd;

                for (var d = 0; d < 6; ++d)
                {
                    t -= uu[d] * uu[d];
                }

                if (t > 0.0)
                {
                    var h = Noise.HashCoordinates(intLoc[0], intLoc[1], intLoc[2], intLoc[3], intLoc[4], intLoc[5], seed);
                    var gr = 0.00;

                    gr += NoiseLookupTable.Gradient6D[h, 0] * uu[0];
                    gr += NoiseLookupTable.Gradient6D[h, 1] * uu[1];
                    gr += NoiseLookupTable.Gradient6D[h, 2] * uu[2];
                    gr += NoiseLookupTable.Gradient6D[h, 3] * uu[3];
                    gr += NoiseLookupTable.Gradient6D[h, 4] * uu[4];
                    gr += NoiseLookupTable.Gradient6D[h, 5] * uu[5];

                    n += gr * t * t * t * t;
                }
                skewOffset += g4;
            }
            n *= valueScaler;
            return n;
        }
        public double[] GetOffset()
        {
            return new double[] { 0, 0, 0, 0 };
        }

        public double[] GetScale()
        {
            return new double[] { 1.0, 1.0, 1.0, 1.0 };
        }
    }
}
