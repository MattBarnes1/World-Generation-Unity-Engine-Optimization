using System;
using Unity.Collections;

public class MapData : IDisposable
{
	private int Width;
	NativeArray<float> HeightMap;
    private NativeArray<float> HeatMap;
    NativeArray<float> MaxMin;
	float MinValue;
	float MaxValue;

	bool MinMaxSet;
	public float Min
	{
		get
		{
			if (!MinMaxSet)
				throw new Exception();
			return MaxValue;
		}
	}
	public float Max
	{
		get
		{
			if (!MinMaxSet)
				throw new Exception();
			return MinValue;
		}
	}

	private void FindMinMax()
	{
		float Min = float.MaxValue; //Flipped on purpose
		float Max = float.MinValue;
		for (int i = 0; i < MaxMin.Length; i += 2)
		{
			if (MaxMin[i] > Max)
				Max = MaxMin[i];
			if (MaxMin[i + 1] < Min)
				Min = MaxMin[i + 1];
		}
		MinValue = Min;
		MaxValue = Max;
		MinMaxSet = true;
		MaxMin.Dispose();
	}


	public MapData(int width, int height)
	{
		MaxMin = new NativeArray<float>(2 * height, Allocator.Persistent);
		HeightMap = new NativeArray<float>(width * height, Allocator.Persistent);
		HeatMap = new NativeArray<float>(width * height, Allocator.Persistent);
		while (!MaxMin.IsCreated) { }
		while (!HeatMap.IsCreated) { }
		this.Width = width;
	}


	public Tile this[int x, int y]
	{
		get
        {
			return GetMapDataAtPoint(x, y);

		}
	}


	public NativeArray<float> HeightMapAsNativeArray()
	{
		return HeightMap;
	}

	public void FromNativeArray(NativeArray<float> newData)
	{
		newData.CopyTo(HeightMap);
	}

	public Tile GetMapDataAtPoint(int x, int y)
	{
		Tile myTileData = new Tile();
		myTileData.HeightValue = HeightMap[y * Width + x];
		myTileData.HeatValue = HeatMap[y * Width + x];
		return myTileData;
	}

	public NativeArray<float> GetMinMaxArray()
    {
		return MaxMin;

	}

    public void Dispose()
	{
		HeightMap.Dispose();
		if (MaxMin.IsCreated)
			MaxMin.Dispose();
		if (HeatMap.IsCreated)
			HeatMap.Dispose();
	}

	public NativeArray<float> HeatMapAsNativeArray()
    {
		return HeatMap;
    }
}
