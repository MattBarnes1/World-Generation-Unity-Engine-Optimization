using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
public struct MultiNoiseFractal : IJobParallelFor
{
    [ReadOnly]
    float Lacunarity;

    [WriteOnly]
    NativeArray<float> myFractalOutput;
    [NativeDisableParallelForRestriction, ReadOnly]
    public NativeHashMap<int, NativeArray<float>> myHashToArray;
    [NativeDisableParallelForRestriction, ReadOnly]
    public NativeHashMap<int, float> myHashToLacunarity;
    [ReadOnly]
    int Width;

    public void Execute(int index)
    {
        NativeArray<double> m_exparray = new NativeArray<double>(myHashToArray.Count(), Allocator.TempJob);
        NativeArray<double> m_correct = new NativeArray<double>(myHashToArray.Count() * 2, Allocator.TempJob);

        for (int i = 0; i < myHashToArray.Count(); ++i)
        {
            m_exparray[i] = Math.Pow(myHashToLacunarity[i], -i * 1.0);
        }

        // Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
        double minvalue = 1.0, maxvalue = 1.0;
        for (int i = 0; i < myHashToArray.Count(); ++i)
        {
            minvalue *= -1.0 * m_exparray[i] + 1.0;
            maxvalue *= 1.0 * m_exparray[i] + 1.0;

            double A = -1.0, B = 1.0;
            double scale = (B - A) / (maxvalue - minvalue);
            double bias = A - minvalue * scale;
            m_correct[i] = scale;
            m_correct[i + myHashToArray.Count()] = bias;
        }

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private double Multi_get(float x, float y, NativeArray<double> m_exparray, NativeArray<double> m_correct)
    {
        return 1;
    }
}
