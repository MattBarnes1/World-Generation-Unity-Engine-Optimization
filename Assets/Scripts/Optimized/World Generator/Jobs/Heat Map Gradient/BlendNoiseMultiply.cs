using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct BlendMultiplyJob : IJobParallelFor
{
    [NativeDisableParallelForRestriction, ReadOnly]
    public NativeArray<float> NoiseSource1;

    [NativeDisableParallelForRestriction, ReadOnly]
    public NativeArray<float> NoiseSource2;

    [NativeDisableParallelForRestriction]
    public NativeArray<float> Output;

    [ReadOnly]
    public int Width;


    public void Execute(int yvalue)
    {

        for (int i = 0; i < Width; i++)
        {
            Output[yvalue * Width + i] = (NoiseSource1[yvalue * Width + i] + NoiseSource2[yvalue * Width + i]) /2;
        }
    }
}
