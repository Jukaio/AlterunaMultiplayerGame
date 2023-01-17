using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Alteruna;
using Unity.Collections;


public class PlayerClient : MonoBehaviour
{
    private Alteruna.Avatar avatar = null;
    private Entity entity = Entity.Null;

    private void Awake()
    {
        avatar = GetComponent<Alteruna.Avatar>();
    }

    private void OnEnable()
    {
        avatar.OnPossessed.AddListener(OnPossess);
    }

    private void OnDestroy()
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        manager.DestroyEntity(entity);
        entity = Entity.Null;
    }

    public void OnPossess(User user)
    {
        CreateUser(user);

    }

    private void CreateUser(User user)
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var clientArchetype = manager.CreateArchetype(
            typeof(Position),
            typeof(Player),
            typeof(Velocity),
            typeof(Rotation),
            typeof(InputComp),
            typeof(SizeComp),
            typeof(ColliderComp));


        //var spawnQuery = manager.CreateEntityQuery(typeof(EntitySpawner));
        //var spawner = spawnQuery.GetSingleton<EntitySpawner>();

        //var syncQuery = manager.CreateEntityQuery(typeof(IDSyncer));
        //var syncer = syncQuery.GetSingleton<IDSyncer>();
        //uint syncID;

        //if (avatar.IsMe)
        //{
        //    syncID = syncer.RequestID();
        //}
        //else
        //{
        //    syncID = syncer.GetLastRequestedID();
        //}

        var e = manager.CreateEntity(clientArchetype);//spawner.SpawnEntity(syncID, manager, clientArchetype); //manager.CreateEntity(clientArchetype);
        manager.SetComponentData(e, new Player { index = user.Index });
        manager.SetComponentData(e, new Velocity { value = new Unity.Mathematics.float3(0.0f, 0.0f, 0.0f) });
        manager.SetComponentData(e, new Rotation { value = 0.0f });
        manager.SetComponentData(e, new SizeComp { size = 1f });

        if (avatar.IsMe)
        {
            manager.AddComponent<Local>(e);
        }
        else
        {
            manager.AddComponent<Remote>(e);
        }
        this.entity = e;

        //Adds the new entity to the local translator hashmap with the key of user index 
        var translatorQuery = manager.CreateEntityQuery(typeof(UserToEntityTranslator));
        var translator = translatorQuery.GetSingleton<UserToEntityTranslator>();
        translator.ClientEntityMap.Add(user.Index, e);
    }

}
