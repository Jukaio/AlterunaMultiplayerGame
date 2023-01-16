using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Alteruna;
using Unity.Entities;
using Unity.Collections;

//TODO whenever a new ID is used to spawn something thie should add a entry to the hashmap with the id and entity
public class IDToEntityTranslator : MonoBehaviour, IComponentData
{
    public NativeHashMap<uint, Entity> IDEntityMap;

    void Start()
    {
        IDEntityMap = new NativeHashMap<uint, Entity>(1024, Allocator.Persistent);
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _ = manager.CreateSingleton(this, "ID To Entity Translator");
    }

    private void OnDestroy()
    {
        IDEntityMap.Dispose();
    }
}
