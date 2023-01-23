using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct AsteroidSpawner : IComponentData
{
    public EntityArchetype asteroidArchetype;
    public float3 SpawnPosition;
    public float NextSpawnTime;
    public float SpawnRate;
}

