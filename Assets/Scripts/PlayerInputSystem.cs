using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine.InputSystem;


public struct InputComp : IComponentData
{ 
    public bool Forward;
    public bool Back;
    public bool TurnLeft;
    public bool TurnRight;
    public bool Shoot;
}


public partial struct PlayerInputSystem : ISystem
{
    private InputActions m_InputActions;
    private InputActions.PlayerActionMapActions m_PlayerInputActions;

    EntityQuery query;
    ComponentLookup<InputComp> m_InputCompLookup;

    public void OnDestroy(ref SystemState state)
    {
        throw new System.NotImplementedException();
    }

    public void OnCreate(ref SystemState state)
    {
        m_InputActions = new InputActions();
        m_InputActions.Enable();

        m_PlayerInputActions = m_InputActions.PlayerActionMap;

        query = new EntityQueryBuilder(Allocator.Temp)
           .WithAllRW<InputComp>()
           .WithAll<Client>()
           .Build(ref state);
        m_InputCompLookup = state.GetComponentLookup<InputComp>();
    }

    public void OnUpdate(ref SystemState state)
    {
        InputSystem.Update();

        InputComp comp= new InputComp();

        comp.Forward = m_PlayerInputActions.MoveForward.ReadValue<bool>();
        comp.Back = m_PlayerInputActions.MoveBackwards.ReadValue<bool>();
        comp.TurnLeft = m_PlayerInputActions.TurnLeft.ReadValue<bool>();
        comp.TurnRight = m_PlayerInputActions.TurnRight.ReadValue<bool>();
        comp.Shoot = m_PlayerInputActions.Shoot.ReadValue<bool>();
 
        var entities = query.ToEntityArray(Allocator.Temp);
        m_InputCompLookup.Update(ref state);

        //TODO should probably only apply to local player
        foreach (var entity in entities)
        {
            m_InputCompLookup[entity] = comp;
        }
    }
}