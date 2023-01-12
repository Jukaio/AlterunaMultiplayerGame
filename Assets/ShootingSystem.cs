using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public partial struct ShootingSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        var entity = state.EntityManager.CreateEntity();
        var position = new Position();
        position.value = math.float3(-2f, -2f, 0.0f);
        
        state.EntityManager.AddComponentData(entity, position);
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        var archetype = state.EntityManager.CreateArchetype(typeof(Position), typeof(Local), typeof(Velocity));
        if(Keyboard.current[Key.Space].wasPressedThisFrame) {
            var bullet = state.EntityManager.CreateEntity(archetype);
            state.EntityManager.SetComponentData(bullet, new Velocity { value = math.float3(0.0f, 0.016f, 0.0f) });
        }
    }
}
