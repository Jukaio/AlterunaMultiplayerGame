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

    [SerializeField]
    private Material team1Material;

    [SerializeField]
    private Material team2Material;

    [SerializeField]
    private Material team1BulletMaterial;

    [SerializeField]
    private Material team2BulletMaterial;

    public Mesh Mesh => quadMesh;
    public Material DefaultMaterial => defaultMaterial;
    public Material BulletMaterial => bulletMaterial;
    public Material Team1Material => team1Material;
    public Material Team2Material => team2Material;
    public Material Team1BulletMaterial => team1BulletMaterial;
    public Material Team2BulletMaterial => team2BulletMaterial;


    void Start()
    {
        var manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _ = manager.CreateSingleton(this, "Sprite Library");
    }
}
