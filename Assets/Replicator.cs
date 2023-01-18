using Alteruna;
using Unity.Entities;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public struct Local : IComponentData
{

}

public struct Remote : IComponentData
{

}

// TODO: Register to multiplayer events to clean up data of users 
public abstract class Replicator : Synchronizable
{
    private NativeArray<UnsafeList<Entity>> pools;

    private ComponentType[] types;
    private EntityQuery localQuery;
    private EntityQuery remoteQuery;

    protected World World => World.DefaultGameObjectInjectionWorld;
    protected EntityManager Manager => World.EntityManager;
    protected EntityQuery RemoteQuery => remoteQuery;


    private void Start()
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;

        pools = new(64, Allocator.Persistent);
        OnCreate(out types);

        var localTypes = new ComponentType[types.Length + 1];
        var remoteTypes = new ComponentType[types.Length + 1];
        for(int i = 0; i < types.Length; i++) {
            localTypes[i] = types[i];
            remoteTypes[i] = types[i];
        }
        localTypes[localTypes.Length - 1] = typeof(Local);
        remoteTypes[remoteTypes.Length - 1] = typeof(Remote);

        types = remoteTypes;

        localQuery = manager.CreateEntityQuery(localTypes);
        remoteQuery = manager.CreateEntityQuery(remoteTypes);
    }


    private void OnDestroy()
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        foreach (var collection in pools) {
            foreach(var entity in collection) {
                manager.DestroyEntity(entity);
            }
            if (collection.IsCreated) {
                collection.Dispose();
            }
        }
        pools.Dispose();
    }

    public abstract void OnCreate(out ComponentType[] types);
    public abstract void OnAssembleData(in EntityQuery query, Writer writer, byte LOD = 100);
    public abstract void OnDisassembleData(in UnsafeList<Entity> entities, Reader reader, byte LOD = 100);


    public override void AssembleData(Writer writer, byte LOD = 100)
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var localInfoQuery = manager.CreateEntityQuery(typeof(LocalInfo));
        var localInfo = localInfoQuery.GetSingleton<LocalInfo>();

        writer.Write(localInfo.userIndex);

        writer.Write(localQuery.CalculateEntityCount());

        OnAssembleData(localQuery, writer, LOD);
    }

    public override void DisassembleData(Reader reader, byte LOD = 100)
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var userIndex = reader.ReadUshort();

        var totalCount = reader.ReadInt();

        if (!pools[userIndex].IsCreated) {
            pools[userIndex] = new(32, AllocatorManager.Persistent);
        }

        var differenceForCreation = totalCount - pools[userIndex].Length;
        for (var i = 0; i < differenceForCreation; i++) {
            var entity = manager.CreateEntity(types);
            var list = pools[userIndex];
            list.Add(entity);
            pools[userIndex] = list;
        }

        var differenceForDeletion = pools[userIndex].Length - totalCount;
        for (var i = 0; i < differenceForDeletion; i++) {
            var list = pools[userIndex];
            var entity = list[(list.Length - 1)];
            manager.DestroyEntity(entity);
            list.Resize(list.Length - 1);
            pools[userIndex] = list;

        }

        OnDisassembleData(pools[userIndex], reader, LOD);
    }

    private void Update()
    {
        Commit();
        SyncUpdate();
    }
}
