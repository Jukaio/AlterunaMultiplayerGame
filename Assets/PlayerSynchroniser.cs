using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;
using Unity.Entities;
using Unity.Collections;
using Unity.VisualScripting.FullSerializer;

public struct Local : IComponentData
{

}

public struct Remote : IComponentData
{

}

public class PlayerSynchroniser : Synchronizable
{
    public override void AssembleData(Writer writer, byte LOD = 100)
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = manager.CreateEntityQuery(typeof(Position), typeof(Local));
        writer.Write(query.ToComponentDataArray<Position>(Allocator.Temp));
    }

    public override void DisassembleData(Reader reader, byte LOD = 100)
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = manager.CreateEntityQuery(typeof(Position), typeof(Remote));
       
        var positions = reader.Read<Position>();
        
        // This becomes valuable when we have more to sync :) 
        var entities = query.ToEntityListAsync(Allocator.Temp, out var handle);
        handle.Complete();
    
        if(entities.Length > positions.Length) {
            // Delete 
            var difference = entities.Length - positions.Length;
            for(int i = 0; i < difference; i++) {
                var backIndex = entities.Length - 1;
                var back = entities[backIndex];
                manager.DestroyEntity(back);
                entities.ResizeUninitialized(backIndex);
            }
        }
        else if(positions.Length > entities.Length) {
            var difference = positions.Length - entities.Length;
            // Create
            for(var i = 0; i < difference; i++) {
                var entity = manager.CreateEntity(typeof(Position), typeof(Remote));
                entities.Add(entity);
            }
        }

        for(int i = 0; i < entities.Length; i++) {
            manager.SetComponentData(entities[i], positions[i]);
        }
    }

    private void Update()
    {
        Commit();
        SyncUpdate();
    }
}