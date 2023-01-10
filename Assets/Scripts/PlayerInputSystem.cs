using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine.InputSystem;


public struct InputComp : IComponentData
{
    public bool[] value;
}

public partial struct PlayerInputSystem : ISystem
{
    private InputActions m_InputActions;
    private InputActions.PlayerActionMapActions m_PlayerInputActions;

    public void OnDestroy(ref SystemState state)
    {
        throw new System.NotImplementedException();
    }

    public void OnCreate(ref SystemState state)
    {
        m_InputActions = new InputActions();
        m_InputActions.Enable();

        m_PlayerInputActions = m_InputActions.PlayerActionMap;
    }

    public void OnUpdate(ref SystemState state)
    {
        InputSystem.Update();

        bool[] value = new bool[5];

        //TODO do this for each action and toggle the inputs
      
        value[0] = m_PlayerInputActions.MoveForward.ReadValue<bool>();


        EntityQuery query = new EntityQueryBuilder(Allocator.Temp)
            .WithAllRW<InputComp>()
            .WithAll<Client>()
            .Build(ref state);

        var entities = query.ToEntityArray(Allocator.Temp);
       // var inputComps = state.GetComponentLookup<InputComp>(false);

        //TODO should probably only apply to local player
        //foreach (var entity in entities)
        //{
        //   var inputComp = inputComps[entity];
        //    //TODO Set inputs
        //}
    }

    void ISystem.OnCreate(ref SystemState state)
    {
        throw new System.NotImplementedException();
    }

    void ISystem.OnUpdate(ref SystemState state)
    {
        throw new System.NotImplementedException();
    }
}