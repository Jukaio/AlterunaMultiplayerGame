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
        if(!avatar.IsMe) {
            return;
        }
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var clientArchetype = manager.CreateArchetype(
            typeof(Position),
            typeof(Player),
            typeof(Velocity),
            typeof(Rotation),
            typeof(InputComp),
            typeof(SizeComp),
            typeof(ColliderComp),
            typeof(Momentum));
   

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
    }
}
