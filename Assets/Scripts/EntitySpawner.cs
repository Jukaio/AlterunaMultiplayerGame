using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.InputSystem;
using Unity.Mathematics;

public class EntitySpawner : MonoBehaviour, IComponentData
{
    // Start is called before the first frame update
    void Start()
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _ = manager.CreateSingleton(this, "Entity Spawner");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //TODO check if this needs to be called on each instance via RPC
    public Entity SpawnEntity(uint syncID, EntityManager manager, EntityArchetype archetype)
    {

        var entity = manager.CreateEntity(archetype);
        manager.AddComponent(entity, typeof(SyncID));
        manager.SetComponentData(entity, new SyncID { value = syncID });
   
        var query = manager.CreateEntityQuery(typeof(IDToEntityTranslator));

        var arr = query.ToComponentArray<IDToEntityTranslator>();
        if (arr.Length != 0) 
        {
            var translator = arr[0];
            translator.IDEntityMap.Add(syncID, entity);
        }
       
        return entity;
    }

    //TODO this should remove entity with id from local IDEntityMap
    public void DestroyEntity()
    {
        //var arr = query.ToComponentArray<IDToEntityTranslator>();
        //if (arr.Length != 0)
        //{
        //    var translator = arr[0];
        //    translator.IDEntityMap.Remove(syncID);
        //}
    }
}
