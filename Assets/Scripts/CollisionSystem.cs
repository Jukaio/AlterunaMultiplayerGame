using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public struct SizeComp : IComponentData
{
    public float2 size;
}

public struct ColliderComp : IComponentData
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


[UpdateBefore(typeof(CollisionOrderSystem))]
public partial struct CollisionSystem : ISystem
{

    private EntityQuery query;
    private ComponentLookup<ColliderComp> m_ColliderCompLookup;
    private ComponentLookup<Team> m_TeamLookup;

    public void OnCreate(ref SystemState state)
    {
        query = new EntityQueryBuilder(Allocator.Temp)
         .WithAllRW<ColliderComp,Team>()
         .Build(ref state);

        m_ColliderCompLookup = state.GetComponentLookup<ColliderComp>();
        m_TeamLookup = state.GetComponentLookup<Team>();
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        var entities = query.ToEntityArray(Allocator.Temp);
        m_ColliderCompLookup.Update(ref state);
        m_TeamLookup.Update(ref state);

        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var orderQuery = manager.CreateEntityQuery(typeof(CollisisonOrderQueue));

        var collisionOrderQueue = orderQuery.GetSingleton<CollisisonOrderQueue>();
        var orderQueue = collisionOrderQueue.Queue;
        var handeledOrders = collisionOrderQueue.HandledList;

        //TODO might want to implement a quadtree for the sake of effiency when checking collisions, instead of checking against all in world
        foreach (var entityA in entities)
        {
            foreach (var entityB in entities)
            {
                if (entityA == entityB) { continue; }
                if (m_TeamLookup[entityA].value == m_TeamLookup[entityB].value) { continue; }

                var aMin = m_ColliderCompLookup[entityA].Min;
                var aMax = m_ColliderCompLookup[entityA].Max;

                var bMin = m_ColliderCompLookup[entityB].Min;
                var bMax = m_ColliderCompLookup[entityB].Max;

                var newOrder = new CollisionOrder(entityA, entityB);

                if (aMax.x >= bMin.x && aMin.x <= bMax.x && aMax.y >= bMin.y && aMin.y <= bMax.y)
                {
                    if(CollisionAlreadyHandeled(handeledOrders, newOrder)) { continue; }

                    if (orderQueue.Count > 0)
                    {
                        var orderArray = orderQueue.ToArray(Allocator.Temp);
                        if (DoesQueueAlreadyHaveCollision(newOrder, orderArray)) { continue; }
                    }

                    orderQueue.Enqueue(newOrder);
                }
                else
                {
                    for (int i = handeledOrders.Length - 1; i >= 0; i--)
                    {
                        var order = handeledOrders[i];
                        if (SameCollision(order, newOrder)) { handeledOrders.RemoveAt(i); }
                    }
                }
            }
        }
    }

    private static bool CollisionAlreadyHandeled(NativeList<CollisionOrder> handeledOrders, CollisionOrder newOrder)
    {
        foreach (var order in handeledOrders)
        {
            if (SameCollision(order, newOrder)) { return true; }
        }
        return false;
    }

    private static bool DoesQueueAlreadyHaveCollision(CollisionOrder newOrder, NativeArray<CollisionOrder> orderArray)
    {
        foreach (var currentOrder in orderArray)
        {
            if (SameCollision(currentOrder, newOrder)) { return true; }
        }
        return false;
    }

    private static bool SameCollision(CollisionOrder currentOrder ,CollisionOrder newOrder)
    {
        return (currentOrder.A ==newOrder.A && currentOrder.B == newOrder.B) || (currentOrder.B == newOrder.A && currentOrder.A == newOrder.B);
    }
}

public partial struct CollisionOrderSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {

    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        //TODO take care of all orders in CollsionOrderQueue and apply whatever is suposed to happen to entites

        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var orderQuery = manager.CreateEntityQuery(typeof(CollisisonOrderQueue));

        var collisionOrderQueue = orderQuery.GetSingleton<CollisisonOrderQueue>();
        var orderQueue = collisionOrderQueue.Queue;
        var handeledOrders = collisionOrderQueue.HandledList;


        while (!orderQueue.IsEmpty())
        {
            var order = orderQueue.Dequeue();
            handeledOrders.Add(order);
            Debug.LogError("Entity: " + order.A.ToString() + " collided with Entity: " + order.B.ToString());
        }
    }
}