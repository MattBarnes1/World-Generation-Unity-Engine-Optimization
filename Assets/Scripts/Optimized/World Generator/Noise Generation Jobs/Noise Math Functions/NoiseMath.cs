using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Mathematics;

public class NoiseMath
{
    static readonly uint FNV_32_PRIME = ((uint)0x01000193);
    static readonly uint FNV_32_INIT = ((uint)2166136261);
    static readonly uint FNV_MASK_8 = (((uint)1 << 8) - 1);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int fast_floor(double t)
    {
        return (t > 0 ? (int)t : (int)t - 1);
    }
    public static NativeArray<float2> gradient2D_lut = new NativeArray<float2>(new float2[]
    {
        new float2(0, 1),
        new float2(0, -1),
        new float2(1, 0),
        new float2(-1, 0),
        new float2(0, 1),
        new float2(0, -1),
        new float2(1, 0),
        new float2(-1, 0),
        new float2(0, 1),
        new float2(0, -1),
        new float2(1, 0),
        new float2(-1, 0),
        new float2(0, 1),
        new float2(0, -1),
        new float2(1, 0),
        new float2(-1, 0),
        new float2(0, 1),
        new float2(0, -1),
        new float2(1, 0),
        new float2(-1, 0),
        new float2(0, 1),
        new float2(0, -1),
        new float2(1, 0),
        new float2(-1, 0),
        new float2(0, 1),
        new float2(0, -1),
        new float2(1, 0),
        new float2(-1, 0),
        new float2(0, 1),
        new float2(0, -1),
        new float2(1, 0),
        new float2(-1, 0),
        new float2(0, 1),
        new float2(0, -1),
        new float2(1, 0),
        new float2(-1, 0),
        new float2(0, 1),
        new float2(0, -1),
        new float2(1, 0),
        new float2(-1, 0),
        new float2(0, 1),
        new float2(0, -1),
        new float2(1, 0),
        new float2(-1, 0),
        new float2(0, 1),
        new float2(0, -1),
        new float2(1, 0),
        new float2(-1, 0),
        new float2(0, 1),
        new float2(0, -1),
        new float2(1, 0),
        new float2(-1, 0),
        new float2(0, 1),
        new float2(0, -1),
        new float2(1, 0),
        new float2(-1, 0),
        new float2(0, 1),
        new float2(0, -1),
        new float2(1, 0),
        new float2(-1, 0),
        new float2(0, 1),
        new float2(0, -1),
        new float2(1, 0),
        new float2(-1, 0),
        new float2(0, 1),
        new float2(0, -1),
        new float2(1, 0),
        new float2(-1, 0),
        new float2(0, 1),
        new float2(0, -1),
        new float2(1, 0),
        new float2(-1, 0),
        new float2(0, 1),
        new float2(0, -1),
        new float2(1, 0),
        new float2(-1, 0),
        new float2(0, 1),
        new float2(0, -1),
        new float2(1, 0),
        new float2(-1, 0),
        new float2(0, 1),
        new float2(0, -1),
        new float2(1, 0),
        new float2(-1, 0),
        new float2(0, 1),
        new float2(0, -1),
        new float2(1, 0),
        new float2(-1, 0),
        new float2(0, 1),
        new float2(0, -1),
       new float2(1, 0),
       new float2(-1, 0),
       new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
       new float2(-1, 0),
       new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
       new float2(-1, 0),
       new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
       new float2(-1, 0),
       new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
       new float2(-1, 0),
       new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
       new float2(-1, 0),
       new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
       new float2(-1, 0),
       new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
      new float2(-1, 0),
      new float2(0, 1),
      new float2(0, -1),
      new float2(1, 0),
      new float2(-1, 0),
      new float2(0, 1),
      new float2(0, -1),
      new float2(1, 0),
      new float2(-1, 0),
      new float2(0, 1),
      new float2(0, -1),
      new float2(1, 0),
      new float2(-1, 0),
      new float2(0, 1),
      new float2(0, -1),
      new float2(1, 0),
      new float2(-1, 0),
       new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
       new float2(-1, 0),
      new float2(0, 1),
      new float2(0, -1),
      new float2(1, 0),
      new float2(-1, 0),
      new float2(0, 1),
      new float2(0, -1),
      new float2(1, 0),
      new float2(-1, 0),
      new float2(0, 1),
      new float2(0, -1),
      new float2(1, 0),
      new float2(-1, 0),
      new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
       new float2(-1, 0),
       new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
       new float2(-1, 0),
       new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
       new float2(-1, 0),
       new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
       new float2(-1, 0),
       new float2(0, 1),
        new float2(0, -1),
       new float2(1, 0),
       new float2(-1, 0),
       new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
      new float2(-1, 0),
       new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
       new float2(-1, 0),
       new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
       new float2(-1, 0),
       new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
       new float2(-1, 0),
       new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
       new float2(-1, 0),
       new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
       new float2(-1, 0),
       new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
       new float2(-1, 0),
       new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
       new float2(-1, 0),
       new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
       new float2(-1, 0),
      new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
       new float2(-1, 0),
      new float2(0, 1),
      new float2(0, -1),
      new float2(1, 0),
      new float2(-1, 0),
       new float2(0, 1),
      new float2(0, -1),
      new float2(1, 0),
      new float2(-1, 0),
      new float2(0, 1),
      new float2(0, -1),
      new float2(1, 0),
      new float2(-1, 0),
       new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
       new float2(-1, 0),
       new float2(0, 1),
      new float2(0, -1),
      new float2(1, 0),
       new float2(-1, 0),
       new float2(0, 1),
       new float2(0, -1),
      new float2(1, 0),
      new float2(-1, 0),
      new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
       new float2(-1, 0),
      new float2(0, 1),
      new float2(0, -1),
       new float2(1, 0),
      new float2(-1, 0),
       new float2(0, 1),
      new float2(0, -1),
      new float2(1, 0),
      new float2(-1, 0),
      new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
      new float2(-1, 0),
       new float2(0, 1),
       new float2(0, -1),
       new float2(1, 0),
      new float2(-1, 0) }, Allocator.Persistent);





    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ArrayDot(float2 arr, float a, float b)
    {
        return math.dot(arr, new float2(a,b)); 
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ArrayDot(float[] arr, float a, float b, float c)
    {
        return a * arr[0] + b * arr[1] + c * arr[2];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ArrayDot(float[] arr, float x, float y, float z, float w)
    {
        return x * arr[0] + y * arr[1] + z * arr[2] + w * arr[3];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ArrayDot(float[] arr, float x, float y, float z, float w, float u, float v)
    {
        return x * arr[0] + y * arr[1] + z * arr[2] + w * arr[3] + u * arr[4] + v * arr[5];
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Quintic_Blend(double f)
    {
        return f * f * f * (f * (f * 6 - 15) + 10);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Lerp(double f, double a, double b)
    {
        return a + f * (b - a);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint hash_coords_2(int x, int y, uint seed)
    {
        uint[] d = new uint[3]
        {
                (uint)x,
                (uint)y,
                seed
        };

        return xor_fold_hash(fnv_32_a_buf(d));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint fnv_32_a_buf(uint[] buf)
    {
        uint hval = FNV_32_INIT;
        byte[] bp = IntsToBytes(buf);
        int i = 0;

        while (i < bp.Length)
        {
            hval ^= (uint)bp[i++];
            hval *= FNV_32_PRIME;
        }

        return hval;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte xor_fold_hash(uint hash)
    {
        // Implement XOR-folding to reduce from 32 to 8-bit hash
        return (byte)((hash >> 8) ^ (hash & FNV_MASK_8));
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] IntsToBytes(uint[] ints)
    {
        byte[] bytes = new byte[ints.Length * 4];
        for (int i = 0, j = 0; i < ints.Length; i++)
        {
            bytes[j++] = (byte)(ints[i] & 0xFF);
            bytes[j++] = (byte)((ints[i] >> 8) & 0xFF);
            bytes[j++] = (byte)((ints[i] >> 16) & 0xFF);
            bytes[j++] = (byte)((ints[i] >> 24) & 0xFF);
        }
        return bytes;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Bias(double b, double t)
    {
        return Math.Pow(t, Math.Log(b) / Math.Log(0.5));
    }
}
