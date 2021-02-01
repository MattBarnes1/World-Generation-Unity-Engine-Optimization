using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace WorldGeneratorFunctions
{
    [BurstCompile]
    public struct NormalizeSimplexValues : IJobParallelFor
    {

        //public NativeArray<Tile> myTileMap;
        [NativeDisableParallelForRestriction]
        public NativeArray<float> SimplexMapToNormalize;

        [ReadOnly, DeallocateOnJobCompletion]
        public NativeArray<float> myMaxMinArray;

        [ReadOnly]
        public int Width;

        public void Execute(int yvalue)
        {
            float Min = myMaxMinArray[1];
            float Max = myMaxMinArray[0];

            if (Max - Min < 0) 
                throw new System.Exception();
            for(int x = 0; x < Width; x++)
            {
                var TileData = SimplexMapToNormalize[(yvalue * Width) + x];
                TileData = (TileData + math.abs(Min)) / (Max - Min);
                if (TileData < 0)
                    throw new System.Exception();
                SimplexMapToNormalize[(yvalue * Width) + x] = TileData;
            }          
        }
    }
}
