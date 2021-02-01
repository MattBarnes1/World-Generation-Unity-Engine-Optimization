using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public struct RiverDistanceKey : IEquatable<RiverDistanceKey>
{
    public override int GetHashCode()
    {
       return Min.GetHashCode() + Max.GetHashCode() + Center.GetHashCode();
    }

    public float3 Min;
    public float3 Max;
    public float3 Center;
    public RiverDistanceKey(float Length, float Width, float Height, float3 aCenter)
    {

        this.Center = aCenter;
        this.Min = float3.zero;
        this.Max = new float3(Length, Width, Height);
        var TestCenter = ((this.Max - this.Min) * 0.5f) + this.Min;
        bool3 Check = TestCenter != aCenter;
        if (Check.x && Check.y && Check.z)
        {
            var offset = CalculateCenterOffset(TestCenter, aCenter);
            this.Max = this.Max + offset;
            this.Min = this.Min + offset;
            Check = (TestCenter + offset) != aCenter;
            if (Check.x && Check.y && Check.z)
            {
                throw new Exception();
            }
        }
/*#if DEBUG
        Debug.Assert(this.Max == Vector3.Max(this.Max, this.Min));
        Debug.Assert(this.Min == Vector3.Min(this.Max, this.Min));
        Debug.Assert(this.Max == Vector3.Max(this.Max, this.Center));
        Debug.Assert(this.Min == Vector3.Min(this.Center, this.Min));
#endif*/
    }
    public bool Equals(RiverDistanceKey box)
    {
        if ((this.Max.x >= box.Min.x) && (this.Min.x <= box.Max.x))
        {
            if ((this.Max.y < box.Min.y) || (this.Min.y > box.Max.y))
            {
                return false;
            }

            return (this.Max.z >= box.Min.z) && (this.Min.z <= box.Max.z);
        }

        return false;
    }
    private float3 CalculateCenterOffset(float3 CurrentCenter, float3 newCenter)
    {
        return newCenter - CurrentCenter;
    }
}
