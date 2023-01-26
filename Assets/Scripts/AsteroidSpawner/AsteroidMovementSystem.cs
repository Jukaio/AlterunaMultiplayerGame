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
    private ComponentLookup<Rotation> rotation;
    private ComponentLookup<Velocity> velocity;

    public void OnCreate(ref SystemState state)
    {
        asteroidQuery = state.GetEntityQuery(typeof(AsteroidTag));
        asteroidLookup = state.GetComponentLookup<AsteroidTag>();
        position = state.GetComponentLookup<Position>();
        rotation = state.GetComponentLookup<Rotation>();
        velocity = state.GetComponentLookup<Velocity>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {

        var asteroids = asteroidQuery.ToEntityArray(Allocator.Temp);
        asteroidLookup.Update(ref state);
        position.Update(ref state);
        rotation.Update(ref state);
        velocity.Update(ref state);
        

        NativeList<Entity> destroyedList = new NativeList<Entity>(Allocator.Temp);
        
        foreach (var asteroid in asteroids)
        {
            if (asteroid == Entity.Null)
            {
                return;
            }
            
            float lifetime = asteroidLookup[asteroid].lifeTime;
            lifetime -= SystemAPI.Time.DeltaTime; 
            var dir = asteroidLookup[asteroid].direction;
            var asteroidPosition = position[asteroid];
            float3 newDir;

           /* if (asteroidLookup[asteroid].direction.x == 0 && asteroidLookup[asteroid].direction.y == 0)
            {
                dir = new float2(Random.Range(-0.5f, 0.5f),-1);
                state.EntityManager.SetComponentData(asteroid,new AsteroidTag{direction = dir, lifeTime = lifetime});
            }*/

            newDir = new float3(dir.x, dir.y, 0);
            state.EntityManager.SetComponentData(asteroid,asteroidPosition);
            state.EntityManager.SetComponentData(asteroid, new AsteroidTag{direction = dir, lifeTime =  lifetime});
            state.EntityManager.SetComponentData(asteroid, new Velocity{ value = newDir * 2f});

            if (lifetime <= 0)
            {
                destroyedList.Add(asteroid);
            }
        }

        foreach (var asteroid in destroyedList)
        {
            state.EntityManager.DestroyEntity(asteroid);
        }

        
    }
    
}
