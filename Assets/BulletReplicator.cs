using Alteruna;
using Unity.Entities;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public class BulletReplicator : Replicator
{
    public override void OnAssembleData(in EntityQuery query, Writer writer, byte LOD = 100)
    {
        writer.Write(query.ToComponentDataArray<Position>(Allocator.Temp));
        writer.Write(query.ToComponentDataArray<Team>(Allocator.Temp));
        writer.Write(query.ToComponentDataArray<ColliderComp>(Allocator.Temp));
    }

    public override void OnCreate(out ComponentType[] types)
    {
        types = new ComponentType[4];
        types[0] = typeof(Bullet);
        types[1] = typeof(Position);
        types[2] = typeof(Team);
        types[3] = typeof(ColliderComp);
    }

    public override void OnDisassembleData(in UnsafeList<Entity> entities, Reader reader, byte LOD = 100)
    {
        var manager = World.EntityManager;
        var positions = reader.ReadArray<Position>();
        var teams = reader.ReadArray<Team>();
        var colliders = reader.ReadArray<ColliderComp>();

        for (int i = 0; i < entities.Length; i++) {
            var entity = entities[i];
            manager.SetComponentData(entity, positions[i]);
            manager.SetComponentData(entity, teams[i]);
            manager.SetComponentData(entity, colliders[i]);
        }
    }
}