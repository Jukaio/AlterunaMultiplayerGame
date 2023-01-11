using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;
using Unity.Entities;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;


public class InputReplicationSystem : Synchronizable
{
    public override void AssembleData(Writer writer, byte LOD = 100)
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = manager.CreateEntityQuery(typeof(InputComp));
        var entities = query.ToEntityArray(Allocator.Temp);
        var inputComps= query.ToComponentDataArray<InputComp>(Allocator.Temp);
        var entitesAsBytes = entities.Reinterpret<byte>(UnsafeUtility.SizeOf<byte>()).ToArray();
        var inputCompsAsBytes = inputComps.Reinterpret<byte>(UnsafeUtility.SizeOf<byte>()).ToArray();
        writer.Write(entitesAsBytes);
        writer.Write(inputCompsAsBytes);
    }

    public override void DisassembleData(Reader reader, byte LOD = 100)
    {
        var entitesAsBytes = reader.ReadByteArray();
        var inputCompsAsBytes = reader.ReadByteArray();
        NativeArray<byte> nativeEntitesAsBytes = new NativeArray<byte>(entitesAsBytes, Allocator.Temp);
        NativeArray<byte> nativeInputCompsAsBytes = new NativeArray<byte>(inputCompsAsBytes, Allocator.Temp);
        var entites = nativeEntitesAsBytes.Reinterpret<Entity>(UnsafeUtility.SizeOf<Entity>());
        var inputComps = nativeInputCompsAsBytes.Reinterpret<InputComp>(UnsafeUtility.SizeOf<InputComp>());

        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        for (int i = 0; i < entites.Length; i++)
        {
            manager.SetComponentData(entites[i], inputComps[i]);
        }
    }

}
