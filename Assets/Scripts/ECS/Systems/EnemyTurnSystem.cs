using JetBrains.Annotations;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class EnemyTurnSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        //// browsing turn(must be fixed)
        //Entities.WithAll<TurnTimer>().ForEach((ref Translation trans, ref Rotation rot, ref MoveForward moveForward, ref TurnTimer turnTimer) =>
        //{
        //    turnTimer.TimerCounter -= Time.DeltaTime;
        //    // on timer out change rotationt
        //    if (turnTimer.TimerCounter <= 0)
        //    {
        //        turnTimer.TimerCounter = SharedMethods.MakeRandom(turnTimer.TimeRange, turnTimer.TimeRange + "timer");

        //        rot.Value = quaternion.Euler(math.mul(rot.Value, new float3(0, SharedMethods.MakeRandom(-90, 90, "rotat"), 0)));
        //    }
        //});

        Entities.WithAll<BehaviourState>().ForEach((
            ref BehaviourState behaviourState,
            ref TurnTimer turnTimer,
            ref Rotation rot,
            ref ChaseTarget chaseTarget,
            ref Translation tran
            ) =>
        {
            switch (behaviourState.Value)
            {
                case ProjectEnums.BehaviourState.Chasing:
                    // chasing turn
                    if (World.DefaultGameObjectInjectionWorld.EntityManager.Exists(chaseTarget.Value))
                    {
                        Translation targetTranslation = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Translation>(chaseTarget.Value);

                        rot.Value = quaternion.LookRotation(targetTranslation.Value - tran.Value, new float3(0, 1, 0));
                    }

                    turnTimer.TimerCounter = 0;
                    break;

                case ProjectEnums.BehaviourState.Patrolling:
                    // patroling turn
                    turnTimer.TimerCounter -= Time.DeltaTime;
                    // on timer out change rotationt
                    if (turnTimer.TimerCounter <= 0)
                    {
                        turnTimer.TimerCounter = SharedMethods.MakeRandom(turnTimer.TimeRange, turnTimer.TimeRange + "timer");

                        rot.Value = quaternion.Euler(math.mul(rot.Value, new float3(0, SharedMethods.MakeRandom(-90, 90, "rotat"), 0)));
                    }
                    break;
            }
        });


    }
}
