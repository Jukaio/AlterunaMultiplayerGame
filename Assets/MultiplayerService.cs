using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Alteruna;
using Unity.Entities;
using Unity.Collections;

public struct Player : IComponentData
{
    public ushort index;
}

public struct LocalInfo : IComponentData
{
    public ushort userIndex;
    public FixedString64Bytes userName;
}

public class MultiplayerService : MonoBehaviour, IComponentData
{
    private Multiplayer multiplayer = null;
    public Multiplayer mp => multiplayer;


    private void Awake()
    {
        World.DefaultGameObjectInjectionWorld.EntityManager.CreateSingleton(this, "Multiplayer Service");
    }

    private void OnEnable()
    {
        multiplayer = GetComponent<Multiplayer>();
        multiplayer.Connected.AddListener(OnConnect);
        multiplayer.Disconnected.AddListener(OnDisconnect);
        multiplayer.NetworkError.AddListener(OnNetworkError);

        multiplayer.OtherUserJoined.AddListener(OnOtherUserJoin);
        multiplayer.OtherUserLeft.AddListener(OnOtherUserLeave);

        multiplayer.RoomJoined.AddListener(OnRoomJoin);
        multiplayer.RoomLeft.AddListener(OnRoomLeave);
        multiplayer.RoomListUpdated.AddListener(OnRoomUserListUpdate);
    }

    private void OnDisable()
    {
        multiplayer.Connected.RemoveListener(OnConnect);
        multiplayer.Disconnected.RemoveListener(OnDisconnect);
        multiplayer.NetworkError.RemoveListener(OnNetworkError);

        multiplayer.OtherUserJoined.RemoveListener(OnOtherUserJoin);
        multiplayer.OtherUserLeft.RemoveListener(OnOtherUserLeave);

        multiplayer.RoomJoined.RemoveListener(OnRoomJoin);
        multiplayer.RoomLeft.RemoveListener(OnRoomLeave);
        multiplayer.RoomListUpdated.RemoveListener(OnRoomUserListUpdate);
    }


    private void OnConnect(Multiplayer mp, Endpoint ep)
    {
        Debug.Log($"Conntected... {ep.Name}");
        LocalInfo localInfo = new();
        localInfo.userIndex = mp.Me.Index;
        localInfo.userName = new(mp.Me.Name);

        World.DefaultGameObjectInjectionWorld.EntityManager.CreateSingleton(localInfo, "Local Info");
    }

    private void OnDisconnect(Multiplayer mp, Endpoint ep)
    {
        Debug.Log($"Disconnected... {ep.Name}");
    }

    private void OnNetworkError(Multiplayer mp, Endpoint ep, int err)
    {
        Debug.LogError($"Error: {ep.Name} - {err}");
    }

    private void OnOtherUserJoin(Multiplayer mp, User user)
    {


        Debug.Log(user);

    }

    private void OnOtherUserLeave(Multiplayer mp, User user)
    {
        Debug.Log(user);
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

    // If player leaves room, just clear everything local and remote
    private void OnRoomLeave(Multiplayer mp)
    {
        if (World.DefaultGameObjectInjectionWorld == null) {
            return;
        }
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        if (manager == null) {
            return;
        }

        var query = manager.CreateEntityQuery(typeof(Local));
        foreach (var entity in query.ToEntityArray(Allocator.Temp)) {
            manager.DestroyEntity(entity);
        }

        query = manager.CreateEntityQuery(typeof(Remote));
        foreach (var entity in query.ToEntityArray(Allocator.Temp)) {
            manager.DestroyEntity(entity);
        }
    }

    private void OnRoomUserListUpdate(Multiplayer mp)
    {
        Debug.Log(mp);
    }

}

