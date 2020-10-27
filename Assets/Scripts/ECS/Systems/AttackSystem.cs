﻿using Unity.Mathematics;
using Unity.Transforms;
using Unity.Entities;

public class AttackSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<BehaviourStateAttacking>().ForEach((
            Entity entity,
            ref Translation translation,
            ref Rotation rotation,
            ref BehaviourStateAttacking behaviourStateAttacking,
            ref Turning turning,
            ref ShootTimer shootTimer
            ) =>
        {
            float3 myPos = translation.Value;
            quaternion myRot = rotation.Value;
            float turningSpeed = turning.TurningSpeed;

            if (EntityManager.HasComponent(behaviourStateAttacking.AttackTarget, typeof(Translation)))
            {
                float3 turgetPoint = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Translation>(behaviourStateAttacking.AttackTarget).Value;

                // rotation
                rotation.Value = SharedMethods.RotateTowardsTarget(myPos, myRot, turgetPoint, turningSpeed, Time.DeltaTime);

                // shooting 
                shootTimer.TimerCounter -= Time.DeltaTime;
                shootTimer.TimerCounter = shootTimer.TimerCounter < 0 ? 0 : shootTimer.TimerCounter;

                if (shootTimer.TimerCounter <= 0)
                {
                    shootTimer.TimerCounter = SharedMethods.MakeRandom(shootTimer.TimeRange, shootTimer.TimeRange + "timer");

                    Entity bulletEntity = EntityManager.Instantiate(BulletSpawner.instance.BulletEntityPrefab);

                    float3 bulletPos = translation.Value + math.mul(rotation.Value, new float3(0f, 0f, 1.5f));
                    EntityManager.SetComponentData(bulletEntity, new Translation { Value = bulletPos });
                    EntityManager.SetComponentData(bulletEntity, new Rotation { Value = rotation.Value });
                }
            }
        });
    }
}