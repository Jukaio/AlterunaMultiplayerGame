using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Alteruna;

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
        var clientArchetype = manager.CreateArchetype(typeof(Position), typeof(Player), typeof(Velocity));
        var e = manager.CreateEntity(clientArchetype);
        manager.AddComponentData(e, new Player { index = user.Index });
        manager.AddComponentData(e, new Velocity { value = new Unity.Mathematics.float3(0.0f, 0.0f, 0.0f) });
        if(avatar.IsMe) {
            manager.AddComponent<Local>(e);
        }
        else {
            manager.AddComponent<Remote>(e);
        }
        this.entity = e;
    }

}
