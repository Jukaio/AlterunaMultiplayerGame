using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Random = UnityEngine.Random;

public partial struct AsteroidMovementSystem : ISystem
{
    
    public void OnCreate(ref SystemState state)
    {
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach((TransformAspect transformAspect, RefRW<AsteroidTag> asteroid) in SystemAPI.Query<TransformAspect,RefRW<AsteroidTag>>())
        {
            if (asteroid.ValueRW.direction.x == 0 && asteroid.ValueRW.direction.y == 0)
            {
                SetAsteroidDir(ref state, asteroid);    
            }
            
            asteroid.ValueRW.lifeTime -= SystemAPI.Time.DeltaTime;
            transformAspect.LocalPosition += new float3( asteroid.ValueRW.direction.x * SystemAPI.Time.DeltaTime, asteroid.ValueRW.direction.y * SystemAPI.Time.DeltaTime,0);
        }

        
    }

    private void SetAsteroidDir(ref SystemState state, RefRW<AsteroidTag> asteroid)
    {
        asteroid.ValueRW.direction = new float2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    }
}
