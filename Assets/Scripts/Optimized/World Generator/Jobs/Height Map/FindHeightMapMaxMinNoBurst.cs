using Unity.Collections;
using Unity.Jobs;

public struct FindHeightMapMaxMinNoBurst : IJob
{

    public NativeArray<float> myMinMax;

    [ReadOnly]
    public NativeArray<float> myMaxMinArray;
    public void Execute()
    {
        myMinMax[0] = float.MaxValue; //Min gets max set and vice versa
        myMinMax[1] = float.MinValue;
        for (int i = 0; i < myMaxMinArray.Length; i++)
        {
            if (myMaxMinArray[i] < myMinMax[0])
            {
                myMinMax[0] = myMaxMinArray[i];
            }
            if (myMaxMinArray[i] > myMinMax[1])
            {
                myMinMax[1] = myMaxMinArray[i];
            }
        }
    }
}
