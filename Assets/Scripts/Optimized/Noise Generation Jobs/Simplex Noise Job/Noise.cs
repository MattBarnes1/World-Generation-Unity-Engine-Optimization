using System;
using System.Runtime.CompilerServices;

namespace NoiseJobs
{
    public delegate Double InterpolationDelegate(Double value);

    public delegate Double Noise2DDelegate(Double x, Double y, Int32 seed, InterpolationDelegate interp);

    public delegate Double Noise3DDelegate(Double x, Double y, Double z, Int32 seed, InterpolationDelegate interp);

    public delegate Double Noise4DDelegate(Double x, Double y, Double z, Double w, Int32 seed, InterpolationDelegate interp);

    public delegate Double Noise6DDelegate(Double x, Double y, Double z, Double w, Double u, Double v, Int32 seed, InterpolationDelegate interp);

    public static class Noise
    {
        public const Int32 MAX_SOURCES = 20;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Int32 FastFloor(Double t)
        {
            return (t > 0 ? (Int32)t : (Int32)t - 1);
        }

        internal static Double ArrayDot(Double[] arr, Double a, Double b)
        {
            return a*arr[0] + b*arr[1];
        }

        internal static Double ArrayDot(Double[] arr, Double a, Double b, Double c)
        {
            return a*arr[0] + b*arr[1] + c*arr[2];
        }

        internal static Double ArrayDot(Double[] arr, Double x, Double y, Double z, Double w)
        {
            return x*arr[0] + y*arr[1] + z*arr[2] + w*arr[3];
        }

        internal static Double ArrayDot(Double[] arr, Double x, Double y, Double z, Double w, Double u, Double v)
        {
            return x*arr[0] + y*arr[1] + z*arr[2] + w*arr[3] + u*arr[4] + v*arr[5];
        }

        internal static void AddDistance(Double[] f, Double[] disp, Double testdist, Double testdisp)
        {
            // Compare the given distance to the ones already in f
            if (testdist >= f[3]) return;

            var index = 3;
            while (index > 0 && testdist < f[index - 1]) index--;

            for (var i = 3; i-- > index; )
            {
                f[i + 1] = f[i];
                disp[i + 1] = disp[i];
            }
            f[index] = testdist;
            disp[index] = testdisp;
        }

        // Interpolation functions
        public static Double NoInterpolation(Double t)
        {
            return 0;
        }

        public static Double LinearInterpolation(Double t)
        {
            return t;
        }

        public static Double HermiteInterpolation(Double t)
        {
            return (t * t * (3 - 2 * t));
        }

