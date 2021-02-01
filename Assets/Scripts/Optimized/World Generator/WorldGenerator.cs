
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;
using WorldGeneratorFunctions;
using static NoiseTextureMods;

[Serializable]
public partial class WorldGenerator : MonoBehaviour
{

    // Adjustable variables for Unity Inspector
    [SerializeField]
    int Width;
    [SerializeField]
    int Height;
    [SerializeField]
    [Tooltip("The higher the value, the more detailed the terrain.")]
    int NoiseOctaves;
    [SerializeField]
    float NoiseFrequency;
    [SerializeField]
    [Tooltip("Set this value is the offset from the x,y position in the generator.")]

    [Range(1, Int32.MaxValue)]
    int Seed;
    [SerializeField]
    [Tooltip("The higher this value, the rougher the terrain.")]
    [Range(0,0.9999999f)]
    float Persistence;
    [SerializeField]
    [Range(0, float.MaxValue)]
    [Tooltip("The higher this value, the more unique features the terrain will have.")]
    float Lacunarity;
    [SerializeField]
    [Tooltip("The higher this value, the more height the terrain will have.")]
    float Amplitude;
    [SerializeField]
    [Range(0, 0.90f)]
    [Tooltip("The higher this value, the more water in the world.")]
    float LandHeightCutoff;

    [SerializeField]
    [Range(.3f, 0.70f)]
    [Tooltip("The higher this value, the bigger the equator on the gradient.")]
    float EquatorSizeAsPercent;

    [SerializeField]
    [Range(2, 6)]
    [Tooltip("The number of heatzones from equator.")]
    float HeatZonesFromEquator;

    [SerializeField]
    [Range(0, 99)]
    [Tooltip("Heat Value at Equator.")]
    float EquatorHeatValue;



    // Noise generator module
    //ImplicitFractalBase HeightMapGenerator;

    // Height map data
    MapData HeightData;

    // Final Objects
    Tile[,] Tiles;

    // Our texture output (unity component)
    MeshRenderer HeightMapRenderer;
    // Our texture output (unity component)
    MeshRenderer BiomeMapRenderer;
    // Our texture output (unity component)
    MeshRenderer HeatMapRenderer;

    public MeshRenderer MoistureMapRenderer;

    public MeshRenderer PaletteMapRenderer { get; private set; }

    private MeshRenderer SettlementMapRenderer;
    [SerializeField]
    [Range(0, 99)]
    [Tooltip("Heat Map Octaves.")]
    private int HeatOctaves;
    [SerializeField]
    [Range(0, 99)]
    [Tooltip("Heat Frequency.")]
    private float HeatFrequency;
    [SerializeField]
    [Range(0, 99)]
    [Tooltip("Heat Power.")]
    public float HeatMapPower;
    [SerializeField]
    [Range(0, 99)]
    [Tooltip("Terrain Power.")]
    public float TerrainPower;

    [SerializeField]
    [Range(0, 99)]
    [Tooltip("Moisture Map Octaves.")]
    private int MoistureOctaves;
    [SerializeField]
    [Range(0, 99)]
    [Tooltip("Moisture Frequency.")]
    private float MoistureFrequency;
    [SerializeField]
    [Range(0, 99)]
    [Tooltip("Moisture Power.")]
    public float MoistureMapPower;
    [SerializeField]
    private float MoisturePersistence;
    [SerializeField]
    private float MoistureAmplitude;
    [SerializeField]
    private int RiverCount;

    private MeshRenderer RailMapManager;
    private MeshRenderer RiverMapManager;
    public MeshRenderer myPlanet;
    [SerializeField]
    private float MinRiverStartHeight;
    [SerializeField]
    private int SpaceBetweenRivers;

