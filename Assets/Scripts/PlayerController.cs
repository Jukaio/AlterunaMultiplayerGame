using System;
using Unity.Entities;
using UnityEngine;

public struct PlayerInput : IComponentData
{
    public float vertical;
    public float horizonal;
}


public class PlayerController : MonoBehaviour
{
    private float rotation;
    
    void Awake()
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        manager.CreateSingleton(new PlayerInput(), "PlayerInput");
    }

    void Update()
    {

        
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = manager.CreateEntityQuery(typeof(PlayerInput));

        rotation += Input.GetAxis("Horizontal");
        
        var input = new PlayerInput();
        input.vertical = Input.GetAxis("Vertical");
        input.horizonal = rotation % 360;
        query.SetSingleton(input);
    }
}