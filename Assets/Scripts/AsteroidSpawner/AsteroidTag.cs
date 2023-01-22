using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public struct AsteroidTag : IComponentData
{
    public float2 direction;
    public float lifeTime;
}