    void Start()
    {
        TextureGenerator myGenerator = new TextureGenerator();

           // Get the mesh we are rendering our output to
           HeightMapRenderer = transform.Find("HeightTexture").GetComponent<MeshRenderer>();
        BiomeMapRenderer = transform.Find("BiomeTexture").GetComponent<MeshRenderer>();
        HeatMapRenderer = transform.Find("HeatTexture").GetComponent<MeshRenderer>();
        MoistureMapRenderer = transform.Find("MoistureTexture").GetComponent<MeshRenderer>();
        PaletteMapRenderer = transform.Find("PaletteTexture").GetComponent<MeshRenderer>();
        SettlementMapRenderer =   transform.Find("SettlementTexture").GetComponent<MeshRenderer>();
        RailMapManager = transform.Find("RailTexture").GetComponent<MeshRenderer>();
        RiverMapManager = transform.Find("RiverTexture").GetComponent<MeshRenderer>();

        // Initialize the generator
        Initialize();
        //LoadTiles(out HeightData);
        //HeightData.Dispose();

        Stopwatch ts = Stopwatch.StartNew(); //BURST NEEDS to be called twice b/c first primes it.
        //CreateHeightMapNoBurst(out HeightData);
        ts.Stop();
        //UnityEngine.Debug.LogWarning("Heightmap Generator w/o Burst: " + ts.ElapsedMilliseconds);
        //HeightData.Dispose();


        ts = Stopwatch.StartNew();
        // Build the height map
        LoadTiles(out HeightData);
        ts.Stop();
        UnityEngine.Debug.LogWarning("Heightmap Generator w/ Burst: " + ts.ElapsedMilliseconds);




        // Build our final objects based on our data

        ts = Stopwatch.StartNew();
        // Render a texture representation of our map

        HeightMapRenderer.materials[0].mainTexture = myGenerator.GetHeightMapTexture(Width, Height, HeightData);

        HeatMapRenderer.materials[0].mainTexture = myGenerator.GetHeatMapTexture(Width, Height, HeightData);

        PaletteMapRenderer.materials[0].mainTexture = myGenerator.GetPalletBase(Width, Height, HeightData);

        MoistureMapRenderer.materials[0].mainTexture = myGenerator.GetMoistureMapTexture(Width, Height, HeightData);

        BiomeMapRenderer.materials[0].mainTexture = myGenerator.GetBiomeMapTexture(Width, Height, HeightData);

        RiverMapManager.materials[0].mainTexture = myGenerator.GetRiverTexture(Width, Height, HeightData);

        myPlanet.materials[0].mainTexture = BiomeMapRenderer.materials[0].mainTexture;




        ts.Stop();
        UnityEngine.Debug.LogWarning("Texture Generator: " + ts.ElapsedMilliseconds);
        HeightData.Dispose();

    }

   



    private void Initialize()
    {
        // Initialize the HeightMap Generator
        /*HeightMapGenerator = new MultiNoise(                                       BasisType.SIMPLEX,
                                       InterpolationType.QUINTIC,
                                       TerrainOctaves,
                                       TerrainFrequency,
                                       UnityEngine.Random.Range(0, int.MaxValue));*/
    }



    public JobHandle CreateNormalizedSimplexNoiseMap(int seed, float power, int octaves, float frequency, float persistence, float amplitude, float lacunarity, int gridWidth, int gridHeight, NativeArray<float> outputArray, JobHandle jobHandle = default)
    {
        NativeArray<float> MinMaxArray = new NativeArray<float>(2 * gridHeight, Allocator.TempJob);
        var Pointer = BurstCompiler.CompileFunctionPointer<NoiseTextureModifier>(new NoiseTextureModifier(NoiseTextureMods.BrownianRedNoise));
        GenerateSimplexNoiseSpherical myGenerator = new GenerateSimplexNoiseSpherical() { Seed = seed, myModifier = Pointer, Height = Height, Power = power, Octaves = octaves, Frequency = frequency, Persistence = persistence, Lacunarity = lacunarity, Width = gridWidth, myOutputArray = outputArray, myMaxMinArray = MinMaxArray };
        var SimplexMapGeneration = myGenerator.Schedule(Height, 32, jobHandle);
        FindHeightMapMaxMin myMaxMin = new FindHeightMapMaxMin { myMaxMinArray = MinMaxArray };
        NormalizeSimplexValues normalizationJob = new NormalizeSimplexValues { Width = this.Width, myMaxMinArray = MinMaxArray, SimplexMapToNormalize = outputArray };
        return normalizationJob.Schedule(Height, 32, myMaxMin.Schedule(SimplexMapGeneration));
    }


