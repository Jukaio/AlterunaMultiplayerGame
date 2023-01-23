using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public partial struct ScreenWrapSystem : ISystem
{
    EntityQuery query;
    ComponentLookup<Position> positions;

    public void OnCreate(ref SystemState state)
    {
        query = new EntityQueryBuilder(Allocator.Temp)
            .WithAllRW<Position>()
            .WithAll<Player, Local>()
            .Build(ref state);
        positions = state.GetComponentLookup<Position>(false);
    }

    public void OnDestroy(ref SystemState state)
    {
      
    }

    public void OnUpdate(ref SystemState state)
    {
        var manager = state.EntityManager;

        var entities = query.ToEntityArray(Allocator.Temp);
        positions.Update(ref state);

        foreach (var entity in entities)
        {
            var currentPos = positions[entity].value;
            float3 posToSet = currentPos;

            if(currentPos.x > 7.5f)
            {
                posToSet.x = -7f;
            }
            else if(currentPos.x < -7.5f)
            {
                posToSet.x = 7f;
            }

            if (currentPos.y > 6.6f)
            {
                posToSet.y = -4.5f;
            }
            else if (currentPos.y < -4.7f)
            {
                posToSet.y = 6.4f;
            }

            positions[entity] = new Position { value = posToSet };

        }
    }
}
