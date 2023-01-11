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
        throw new System.NotImplementedException();
    }

    public override void DisassembleData(Reader reader, byte LOD = 100)
    {
        throw new System.NotImplementedException();
    }
}
