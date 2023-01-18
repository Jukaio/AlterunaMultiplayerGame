using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;
using Unity.Entities;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;


public class PlayerSynchroniser : Replicator
{
    public override void OnAssembleData(in EntityQuery query, Writer writer, byte LOD = 100)
    {
        writer.Write(query.ToComponentDataArray<Position>(Allocator.Temp));
        writer.Write(query.ToComponentDataArray<Rotation>(Allocator.Temp));
    }

    public override void OnCreate(out ComponentType[] types)
    {
        types = new ComponentType[3];
        types[0] = typeof(Player);
        types[1] = typeof(Position);
        types[2] = typeof(Rotation);
    }

    public override void OnDisassembleData(in UnsafeList<Entity> entities, Reader reader, byte LOD = 100)
    {
        var manager = World.EntityManager;
        var positions = reader.ReadArray<Position>();
        var rotations = reader.ReadArray<Rotation>();

        for (int i = 0; i < entities.Length; i++) {
            var entity = entities[i];
            manager.AddComponent(entity, typeof(Player));
            manager.RemoveComponent(entity, typeof(Bullet));
            manager.SetComponentData(entity, positions[i]);
            manager.SetComponentData(entity, rotations[i]);
        }
    }
}
