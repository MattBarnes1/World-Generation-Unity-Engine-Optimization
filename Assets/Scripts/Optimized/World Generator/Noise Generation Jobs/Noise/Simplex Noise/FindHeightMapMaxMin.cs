using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
namespace WorldGeneratorFunctions
{
    [BurstCompile]
    public struct FindHeightMapMaxMin : IJob
    {


        public NativeArray<float> myMaxMinArray;
        public void Execute()
        {
            var myMaxMin = new NativeArray<float>(2, Allocator.Temp);
            myMaxMin[1] = float.MaxValue; //this is our min search
            myMaxMin[0] = float.MinValue; //This is our max search
            for(int i = 0; i < myMaxMinArray.Length; i++)
            {
                if(myMaxMinArray[i] < myMaxMin[1])
                {
                    myMaxMin[1] = myMaxMinArray[i];
                }
                if (myMaxMinArray[i] > myMaxMin[0])
                {
                    myMaxMin[0] = myMaxMinArray[i];
                }
            }
            myMaxMinArray[0] = myMaxMin[0];
            myMaxMinArray[1] = myMaxMin[1];

            myMaxMin.Dispose();
        }
    }
}
