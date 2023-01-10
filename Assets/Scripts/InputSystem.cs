using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;


public struct Input : IComponentData
{
    public BitField32 value;
}

public partial class InputSystem : SystemBase
{
    protected override void OnUpdate()
    {

    }
}