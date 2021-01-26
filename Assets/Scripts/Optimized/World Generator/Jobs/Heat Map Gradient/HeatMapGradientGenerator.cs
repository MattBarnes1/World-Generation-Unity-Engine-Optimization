using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace WorldGeneratorFunctions
{
    [BurstCompile]
    public struct HeatMapGradientGenerator : IJobParallelFor
    {

        [ReadOnly]
        public int Seed;

        [ReadOnly]
        public int Width;

        [ReadOnly]
        internal float EquatorSize;

        [NativeDisableParallelForRestriction]
        internal NativeArray<float> HeatMapBase;
        [ReadOnly]
        internal float HeatZonesInSingleDirectionFromEquator;
        [ReadOnly]
        internal float HeatZoneSize;
        [ReadOnly]
        internal float EquatorHeatValue;

        public void Execute(int yvalue)
        {
            float Height = (int)(HeatMapBase.Length / Width);
            float EquMidpoint = math.floor(Height / 2);
            for (int i = 0; i < Width; i++)
            {
                    HeatMapBase[yvalue * Width + i] = math.lerp(1, 0, math.abs(EquMidpoint - yvalue)/ EquMidpoint);
            }
        }
    }
}
