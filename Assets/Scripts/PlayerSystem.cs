using Alteruna;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections.LowLevel.Unsafe;

public struct Velocity : IComponentData
{
    public float3 value;
}

[UpdateBefore(typeof(MovementSystem))]
public partial struct InputToVelocitySystem : ISystem
{
    EntityQuery query;
    ComponentLookup<Velocity> velocities;
    public void OnCreate(ref SystemState state)
    {
        query = new EntityQueryBuilder(Allocator.Temp)
            .WithAllRW<Velocity>()
            .WithAll<Client>()
            .Build(ref state);
        velocities = state.GetComponentLookup<Velocity>(false);
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {   
        var entities = query.ToEntityArray(Allocator.Temp);
        velocities.Update(ref state);

        foreach (var entity in entities) {
            velocities[entity] = new Velocity { value = math.float3(0.0f, 0.016f, 0.0f) };
        }
    }
}

public partial struct MovementSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {

    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        EntityQuery query = new EntityQueryBuilder(Allocator.Temp)
            .WithAllRW<Position>()
            .WithAll<Velocity, Client>()
            .Build(ref state);

        var entities = query.ToEntityArray(Allocator.Temp);
        var positions = state.GetComponentLookup<Position>(false);
        var velocities = state.GetComponentLookup<Velocity>(false);

        foreach (var entity in entities) {
            var pos = positions[entity].value;
            var vel = velocities[entity].value;
            positions[entity] = new Position { value = pos + vel };
        }

        Debug.Log("Update");
    }
}