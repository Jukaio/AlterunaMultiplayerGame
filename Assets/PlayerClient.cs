using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Alteruna;
using Unity.Collections;
using Unity.Mathematics;

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
        if(World.DefaultGameObjectInjectionWorld == null) {
            return;
        }
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        if(manager == null) {
            return;
        }

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

        //TODO so we have and issue with host and client ID managers not being synced.
        // when we join our ID manager looks like the hosts manager.
        // However on the hosts side when the joiner entity is added as a remote the ID manager has not yet been synced from the joiners game instance.
        // Meanign the remote on the hosts side will try to get the hosts last syncID  whihc is the one the host local player is already using.
        // likewise another problem occurs on the joining players world, the local player gets created first gaining a new syncID,
        // however now when it tries to ccreate the remotes it will not use the syncID of the host, it will use the last requested which is the local users index.
        // A probable solution for this would be to not have players use sync IDs and let them only use user indexes.
        // WHile for spawning projectiles and stuff we would have to do RPC calls for spawning where we forward the syncID of the spawned entity.

        //Antoher problem that we have is that the order of instantiation is weird.
        // If we are joining our player entity gets spawend first, then all remotes.
        // If we are hosting remotes get spawned before a new ID has been requested and synced
   

        var e = manager.CreateEntity(clientArchetype);
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
