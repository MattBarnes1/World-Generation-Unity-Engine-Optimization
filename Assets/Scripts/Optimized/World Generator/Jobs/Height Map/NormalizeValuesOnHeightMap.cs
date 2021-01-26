using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
namespace WorldGeneratorFunctions
{
    [BurstCompile]
    public struct NormalizeValuesOnHeightMap : IJobParallelFor
    {

        //public NativeArray<Tile> myTileMap;
        [NativeDisableParallelForRestriction]
        public NativeArray<float> myHeightMap;

        [ReadOnly]
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
                var TileData = myHeightMap[(yvalue * Width) + x];
                TileData = (TileData - Min) / (Max - Min);
                if (TileData < 0)
                    throw new System.Exception();
                myHeightMap[(yvalue * Width) + x] = TileData;
            }          
        }
    }
}
