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
        NativeList<Matrix4x4> team1Matrices = new(Allocator.Temp);
        NativeList<Matrix4x4> team2Matrices = new(Allocator.Temp);
        foreach (var (position,rotation,team) in SystemAPI.Query<Position,Rotation,Team>().WithAll<Player>()) {
            var matrix = Matrix4x4.TRS(position.value, Quaternion.Euler(0,0,rotation.value), Vector3.one);
            if (!team.value)
            {
                team1Matrices.Add(matrix);
            }
            else
            {
                team2Matrices.Add(matrix);
            }
        }

        var spriteLib = spriteLibQuery.GetSingleton<SpriteLibrary>();
        if (!team1Matrices.IsEmpty) 
        {
            RenderParams team1RenderParams = new(spriteLib.Team1Material); // Material in here
            int count = Mathf.Min(1023, team1Matrices.Length);
            Assert.IsTrue(team1Matrices.Length < 1024, "Too many matrices");
            Graphics.RenderMeshInstanced(team1RenderParams, spriteLib.Mesh, 0, team1Matrices.AsArray(), count);
        }

        if(!team2Matrices.IsEmpty)
        {
            RenderParams team2RenderParams = new(spriteLib.Team2Material); // Material in here
            int count = Mathf.Min(1023, team2Matrices.Length);
            Assert.IsTrue(team2Matrices.Length < 1024, "Too many matrices");
            Graphics.RenderMeshInstanced(team2RenderParams, spriteLib.Mesh, 0, team2Matrices.AsArray(), count);
        }
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
        NativeList<Matrix4x4> team1Matrices = new(Allocator.Temp);
        NativeList<Matrix4x4> team2Matrices = new(Allocator.Temp);
        foreach (var (position, team) in SystemAPI.Query<Position, Team>().WithAll<Bullet>())
        {
            var matrix = Matrix4x4.TRS(position.value, Quaternion.identity, Vector3.one * 0.3f);
            if (!team.value)
            {
                team1Matrices.Add(matrix);
            }
            else
            {
                team2Matrices.Add(matrix);
            }
        }

        var spriteLib = spriteLibQuery.GetSingleton<SpriteLibrary>();
        if (!team1Matrices.IsEmpty)
        {
            RenderParams team1RenderParams = new(spriteLib.Team1BulletMaterial); // Material in here
            int count = Mathf.Min(1023, team1Matrices.Length);
            Assert.IsTrue(team1Matrices.Length < 1024, "Too many matrices");
            Graphics.RenderMeshInstanced(team1RenderParams, spriteLib.Mesh, 0, team1Matrices.AsArray(), count);
        }

        if (!team2Matrices.IsEmpty)
        {
            RenderParams team2RenderParams = new(spriteLib.Team2BulletMaterial); // Material in here
            int count = Mathf.Min(1023, team2Matrices.Length);
            Assert.IsTrue(team2Matrices.Length < 1024, "Too many matrices");
            Graphics.RenderMeshInstanced(team2RenderParams, spriteLib.Mesh, 0, team2Matrices.AsArray(), count);
        }

    }
}

public partial struct RenderAsteroidSystem : ISystem
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
        NativeList<Matrix4x4> team1Matrices = new(Allocator.Temp);
        foreach (var position in SystemAPI.Query<Position>().WithAll<AsteroidTag>())
        {
            var matrix = Matrix4x4.TRS(position.value, Quaternion.identity, Vector3.one * 0.3f);
            team1Matrices.Add(matrix);
        }

        var spriteLib = spriteLibQuery.GetSingleton<SpriteLibrary>();
        if (!team1Matrices.IsEmpty)
        {
            RenderParams team1RenderParams = new(spriteLib.Team1BulletMaterial); // Material in here
            int count = Mathf.Min(1023, team1Matrices.Length);
            Assert.IsTrue(team1Matrices.Length < 1024, "Too many matrices");
            Graphics.RenderMeshInstanced(team1RenderParams, spriteLib.Mesh, 0, team1Matrices.AsArray(), count);
        }

    }
}

public partial struct RenderColliderSystem : ISystem
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
   
        foreach (var collider in SystemAPI.Query<ColliderComp>())
        {
            var matrix = Matrix4x4.TRS(new Vector3( collider.Min.x, collider.Min.y,0f), Quaternion.identity, new Vector3(0.1f, 0.1f, 0.1f));         
            matrices.Add(matrix);
            matrix = Matrix4x4.TRS(new Vector3(collider.Max.x, collider.Max.y, 0f), Quaternion.identity, new Vector3(0.1f, 0.1f, 0.1f));
            matrices.Add(matrix);
            matrix = Matrix4x4.TRS(new Vector3(collider.Max.x, collider.Min.y, 0f), Quaternion.identity, new Vector3(0.1f, 0.1f, 0.1f));
            matrices.Add(matrix);
            matrix = Matrix4x4.TRS(new Vector3(collider.Min.x, collider.Max.y, 0f), Quaternion.identity, new Vector3(0.1f, 0.1f, 0.1f));
            matrices.Add(matrix);
        }

        var spriteLib = spriteLibQuery.GetSingleton<SpriteLibrary>();
        if (!matrices.IsEmpty)
        {
            RenderParams RenderParams = new(spriteLib.DefaultMaterial); // Material in here
            int count = Mathf.Min(1023, matrices.Length);
            Assert.IsTrue(matrices.Length < 1024, "Too many matrices");
            Graphics.RenderMeshInstanced(RenderParams, spriteLib.Mesh, 0, matrices.AsArray(), count);
        }

      
    }
}
