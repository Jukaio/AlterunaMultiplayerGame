using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Alteruna;
using Unity.Entities;
using Unity.Collections;



public class ClientToEntityTranslator : MonoBehaviour, IComponentData
{
    public NativeHashMap<ushort,Entity> ClientEntityMap;

    void Start()
    {
        ClientEntityMap = new NativeHashMap<ushort, Entity>(1024, Allocator.Persistent);
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _ = manager.CreateSingleton(this, "Client To Entity Translator");
    }

    private void OnDestroy()
    {
        ClientEntityMap.Dispose();
    }
}
