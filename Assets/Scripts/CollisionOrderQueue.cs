using UnityEngine;
using Unity.Collections;
using Unity.Entities;

public class CollisisonOrderQueue : MonoBehaviour, IComponentData
{
    public NativeQueue<CollisionOrder> Queue;
    public NativeList<CollisionOrder> HandledList;
    void Start()
    {
        Queue = new NativeQueue<CollisionOrder>(Allocator.Persistent);
        HandledList = new NativeList<CollisionOrder>(Allocator.Persistent);
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _ = manager.CreateSingleton(this, "Collision Order Queue");
    }

    private void OnDestroy()
    {
        Queue.Dispose();
        HandledList.Dispose();
    }
}

public struct CollisionOrder
{
    public CollisionOrder(Entity a, Entity b)
    {
        A = a;
        B = b;
    }

    public Entity A;
    public Entity B;
};


