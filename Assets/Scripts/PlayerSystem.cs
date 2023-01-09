using Alteruna;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;


//public struct PlayerSystem : ISystem
//{
//    public void OnCreate(ref SystemState state)
//    {
//        var manager = state.EntityManager;

//        Debug.Log("Create");
//    }

//    public void OnDestroy(ref SystemState state)
//    {
//        Debug.Log("Destroy");
//    }

//    public void OnUpdate(ref SystemState state)
//    {
//        Debug.Log("Update");

//    }
//}


public partial class PlayerSystem : SystemBase
{

    protected override void OnCreate()
    {
        base.OnCreate();
        
    }

    public void OnConnect(Multiplayer multiplayer, Endpoint endpoint)
    {
        
    }

    protected override void OnUpdate()
    {
        
        Debug.Log("Update");

    }
}