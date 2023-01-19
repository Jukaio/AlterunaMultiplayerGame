using Unity.Entities;
using UnityEngine;
using Alteruna;
using JetBrains.Annotations;

public struct ExplosionMessage : IMessage
{
    public void Read(Reader reader, int LOD)
    {

    }

    public void Write(Writer writer, int LOD)
    {

    }
}


public class ExplosionMessenger : Messenger<ExplosionMessage>, IComponentData
{

    private void Awake()
    {
        Manager.CreateSingleton(this);
    }

    public override void OnReceive(int user, ExplosionMessage data)
    {
       
    }
}