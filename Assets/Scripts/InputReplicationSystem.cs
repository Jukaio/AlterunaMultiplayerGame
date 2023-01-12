using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;
using Unity.Entities;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;


public class InputReplicationSystem : Synchronizable
{
    struct ID : System.IEquatable<ID>
    {
        public ushort client;
        public int entityIndex;

        public bool Equals(ID other)
        {
            return client == other.client && entityIndex == other.entityIndex;
        }
    }

    NativeHashMap<ID, Entity> _entityLookup;

    private void Awake()
    {
        _entityLookup = new NativeHashMap<ID, Entity>(1024, Allocator.Persistent);
    }

    private void OnDestroy()
    {
        _entityLookup.Dispose();
    }

    public override void AssembleData(Writer writer, byte LOD = 100)
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = manager.CreateEntityQuery(typeof(InputComp), typeof(Player));

        var entities = query.ToEntityArray(Allocator.Temp);
        var inputComps = query.ToComponentDataArray<InputComp>(Allocator.Temp);
        var clients = query.ToComponentDataArray<Player>(Allocator.Temp);

        var entitesAsBytes = entities.Reinterpret<byte>(UnsafeUtility.SizeOf<byte>()).ToArray();
        var clientsAsBytes = clients.Reinterpret<byte>(UnsafeUtility.SizeOf<byte>()).ToArray();
        var inputCompsAsBytes = inputComps.Reinterpret<byte>(UnsafeUtility.SizeOf<byte>()).ToArray();

        writer.Write(entitesAsBytes);
        writer.Write(clientsAsBytes);
        writer.Write(inputCompsAsBytes);
    }

    public override void DisassembleData(Reader reader, byte LOD = 100)
    {
        var entitesAsBytes = reader.ReadByteArray();
        NativeArray<byte> nativeEntitesAsBytes = new NativeArray<byte>(entitesAsBytes, Allocator.Temp);
        var entites = nativeEntitesAsBytes.Reinterpret<Entity>(UnsafeUtility.SizeOf<Entity>());

        var clientsAsBytes = reader.ReadByteArray();
        NativeArray<byte> nativeClientsAsBytes = new NativeArray<byte>(clientsAsBytes, Allocator.Temp);
        var clients = nativeClientsAsBytes.Reinterpret<Player>(UnsafeUtility.SizeOf<Player>());

        var inputCompsAsBytes = reader.ReadByteArray();
        NativeArray<byte> nativeInputCompsAsBytes = new NativeArray<byte>(inputCompsAsBytes, Allocator.Temp);
        var inputComps = nativeInputCompsAsBytes.Reinterpret<InputComp>(UnsafeUtility.SizeOf<InputComp>());

        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        for (int i = 0; i < entites.Length; i++)
        {
            Entity oEntity;
            ID tempID;
            tempID.entityIndex = entites[i].Index;
            tempID.client = clients[i].index;

            if (_entityLookup.TryGetValue(tempID, out oEntity))
            {
                manager.SetComponentData(oEntity, inputComps[i]);
            }
        }
    }

}
