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

    NativeHashMap<ID, Entity> m_EntityLookup;

    private void Awake()
    {
        m_EntityLookup = new NativeHashMap<ID, Entity>(1024, Allocator.Persistent);
    }

    private void OnDestroy()
    {
        m_EntityLookup.Dispose();
    }

    public override void AssembleData(Writer writer, byte LOD = 100)
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = manager.CreateEntityQuery(typeof(InputComp), typeof(Client));

        var entities = query.ToEntityArray(Allocator.Temp);
        var inputComps = query.ToComponentDataArray<InputComp>(Allocator.Temp);
        var clients = query.ToComponentDataArray<Client>(Allocator.Temp);

        writer.Write(entities);
        writer.Write(clients);
        writer.Write(inputComps);
    }

    public override void DisassembleData(Reader reader, byte LOD = 100)
    {
        var entites = reader.Read<Entity>();
        var clients = reader.Read<Client>();
        var inputComps = reader.Read<InputComp>();

        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        for (int i = 0; i < entites.Length; i++)
        {
            //TODO we do this ID thing to make sure that we get the correct entites on each client, however Right now I think there is a problem.
            //mostlikley we dont actually populate the ID hashmap which means, we never find the correct entity and component
            Entity oEntity;
            ID tempID;
            tempID.entityIndex = entites[i].Index;
            tempID.client = clients[i].index;

            if (m_EntityLookup.TryGetValue(tempID, out oEntity))
            {
                manager.SetComponentData(oEntity, inputComps[i]);
            }
        }
    }

    private void Update()
    {
        Commit();
        base.SyncUpdate();
    }

}
