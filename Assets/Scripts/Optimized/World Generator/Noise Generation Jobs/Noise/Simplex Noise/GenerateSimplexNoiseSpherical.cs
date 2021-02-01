using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using static NoiseTextureMods;

[BurstCompile]
public struct GenerateSimplexNoiseSpherical : IJobParallelFor
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
    public float Power;

    [ReadOnly]
    public float Persistence;

    [ReadOnly]
    public float Lacunarity;

    public FunctionPointer<NoiseTextureModifier> myModifier;

    public void Execute(int RowValue)
    {

        //We use the Max set to float.MinValue and Min to Float.MaxValue so that we know they always will be used.
        float currentThreadMaxValue = float.MinValue;
        float currentThreadMinValue = float.MaxValue;
        //Doing this locally allows for one check at the end of our loop instead of locking up each loop
        //NativeArray<float2> myCoordinateGroups = new NativeArray<float2>(4, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        //Since our Y will never change we can keep the value constant for all groups;

        float YInputValue = (float)(RowValue + Seed )/ (float)Height;

        for (var x = 0; x < Width; x++) //so we can cast the position instead;
        {
            float frequency = Frequency;
            float persistance = Persistence;
            float amplitude = Lacunarity;
            float lacunarity = Lacunarity;
            float MaxAmp = 0;
            float ReturnValue = new float();
            float2 ST = new float2((float)(x + Seed) / (float)Width, YInputValue);





            float4 myCoordinates = new float4(2 + math.cos(ST*2*math.PI) * (2*math.PI), 2  + math.sin(ST * 2 * math.PI) * (2*math.PI));
            for (var i = 0; i < Octaves; i++)
            {

                ReturnValue += (noise.snoise(myCoordinates * frequency * math.pow(2, i)) * amplitude * math.pow(2, -i));
                ReturnValue = math.pow(ReturnValue, Power);
                MaxAmp += 1;
            }
            ReturnValue /= MaxAmp;
            /*if (x - 1 >= 0 && myModifier.IsCreated)
            {
                //Use noise function smoothing;
                myOutputArray[(int)(Width * RowValue) + x - 1] = myModifier.Invoke(myOutputArray[(int)(Width * RowValue) + x - 1], ReturnValue);
            }
            if (x + 1 > Width && myModifier.IsCreated)
            {
                ReturnValue = myOutputArray[(int)(Width * RowValue) + x] = myModifier.Invoke(myOutputArray[(int)(Width * RowValue)], ReturnValue);
            }*/
            if(!myModifier.IsCreated)
            {
            }
            myOutputArray[(int)(Width * RowValue) + x] = ReturnValue;
            if (ReturnValue > currentThreadMaxValue)
                currentThreadMaxValue = ReturnValue;
            if (ReturnValue < currentThreadMinValue)
                currentThreadMinValue = ReturnValue;
            // myOutputArray[(int)(Width * RowValue) + x + 1] = ReturnValue[1];
        }
        //TODO FINAL CHECK FOR HEIGHT THAT ISN"T DIVISIBLE BY 4
        //myCoordinateGroups.Dispose();
        myMaxMinArray[RowValue * 2] = currentThreadMaxValue;
        myMaxMinArray[(RowValue * 2) + 1] = currentThreadMinValue;
    }


}
