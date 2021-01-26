using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public  class TextureGenerator
{



	// Height Map Colors
	private  Color DeepColor = new Color(15/255f, 30/255f, 80/255f, 1);
	private  Color ShallowColor = new Color(15/255f, 40/255f, 90/255f, 1);
	private  Color RiverColor = new Color(30/255f, 120/255f, 200/255f, 1);
	private  Color SandColor = new Color(198 / 255f, 190 / 255f, 31 / 255f, 1);
	private  Color GrassColor = new Color(50 / 255f, 220 / 255f, 20 / 255f, 1);
	private  Color ForestColor = new Color(16 / 255f, 160 / 255f, 0, 1);
	private  Color RockColor = new Color(0.5f, 0.5f, 0.5f, 1);            
	private  Color SnowColor = new Color(1, 1, 1, 1);

	private  Color IceWater = new Color (210/255f, 255/255f, 252/255f, 1);
	private  Color ColdWater = new Color (119/255f, 156/255f, 213/255f, 1);
	private  Color RiverWater = new Color (65/255f, 110/255f, 179/255f, 1);

	// Height Map Colors
	private  Color Coldest = new Color(0, 1, 1, 1);
	private  Color Colder = new Color(170/255f, 1, 1, 1);
	private  Color Cold = new Color(0, 229/255f, 133/255f, 1);
	private  Color Warm = new Color(1, 1, 100/255f, 1);
	private  Color Warmer = new Color(1, 100/255f, 0, 1);
	private  Color Warmest = new Color(241/255f, 12/255f, 0, 1);

	//Moisture map
	private  Color Dryest = new Color(255/255f, 139/255f, 17/255f, 1);
	private  Color Dryer = new Color(245/255f, 245/255f, 23/255f, 1);
	private  Color Dry = new Color(80/255f, 255/255f, 0/255f, 1);
	private  Color Wet = new Color(85/255f, 255/255f, 255/255f, 1);
	private  Color Wetter = new Color(20/255f, 70/255f, 255/255f, 1);
	private  Color Wettest = new Color(0/255f, 0/255f, 100/255f, 1);

	//biome map
	private  Color Ice = Color.white;
	private  Color Desert = new Color(238/255f, 218/255f, 130/255f, 1);
	private  Color Savanna = new Color(177/255f, 209/255f, 110/255f, 1);
	private  Color TropicalRainforest = new Color(66/255f, 123/255f, 25/255f, 1);
	private  Color Tundra = new Color(96/255f, 131/255f, 112/255f, 1);
	private  Color TemperateRainforest = new Color(29/255f, 73/255f, 40/255f, 1);

    public  Texture GetBiomeMap(int width, int height, MapData tiles)
	{
		var texture = new Texture2D(width, height);
		var pixels = new Color[width * height];

		for (var x = 0; x < width; x++)
		{
			for (var y = 0; y < height; y++)
			{
				float value = tiles[x,y].HeightValue;
				if (value < 0.60f)
					pixels[x + y * width] = Color.blue;
				else
					pixels[x + y * width] = Color.green;
			}
		}

	
		texture.SetPixels(pixels);
		texture.wrapMode = TextureWrapMode.Clamp;

		texture.Apply();
		return texture;
	}

    private  Color Grassland = new Color(164/255f, 225/255f, 99/255f, 1);
	private  Color SeasonalForest = new Color(73/255f, 100/255f, 35/255f, 1);
	private  Color BorealForest = new Color(95/255f, 115/255f, 62/255f, 1);
	private  Color Woodland = new Color(139/255f, 175/255f, 90/255f, 1);

	public Texture2D GetHeatMapGradientTexture(int width, int height,  MapData tiles)
	{
		var texture = new Texture2D(width, height);
		var pixels = new Color[width * height];

		for (var x = 0; x < width; x++)
		{
			for (var y = 0; y < height; y++)
			{
				pixels[x + y * width] = Color.Lerp(Color.blue, Color.red, tiles[x, y].HeatValue);

			}
		}

		texture.SetPixels(pixels);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.Apply();
		return texture;
	}


	public  Texture2D GetHeatMapTexture(int width, int height, MapData myData)
	{

		var texture = new Texture2D(width, height);
		var pixels = new Color[width * height];

		for (var x = 0; x < width; x++)
		{
			for (var y = 0; y < height; y++)
			{
				switch (myData[x, y].HeatType)
				{
					case HeatType.Coldest:
						pixels[x + y * width] = Coldest;
						break;
					case HeatType.Colder:
						pixels[x + y * width] = Colder;
						break;
					case HeatType.Cold:
						pixels[x + y * width] = Cold;
						break;
					case HeatType.Warm:
						pixels[x + y * width] = Warm;
						break;
					case HeatType.Warmer:
						pixels[x + y * width] = Warmer;
						break;
					case HeatType.Warmest:
						pixels[x + y * width] = Warmest;
						break;
				}

				//darken the color if a edge tile
				//if (tiles[(y * width) + x].Bitmask != 15)
					//pixels[x + y * width] = Color.Lerp(pixels[x + y * width], Color.black, 0.4f);
			}
		}

		texture.SetPixels(pixels);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.Apply();
		return texture;
	}

	public RuleSystem myHeightMapSystem;


	public  Texture2D GetHeightMapTexture(int width, int height, MapData myData)
	{
		var texture = new Texture2D(width, height);
		var pixels = new Color[width * height];

		for (var x = 0; x < width; x++)
		{
			for (var y = 0; y < height; y++)
			{
				float value = myData[x, y].HeightValue;

				//Set color range, 0 = black, 1 = white
				pixels[(y * width) + x] = Color.Lerp(Color.black, Color.white, value);
			}
		}

		texture.SetPixels(pixels);
		texture.wrapMode = TextureWrapMode.Clamp;
	
		texture.Apply();
		return texture;
	}

}