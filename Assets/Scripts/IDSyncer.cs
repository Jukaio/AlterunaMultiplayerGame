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
    uint m_LastID = 0;

    public uint RequestID()
    {
        var id = m_AvailableIDs.Dequeue();
        m_InUseIDs.Add(id);
        m_LastID = id;
        return id;
    }

    public void ReleaseID(uint id)
    {
        m_InUseIDs.Remove(id);
        m_AvailableIDs.Enqueue(id);
    }

    public uint GetLastRequestedID()
    {
        return m_LastID;
    }

    //TODO make sure that themember variables are synced
    public override void AssembleData(Writer writer, byte LOD = 100)
    {
        throw new System.NotImplementedException();
    }

    public override void DisassembleData(Reader reader, byte LOD = 100)
    {
        throw new System.NotImplementedException();
    }

    //Populates the queue with all avilable IDs
    void Start()
    {
        m_AvailableIDs = new NativeQueue<uint>(Allocator.Persistent);

        for (uint i = 0; i < 1024; ++i)
        {
            m_AvailableIDs.Enqueue(i);
        }
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _ = manager.CreateSingleton(this, "ID Syncer");
    }

    void OnDestroy()
    {
        m_AvailableIDs.Dispose();
    }

    //TODO should only update if there are changes to the member variables
    void Update()
    {
        Commit();
        base.SyncUpdate();
    }

}
