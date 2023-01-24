using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct AsteroidSpawnerSystem : ISystem
{
    private EntityQuery spawnerQuery;
    private EntityArchetype asteroidArchetype;
    private ComponentLookup<AsteroidSpawner> spawnerLookup;
    private EntityQuery playerQuery;
    private float nextAsteroidSpawn;
    private float spawnRate;
    private float3 spawnPos;
    public void OnCreate(ref SystemState state)
    {
        spawnRate = 1f;
        spawnerQuery = state.GetEntityQuery(typeof(AsteroidSpawner));
        spawnerLookup = state.GetComponentLookup<AsteroidSpawner>();
        spawnPos = new float3(0f, 12f, 0);

        playerQuery = state.GetEntityQuery(typeof(Player));
        
        var spawnerArchetype = state.EntityManager.CreateArchetype(typeof(AsteroidSpawner),typeof(Position));
        asteroidArchetype = state.EntityManager.CreateArchetype(typeof(AsteroidTag), typeof(LocalTransform),typeof(Position),typeof(Velocity),typeof(Rotation),typeof(Local));

        var spawnerEntity = state.EntityManager.CreateEntity(spawnerArchetype);
        state.EntityManager.SetComponentData(spawnerEntity, new AsteroidSpawner{NextSpawnTime = 0, asteroidArchetype = asteroidArchetype, SpawnPosition = spawnPos, SpawnRate = spawnRate});
        state.EntityManager.SetComponentData(spawnerEntity, new Position{value = spawnPos});


    }

    public void OnDestroy(ref SystemState state)
    {
        
    }


    public void OnUpdate(ref SystemState state)
    {
        spawnerLookup.Update(ref state);
        var players = playerQuery.ToEntityArray(Allocator.Temp);
        var spawners = spawnerQuery.ToEntityArray(Allocator.Temp);

        if (players.Length < 2)
        {
            return;
        }
        
        spawnerLookup.Update(ref state);

        foreach (var spawner in spawners)
        {
            if (nextAsteroidSpawn < SystemAPI.Time.ElapsedTime)
            {
                var newAsteroid = state.EntityManager.CreateEntity(asteroidArchetype);

                state.EntityManager.SetComponentData(newAsteroid, new AsteroidTag{direction = 0, lifeTime = 25f});
                state.EntityManager.SetComponentData(newAsteroid, new Position{value = spawnPos});

                nextAsteroidSpawn = (float)SystemAPI.Time.ElapsedTime + spawnRate;
            }
        }
    }

}
