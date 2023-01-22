using Unity.Entities;
using UnityEngine;

public class AsteroidSpawnerAuthoring : MonoBehaviour
{
    public GameObject Prefab;
    public float SpawnRate;
}

class AsteroidSpawnerBaker : Baker<AsteroidSpawnerAuthoring>
{
    public override void Bake(AsteroidSpawnerAuthoring authoring)
    {
        AddComponent(new AsteroidSpawner
        {
            Prefab = GetEntity(authoring.Prefab),
            SpawnPosition = authoring.transform.position,
            NextSpawnTime = 0.0f,
            SpawnRate = authoring.SpawnRate
        });
    }

}
