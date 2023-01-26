using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public partial struct AsteroidSpawnerSystem : ISystem
{
    private EntityQuery spawnerQuery;
    private EntityArchetype asteroidArchetype;
    private ComponentLookup<AsteroidSpawner> spawnerLookup;
    private EntityQuery playerQuery;
    private float nextAsteroidSpawn;
    private float spawnRate;
    private float3 spawnPos;
    int indexPos;
    float2 dir;

    private EntityQuery asteroidQuery;
    public void OnCreate(ref SystemState state)
    {
        indexPos = 0;
        spawnRate = 6f;
        spawnerQuery = state.GetEntityQuery(typeof(AsteroidSpawner));
        spawnerLookup = state.GetComponentLookup<AsteroidSpawner>();
        spawnPos = new float3(0f, 12f, 0);

        asteroidQuery = state.GetEntityQuery(typeof(AsteroidTag));

        playerQuery = state.GetEntityQuery(typeof(Player));
        
        var spawnerArchetype = state.EntityManager.CreateArchetype(typeof(AsteroidSpawner),typeof(Position));
        asteroidArchetype = state.EntityManager.CreateArchetype(typeof(AsteroidTag), typeof(LocalTransform),typeof(Position),typeof(Velocity),typeof(Rotation),typeof(Local),typeof(ColliderComp),typeof(SizeComp));

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

        var asteroids = asteroidQuery.ToEntityArray(Allocator.Temp);

        if (players.Length < 2)
        {
            return;
        }

        foreach (var spawner in spawners)
        {
            if (nextAsteroidSpawn < SystemAPI.Time.ElapsedTime)
            {
                var newAsteroid = state.EntityManager.CreateEntity(asteroidArchetype);
                switch (indexPos)
                {
                    case 0: SetPos(1,new float2(0, -1));
                        break;
                    case 1: SetPos(2,new float2(-0.4f, -1));
                        break;
                    case 2: SetPos(0,new float2(0.4f, -1));
                        break;
                }
                 

                state.EntityManager.SetComponentData(newAsteroid, new AsteroidTag{direction = dir, lifeTime = 10f});
                state.EntityManager.SetComponentData(newAsteroid, new Position{value = spawnPos});
                state.EntityManager.SetComponentData(newAsteroid, new SizeComp{size = 0.3f});

                nextAsteroidSpawn = (float)SystemAPI.Time.ElapsedTime + spawnRate;
            }
        }
    }

    private void SetPos(int newIndex, float2 newDir)
    {
        dir = newDir;
        indexPos = newIndex;
    }
}
