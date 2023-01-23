using Unity.Entities;
using UnityEngine;
using Alteruna;
using JetBrains.Annotations;
using Unity.Mathematics;

public struct ExplosionMessage : IMessage
{
    public float3 Position;

    public ExplosionMessage(float3 position) { Position = position; }

    public void Read(Reader reader, int LOD)
    {
        Position = reader.ReadVector3();
    }

    public void Write(Writer writer, int LOD)
    {
        writer.Write(Position);
    }
}


public class ExplosionMessenger : Messenger<ExplosionMessage>, IComponentData
{
    public GameObject ExplosionVFX;
    private void Awake()
    {
        Manager.CreateSingleton(this);
    }

    public override void OnReceive(int user, ExplosionMessage data)
    {
        Instantiate(ExplosionVFX, data.Position, Quaternion.identity);
    }
}