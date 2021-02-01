using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
[BurstCompile]
public struct DigRiversOutJob : IJobParallelFor
{

    [NativeDisableParallelForRestriction, WriteOnly]
    public NativeArray<bool> RiverMap;

    [ReadOnly]
    public float MinRiverStartHeight;

    [ReadOnly]
    public int Width;

    [ReadOnly]
    public float OceanPoint;
    [ReadOnly]
    public int SpaceBetweenRiversDistance;

    [ReadOnly, DeallocateOnJobCompletion]
    public NativeArray<float2> myRiverPositions;



    [ReadOnly]
    public int ItemsPerThreads;
    public void Execute(int yvalue)
    {

        RiverMap[(int)myRiverPositions[yvalue].x + (int)myRiverPositions[yvalue].y * Width] = true; //TODO Optimize
    }


}
