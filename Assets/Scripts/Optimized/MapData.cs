using System;
using Unity.Collections;

public class MapData : IDisposable
{
	private int Width;
    private NativeArray<float> TileRandomSeed;
    NativeArray<float> HeightMap;
    private NativeArray<float> HeatMap;
    private NativeArray<float> MoistureMap;
	private NativeArray<short> Collision;
	private NativeArray<bool> RiverData;



	public MapData(int width, int height)
	{
		RiverData = new NativeArray<bool>(width * height, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		Collision = new NativeArray<short>(width * height, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		TileRandomSeed = new NativeArray<float>(width * height, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		HeightMap = new NativeArray<float>(width * height, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		HeatMap = new NativeArray<float>(width * height, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		MoistureMap = new NativeArray<float>(width * height, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
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
		myTileData.TileRNG = TileRandomSeed[x + y * Width];
		myTileData.HeightType = GetHeightTypeEnum(x, y);
		myTileData.MoistureType = GetMoistureEnum(x, y);
		myTileData.HeatType = GetHeatTypeEnum(x, y);
		myTileData.BiomeType = GetBiomeType(myTileData);
		myTileData.Collidable = Collision[x + y * Width] != 0;
		myTileData.IsRiver = RiverData[x + y * Width];
		return myTileData;
	}
	float DeepWater = 0.2f;
	float ShallowWater = 0.4f;
	float Sand = 0.5f;
	float Grass = 0.7f;
	float Forest = 0.8f;
	float Rock = 0.9f;
	private HeightType GetHeightTypeEnum(int x, int y)
	{
		var value = HeightMap[x + y * Width];
		if (value < DeepWater)
		{
			return HeightType.DeepWater;
		}
		else if (value < ShallowWater)
		{
			return HeightType.ShallowWater;
		}
		else if (value < Sand)
		{
			return HeightType.Sand;
		}
		else if (value < Grass)
		{
			return HeightType.Grass;
		}
		else if (value < Forest)
		{
			return HeightType.Forest;
		}
		else if (value < Rock)
		{
			return HeightType.Rock;
		}
		else
		{
			return HeightType.Snow;
		}
	}

    BiomeType[,] BiomeTable = new BiomeType[6, 6] {   
    //COLDEST        //COLDER          //COLD                  //HOT                          //HOTTER                       //HOTTEST
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYEST
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.Grassland,    BiomeType.Desert,              BiomeType.Desert,              BiomeType.Desert },              //DRYER
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.Woodland,     BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //DRY
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.Woodland,            BiomeType.Savanna,             BiomeType.Savanna },             //WET
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.SeasonalForest,      BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest },  //WETTER
    { BiomeType.Ice, BiomeType.Tundra, BiomeType.BorealForest, BiomeType.TemperateRainforest, BiomeType.TropicalRainforest,  BiomeType.TropicalRainforest }   //WETTEST
	};
	public BiomeType GetBiomeType(Tile aTile)
	{
		return BiomeTable[(int)aTile.MoistureType, (int)aTile.HeatType];
	}
	float ColdestValue = 0.05f;
	float ColderValue = 0.18f;
	float ColdValue = 0.4f;
	float WarmValue = 0.6f;
	float WarmerValue = 0.8f;
	private HeatType GetHeatTypeEnum(int x, int y)
    {
		var heatValue = HeatMap[x + y*Width];
		if (heatValue < ColdestValue)
			return HeatType.Coldest;
		else if (heatValue < ColderValue)
			return HeatType.Colder;
		else if (heatValue < ColdValue)
			return HeatType.Cold;
		else if (heatValue < WarmValue)
			return HeatType.Warm;
		else if (heatValue < WarmerValue)
			return HeatType.Warmer;
		else
			return HeatType.Warmest;
	}

	float DryerValue = 0.27f;
	float DryValue = 0.4f;
	float WetValue = 0.6f;
	float WetterValue = 0.8f;
	float WettestValue = 0.9f;
	private MoistureType GetMoistureEnum(int x, int y)
    {
		var moistureValue = MoistureMap[x + y * Width];
		if (moistureValue < DryerValue) return MoistureType.Dryest;
		else if (moistureValue < DryValue) return MoistureType.Dryer;
		else if (moistureValue < WetValue) return MoistureType.Dry;
		else if (moistureValue < WetterValue) return MoistureType.Wet;
		else if (moistureValue < WettestValue) return MoistureType.Wetter;
		else 
			return  MoistureType.Wettest;
	}

    public void Dispose()
	{
		HeightMap.Dispose();
		if (HeatMap.IsCreated)
			HeatMap.Dispose();
		if (MoistureMap.IsCreated)
			MoistureMap.Dispose();
		if (TileRandomSeed.IsCreated)
			TileRandomSeed.Dispose();
	}

	public NativeArray<float> HeatMapAsNativeArray()
    {
		return HeatMap;
    }

    internal float GetHeightAtPoint(int x, int y)
    {
		return HeightMap[x + y * Width];
    }

    internal NativeArray<float> MostureMapAsNativeArray()
    {
		return this.MoistureMap;
    }

    internal NativeArray<bool> RiverMapAsNativeArray()
    {
		return this.RiverData;
    }
}
