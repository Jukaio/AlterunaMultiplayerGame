using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

//TODO add a component for AABB  collisions

struct SizeComp : IComponentData
{
    public float2 size;
}

struct ColliderComp : IComponentData
{
    public float2 Min;
    public float2 Max;
}

[UpdateBefore(typeof(CollisionSystem))]
public partial struct UpdateColliderSystem : ISystem
{

    private EntityQuery query;
    private ComponentLookup<Position> m_PositionLookup;
    private ComponentLookup<SizeComp> m_SizeLookup;
    private ComponentLookup<ColliderComp> m_ColliderCompLookup;

    public void OnCreate(ref SystemState state)
    {
        query = new EntityQueryBuilder(Allocator.Temp)
         .WithAllRW<Position, SizeComp>()
         .WithAllRW<ColliderComp>()
         .WithAll<Local>()
         .Build(ref state);
        m_PositionLookup = state.GetComponentLookup<Position>();
        m_SizeLookup = state.GetComponentLookup<SizeComp>();
        m_ColliderCompLookup = state.GetComponentLookup<ColliderComp>();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var entities = query.ToEntityArray(Allocator.Temp);
        m_PositionLookup.Update(ref state);
        m_SizeLookup.Update(ref state);
        m_ColliderCompLookup.Update(ref state);
        foreach (var entity in entities)
        {
            ColliderComp col = new ColliderComp();
            col.Min = new float2(m_PositionLookup[entity].value.x, m_PositionLookup[entity].value.y) - (m_SizeLookup[entity].size * 0.5f);
            col.Max = new float2(m_PositionLookup[entity].value.x, m_PositionLookup[entity].value.y) + (m_SizeLookup[entity].size * 0.5f);
            m_ColliderCompLookup[entity] = col;
        }
    }
}


//TODO might want to implement a quadtree for the skae of effiency when checking collisions
public partial struct CollisionSystem : ISystem
{

    private EntityQuery query;
    private ComponentLookup<Position> m_PositionLookup;

    public void OnCreate(ref SystemState state)
    {
        throw new System.NotImplementedException();
    }

    public void OnDestroy(ref SystemState state)
    {
        throw new System.NotImplementedException();
    }

    public void OnUpdate(ref SystemState state)
    {
        throw new System.NotImplementedException();
    }
}
