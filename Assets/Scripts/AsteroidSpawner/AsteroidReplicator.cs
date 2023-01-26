using Alteruna;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

public class AsteroidReplicator : Replicator
{
    public override void OnAssembleData(in EntityQuery query, Writer writer, byte LOD = 100)
    {
        writer.Write(query.ToComponentDataArray<AsteroidTag>(Allocator.Temp));
        writer.Write(query.ToComponentDataArray<Position>(Allocator.Temp));
        writer.Write(query.ToComponentDataArray<Velocity>(Allocator.Temp));
        writer.Write(query.ToComponentDataArray<ColliderComp>(Allocator.Temp));
    }
    public override void OnCreate(out ComponentType[] types)
    {
        types = new ComponentType[4];
        types[0] = typeof(AsteroidTag);
        types[1] = typeof(Position);
        types[2] = typeof(Velocity);
        types[3] = typeof(ColliderComp);
    }

    public override void OnDisassembleData(in UnsafeList<Entity> entities, Reader reader, byte LOD = 100)
    {
        var manager = World.EntityManager;
        var asteroidData = reader.ReadArray<AsteroidTag>();
        var positions = reader.ReadArray<Position>();
        var velocity = reader.ReadArray<Velocity>();
        var colliders = reader.ReadArray<ColliderComp>();

        for (int i = 0; i < entities.Length; i++)
        {
            if (entities[i] != null)
            {
                var entity = entities[i];
                manager.SetComponentData(entity,asteroidData[i]);
                manager.SetComponentData(entity, positions[i]);
                manager.SetComponentData(entity, velocity[i]);
                manager.SetComponentData(entity, colliders[i]);   
            }
        }
    }
}
