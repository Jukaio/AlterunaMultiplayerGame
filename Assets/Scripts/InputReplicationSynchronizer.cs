using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;
using Unity.Entities;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;


public class InputReplicationSynchronizer : Synchronizable
{

    public override void AssembleData(Writer writer, byte LOD = 100)
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = manager.CreateEntityQuery(typeof(InputComp), typeof(Player));

        var entities = query.ToEntityArray(Allocator.Temp);
        var inputComps = query.ToComponentDataArray<InputComp>(Allocator.Temp);
        var clients = query.ToComponentDataArray<Player>(Allocator.Temp);

        writer.Write(entities);
        writer.Write(clients);
        writer.Write(inputComps);
    }

    public override void DisassembleData(Reader reader, byte LOD = 100)
    {

        var entites = reader.ReadArray<Entity>();
        var clients = reader.ReadArray<Player>();
        var inputComps = reader.ReadArray<InputComp>();

        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = manager.CreateEntityQuery(typeof(UserToEntityTranslator));
        var translator = query.GetSingleton<UserToEntityTranslator>();

        for (int i = 0; i < entites.Length; i++)
        {
            Entity oEntity;
            if (translator.ClientEntityMap.TryGetValue(clients[i].index, out oEntity))
            {
                manager.SetComponentData(oEntity, inputComps[i]);
            }
        }
    }

    private void Update()
    {
        Commit();
        base.SyncUpdate();
    }

}
