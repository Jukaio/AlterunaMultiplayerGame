using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct AsteroidSpawnerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        
    }

    public void OnDestroy(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var spawner in SystemAPI.Query<RefRW<AsteroidSpawner>>())
        {
            ProcessSpawner(ref state, spawner);
        }
    }

    private void ProcessSpawner(ref SystemState state, RefRW<AsteroidSpawner> asteroidSpawner)
    {
        if (asteroidSpawner.ValueRO.NextSpawnTime < SystemAPI.Time.ElapsedTime)
        {
            Debug.Log("Spawned new asteroid");
            Entity newEntity = state.EntityManager.Instantiate(asteroidSpawner.ValueRO.Prefab);
            state.EntityManager.SetComponentData(newEntity,LocalTransform.FromPosition(asteroidSpawner.ValueRO.SpawnPosition));

            asteroidSpawner.ValueRW.NextSpawnTime = (float)SystemAPI.Time.ElapsedTime + asteroidSpawner.ValueRO.SpawnRate;
        }
    }
}
