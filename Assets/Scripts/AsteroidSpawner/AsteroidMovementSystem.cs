using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public partial struct AsteroidMovementSystem : ISystem
{
    private EntityQuery asteroidQuery;
    private ComponentLookup<AsteroidTag> asteroidLookup;
    private ComponentLookup<Position> position;

    public void OnCreate(ref SystemState state)
    {
        asteroidQuery = state.GetEntityQuery(typeof(AsteroidTag));
        asteroidLookup = state.GetComponentLookup<AsteroidTag>();
        position = state.GetComponentLookup<Position>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {

        var asteroids = asteroidQuery.ToEntityArray(Allocator.Temp);
        asteroidLookup.Update(ref state);
        position.Update(ref state);

        NativeList<Entity> destroyedList = new NativeList<Entity>(Allocator.Temp);
        
        foreach (var asteroid in asteroids)
        {
            float lifetime = asteroidLookup[asteroid].lifeTime;
            lifetime -= SystemAPI.Time.DeltaTime;
            var dir = asteroidLookup[asteroid].direction;
            
            if (asteroidLookup[asteroid].direction.x == 0 && asteroidLookup[asteroid].direction.y == 0)
            {
                state.EntityManager.SetComponentData(asteroid,new AsteroidTag{direction = new float2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)), lifeTime = lifetime});
            }
            float3 newPos = new float3(position[asteroid].value.x + asteroidLookup[asteroid].direction.x * SystemAPI.Time.DeltaTime,  position[asteroid].value.y+ asteroidLookup[asteroid].direction.y * SystemAPI.Time.DeltaTime,0);
            state.EntityManager.SetComponentData(asteroid, new Position{value = newPos});
            state.EntityManager.SetComponentData(asteroid, new AsteroidTag{direction = dir, lifeTime =  lifetime});

            if (lifetime <= 0)
            {
                destroyedList.Add(asteroid);
            }
        }

        foreach (var asteroid in destroyedList)
        {
            state.EntityManager.DestroyEntity(asteroid);
            Debug.Log("Destroyed asteroid");
        }
        
        /*foreach((TransformAspect transformAspect, RefRW<AsteroidTag> asteroid) in SystemAPI.Query<TransformAspect,RefRW<AsteroidTag>>())
        {
            if (asteroid.ValueRW.direction.x == 0 && asteroid.ValueRW.direction.y == 0)
            {
                SetAsteroidDir(ref state, asteroid);    
            }
            
            asteroid.ValueRW.lifeTime -= SystemAPI.Time.DeltaTime;
            transformAspect.LocalPosition += new float3( asteroid.ValueRW.direction.x * SystemAPI.Time.DeltaTime, asteroid.ValueRW.direction.y * SystemAPI.Time.DeltaTime,0);
        }*/

        
    }

    /*private void SetAsteroidDir(ref SystemState state, RefRW<AsteroidTag> asteroid)
    {
        asteroid.ValueRW.direction = new float2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    }*/
}