        public static Double QuinticInterpolation(Double t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        // Edge/Face/Cube/Hypercube interpolation
        internal static Double Lerp(Double s, Double v1, Double v2)
        {
            return v1 + s * (v2 - v1);
        }

        #region Hashing

        // The "new" FNV-1A hashing
        private const UInt32 FNV_32_PRIME = 0x01000193;
        private const UInt32 FNV_32_INIT = 2166136261;
        private const UInt32 FNV_MASK_8 = (1 << 8) - 1;

        internal static UInt32 FNV32Buffer(Int32[] uintBuffer, UInt32 len)
        {
            //NOTE: Completely untested.
            var buffer = new byte[len];
            Buffer.BlockCopy(uintBuffer, 0, buffer, 0, buffer.Length);

            var hval = FNV_32_INIT;
            for (var i = 0; i < len;)
            {
                hval ^= buffer[i++];
                hval *= FNV_32_PRIME;
            }
            return hval;
        }

        internal static UInt32 FNV32Buffer(Double[] doubleBuffer, UInt32 len)
        {
            //NOTE: Completely untested.
            var buffer = new byte[len];
            Buffer.BlockCopy(doubleBuffer, 0, buffer, 0, buffer.Length);

            var hval = FNV_32_INIT;
            for (var i = 0; i < len;)
            {
                hval ^= buffer[i++];
                hval *= FNV_32_PRIME;
            }
            return hval;
        }

        internal static UInt32 FNV32Buffer(Byte[] buffer, UInt32 len)
        {
            var hval = FNV_32_INIT;
            for (var i = 0; i < len;)
            {
                hval ^= buffer[i++];
                hval *= FNV_32_PRIME;
            }
            return hval;
        }

        internal static UInt32 FNV1A_3d(Double x, Double y, Double z, Int32 seed)
        {
            Double[] d = {x, y, z, seed};
            return FNV32Buffer(d, sizeof (Double)*4);
        }

        internal static Byte XORFoldHash(UInt32 hash)
        {
            // Implement XOR-folding to reduce from 32 to 8-bit hash
            return (byte)((hash >> 8) ^ (hash & FNV_MASK_8));
        }

        // FNV-based coordinate hashes
        internal static UInt32 HashCoordinates(Int32 x, Int32 y, Int32 seed)
        {
            Int32[] d = { x, y, seed };
            return XORFoldHash(FNV32Buffer(d, sizeof(Int32) * 3));
        }

        internal static UInt32 HashCoordinates(Int32 x, Int32 y, Int32 z, Int32 seed)
        {
            Int32[] d = { x, y, z, seed };
            return XORFoldHash(FNV32Buffer(d, sizeof(Int32) * 4));
        }

        internal static UInt32 HashCoordinates(Int32 x, Int32 y, Int32 z, Int32 w, Int32 seed)
        {
            Int32[] d = { x, y, z, w, seed };
            return XORFoldHash(FNV32Buffer(d, sizeof(Int32) * 5));
        }

        internal static UInt32 HashCoordinates(Int32 x, Int32 y, Int32 z, Int32 w, Int32 u, Int32 v, Int32 seed)
        {
            Int32[] d = { x, y, z, w, u, v, seed };
            return XORFoldHash(FNV32Buffer(d, sizeof(Int32) * 7));
        }

        internal static UInt32 HashCoordinates(Double x, Double y, Int32 seed)
        {
            Double[] d = {x, y, seed};
            return XORFoldHash(FNV32Buffer(d, sizeof (Double)*3));
        }

        internal static UInt32 HashCoordinates(Double x, Double y, Double z, Int32 seed)
        {
            Double[] d = {x, y, z, seed};
            return XORFoldHash(FNV32Buffer(d, sizeof (Double)*4));
        }

        internal static UInt32 HashCoordinates(Double x, Double y, Double z, Double w, Int32 seed)
        {
            Double[] d = {x, y, z, w, seed};
            return XORFoldHash(FNV32Buffer(d, sizeof (Double)*5));
        }

        internal static UInt32 HashCoordinates(Double x, Double y, Double z, Double w, Double u, Double v, Int32 seed)
        {
            Double[] d = {x, y, z, w, u, v, seed};
            return XORFoldHash(FNV32Buffer(d, sizeof (Double)*7));
        }

        internal delegate Double WorkerNoise2(Double x, Double y, Int32 ix, Int32 iy, Int32 seed);

        internal delegate Double WorkerNoise3(Double x, Double y, Double z, Int32 ix, Int32 iy, Int32 iz, Int32 seed);

        internal delegate Double WorkerNoise4(Double x, Double y, Double z, Double w, Int32 ix, Int32 iy, Int32 iz, Int32 iw, Int32 seed);

        internal delegate Double WorkerNoise6(Double x, Double y, Double z, Double w, Double u, Double v, Int32 ix, Int32 iy, Int32 iz, Int32 iw, Int32 iu, Int32 iv, Int32 seed);

        // Noise generators
        internal static Double InternalValueNoise(
            Double x, Double y, 
            Int32 ix, Int32 iy, 
            Int32 seed)
        {
            var noise = Noise.HashCoordinates(ix, iy, seed) / 255.0;
            return noise * 2.0 - 1.0;
        }

        internal static Double InternalValueNoise(
            Double x, Double y, Double z, 
            Int32 ix, Int32 iy, Int32 iz, 
            Int32 seed)
        {
            var noise = Noise.HashCoordinates(ix, iy, iz, seed) / (255.0);
            return noise * 2.0 - 1.0;
        }

        internal static Double InternalValueNoise(
            Double x, Double y, Double z, Double w, 
            Int32 ix, Int32 iy, Int32 iz, Int32 iw, 
            Int32 seed)
        {
            var noise = Noise.HashCoordinates(ix, iy, iz, iw, seed) / 255.0;
            return noise * 2.0 - 1.0;
        }

        internal static Double InternalValueNoise(
            Double x, Double y, Double z, Double w, Double u, Double v, 
            Int32 ix, Int32 iy, Int32 iz, Int32 iw, Int32 iu, Int32 iv, 
            Int32 seed)
        {
            var noise = Noise.HashCoordinates(ix, iy, iz, iw, iu, iv, seed) / 255.0;
            return noise * 2.0 - 1.0;
        }


       
        internal static Double interpolate_X_2(
            Double x, Double y, Double xs,
            Int32 x0, Int32 x1, Int32 iy,
            Int32 seed, WorkerNoise2 noisefunc)
        {
            var v1 = noisefunc(x, y, x0, iy, seed);
            var v2 = noisefunc(x, y, x1, iy, seed);

            return Lerp(xs, v1, v2);
        }

        internal static Double interpolate_XY_2(
            Double x, Double y, Double xs, Double ys, 
            Int32 x0, Int32 x1, Int32 y0, Int32 y1,
            Int32 seed, WorkerNoise2 noisefunc)
        {
            var v1 = interpolate_X_2(x, y, xs, x0, x1, y0, seed, noisefunc);
            var v2 = interpolate_X_2(x, y, xs, x0, x1, y1, seed, noisefunc);

            return Lerp(ys, v1, v2);
        }

        internal static Double interpolate_X_3(
            Double x, Double y, Double z, Double xs,
            Int32 x0, Int32 x1, Int32 iy, Int32 iz,
            Int32 seed, WorkerNoise3 noisefunc)
        {
            var v1 = noisefunc(x, y, z, x0, iy, iz, seed);
            var v2 = noisefunc(x, y, z, x1, iy, iz, seed);

            return Lerp(xs, v1, v2);
        }

        internal static Double interpolate_XY_3(
            Double x, Double y, Double z, Double xs, Double ys,
            Int32 x0, Int32 x1, Int32 y0, Int32 y1, Int32 iz, 
            Int32 seed, WorkerNoise3 noisefunc)
        {
            var v1 = interpolate_X_3(x, y, z, xs, x0, x1, y0, iz, seed, noisefunc);
            var v2 = interpolate_X_3(x, y, z, xs, x0, x1, y1, iz, seed, noisefunc);

            return Lerp(ys, v1, v2);
        }

        internal static Double interpolate_XYZ_3(
            Double x, Double y, Double z, Double xs, Double ys, Double zs,
            Int32 x0, Int32 x1, Int32 y0, Int32 y1, Int32 z0, Int32 z1,
            Int32 seed, WorkerNoise3 noisefunc)
        {
            var v1 = interpolate_XY_3(x, y, z, xs, ys, x0, x1, y0, y1, z0, seed, noisefunc);
            var v2 = interpolate_XY_3(x, y, z, xs, ys, x0, x1, y0, y1, z1, seed, noisefunc);

            return Lerp(zs, v1, v2);
        }

        internal static Double interpolate_X_4(
            Double x, Double y, Double z, Double w, Double xs,
            Int32 x0, Int32 x1, Int32 iy, Int32 iz, Int32 iw,
            Int32 seed, WorkerNoise4 noisefunc)
        {
            var v1 = noisefunc(x, y, z, w, x0, iy, iz, iw, seed);
            var v2 = noisefunc(x, y, z, w, x1, iy, iz, iw, seed);

            return Lerp(xs, v1, v2);
        }

        internal static Double interpolate_XY_4(
            Double x, Double y, Double z, Double w, Double xs, Double ys,
            Int32 x0, Int32 x1, Int32 y0, Int32 y1, Int32 iz, Int32 iw,
            Int32 seed, WorkerNoise4 noisefunc)
        {
            var v1 = interpolate_X_4(x, y, z, w, xs, x0, x1, y0, iz, iw, seed, noisefunc);
            var v2 = interpolate_X_4(x, y, z, w, xs, x0, x1, y1, iz, iw, seed, noisefunc);

            return Lerp(ys, v1, v2);
        }

        internal static Double interpolate_XYZ_4(
            Double x, Double y, Double z, Double w, Double xs, Double ys, Double zs,
            Int32 x0, Int32 x1, Int32 y0, Int32 y1, Int32 z0, Int32 z1, Int32 iw,
            Int32 seed, WorkerNoise4 noisefunc)
        {
            var v1 = interpolate_XY_4(x, y, z, w, xs, ys, x0, x1, y0, y1, z0, iw, seed, noisefunc);
            var v2 = interpolate_XY_4(x, y, z, w, xs, ys, x0, x1, y0, y1, z1, iw, seed, noisefunc);

            return Lerp(zs, v1, v2);
        }

        internal static Double interpolate_XYZW_4(
            Double x, Double y, Double z, Double w, Double xs, Double ys, Double zs, Double ws,
            Int32 x0, Int32 x1, Int32 y0, Int32 y1, Int32 z0, Int32 z1, Int32 w0, Int32 w1,
            Int32 seed, WorkerNoise4 noisefunc)
        {
            var v1 = interpolate_XYZ_4(x, y, z, w, xs, ys, zs, x0, x1, y0, y1, z0, z1, w0, seed, noisefunc);
            var v2 = interpolate_XYZ_4(x, y, z, w, xs, ys, zs, x0, x1, y0, y1, z0, z1, w1, seed, noisefunc);

            return Lerp(ws, v1, v2);
        }

        internal static Double interpolate_X_6(
            Double x, Double y, Double z, Double w, Double u, Double v, Double xs,
            Int32 x0, Int32 x1, Int32 iy, Int32 iz, Int32 iw, Int32 iu, Int32 iv,
            Int32 seed, WorkerNoise6 noisefunc)
        {
            var v1 = noisefunc(x, y, z, w, u, v, x0, iy, iz, iw, iu, iv, seed);
            var v2 = noisefunc(x, y, z, w, u, v, x1, iy, iz, iw, iu, iv, seed);

            return Lerp(xs, v1, v2);
        }

        internal static Double interpolate_XY_6(
            Double x, Double y, Double z, Double w, Double u, Double v, 
            Double xs, Double ys,
            Int32 x0, Int32 x1, Int32 y0, Int32 y1, Int32 iz, Int32 iw, Int32 iu, Int32 iv,
            Int32 seed, WorkerNoise6 noisefunc)
        {
            var v1 = interpolate_X_6(x, y, z, w, u, v, xs, x0, x1, y0, iz, iw, iu, iv, seed, noisefunc);
            var v2 = interpolate_X_6(x, y, z, w, u, v, xs, x0, x1, y1, iz, iw, iu, iv, seed, noisefunc);

            return Lerp(ys, v1, v2);
        }

        internal static Double interpolate_XYZ_6(
            Double x, Double y, Double z, Double w, Double u, Double v, 
            Double xs, Double ys, Double zs,
            Int32 x0, Int32 x1, Int32 y0, Int32 y1, Int32 z0, Int32 z1, Int32 iw, Int32 iu, Int32 iv,
            Int32 seed, WorkerNoise6 noisefunc)
        {
            var v1 = interpolate_XY_6(x, y, z, w, u, v, xs, ys, x0, x1, y0, y1, z0, iw, iu, iv, seed, noisefunc);
            var v2 = interpolate_XY_6(x, y, z, w, u, v, xs, ys, x0, x1, y0, y1, z1, iw, iu, iv, seed, noisefunc);

            return Lerp(zs, v1, v2);
        }

        internal static Double interpolate_XYZW_6(
            Double x, Double y, Double z, Double w, Double u, Double v, 
            Double xs, Double ys, Double zs, Double ws,
            Int32 x0, Int32 x1, Int32 y0, Int32 y1, Int32 z0, Int32 z1, Int32 w0, Int32 w1, Int32 iu, Int32 iv,
            Int32 seed, WorkerNoise6 noisefunc)
        {
            var v1 = interpolate_XYZ_6(x, y, z, w, u, v, xs, ys, zs, x0, x1, y0, y1, z0, z1, w0, iu, iv, seed, noisefunc);
            var v2 = interpolate_XYZ_6(x, y, z, w, u, v, xs, ys, zs, x0, x1, y0, y1, z0, z1, w1, iu, iv, seed, noisefunc);

            return Lerp(ws, v1, v2);
        }

        internal static Double interpolate_XYZWU_6(
            Double x, Double y, Double z, Double w, Double u, Double v, 
            Double xs, Double ys, Double zs, Double ws, Double us,
            Int32 x0, Int32 x1, Int32 y0, Int32 y1, Int32 z0, Int32 z1, Int32 w0, Int32 w1, Int32 u0, Int32 u1, Int32 iv,
            Int32 seed, WorkerNoise6 noisefunc)
        {
            var v1 = interpolate_XYZW_6(x, y, z, w, u, v, xs, ys, zs, ws, x0, x1, y0, y1, z0, z1, w0, w1, u0, iv, seed, noisefunc);
            var v2 = interpolate_XYZW_6(x, y, z, w, u, v, xs, ys, zs, ws, x0, x1, y0, y1, z0, z1, w0, w1, u1, iv, seed, noisefunc);

            return Lerp(us, v1, v2);
        }

        internal static Double interpolate_XYZWUV_6(
            Double x, Double y, Double z, Double w, Double u, Double v, 
            Double xs, Double ys, Double zs, Double ws, Double us, Double vs,
            Int32 x0, Int32 x1, Int32 y0, Int32 y1, Int32 z0, Int32 z1, Int32 w0, Int32 w1, Int32 u0, Int32 u1, Int32 v0, Int32 v1,
            Int32 seed, WorkerNoise6 noisefunc)
        {
            var val1 = interpolate_XYZWU_6(x, y, z, w, u, v, xs, ys, zs, ws, us, x0, x1, y0, y1, z0, z1, w0, w1, u0, u1, v0, seed, noisefunc);
            var val2 = interpolate_XYZWU_6(x, y, z, w, u, v, xs, ys, zs, ws, us, x0, x1, y0, y1, z0, z1, w0, w1, u0, u1, v1, seed, noisefunc);

            return Lerp(vs, val1, val2);
        }

        // The usable noise functions
      

        

       

        public static void CellularFunction(Double x, Double y, Int32 seed, Double[] f, Double[] disp)
        {
            var xInt = FastFloor(x);
            var yInt = FastFloor(y);

            for (var c = 0; c < 4; ++c)
            {
                f[c] = 99999.0;
                disp[c] = 0.0;
            }

            for (var ycur = yInt - 3; ycur <= yInt + 3; ++ycur)
            {
                for (var xcur = xInt - 3; xcur <= xInt + 3; ++xcur)
                {
                    var xpos = xcur + InternalValueNoise(x, y, xcur, ycur, seed);
                    var ypos = ycur + InternalValueNoise(x, y, xcur, ycur, seed + 1);
                    var xdist = xpos - x;
                    var ydist = ypos - y;
                    var dist = (xdist * xdist + ydist * ydist);
                    var xval = FastFloor(xpos);
                    var yval = FastFloor(ypos);
                    var dsp = InternalValueNoise(x, y, xval, yval, seed + 3);
                    AddDistance(f, disp, dist, dsp);
                }
            }
        }

        public static void CellularFunction(Double x, Double y, Double z, Int32 seed, Double[] f, Double[] disp)
        {
            var xInt = FastFloor(x);
            var yInt = FastFloor(y);
            var zInt = FastFloor(z);

            for (var c = 0; c < 4; ++c)
            {
                f[c] = 99999.0;
                disp[c] = 0.0;
            }

            for (var zcur = zInt - 2; zcur <= zInt + 2; ++zcur)
            {
                for (var ycur = yInt - 2; ycur <= yInt + 2; ++ycur)
                {
                    for (var xcur = xInt - 2; xcur <= xInt + 2; ++xcur)
                    {
                        var xpos = xcur + InternalValueNoise(x, y, z, xcur, ycur, zcur, seed);
                        var ypos = ycur + InternalValueNoise(x, y, z, xcur, ycur, zcur, seed + 1);
                        var zpos = zcur + InternalValueNoise(x, y, z, xcur, ycur, zcur, seed + 2);
                        var xdist = xpos - x;
                        var ydist = ypos - y;
                        var zdist = zpos - z;
                        var dist = (xdist * xdist + ydist * ydist + zdist * zdist);
                        var xval = FastFloor(xpos);
                        var yval = FastFloor(ypos);
                        var zval = FastFloor(zpos);
                        var dsp = InternalValueNoise(x, y, z, xval, yval, zval, seed + 3);
                        AddDistance(f, disp, dist, dsp);
                    }
                }
            }
        }

        public static void CellularFunction(Double x, Double y, Double z, Double w, Int32 seed, Double[] f, Double[] disp)
        {
            var xInt = FastFloor(x);
            var yInt = FastFloor(y);
            var zInt = FastFloor(z);
            var wInt = FastFloor(w);

            for (var c = 0; c < 4; ++c)
            {
                f[c] = 99999.0;
                disp[c] = 0.0;
            }

            for (var wcur = wInt - 2; wcur <= wInt + 2; ++wcur)
            {
                for (var zcur = zInt - 2; zcur <= zInt + 2; ++zcur)
                {
                    for (var ycur = yInt - 2; ycur <= yInt + 2; ++ycur)
                    {
                        for (var xcur = xInt - 2; xcur <= xInt + 2; ++xcur)
                        {
                            var xpos = xcur + InternalValueNoise(x, y, z, w, xcur, ycur, zcur, wcur, seed);
                            var ypos = ycur + InternalValueNoise(x, y, z, w, xcur, ycur, zcur, wcur, seed + 1);
                            var zpos = zcur + InternalValueNoise(x, y, z, w, xcur, ycur, zcur, wcur, seed + 2);
                            var wpos = wcur + InternalValueNoise(x, y, z, w, xcur, ycur, zcur, wcur, seed + 3);
                            var xdist = xpos - x;
                            var ydist = ypos - y;
                            var zdist = zpos - z;
                            var wdist = wpos - w;
                            var dist = (xdist * xdist + ydist * ydist + zdist * zdist + wdist * wdist);
                            var xval = FastFloor(xpos);
                            var yval = FastFloor(ypos);
                            var zval = FastFloor(zpos);
                            var wval = FastFloor(wpos);
                            var dsp = InternalValueNoise(x, y, z, w, xval, yval, zval, wval, seed + 3);
                            AddDistance(f, disp, dist, dsp);
                        }
                    }
                }
            }
        }

        public static void CellularFunction(Double x, Double y, Double z, Double w, Double u, Double v, Int32 seed, Double[] f, Double[] disp)
        {
            var xInt = FastFloor(x);
            var yInt = FastFloor(y);
            var zInt = FastFloor(z);
            var wInt = FastFloor(w);
            var uInt = FastFloor(u);
            var vInt = FastFloor(v);

            for (var c = 0; c < 4; ++c)
            {
                f[c] = 99999.0;
                disp[c] = 0.0;
            }

            for (var vcur = vInt - 1; vcur <= vInt + 1; ++vcur)
            {
                for (var ucur = uInt - 1; ucur <= uInt + 1; ++ucur)
                {

                    for (var wcur = wInt - 2; wcur <= wInt + 2; ++wcur)
                    {
                        for (var zcur = zInt - 2; zcur <= zInt + 2; ++zcur)
                        {
                            for (var ycur = yInt - 2; ycur <= yInt + 2; ++ycur)
                            {
                                for (var xcur = xInt - 2; xcur <= xInt + 2; ++xcur)
                                {
                                    var xpos = xcur + InternalValueNoise(x, y, z, w, u, v, xcur, ycur, zcur, wcur, ucur, vcur, seed);
                                    var ypos = ycur + InternalValueNoise(x, y, z, w, u, v, xcur, ycur, zcur, wcur, ucur, vcur, seed + 1);
                                    var zpos = zcur + InternalValueNoise(x, y, z, w, u, v, xcur, ycur, zcur, wcur, ucur, vcur, seed + 2);
                                    var wpos = wcur + InternalValueNoise(x, y, z, w, u, v, xcur, ycur, zcur, wcur, ucur, vcur, seed + 3);
                                    var upos = ucur + InternalValueNoise(x, y, z, w, u, v, xcur, ycur, zcur, wcur, ucur, vcur, seed + 4);
                                    var vpos = vcur + InternalValueNoise(x, y, z, w, u, v, xcur, ycur, zcur, wcur, ucur, vcur, seed + 5);
                                    var xdist = xpos - x;
                                    var ydist = ypos - y;
                                    var zdist = zpos - z;
                                    var wdist = wpos - w;
                                    var udist = upos - u;
                                    var vdist = vpos - v;
                                    var dist = (xdist * xdist + ydist * ydist + zdist * zdist + wdist * wdist + udist * udist + vdist * vdist);
                                    var xval = FastFloor(xpos);
                                    var yval = FastFloor(ypos);
                                    var zval = FastFloor(zpos);
                                    var wval = FastFloor(wpos);
                                    var uval = FastFloor(upos);
                                    var vval = FastFloor(vpos);
                                    var dsp = InternalValueNoise(x, y, z, w, u, v, xval, yval, zval, wval, uval, vval, seed + 6);
                                    AddDistance(f, disp, dist, dsp);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static readonly Double F2 = 0.36602540378443864676372317075294;
        public static readonly Double G2 = 0.21132486540518711774542560974902;
        public static readonly Double F3 = 1.0 / 3.0;
        public static readonly Double G3 = 1.0 / 6.0;



        internal struct VectorOrdering4
        {
            internal Double Coordinates;

            internal Int32 X;

            internal Int32 Y;

            internal Int32 Z;

            internal Int32 W;

            internal VectorOrdering4(Double v, Int32 x, Int32 y, Int32 z, Int32 w)
            {
                this.Coordinates = v;
                this.X = x;
                this.Y = y;
                this.Z = z;
                this.W = w;
            }
        }

        internal static Int32 VectorOrdering4Compare(VectorOrdering4 v1, VectorOrdering4 v2)
        {
            return v1.Coordinates.CompareTo(v2.Coordinates);
        }

        internal struct VectorOrdering
        {
            internal Double Value;

            internal Int32 Axis;

            internal VectorOrdering(Double v, Int32 a)
            {
                Value = v;
                Axis = a;
            }
        }

        internal static Int32 VectorOrderingCompare(VectorOrdering v1, VectorOrdering v2)
        {
            return v1.Value.CompareTo(v2.Value);
        }

        internal static void SortBy4(Double[] l1, Int32[] l2)
        {
            var a = new VectorOrdering[4];
            for (var c = 0; c < 4; c += 1)
            {
                a[c].Value = l1[c];
                a[c].Axis = l2[c];
            }

            Array.Sort(a, VectorOrderingCompare);

            for (var c = 0; c < 4; c += 1)
            {
                l2[c] = a[c].Axis;
            }
        }

        internal static void SortBy6(Double[] l1, Int32[] l2)
        {
            var a = new VectorOrdering[6];
            for (var c = 0; c < 6; c += 1)
            {
                a[c].Value = l1[c];
                a[c].Axis = l2[c];
            }

            Array.Sort(a, VectorOrderingCompare);

            for (var c = 0; c < 6; c += 1)
            {
                l2[c] = a[c].Axis;
            }
        }
         



        public static Double NewSimplexNoise4D(Double x, Double y, Double z, Double w, Int32 seed, InterpolationDelegate interp)
        {
            var f4 = (Math.Sqrt(5.0) - 1.0)/4.0;
            var sideLength = 2.0/(4.0*f4 + 1.0);
            var a = Math.Sqrt((sideLength*sideLength) - ((sideLength/2.0)*(sideLength/2.0)));
            var cornerToFace = Math.Sqrt((a*a + (a/2.0)*(a/2.0)));
            var cornerToFaceSquared = cornerToFace*cornerToFace;

            var valueScaler = Math.Pow(3.0, -0.5);

            var g4 = f4/(1.0 + 4.0*f4);
            valueScaler *= Math.Pow(3.0, -3.5)*100.0 + 13.0;

            Double[] loc = {x, y, z, w};
            Double s = 0;
            for (var c = 0; c < 4; ++c)
                s += loc[c];
            s *= f4;

            var skewLoc = new[] {FastFloor(x + s), FastFloor(y + s), FastFloor(z + s), FastFloor(w + s)};
            var intLoc = new[] {FastFloor(x + s), FastFloor(y + s), FastFloor(z + s), FastFloor(w + s)};
            var unskew = 0.00;
            for (var c = 0; c < 4; ++c)
                unskew += skewLoc[c];
            unskew *= g4;
            var cellDist = new[]
            {
                loc[0] - skewLoc[0] + unskew, loc[1] - skewLoc[1] + unskew,
                loc[2] - skewLoc[2] + unskew, loc[3] - skewLoc[3] + unskew
            };
            var distOrder = new[] {0, 1, 2, 3};
            SortBy4(cellDist, distOrder);

            var newDistOrder = new[] {-1, distOrder[0], distOrder[1], distOrder[2], distOrder[3]};

            var n = 0.00;
            var skewOffset = 0.00;

            for (var c = 0; c < 5; ++c)
            {
                var i = newDistOrder[c];
                if (i != -1)
                    intLoc[i] += 1;

                var u = new Double[4];
                for (var d = 0; d < 4; ++d)
                {
                    u[d] = cellDist[d] - (intLoc[d] - skewLoc[d]) + skewOffset;
                }

                var t = cornerToFaceSquared;

                for (var d = 0; d < 4; ++d)
                {
                    t -= u[d]*u[d];
                }

                if (t > 0.0)
                {
                    var h = HashCoordinates(intLoc[0], intLoc[1], intLoc[2], intLoc[3], seed);
                    var gr = 0.00;
                    for (var d = 0; d < 4; ++d)
                    {
                        gr += NoiseLookupTable.Gradient4D[h, d]*u[d];
                    }

                    n += gr*t*t*t*t;
                }
                skewOffset += g4;
            }
            n *= valueScaler;
            return n;
        }

        #endregion
    }
}