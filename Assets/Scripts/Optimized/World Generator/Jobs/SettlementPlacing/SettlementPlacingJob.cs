using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
[BurstCompile]
public struct SettlementPlacingJob : IJobParallelFor
{
    [NativeDisableParallelForRestriction]
    public NativeArray<float> RiverMap;
    [NativeDisableParallelForRestriction, ReadOnly]
    public NativeArray<float> HeightMap;
      
    [ReadOnly]
    public float Width;
    [ReadOnly]
    public float Height;


    public void Execute(int yvalue)
    {

        //Convert it to the start of the region value



    }
}