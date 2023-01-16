using UnityEngine;
using Unity.Collections;
using Unity.Entities;


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

public class CollisisonOrderQueue : MonoBehaviour, IComponentData
{
    public NativeQueue<CollisionOrder> Queue;

    void Start()
    {
        Queue = new NativeQueue<CollisionOrder>();
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _ = manager.CreateSingleton(this, "Collision Order Queue");
    }

    private void OnDestroy()
    {
        Queue.Dispose();
    }
}

