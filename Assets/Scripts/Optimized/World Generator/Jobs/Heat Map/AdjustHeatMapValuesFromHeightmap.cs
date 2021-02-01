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
public struct AdjustHeatMapValuesFromHeightmap : IJobParallelFor
{
    [NativeDisableParallelForRestriction]
    public NativeArray<float> HeatMap;
    [NativeDisableParallelForRestriction, ReadOnly]
    public NativeArray<float> HeightMap;


    public void Execute(int yvalue)
    {
        float HeightTypeGrassArea = 0.3f;
        float HeightTypeForestArea = 0.6f;
        float HeightTypeRockArea = 0.8f;
        float HeightTypeSnow = 1f;
        var HeightValue = HeightMap[yvalue];
        if (HeightValue < HeightTypeGrassArea)
        {
            HeatMap[yvalue] -= 0.1f * HeightValue;
        }
        else
        if (HeightValue < HeightTypeForestArea)
        {
            HeatMap[yvalue] -= 0.2f * HeightValue;
        }
        else
        if (HeightValue < HeightTypeRockArea)
        {
            HeatMap[yvalue] -= 0.3f * HeightValue;
        }
        else
        if (HeightValue < HeightTypeSnow)
        {
            HeatMap[yvalue] -= 0.4f * HeightValue;
        }
    }
}

