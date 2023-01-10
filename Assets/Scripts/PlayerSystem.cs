using Alteruna;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Alteruna;
using Unity.Collections.LowLevel.Unsafe;

public struct Velocity : IComponentData
{
    public float3 value;
}

public partial struct PlayerSystemAsStruct : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        var manager = state.EntityManager;
        //manager.CreateEntity()
        Debug.Log("Create");
    }

    public void OnDestroy(ref SystemState state)
    {
        Debug.Log("Destroy");
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


public partial class PlayerSystem : SystemBase
{

    protected override void OnCreate()
    {
        base.OnCreate();
        
    }

    protected override void OnUpdate()
    {
      
        Debug.Log("Update");

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

public partial struct ShootingSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {

    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {

    }
}


public partial struct CollisionSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {

    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {

    }
}

public partial struct MovementReplicationSystem : ISystem
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
            .WithAll<Position, Client>()
            .Build(ref state);

        var entities = query.ToEntityArray(Allocator.Temp);
        var positions = state.GetComponentLookup<Position>(false);

        NativeList<Position> buffer = new NativeList<Position>(Allocator.Temp);
        foreach (var entity in entities) {
            buffer.Add(positions[entity]);
        }

        NativeArray<byte> bufferData = buffer.AsArray().Reinterpret<byte>();
        Writer writer = new(null);
        writer.Write(bufferData.ToArray());
    }
}

public partial struct ShootingReplicationSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {

    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {

    }
}


public partial struct CollisionReplicationSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {

    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {

    }
}


