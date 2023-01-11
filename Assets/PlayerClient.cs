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

        EntityArchetype clientArchetype;
        //this is how you check if the user is a local player
        if (avatar.IsMe)
        {
            clientArchetype = manager.CreateArchetype(
                typeof(Position),
                typeof(Client),
                typeof(Velocity),
                typeof(InputComp),
                typeof(LocalPlayer));  
        }
        else
        {
            clientArchetype = manager.CreateArchetype(
                typeof(Position),
                typeof(Client),
                typeof(Velocity),
                typeof(InputComp));
        }

        var e = manager.CreateEntity(clientArchetype);
        manager.SetComponentData(e, new Client { index = user.Index });
        this.entity = e;


    }

}
