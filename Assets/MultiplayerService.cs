using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Alteruna;
using Unity.Entities;
using Unity.Collections;

public struct Client : IComponentData
{
    public ushort index;
}

public struct LocalInfo : IComponentData
{
    public ushort index;
}

public class MultiplayerService : MonoBehaviour
{
    private Multiplayer multiplayer = null;

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

    private void CreateUser(ushort index)
    {
        //Debug.Log(ep);
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var clientArchetype = manager.CreateArchetype(typeof(Position), typeof(Client));
        var e = manager.CreateEntity(clientArchetype);
        manager.AddComponentData(e, new Client { index = index });
        Debug.Log(manager.GetComponentData<Client>(e).index);
        
    }

    private void OnConnect(Multiplayer mp, Endpoint ep)
    {
        Debug.Log($"Conntected... {ep.Name}");
        LocalInfo localInfo = new();
        localInfo.index = mp.Me.Index;

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
        Debug.Log(user);
    }

    private void OnRoomLeave(Multiplayer mp)
    {
        Debug.Log(mp);
    }

    private void OnRoomUserListUpdate(Multiplayer mp)
    {
        Debug.Log(mp);
    }

}

