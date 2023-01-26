using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public partial class AsteroidDestroyerSystem : SystemBase
{
    

    protected override void OnCreate()
    {
    }

    protected override void OnUpdate()
    {
       /* var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        Entities.WithStructuralChanges().ForEach((Entity entity, AsteroidTag asteroid) =>
        {
            if (asteroid.lifeTime <= 0)
            {
                manager.DestroyEntity(entity);
            }
        }).Run();*/
    }
}
