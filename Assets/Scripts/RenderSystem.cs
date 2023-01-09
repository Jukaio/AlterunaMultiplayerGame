using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

public struct Position : IComponentData
{
    public float3 value;
}

public partial class RenderSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // SystemAPI is single threaded
        foreach(var position in SystemAPI.Query<RefRW<Position>>()) {

        }
        
    }
}
