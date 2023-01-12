using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Assertions;

public struct Position : IComponentData
{
    public float3 value;
}

public partial struct RenderSystem : ISystem
{
    EntityQuery spriteLibQuery;

    public void OnCreate(ref SystemState state)
    {
        spriteLibQuery = state.GetEntityQuery(typeof(SpriteLibrary));
    }

    public void OnDestroy(ref SystemState state)
    {
        
    }

    public void OnUpdate(ref SystemState state)
    {
        // SystemAPI is single threaded
        NativeList<Matrix4x4> matrices = new(Allocator.Temp);
        foreach (var position in SystemAPI.Query<Position>().WithAll<Player>()) {
            var matrix = Matrix4x4.TRS(position.value, Quaternion.identity, Vector3.one);
            matrices.Add(matrix);
        }
        if (matrices.IsEmpty) {
            return;
        }
        var spriteLib = spriteLibQuery.GetSingleton<SpriteLibrary>();
        RenderParams renderParams = new(spriteLib.DefaultMaterial); // Material in here
        int count = Mathf.Min(1023, matrices.Length);
        Assert.IsTrue(matrices.Length < 1024, "Too many matrices");
        Graphics.RenderMeshInstanced(renderParams, spriteLib.Mesh, 0, matrices.AsArray(), count);
    }
}

public partial struct RenderSmallSystem : ISystem
{
    EntityQuery spriteLibQuery;

    public void OnCreate(ref SystemState state)
    {
        spriteLibQuery = state.GetEntityQuery(typeof(SpriteLibrary));
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        // SystemAPI is single threaded
        NativeList<Matrix4x4> matrices = new(Allocator.Temp);
        foreach (var position in SystemAPI.Query<Position>().WithNone<Player>()) {
            var matrix = Matrix4x4.TRS(position.value, Quaternion.identity, Vector3.one * 0.5f);
            matrices.Add(matrix);
        }
        if (matrices.IsEmpty) {
            return;
        }
        var spriteLib = spriteLibQuery.GetSingleton<SpriteLibrary>();
        RenderParams renderParams = new(spriteLib.DefaultMaterial); // Material in here
        int count = Mathf.Min(1023, matrices.Length);
        Assert.IsTrue(matrices.Length < 1024, "Too many matrices");
        Graphics.RenderMeshInstanced(renderParams, spriteLib.Mesh, 0, matrices.AsArray(), count);
    }
}
