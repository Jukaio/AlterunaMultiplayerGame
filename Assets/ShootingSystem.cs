using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using Unity.Entities.CodeGeneratedJobForEach;
using Unity.Collections;

public struct Bullet : IComponentData
{
    public Entity owner;
}

public partial struct ShootingSystem : ISystem
{
    private EntityQuery playerQuery;
    private EntityArchetype bulletArchetype;
    private ComponentLookup<Position> positions;
    private ComponentLookup<Rotation> rotations;
    private ComponentLookup<Team> teams;

    public void OnCreate(ref SystemState state)
    {
        positions = state.GetComponentLookup<Position>();
        rotations = state.GetComponentLookup<Rotation>();
        teams = state.GetComponentLookup<Team>();
        playerQuery = state.GetEntityQuery(typeof(Player), typeof(Position), typeof(Rotation), typeof(Local),typeof(Team));
        bulletArchetype = state.EntityManager.CreateArchetype(typeof(Bullet), typeof(Position), typeof(Local), typeof(Velocity), typeof(Team));
    }

    public void OnDestroy(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        var manager = state.EntityManager;
        var players = playerQuery.ToEntityArray(Allocator.Temp);

        positions.Update(ref state);
        rotations.Update(ref state);
        teams.Update(ref state);

      
      

        foreach(var player in players) {
            // TODO: Implement input component 
            if (Keyboard.current[Key.Space].wasPressedThisFrame) {

              

                var angle = rotations[player].radians;
                var direction = math.float3(math.cos(angle), math.sin(angle), 0.0f);

                var position = positions[player];
                var bullet = manager.CreateEntity(bulletArchetype);
                manager.SetComponentData(bullet, position);
                manager.SetComponentData(bullet, new Velocity { value = direction * 10.0f });
                manager.SetComponentData(bullet, new Bullet { owner = player });
                manager.SetComponentData(bullet, new Team { value = teams[player].value });
            }
        }
    }
}

public partial struct BulletCleanupSystem : ISystem
{
    //private EntityQuery playerQuery;
    private EntityQuery bulletQuery;
    private ComponentLookup<Bullet> bullets;
    private ComponentLookup<Position> positions;

    public void OnCreate(ref SystemState state)
    {
        positions = state.GetComponentLookup<Position>();
        bullets = state.GetComponentLookup<Bullet>();
        bulletQuery = state.GetEntityQuery(typeof(Bullet), typeof(Position), typeof(Local));
        //playerQuery = state.GetEntityQuery(typeof(Player), typeof(Position), typeof(Local));
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        positions.Update(ref state);
        bullets.Update(ref state);
        var bulletEntities = bulletQuery.ToEntityArray(Allocator.Temp);

        var manager = state.EntityManager;
        var explosionQuery = manager.CreateEntityQuery(typeof(ExplosionMessenger));
        var messenger = explosionQuery.GetSingleton<ExplosionMessenger>();

        var destroyList = new NativeList<Entity>(Allocator.Temp);
        foreach(var bullet in bulletEntities) {
            var bulletPosition = positions[bullet];
            var playerPosition = positions[bullets[bullet].owner];
            if(math.distance(bulletPosition.value, playerPosition.value) >= 5.0f) {
                destroyList.Add(bullet);
            }
        }
        foreach(var bullet in destroyList) 
        {
            messenger.Notify(new ExplosionMessage(positions[bullet].value));
            state.EntityManager.DestroyEntity(bullet);
        }
    }
}
