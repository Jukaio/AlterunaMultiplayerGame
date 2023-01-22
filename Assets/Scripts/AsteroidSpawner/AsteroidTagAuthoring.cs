using Unity.Entities;
using UnityEngine;

public class AsteroidTagAuthoring : MonoBehaviour
{
    public float lifeTime;
}

public class AsteroidTagBaker : Baker<AsteroidTagAuthoring>
{
    public override void Bake(AsteroidTagAuthoring authoring)
    {
        AddComponent(new AsteroidTag{direction = 0, lifeTime = authoring.lifeTime});
    }
}
