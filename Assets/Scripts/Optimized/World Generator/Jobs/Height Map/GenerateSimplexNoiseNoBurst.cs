
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
public struct GenerateSimplexNoiseNoBurst : IJobParallelFor
{
    [NativeDisableParallelForRestriction]
    public NativeArray<float> myOutputArray;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> myMaxMinArray;

    [ReadOnly]
    public int Width;

    [ReadOnly]
    public int Height;

    [ReadOnly]
    public int Seed;

    [ReadOnly]
    public int Octaves;

    [ReadOnly]
    public float Frequency;

    [ReadOnly]
    public float Amplitude;

    [ReadOnly]
    public float Persistence;

    [ReadOnly]
    public float Lacunarity;

    public void Execute(int RowValue)
    {

        //We use the Max set to float.MinValue and Min to Float.MaxValue so that we know they always will be used.
        float currentThreadMaxValue = float.MinValue;
        float currentThreadMinValue = float.MaxValue;
        //Doing this locally allows for one check at the end of our loop instead of locking up each loop
        //NativeArray<float2> myCoordinateGroups = new NativeArray<float2>(4, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        //Since our Y will never change we can keep the value constant for all groups;



        for (var x = 0; x < Width; x += 2) //so we can cast the position instead;
        {
            float frequency = Frequency;
            float amplitude = Amplitude;
            float persistance = Persistence;
            float lacunarity = Lacunarity;
            float MaxAmp = 0;
            float2 ReturnValue = new float2();
            for (var i = 0; i < Octaves; i++)
            {
                float2 myValuesAtOnce1 = new float2(x + Seed, RowValue + Seed);
                float2 myValuesAtOnce2 = new float2(x + Seed + 1, RowValue + Seed);

                ReturnValue = new float2(ReturnValue.x += noise.snoise(myValuesAtOnce1 * frequency) * amplitude, ReturnValue.y += noise.snoise(myValuesAtOnce2 * frequency) * amplitude);
                MaxAmp += amplitude;
                amplitude *= persistance;
                frequency *= lacunarity;
            }
            ReturnValue /= MaxAmp;
            if (ReturnValue[0] > currentThreadMaxValue)
                currentThreadMaxValue = ReturnValue[0];
            if (ReturnValue[0] < currentThreadMinValue)
                currentThreadMinValue = ReturnValue[0];
            if (ReturnValue[1] > currentThreadMaxValue)
                currentThreadMaxValue = ReturnValue[1];
            if (ReturnValue[1] < currentThreadMinValue)
                currentThreadMinValue = ReturnValue[1];

            myOutputArray[(int)(Width * RowValue) + x] = ReturnValue[0];
            myOutputArray[(int)(Width * RowValue) + x + 1] = ReturnValue[1];
        }

        //myCoordinateGroups.Dispose();
        myMaxMinArray[RowValue * 2] = currentThreadMaxValue;
        myMaxMinArray[(RowValue * 2) + 1] = currentThreadMinValue;
    }
}
