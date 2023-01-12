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

public struct Rotation : IComponentData
{
    public float value;
}

[UpdateBefore(typeof(MovementSystem))]
public partial struct InputToVelocitySystem : ISystem
{
    EntityQuery query;
    ComponentLookup<Velocity> velocities;
    ComponentLookup<InputComp> inputComps;
    public void OnCreate(ref SystemState state)
    {
        query = new EntityQueryBuilder(Allocator.Temp)
            .WithAllRW<Velocity,InputComp>()
            .WithAll<Player>()
            .Build(ref state);
        velocities = state.GetComponentLookup<Velocity>(false);
        inputComps = state.GetComponentLookup<InputComp>(false);
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {   
        var manager = state.EntityManager;

        var entities = query.ToEntityArray(Allocator.Temp);
        velocities.Update(ref state);
        inputComps.Update(ref state);
        
        
        float speed = 1f;

        foreach (var entity in entities) 
        {
            var input = inputComps[entity];
            var vel = (input.Forward ? speed : input.Back ? -speed : 0f);
            velocities[entity] = new Velocity { value = math.float3(0.0f,vel, 0.0f) };
        }
    }
}
public partial struct InputToRotationSystem : ISystem
{
    EntityQuery query;
    ComponentLookup<Rotation> rotation;
    ComponentLookup<InputComp> inputComps;
    public void OnCreate(ref SystemState state)
    {
        query = new EntityQueryBuilder(Allocator.Temp)
            .WithAllRW<Rotation,InputComp>()
            .WithAll<Local>()
            .Build(ref state);
        rotation = state.GetComponentLookup<Rotation>(false);
        inputComps = state.GetComponentLookup<InputComp>(false);
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        
        var manager = state.EntityManager;
        
        var entities = query.ToEntityArray(Allocator.Temp);
        rotation.Update(ref state);
        inputComps.Update(ref state);

        float speed = 1f;
        foreach (var entity in entities) 
        {
            var input = inputComps[entity];
            var rotAdd = (input.TurnRight ? -speed : input.TurnLeft ? speed : 0f);
            var currentRot = rotation[entity].value;
            rotation[entity] = new Rotation { value = currentRot + rotAdd};
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
        var dt = SystemAPI.Time.DeltaTime;
        EntityQuery query = new EntityQueryBuilder(Allocator.Temp)
            .WithAllRW<Position>()
            .WithAll<Velocity, Local>()
            .Build(ref state);

        var entities = query.ToEntityArray(Allocator.Temp);
        var positions = state.GetComponentLookup<Position>(false);
        var velocities = state.GetComponentLookup<Velocity>(false);
        var rotation = state.GetComponentLookup<Rotation>(false);

        foreach (var entity in entities)
        {
            var pos = positions[entity].value;
            var vel = velocities[entity].value;
            var oldRot = rotation[entity].value;
            positions[entity] = new Position { value = pos + (vel * dt) };
        }

        Debug.Log("Update");
    }
}