using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
[BurstCompile]
public struct AdjustMoistureMapFromHeightMapValues : IJobParallelFor
{
    [NativeDisableParallelForRestriction]
    public NativeArray<float> MoistureMap;
    [NativeDisableParallelForRestriction, ReadOnly]
    public NativeArray<float> HeightMap;


    public void Execute(int yvalue)
    {
        float HeightTypeDeepWater = 0.3f;
        float HeightTypeShallowWater = 0.6f;
        float HeightTypeShore = 0.8f;
        float HeightTypeSand = 1f;
        var HeightValue = HeightMap[yvalue];
        //adjust moisture based on height
        if (HeightValue <= HeightTypeDeepWater)
        {
            MoistureMap[yvalue] += 1f * HeightValue;
        }
        else if (HeightValue <= HeightTypeShallowWater)
        {
            MoistureMap[yvalue] += .375f * HeightValue;
        }
        else if (HeightValue <= HeightTypeShore)
        {
            MoistureMap[yvalue] += 0.125f * HeightValue;
        }
        else if (HeightValue <= HeightTypeSand)
        {
            MoistureMap[yvalue] += 0.03f * HeightValue;
        }
    }
}