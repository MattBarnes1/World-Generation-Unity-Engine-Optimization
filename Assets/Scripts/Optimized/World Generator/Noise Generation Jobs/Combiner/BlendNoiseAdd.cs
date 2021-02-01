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
public struct BlendNoiseAddAndDeallocateJob : IJobParallelFor
{
    [NativeDisableParallelForRestriction, ReadOnly, DeallocateOnJobCompletion]
    public NativeArray<float> NoiseSource1;

    [NativeDisableParallelForRestriction, ReadOnly, DeallocateOnJobCompletion]
    public NativeArray<float> NoiseSource2;

    [NativeDisableParallelForRestriction, WriteOnly]
    public NativeArray<float> Output;

    [ReadOnly]
    public int Width;


    public void Execute(int yvalue)
    {
        Output[yvalue] = math.min(NoiseSource1[yvalue] + NoiseSource2[yvalue], 1);

    }
}
