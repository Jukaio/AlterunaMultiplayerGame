using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;
using Unity.Entities;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public struct SyncID : IComponentData
{
    public uint value;
}

//TODO this class is intended to keep track of used IDs and sync them across game instances
//TODO when spawning stuff we should request an id that we then send with the RPC to all other remote clients when doing the spawn request
public class IDSyncer : Synchronizable, IComponentData
{

    NativeQueue<uint> m_AvailableIDs;
    NativeHashSet<uint> m_InUseIDs;


    uint m_LastRequestID = 0;
    uint m_OLDLastRequestID = 0;

    uint m_LastReleasedID = 0;
    uint m_OLDLastReleasedID = 0;

    public uint RequestID()
    {
        var id = m_AvailableIDs.Dequeue();
        m_InUseIDs.Add(id);
        m_LastRequestID = id;
        return id;
    }

    public void ReleaseID(uint id)
    {
        m_InUseIDs.Remove(id);
        m_AvailableIDs.Enqueue(id);
        m_LastReleasedID = id;
    }

    public uint GetLastRequestedID()
    {
        return m_LastRequestID;
    }

    //TODO make sure that the member variables are synced
    public override void AssembleData(Writer writer, byte LOD = 100)
    {
        writer.Write(m_AvailableIDs.ToArray(Allocator.Temp));
        writer.Write(m_InUseIDs.ToNativeArray(Allocator.Temp));
        writer.Write(m_LastRequestID);
    }

    public override void DisassembleData(Reader reader, byte LOD = 100)
    {
        var available = reader.Read<uint>();
        var inUse = reader.Read<uint>();
        var last = reader.ReadUint();


        m_AvailableIDs = new NativeQueue<uint>();
        foreach (var item in available)
        {
            m_AvailableIDs.Enqueue(item);
        }

        m_InUseIDs = new NativeHashSet<uint>();
        foreach (var item in inUse)
        {
            m_InUseIDs.Add(item);
        }

        m_LastRequestID = last;
    }

    //Populates the queue with all available IDs
    void Start()
    {
        m_AvailableIDs = new NativeQueue<uint>(Allocator.Persistent);
        m_InUseIDs = new NativeHashSet<uint>(1024,Allocator.Persistent);

        for (uint i = 0; i < 1024; ++i)
        {
            m_AvailableIDs.Enqueue(i);
        }
        m_OLDLastRequestID = m_LastRequestID;
        m_OLDLastReleasedID = m_LastReleasedID;

        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _ = manager.CreateSingleton(this, "ID Syncer");
    }

    void OnDestroy()
    {
        m_AvailableIDs.Dispose();
        m_InUseIDs.Dispose();
    }

    void Update()
    {
        if (m_OLDLastRequestID != m_LastRequestID ||
            m_OLDLastReleasedID != m_LastReleasedID)
        {
            m_OLDLastRequestID = m_LastRequestID;
            m_OLDLastReleasedID = m_LastReleasedID;

            Commit();
        }
        base.SyncUpdate();
    }

}
