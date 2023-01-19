using Alteruna;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public struct Velocity : IComponentData
{
    public float3 value;
}

public struct Rotation : IComponentData
{
    public float value;

    public float degrees => value;
    public float radians => math.radians(value);
}

[UpdateBefore(typeof(MovementSystem))]
public partial struct MomentumToVelocitySystem : ISystem
{
    EntityQuery query;
    ComponentLookup<Velocity> velocities;
    ComponentLookup<Momentum> momentums;
    ComponentLookup<Rotation> rotations;
    public void OnCreate(ref SystemState state)
    {
        query = new EntityQueryBuilder(Allocator.Temp)
            .WithAllRW<Velocity,InputComp>()
            .WithAllRW<Rotation>()
            .WithAll<Player, Local>()
            .Build(ref state);
        velocities = state.GetComponentLookup<Velocity>(false);
        momentums = state.GetComponentLookup<Momentum>(false);
        rotations = state.GetComponentLookup<Rotation>(false);
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {   
        var manager = state.EntityManager;

        var entities = query.ToEntityArray(Allocator.Temp);
        velocities.Update(ref state);
        momentums.Update(ref state);
        rotations.Update(ref state);
        
        const float SPEED = 3f;

        foreach (var entity in entities) 
        {
            var momentum = momentums[entity].value;
            //var vel = (input.Forward ? speed : input.Back ? -speed : 0f);        
            //var quat = Quaternion.Euler(0, 0, rotations[entity].value);
            //var speed = (input.Forward ? SPEED : input.Back ? -SPEED : 0f);
            var angle = rotations[entity].radians;
            var direction = math.float3(math.cos(angle), math.sin(angle), 0.0f);

            velocities[entity] = new Velocity { value =  direction * momentum * SPEED};
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
        var entities = query.ToEntityArray(Allocator.Temp);
        rotation.Update(ref state);
        inputComps.Update(ref state);

        float speed = 3f;
        foreach (var entity in entities) 
        {
            var input = inputComps[entity];
            var rotAdd = (input.TurnRight ? -speed : input.TurnLeft ? speed : 0f);
            var currentRot = rotation[entity].value;
            var nextRot = (currentRot + rotAdd);
            // Angles: [0, 360]
            rotation[entity] = new Rotation { value = (nextRot + 360.0f) % 360.0f};
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
            //var oldRot = rotation[entity].value;
            positions[entity] = new Position { value = pos + (vel * dt) };
        }
    }
}
