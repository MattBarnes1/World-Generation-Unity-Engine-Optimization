using AOT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
[BurstCompile]
public class NoiseTextureMods
{
    public delegate float NoiseTextureModifier(float n, float nplusone);
    [BurstCompile]
    [MonoPInvokeCallback(typeof(NoiseTextureModifier))]
    public static float BrownianRedNoise(float n, float nplusone)
    {
        return (n + nplusone) / 2f;
    }


}