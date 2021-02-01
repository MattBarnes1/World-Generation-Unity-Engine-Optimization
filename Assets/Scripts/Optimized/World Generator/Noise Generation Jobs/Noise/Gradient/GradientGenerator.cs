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
    public struct GradientGenerator : IJobParallelFor
    {

        [ReadOnly]
        public int Width;
        [ReadOnly]
        public int Height;

        [ReadOnly]
        public int EquatorSize;

        [NativeDisableParallelForRestriction, WriteOnly]
        public NativeArray<float> HeatMapBase;



        public void Execute(int yvalue)
        {
            float EquMidpoint = math.floor(Height / 2);
            float ImageTop = EquMidpoint - (EquatorSize / 2);
            float ImageBottom = EquMidpoint + (EquatorSize / 2);
            for (int i = 0; i < Width; i++)
            {
                if(ImageTop <= yvalue && ImageBottom >= yvalue )//Equator
                {
                    HeatMapBase[yvalue * Width + i] = 1f;
                }
                else if(yvalue < ImageTop)
                {
                    HeatMapBase[yvalue * Width + i] = yvalue / ImageTop;
                }
                else if (yvalue > ImageBottom)
                {
                    HeatMapBase[yvalue * Width + i] = math.abs(Height - yvalue) / (Height - ImageBottom);
                }
                    
            }
        }
    }
}
