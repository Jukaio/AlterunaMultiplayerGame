using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;
using Unity.Entities;
using Unity.Collections;
using Unity.VisualScripting.FullSerializer;
using Unity.Collections.LowLevel.Unsafe;

public struct Local : IComponentData
{

}

public struct Remote : IComponentData
{

}

public class PlayerSynchroniser : Synchronizable
{
    [SerializeField]
    public EntityArchetype archetypes;

    private NativeArray<UnsafeList<Entity>> userPlayers;

    private void Start()
    {
        // 64 players is hard limit
        userPlayers = new(64, Allocator.Persistent);

        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = manager.CreateEntityQuery(typeof(MultiplayerService));
        var service = query.GetSingleton<MultiplayerService>();
    }

    private void OnRoomJoin(Multiplayer mp, Room room, User user)
    {
        LocalInfo localInfo = new();
        localInfo.userIndex = mp.Me.Index;
        localInfo.userName = new(mp.Me.Name);

        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = manager.CreateEntityQuery(typeof(LocalInfo));
        query.SetSingleton(localInfo);

        Debug.Log(user);
    }

    private void OnDestroy()
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var query = manager.CreateEntityQuery(typeof(MultiplayerService));
        var service = query.GetSingleton<MultiplayerService>();

        foreach (var collection in userPlayers) {
            if (collection.IsCreated) {
                collection.Dispose();
            }
        }
        userPlayers.Dispose();
    }

    public override void AssembleData(Writer writer, byte LOD = 100)
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var playerQuery = manager.CreateEntityQuery(typeof(Player), typeof(Position), typeof(Local));

        var localInfoQuery = manager.CreateEntityQuery(typeof(LocalInfo));
        var localInfo = localInfoQuery.GetSingleton<LocalInfo>();
        writer.Write(localInfo.userIndex);

        writer.Write(playerQuery.ToComponentDataArray<Position>(Allocator.Temp));
        writer.Write(playerQuery.ToComponentDataArray<Rotation>(Allocator.Temp));
    }

    public override void DisassembleData(Reader reader, byte LOD = 100)
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;

        var userIndex = reader.ReadUshort();
        var positions = reader.ReadArray<Position>();
        var rotations = reader.ReadArray<Rotation>();


        var totalCount = positions.Length;
        var differenceForCreation = totalCount - userPlayers[userIndex].Length;
        for(var i = 0; i < differenceForCreation; i++) {
            var entity = manager.CreateEntity(typeof(Position), typeof(Rotation), typeof(Remote));
            var list = userPlayers[userIndex];
            list.Add(entity);
            userPlayers[userIndex] = list;
        }
        if (!userPlayers[userIndex].IsCreated) {
            userPlayers[userIndex] = new(32, AllocatorManager.Persistent);
        }

        var differenceForDeletion = userPlayers[userIndex].Length - totalCount;
        for (var i = 0; i < differenceForDeletion; i++) {
            var list = userPlayers[userIndex];
            var entity = list[(list.Length - 1)];
            manager.DestroyEntity(entity);
            list.Resize(list.Length - 1);
            userPlayers[userIndex] = list;

        }

        for (int i = 0; i < positions.Length; i++) {
            var entity = userPlayers[userIndex][i];
            manager.AddComponent(entity, typeof(Player));
            manager.RemoveComponent(entity, typeof(Bullet));
            manager.SetComponentData(entity, positions[i]);
            manager.SetComponentData(entity, rotations[i]);
        }
    }

    private void Update()
    {
        Commit();
        SyncUpdate();
    }
}