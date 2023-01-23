using Alteruna;
using Unity.Entities;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using System;
using UnityEngine;
using Unity.VisualScripting;

public interface IMessage
{
    public void Read(Reader reader, int LOD);
    public void Write(Writer writer, int LOD);
}


public abstract class Messenger<MessageDataType> : Synchronizable where MessageDataType : unmanaged, IMessage
{
    private struct Message
    {
        public int id;
        public float clearAt;
        public MessageDataType data;
    }
    private struct Ack : IEquatable<Ack>
    {
        public float clearAt;
        public int messageId;
        public int userId;

        public bool Equals(Ack other)
        {
            return this.messageId == other.messageId && this.userId == other.userId;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(messageId, userId);
        }

        public override string ToString()
        {
            return $"MessageID: {messageId} - UserID: {userId} - TimeToClear: {clearAt}";
        }

        public static bool operator ==(Ack left, Ack right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Ack left, Ack right)
        {
            return !(left == right);
        }
    }

    private int uniqueIDCounter = 0;
    private UnsafeRingQueue<Message> queue;
    private UnsafeHashMap<int, Message> sending;

    private UnsafeHashMap<int, BitField64> acknowledgements;

    private UnsafeHashSet<Ack> received;

    public Multiplayer multiplayer;
    public Room Room => multiplayer.CurrentRoom;
    public User Me => multiplayer.Me;
    public LocalInfo LocalInfo => localInfoQuery.GetSingleton<LocalInfo>();
    public int connectedUsers => Room.Users.Count;
    private EntityQuery localInfoQuery;
    protected World World => World.DefaultGameObjectInjectionWorld;
    protected EntityManager Manager => World.EntityManager;

    const float MESSAGE_TIMEOUT = 1.0f;
    const float ACK_TIMEOUT = 1.25f;
    public abstract void OnReceive(int user, MessageDataType data);


    private void Start()
    {
        queue = new UnsafeRingQueue<Message>(1024, Allocator.Persistent);
        sending = new(1024, Allocator.Persistent);
        received = new UnsafeHashSet<Ack>(1024, Allocator.Persistent);
        acknowledgements = new(1024, Allocator.Persistent);

        var manager = Manager;
        var mp = manager.CreateEntityQuery(typeof(MultiplayerService));
        multiplayer = mp.GetSingleton<MultiplayerService>().mp;
        localInfoQuery = manager.CreateEntityQuery(typeof(LocalInfo));
    }


    private void OnDestroy()
    {
        queue.Dispose();
        sending.Dispose();
        received.Dispose();
        acknowledgements.Dispose();

        base.OnDestroy();
    }

    public void Notify(MessageDataType data, bool notifyLocally=true)
    {
        Message message = new();
        message.id = uniqueIDCounter;
        message.data = data;
        message.clearAt = Time.realtimeSinceStartup + MESSAGE_TIMEOUT;
        uniqueIDCounter++;
        queue.Enqueue(message);

        if(notifyLocally)
        OnReceive(LocalInfo.userIndex ,data);
    }

    public override void AssembleData(Writer writer, byte LOD = 100)
    {
        var localInfo = LocalInfo;

        writer.Write(localInfo.userIndex);

        while (!queue.IsEmpty) {
            var message = queue.Dequeue();
            sending.Add(message.id, message);
            acknowledgements.Add(message.id, new BitField64(0));
        }

        // Message count
        writer.Write(sending.Count);
        foreach (var item in sending) {
            writer.Write(item.Value.id);
            var data = item.Value.data;
            data.Write(writer, LOD);
        }

        // Acknowledge
        writer.Write(received.Count);
        foreach (var item in received) {
            writer.Write(item.messageId);
            writer.Write(item.userId);
        }
    }

    public override void DisassembleData(Reader reader, byte LOD = 100)
    {
        var localInfo = LocalInfo;

        var userIndex = reader.ReadUshort();

        // queue acknowledgements
        var messageCount = reader.ReadInt();
        for (int i = 0; i < messageCount; i++) {
            var id = reader.ReadInt();
            // In this case we read the data, even if we do not need to
            // Later on, it is possible to override the serializer and skip memory
            MessageDataType dataType = new();
            dataType.Read(reader, LOD);
            var ack = new Ack { messageId = id, userId = userIndex, clearAt = Time.realtimeSinceStartup + ACK_TIMEOUT };
            if (!received.Contains(ack)) {
                // On Message read?
                OnReceive(userIndex, dataType);
                received.Add(ack);
            }
        }

        // verify Acknowledgements
        var ackCount = reader.ReadInt();
        for (int i = 0; i < ackCount; i++) {
            var messageId = reader.ReadInt();
            var userId = reader.ReadInt();
            // if is me
            if (userId == localInfo.userIndex) {
                var bits = acknowledgements[messageId];
                bits.SetBits(userIndex, true);
                acknowledgements[messageId] = bits;

                if(bits.CountBits() >= connectedUsers) {
                    acknowledgements.Remove(messageId);
                    sending.Remove(messageId);

                };
            }
        }
    }

    private void Update()
    {
        // Clean up timeouts
        {
            NativeList<int> toRemove = new(Allocator.Temp);
            foreach (var item in sending) {
                var current = item.Value;
                if (current.clearAt < Time.realtimeSinceStartup) {
                    toRemove.Add(item.Key);
                }
            }

            foreach (var index in toRemove) {
                sending.Remove(index);
            }
        }
        {
            NativeList<Ack> toRemove = new(Allocator.Temp);
            foreach (var ack in received) {
                if (ack.clearAt < Time.realtimeSinceStartup) {
                    toRemove.Add(ack);
                }
            }

            foreach (var ack in toRemove) {
                received.Remove(ack);
            }
        }

        Commit();
        SyncUpdate();
    }
}