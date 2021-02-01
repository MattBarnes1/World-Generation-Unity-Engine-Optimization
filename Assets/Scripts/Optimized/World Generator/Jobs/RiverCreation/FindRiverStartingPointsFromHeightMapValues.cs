using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct FindRiverStartingPointsFromHeightMapValues : IJobParallelFor
{

    [NativeDisableParallelForRestriction, ReadOnly]
    public NativeArray<float> HeightMap;

    [ReadOnly]
    public float MinRiverStartHeight;
    
    [ReadOnly]
    public int Width;

    public int RiversRemaining;

    [ReadOnly]
    public int SeedID;
    [ReadOnly]
    public int SpaceBetweenRiversDistance;

    public NativeHashMap<RiverDistanceKey, float2>.ParallelWriter myRiverPositions;

    public void Execute(int yvalue)
    {
        Unity.Mathematics.Random myRandom = new Unity.Mathematics.Random((uint)SeedID);
        int Start = myRandom.NextInt(0, Width-1);
        int ItemsBeforeMe = Width - Start;
        int ItemsAfterMe = Width - Start;

        for (int x = ItemsBeforeMe; x < Width; x++)
        {
            if (RiversRemaining <= 0) return;
            if (HeightMap[x + yvalue *Width] >= MinRiverStartHeight)
            {
                Interlocked.Decrement(ref RiversRemaining);
                if(RiversRemaining > 0)
                {
                    RiverDistanceKey myKey = new RiverDistanceKey(SpaceBetweenRiversDistance, SpaceBetweenRiversDistance, SpaceBetweenRiversDistance, new float3(x, yvalue, 0));
                    myRiverPositions.TryAdd(myKey, new float2(x, yvalue));
                    x += SpaceBetweenRiversDistance;
                }
            }
        }
        for (int x = 0; x < ItemsAfterMe; x++)
        {
            if (RiversRemaining <= 0) return;
            if (HeightMap[x + yvalue * Width] >= MinRiverStartHeight)
            {
                Interlocked.Decrement(ref RiversRemaining);
                if (RiversRemaining > 0)
                {
                    RiverDistanceKey myKey = new RiverDistanceKey(SpaceBetweenRiversDistance, SpaceBetweenRiversDistance, SpaceBetweenRiversDistance, new float3(x, yvalue, 0));
                    myRiverPositions.TryAdd(myKey, new float2(x, yvalue));
                    x += SpaceBetweenRiversDistance;
                }
            }
        }
    }


}