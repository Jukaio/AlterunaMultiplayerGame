using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial struct AsteroidSpawnerSystem : ISystem
{
    private EntityQuery spawnerQuery;
    private EntityArchetype asteroidArchetype;
    private ComponentLookup<AsteroidSpawner> spawnerLookup;
    private float nextAsteroidSpawn;
    private float spawnRate;
    public void OnCreate(ref SystemState state)
    {
        spawnRate = 2f;
        spawnerQuery = state.GetEntityQuery(typeof(AsteroidSpawner));
        spawnerLookup = state.GetComponentLookup<AsteroidSpawner>();
        
        var spawnerArchetype = state.EntityManager.CreateArchetype(typeof(AsteroidSpawner));
        asteroidArchetype = state.EntityManager.CreateArchetype(typeof(AsteroidTag), typeof(LocalTransform),typeof(Position));

        var spawnerEntity = state.EntityManager.CreateEntity(spawnerArchetype);
        state.EntityManager.SetComponentData(spawnerEntity, new AsteroidSpawner{NextSpawnTime = 0, asteroidArchetype = asteroidArchetype, SpawnPosition = 0, SpawnRate = 1f});
        
        

    }

    public void OnDestroy(ref SystemState state)
    {
        
    }


    public void OnUpdate(ref SystemState state)
    {
        var spawners = spawnerQuery.ToEntityArray(Allocator.Temp);
        
        spawnerLookup.Update(ref state);
        
        foreach (var spawner in spawners)
        {
            if (nextAsteroidSpawn < SystemAPI.Time.ElapsedTime)
            {
                Debug.Log("Spawned new asteroid");
                var newAsteroid = state.EntityManager.CreateEntity(asteroidArchetype);

                state.EntityManager.SetComponentData(newAsteroid, new AsteroidTag{direction = 0, lifeTime = 10f});
                state.EntityManager.SetComponentData(newAsteroid,LocalTransform.FromPosition(spawnerLookup[spawner].SpawnPosition));

                nextAsteroidSpawn = (float)SystemAPI.Time.ElapsedTime + spawnRate;
            }
        }
    }

}
