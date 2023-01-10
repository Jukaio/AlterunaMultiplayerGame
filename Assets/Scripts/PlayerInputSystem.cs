using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine.InputSystem;


public struct InputComp : IComponentData
{
    public BitField32 value;
}

public partial class PlayerInputSystem : SystemBase
{
    private InputActions m_InputActions;

    protected override void OnCreate()
    {
        m_InputActions = new InputActions();
        m_InputActions.Enable();
    }

    protected override void OnUpdate()
    {
        InputSystem.Update();

      
    }
}