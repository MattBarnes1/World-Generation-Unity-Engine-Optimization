
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;
using WorldGeneratorFunctions;

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
    MeshRenderer LandMapRenderer;
    // Our texture output (unity component)
    MeshRenderer HeatMapRenderer;
    [SerializeField]
    [Range(0, 99)]
    [Tooltip("Heat Map Octaves.")]
    private int HeatOctaves;
    [SerializeField]
    [Range(0, 99)]
    [Tooltip("Heat Frequency.")]
    private float HeatFrequency;

    void Start()
    {
        TextureGenerator myGenerator = new TextureGenerator();

           // Get the mesh we are rendering our output to
           HeightMapRenderer = transform.Find("HeightTexture").GetComponent<MeshRenderer>();
        LandMapRenderer = transform.Find("BiomeTexture").GetComponent<MeshRenderer>();
        HeatMapRenderer = transform.Find("HeatTexture").GetComponent<MeshRenderer>();

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

        // Render a texture representation of our map
        HeightMapRenderer.materials[0].mainTexture = myGenerator.GetHeightMapTexture(Width, Height, HeightData);

        LandMapRenderer.materials[0].mainTexture = myGenerator.GetBiomeMap(Width, Height, HeightData);

        HeatMapRenderer.materials[0].mainTexture = myGenerator.GetHeatMapGradientTexture(Width, Height, HeightData);
        HeightData.Dispose();

    }

    private void CreateHeightMapNoBurst(out MapData myData)
    {
        myData = new MapData(Width, Height);
        GenerateSimplexNoiseNoBurst myJob = new GenerateSimplexNoiseNoBurst() { Seed = this.Seed, Octaves = this.NoiseOctaves, Frequency = this.NoiseFrequency, Amplitude = this.Amplitude, Persistence = this.Persistence, Lacunarity = this.Lacunarity, Width = Width, Height = Height, myOutputArray = myData.HeightMapAsNativeArray(), myMaxMinArray = myData.GetMinMaxArray() };
        var HeightMapGeneration = myJob.Schedule(Height, 32);
        HeatMapGradientGenerator myGradientGenerator = new HeatMapGradientGenerator() { EquatorSize = EquatorSizeAsPercent * Height, Width = this.Width, HeatMapBase = myData.HeatMapAsNativeArray(), HeatZonesInSingleDirectionFromEquator = HeatZonesFromEquator, HeatZoneSize = Height - (EquatorSizeAsPercent * Height) / HeatZonesFromEquator, Seed = this.Seed, EquatorHeatValue = EquatorHeatValue };
        var HeatMapGradientGeneration = myGradientGenerator.Schedule(Height, 32);
        var MapGeneration = JobHandle.CombineDependencies(HeatMapGradientGeneration, HeightMapGeneration);

        FindHeightMapMaxMin myMaxMin = new FindHeightMapMaxMin { myMaxMinArray = myData.GetMinMaxArray() };
        var Handle = myMaxMin.Schedule(MapGeneration);
        NormalizeValuesOnHeightMap normalizationJob = new NormalizeValuesOnHeightMap { Width = this.Width, myMaxMinArray = myData.GetMinMaxArray(), myHeightMap = myData.HeightMapAsNativeArray() };
        Handle = normalizationJob.Schedule(Height, 32, Handle);



        Handle.Complete();
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

    // Extract data from a noise module
    private void LoadTiles(out MapData myData)
    {
        myData = new MapData(Width, Height);
        var OtherData = myData.GetMinMaxArray();


        //////
        ///Generate Height Map
        //////
        GenerateSimplexNoiseWithBurst myGenerator = new GenerateSimplexNoiseWithBurst() { Seed = this.Seed, Octaves = this.NoiseOctaves, Frequency = this.NoiseFrequency, Amplitude = this.Amplitude, Persistence = this.Persistence, Lacunarity = this.Lacunarity, Width = Width, Height = Height, myOutputArray = myData.HeightMapAsNativeArray(), myMaxMinArray = myData.GetMinMaxArray() };
        
        
        var HeightMapGeneration = myGenerator.Schedule(Height, 32);

        /////
        ///Generate SimplexHeatMap;
        ///




        FindHeightMapMaxMin myMaxMin = new FindHeightMapMaxMin { myMaxMinArray = myData.GetMinMaxArray() };
        NormalizeValuesOnHeightMap normalizationJob = new NormalizeValuesOnHeightMap { Width = this.Width, myMaxMinArray = myData.GetMinMaxArray(), myHeightMap = myData.HeightMapAsNativeArray() };
        HeightMapGeneration = normalizationJob.Schedule(Height, 32, myMaxMin.Schedule(HeightMapGeneration));



        NativeArray<float> HeatMapGradient = new NativeArray<float>(Width * Height, Allocator.TempJob);
        HeatMapGradientGenerator myGradientGenerator = new HeatMapGradientGenerator() { EquatorSize = EquatorSizeAsPercent * Height, Width = this.Width, HeatMapBase = HeatMapGradient, HeatZonesInSingleDirectionFromEquator = HeatZonesFromEquator, HeatZoneSize = Height - (EquatorSizeAsPercent * Height) / HeatZonesFromEquator, Seed = this.Seed, EquatorHeatValue = EquatorHeatValue };
        var HeatMapGradientGeneration = myGradientGenerator.Schedule(Height, 32);
        var MapBatch1Generation = JobHandle.CombineDependencies(HeatMapGradientGeneration, HeightMapGeneration);

        //////
        ///Batch 2
        //////

        NativeArray<float> HeatMapSimplex = new NativeArray<float>(Width * Height, Allocator.TempJob);
        GenerateSimplexNoiseWithBurst myHeatMapSimplexGenerator = new GenerateSimplexNoiseWithBurst() { Seed = this.Seed , Octaves = this.HeatOctaves, Frequency = this.HeatFrequency, Amplitude = this.Amplitude, Persistence = this.Persistence, Lacunarity = this.Lacunarity, Width = Width, Height = Height, myOutputArray = HeatMapSimplex, myMaxMinArray = myData.GetMinMaxArray() };
        var SimplexHeatMapGeneration = myHeatMapSimplexGenerator.Schedule(Height, 32, MapBatch1Generation);
        FindHeightMapMaxMin myHeatyMaxMin = new FindHeightMapMaxMin { myMaxMinArray = myData.GetMinMaxArray() };
        var MapBatchGeneration2 = myHeatyMaxMin.Schedule(SimplexHeatMapGeneration);
        NormalizeValuesOnHeightMap normalizationHeatJob = new NormalizeValuesOnHeightMap { Width = this.Width, myMaxMinArray = myData.GetMinMaxArray(), myHeightMap = HeatMapGradient };
        MapBatchGeneration2 = normalizationHeatJob.Schedule(Height, 32, MapBatchGeneration2);
        BlendMultiplyJob myBlendJob = new BlendMultiplyJob() { NoiseSource1 = HeatMapGradient, NoiseSource2 = HeatMapSimplex , Width = Width, Output = myData.HeatMapAsNativeArray() };

        MapBatchGeneration2 = myBlendJob.Schedule(Height, 32, MapBatchGeneration2);



        MapBatchGeneration2 = JobHandle.CombineDependencies(MapBatch1Generation, MapBatchGeneration2);





        MapBatchGeneration2.Complete();
        HeatMapGradient.Dispose();
        HeatMapSimplex.Dispose();
    }



}