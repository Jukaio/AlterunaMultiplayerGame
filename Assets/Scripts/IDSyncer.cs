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
    IDManager m_IDManager;


    //TODO make sure that the member variables are synced
    public override void AssembleData(Writer writer, byte LOD = 100)
    {
        var manager = m_IDManager;
        writer.Write(manager.m_AvailableIDs.ToArray(Allocator.Temp));
        writer.Write(manager.m_InUseIDs.ToNativeArray(Allocator.Temp));
        writer.Write(manager.m_LastRequestID);
    }

    public override void DisassembleData(Reader reader, byte LOD = 100)
    {
        var available = reader.Read<uint>();
        var inUse = reader.Read<uint>();
        var last = reader.ReadUint();

        var manager = m_IDManager;

        manager.m_AvailableIDs = new NativeQueue<uint>();
        foreach (var item in available)
        {
            manager.m_AvailableIDs.Enqueue(item);
        }

        manager.m_InUseIDs = new NativeHashSet<uint>();
        foreach (var item in inUse)
        {
            manager.m_InUseIDs.Add(item);
        }

        manager.m_LastRequestID = last;
    }

    //Populates the queue with all available IDs
    void Start()
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = manager.CreateEntityQuery(typeof(IDManager));
        m_IDManager = query.GetSingleton<IDManager>();
    }

    void OnDestroy()
    {
        
    }

    void Update()
    {
        var manager = m_IDManager;
        
        if (manager.m_OLDLastRequestID != manager.m_LastRequestID ||
           manager.m_OLDLastReleasedID != manager.m_LastReleasedID)
        {
            manager.m_OLDLastRequestID = manager.m_LastRequestID;
            manager.m_OLDLastReleasedID = manager.m_LastReleasedID;

            Commit();
        }
        base.SyncUpdate();
    }

}
