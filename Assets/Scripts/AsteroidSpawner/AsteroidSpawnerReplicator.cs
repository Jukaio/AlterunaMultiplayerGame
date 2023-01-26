using System.Collections;
using System.Collections.Generic;
using Alteruna;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using UnityEngine;

public class AsteroidSpawnerReplicator : Replicator
{
    public override void OnCreate(out ComponentType[] types)
    {
        types = new ComponentType[2];
        types[0] = typeof(AsteroidSpawner);
        types[1] = typeof(Position);
    }

    public override void OnAssembleData(in EntityQuery query, Writer writer, byte LOD = 100)
    {
        writer.Write(query.ToComponentDataArray<AsteroidSpawner>(Allocator.Temp));
        writer.Write(query.ToComponentDataArray<Position>(Allocator.Temp));
    }

    public override void OnDisassembleData(in UnsafeList<Entity> entities, Reader reader, byte LOD = 100)
    {
        var manager = World.EntityManager;
        var asteroidSpawner = reader.ReadArray<AsteroidSpawner>();
        var positions = reader.ReadArray<Position>();

        for (int i = 0; i < entities.Length; i++)
        {
            var entity = entities[i];
            manager.SetComponentData(entity, asteroidSpawner[i]);
            manager.SetComponentData(entity, positions[i]);

        }
    }
}
