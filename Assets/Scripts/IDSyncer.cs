using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;
using Unity.Entities;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;



//TODO this class is intended to keep track of used IDs and sync them across game instances
//TODO when spawning stuff we should request an id that we then send with the RPC to all other remote clients when doing the spawn request
public class IDSyncer : Synchronizable
{

    //TODO make sure that the member variables are synced
    public override void AssembleData(Writer writer, byte LOD = 100)
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = manager.CreateEntityQuery(typeof(IDManager));
        var idManager = query.GetSingleton<IDManager>();

        writer.Write(idManager.m_AvailableIDs.ToArray(Allocator.Temp));
        writer.Write(idManager.m_InUseIDs.ToNativeArray(Allocator.Temp));
        writer.Write(idManager.m_LastRequestID);
    }

    public override void DisassembleData(Reader reader, byte LOD = 100)
    {
        var available = reader.Read<uint>();
        var inUse = reader.Read<uint>();
        var last = reader.ReadUint();

        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = manager.CreateEntityQuery(typeof(IDManager));
        var idManager = query.GetSingleton<IDManager>();

        idManager.m_AvailableIDs = new NativeQueue<uint>(Allocator.Temp);
        foreach (var item in available)
        {
            idManager.m_AvailableIDs.Enqueue(item);
        }

        idManager.m_InUseIDs = new NativeHashSet<uint>(1024,Allocator.Temp);
        foreach (var item in inUse)
        {
            idManager.m_InUseIDs.Add(item);
        }

        idManager.m_LastRequestID = last;
    }

    //Populates the queue with all available IDs
    void Start()
    {
       
    }

    void OnDestroy()
    {
        
    }

    void Update()
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = manager.CreateEntityQuery(typeof(IDManager));
        var idManager = query.GetSingleton<IDManager>();

        if (idManager.m_OLDLastRequestID != idManager.m_LastRequestID ||
           idManager.m_OLDLastReleasedID != idManager.m_LastReleasedID)
        {
            idManager.m_OLDLastRequestID = idManager.m_LastRequestID;
            idManager.m_OLDLastReleasedID = idManager.m_LastReleasedID;

            Commit();
        }
        base.SyncUpdate();
    }

}
