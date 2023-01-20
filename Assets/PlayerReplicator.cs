
using Alteruna;
using Unity.Entities;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;


public class PlayerReplicator : Replicator
{
    public override void OnAssembleData(in EntityQuery query, Writer writer, byte LOD = 100)
    {
        writer.Write(query.ToComponentDataArray<Position>(Allocator.Temp));
        writer.Write(query.ToComponentDataArray<Rotation>(Allocator.Temp));
        writer.Write(query.ToComponentDataArray<ColliderComp>(Allocator.Temp));
        writer.Write(query.ToComponentDataArray<InputComp>(Allocator.Temp));
        writer.Write(query.ToComponentDataArray<Momentum>(Allocator.Temp));
        writer.Write(query.ToComponentDataArray<Team>(Allocator.Temp));
    }

    public override void OnCreate(out ComponentType[] types)
    {
        types = new ComponentType[7];
        types[0] = typeof(Player);
        types[1] = typeof(Position);
        types[2] = typeof(Rotation);
        types[3] = typeof(ColliderComp);
        types[4] = typeof(InputComp);
        types[5] = typeof(Momentum);
        types[6] = typeof(Team);
    }

    public override void OnDisassembleData(in UnsafeList<Entity> entities, Reader reader, byte LOD = 100)
    {
        var manager = World.EntityManager;
        var positions = reader.ReadArray<Position>();
        var rotations = reader.ReadArray<Rotation>();
        var colliders = reader.ReadArray<ColliderComp>();
        var inputComps = reader.ReadArray<InputComp>();
        var momentums = reader.ReadArray<Momentum>();
        var teams = reader.ReadArray<Team>();

        for (int i = 0; i < entities.Length; i++) {
            var entity = entities[i];
            manager.SetComponentData(entity, positions[i]);
            manager.SetComponentData(entity, rotations[i]);
            manager.SetComponentData(entity, colliders[i]);
            manager.SetComponentData(entity, inputComps[i]);
            manager.SetComponentData(entity, momentums[i]);
            manager.SetComponentData(entity, teams[i]);
        }
    }
}
