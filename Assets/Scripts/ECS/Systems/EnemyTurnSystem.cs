using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class EnemyTurnSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithNone<ChaseTarget>().WithAll<BehaviourState>().ForEach((
             ref Turn turn,
             ref Rotation rot,
             ref Translation tran
             ) =>
        {
            turn.TimerCounter -= Time.DeltaTime;
            // on timer out change rotationt
            if (turn.TimerCounter <= 0)
            {
                float rnd = SharedMethods.MakeRandom(turn.TimeRange, turn.TimeRange + "random for turn timer", true);
                turn.TimerCounter = rnd;
                turn.TurgetRotation = quaternion.Euler(math.mul(rot.Value, new float3(0, SharedMethods.MakeRandom(-90, 90, "rotat"), 0)));
            }

            rot.Value = math.nlerp(rot.Value, turn.TurgetRotation, Time.DeltaTime * turn.RotationSpeed);
        });

        Entities.WithAll<ChaseTarget, BehaviourState>().ForEach((
             ref Turn turn,
             ref Rotation rot,
             ref ChaseTarget chaseTarget,
             ref Translation tran
             ) =>
         {
             // chasing and attack turn
             if (World.DefaultGameObjectInjectionWorld.EntityManager.Exists(chaseTarget.Turget))
             {
                 Translation targetTranslation = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Translation>(chaseTarget.Turget);

                 rot.Value = math.nlerp(rot.Value,
                    quaternion.LookRotation(targetTranslation.Value - tran.Value, new float3(0, 1, 0)),
                    Time.DeltaTime * turn.RotationSpeed);
             }

             turn.TimerCounter = 0;


         });


    }
}
