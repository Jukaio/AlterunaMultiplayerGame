using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

struct Momentum : IComponentData
{
    public float value;
}

public partial struct InputToMomentumSystem : ISystem
{
    EntityQuery query;
    ComponentLookup<Velocity> velocities;
    ComponentLookup<InputComp> inputComps;
    ComponentLookup<Momentum> momentums;
    public void OnCreate(ref SystemState state)
    {
        query = new EntityQueryBuilder(Allocator.Temp)
            .WithAllRW<Velocity, InputComp>()
            .WithAllRW<Momentum>()
            .WithAll<Player, Local>()
            .Build(ref state);
        velocities = state.GetComponentLookup<Velocity>(false);
        inputComps = state.GetComponentLookup<InputComp>(false);
        momentums = state.GetComponentLookup<Momentum>(false);
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        var manager = state.EntityManager;

        var entities = query.ToEntityArray(Allocator.Temp);
        velocities.Update(ref state);
        inputComps.Update(ref state);
        momentums.Update(ref state);

        const float SPEED = 3f;
        var dt = SystemAPI.Time.DeltaTime;
        foreach (var entity in entities)
        {
            var input = inputComps[entity];

            var speed = (input.Forward ? SPEED : input.Back ? -SPEED : 0f) * dt;

            var currentValue = momentums[entity].value;
            momentums[entity] = new Momentum { value = math.clamp(currentValue + speed, -1f, 1f) };


        }
    }
}

public partial struct MomentumDecelerationSystem : ISystem
{
    EntityQuery query;

    ComponentLookup<Momentum> momentums;
    ComponentLookup<InputComp> inputComps;
    public void OnCreate(ref SystemState state)
    {
        query = new EntityQueryBuilder(Allocator.Temp)
            .WithAllRW<Momentum, InputComp>()
            .WithAll<Player, Local>()
            .Build(ref state);

        momentums = state.GetComponentLookup<Momentum>(false);
        inputComps = state.GetComponentLookup<InputComp>(false);
    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        var manager = state.EntityManager;

        var entities = query.ToEntityArray(Allocator.Temp);
        momentums.Update(ref state);
        inputComps.Update(ref state);

        const float SPEED = 3f;
        var dt = SystemAPI.Time.DeltaTime;
        foreach (var entity in entities)
        {
            if (inputComps[entity].Forward || inputComps[entity].Back) { continue; }

            var currentValue = momentums[entity].value;
            float newValue = 0f;
            if (!Mathf.Approximately(currentValue, 0f))
            {
                if (currentValue > 0f)
                {
                    newValue = currentValue - (SPEED / 4f) * dt;
                }
                else if (currentValue < 0f)
                {
                    newValue = currentValue + (SPEED / 4f) * dt;
                }
            }
            newValue = math.clamp(CheckDirecitonFlip(newValue, currentValue), -1f, 1f);
            momentums[entity] = new Momentum { value = newValue };
        }
    }

    float CheckDirecitonFlip(float a, float b)
    {
        float result = 0;

        if ((a > 0f && b > 0f) || (a < 0f && b < 0f)) { result = a; }

        return result;
    }
}