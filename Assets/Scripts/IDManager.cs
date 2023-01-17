using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;



public class IDManager : MonoBehaviour, IComponentData
{
    public NativeQueue<uint> m_AvailableIDs;
    public NativeHashSet<uint> m_InUseIDs;


    public uint m_LastRequestID = 0;
    public uint m_OLDLastRequestID = 0;

    public uint m_LastReleasedID = 0;
    public uint m_OLDLastReleasedID = 0;

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



    void Start()
    {
        m_AvailableIDs = new NativeQueue<uint>(Allocator.Persistent);
        m_InUseIDs = new NativeHashSet<uint>(1024, Allocator.Persistent);

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
}