    // Extract data from a noise module
    private void LoadTiles(out MapData myData)
    {
        myData = new MapData(Width, Height);

        //////
        ///Batch 1
        //////
        ///Generate Height Map
        var HeightMapGeneration = CreateNormalizedSimplexNoiseMap(this.Seed, TerrainPower, this.NoiseOctaves, this.NoiseFrequency, this.Persistence, this.Amplitude, this.Lacunarity, Width, Height, myData.HeightMapAsNativeArray());
        ///Generate HeatMap;
       
        JobHandle HeatMapCreationJob = GenerateHeatMap(myData);

        var SimplexWaterMapGeneration = CreateNormalizedSimplexNoiseMap(this.Seed << 10, MoistureMapPower, this.MoistureOctaves, this.MoistureFrequency, this.MoisturePersistence, this.MoistureAmplitude, this.Lacunarity, Width, Height, myData.MostureMapAsNativeArray());
        
        
        
       // var AwaitHeatMapAndMoistureMap = JobHandle.CombineDependencies(SimplexWaterMapGeneration, HeatMapCreationJob);
        //Finished Heat map and Heat map processing to continue.
        var AwaitHeightMapAndBiomeData = JobHandle.CombineDependencies(SimplexWaterMapGeneration, HeatMapCreationJob, HeightMapGeneration);
        //Adjust the values of the heat map based on the Height Map. TODO Add Adjustable Values
        AdjustHeatMapValuesFromHeightmap myHeatMapAdjustment = new AdjustHeatMapValuesFromHeightmap() { HeatMap = myData.HeatMapAsNativeArray(), HeightMap = myData.HeightMapAsNativeArray() };
        var myHeatmapAdjustmentJob = myHeatMapAdjustment.Schedule(Width * Height, 32, AwaitHeightMapAndBiomeData);

        NativeHashMap<RiverDistanceKey, float2> myRiverLocations = new NativeHashMap<RiverDistanceKey, float2>(RiverCount, Allocator.TempJob);
        FindRiverStartingPointsFromHeightMapValues myRivers = new FindRiverStartingPointsFromHeightMapValues() {RiversRemaining = RiverCount, SeedID = this.Seed, HeightMap = myData.HeightMapAsNativeArray(), myRiverPositions = myRiverLocations.AsParallelWriter(), MinRiverStartHeight = this.MinRiverStartHeight, SpaceBetweenRiversDistance = this.SpaceBetweenRivers, Width = this.Width };
        var RiverPreprocessing = myRivers.Schedule(Height, 32, AwaitHeightMapAndBiomeData);


        AdjustMoistureMapFromHeightMapValues myMoistureValueAdjuster = new AdjustMoistureMapFromHeightMapValues() { HeightMap = myData.HeightMapAsNativeArray(), MoistureMap = myData.MostureMapAsNativeArray() };
        var myMoistureAdjusterJob = myMoistureValueAdjuster.Schedule(Width * Height, 32, RiverPreprocessing);


        var MapGenerationJob = JobHandle.CombineDependencies(myMoistureAdjusterJob, myHeatmapAdjustmentJob);
        MapGenerationJob.Complete();
        //////
        ///Batch 2
        //////



        DigRiversOutJob myRiversToDig = new DigRiversOutJob() { myRiverPositions = myRiverLocations.GetValueArray(Allocator.TempJob), MinRiverStartHeight = this.MinRiverStartHeight, RiverMap = myData.RiverMapAsNativeArray(), SpaceBetweenRiversDistance = this.SpaceBetweenRivers, Width = this.Width };
        var Rivers = myRiversToDig.Schedule(myRiverLocations.Count(), 1);
        Rivers.Complete();


        myRiverLocations.Dispose();
    }

    private JobHandle GenerateHeatMap(MapData myData)
    { 
        //Gradiant Map
        NativeArray<float> HeatMapGradient = new NativeArray<float>(Width * Height, Allocator.TempJob);
        GradientGenerator myGradientGenerator = new GradientGenerator() { EquatorSize = (int)Math.Floor(EquatorSizeAsPercent * Height), Width = this.Width, Height = this.Height, HeatMapBase = HeatMapGradient };
        var HeatMapGradientGeneration = myGradientGenerator.Schedule(Height, 32);

        //Simplex Map
        var HeatMapSimplex = new NativeArray<float>(Width * Height, Allocator.TempJob);
        var SimplexHeatMapGeneration = CreateNormalizedSimplexNoiseMap(this.Seed << 1, HeatMapPower, this.HeatOctaves, this.HeatFrequency, this.Persistence, this.Amplitude, this.Lacunarity, Width, Height, HeatMapSimplex);
        var AwaitGradientAndSimplexHeatMaps = JobHandle.CombineDependencies(HeatMapGradientGeneration, SimplexHeatMapGeneration);

        //Merge Heat Simplex Map and Gradient Map with blend Job
        BlendMultiplyAndDeallocateJob myBlendJob = new BlendMultiplyAndDeallocateJob() { NoiseSource1 = HeatMapGradient, NoiseSource2 = HeatMapSimplex, Width = Width, Output = myData.HeatMapAsNativeArray() };


        return myBlendJob.Schedule(Width * Height, 32, AwaitGradientAndSimplexHeatMaps);
    }


}