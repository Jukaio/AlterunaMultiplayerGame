using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpriteLibrary : MonoBehaviour, IComponentData
{
    [SerializeField]
    private Mesh quadMesh;

    [SerializeField]
    private Material defaultMaterial;

    [SerializeField]
    private Material bulletMaterial;

    public Mesh Mesh => quadMesh;
    public Material DefaultMaterial => defaultMaterial;
    public Material BulletMaterial => bulletMaterial;


    void Start()
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _ = manager.CreateSingleton(this, "Sprite Library");
    }
}
