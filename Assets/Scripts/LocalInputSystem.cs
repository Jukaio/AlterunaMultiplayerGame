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

[UpdateBefore(typeof(MomentumToVelocitySystem))]
public partial class LocalInputSystem : SystemBase
{
    private InputActions m_InputActions;
    private InputActions.PlayerActionMapActions m_PlayerInputActions;

    EntityQuery query;
    ComponentLookup<InputComp> m_InputCompLookup;

    protected override void OnDestroy()
    {
        
    }

    protected override void OnCreate()
    {
        m_InputActions = new InputActions();
        m_InputActions.Enable();

        m_PlayerInputActions = m_InputActions.PlayerActionMap;

        query = new EntityQueryBuilder(Allocator.Temp)
           .WithAllRW<InputComp>()
           .WithAll<Local>()
           .Build(ref CheckedStateRef);
        m_InputCompLookup = CheckedStateRef.GetComponentLookup<InputComp>();
    }

    protected override void OnUpdate()
    {
        InputSystem.Update();

        InputComp comp = new InputComp();

        comp.Forward = m_PlayerInputActions.MoveForward.ReadValue<float>() > 0.1f;
        comp.Back = m_PlayerInputActions.MoveBackwards.ReadValue<float>() > 0.1f;
        comp.TurnLeft = m_PlayerInputActions.TurnLeft.ReadValue<float>() > 0.1f;
        comp.TurnRight = m_PlayerInputActions.TurnRight.ReadValue<float>() > 0.1f;
        comp.Shoot = m_PlayerInputActions.Shoot.ReadValue<float>() > 0.1f;

        var entities = query.ToEntityArray(Allocator.Temp);
        m_InputCompLookup.Update(ref CheckedStateRef);
  
        foreach (var entity in entities)
        {    
                m_InputCompLookup[entity] = comp;
        }
    }
}