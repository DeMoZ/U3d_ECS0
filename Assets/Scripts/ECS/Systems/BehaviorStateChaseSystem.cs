using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class BehaviorStateChaseSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        // for all with no valid target
        Entities.WithAll<ChaseTarget, BehaviourState>().ForEach((
           Entity entity,
            ref Translation tran,
            ref ChaseTarget chaseTarget,
            ref TeamTag teamTag,
            ref BehaviourState behaviourState,
            ref AttackDistance attackDistance) =>
        {
            if (behaviourState.Value == ProjectEnums.BehaviourState.Chasing)
            {
                if (!World.DefaultGameObjectInjectionWorld.EntityManager.Exists(chaseTarget.Target))
                {
                    behaviourState.Value = ProjectEnums.BehaviourState.Patrolling;
                    //PostUpdateCommands.RemoveComponent(entity, typeof(ChaseTarget));
                    EntityManager.RemoveComponent(entity, typeof(ChaseTarget));
                }
                else
                {
                    float3 myPos = tran.Value;

                    if (CanAttackTarget(chaseTarget.Target, myPos, attackDistance.Value))
                        behaviourState.Value = ProjectEnums.BehaviourState.Attack;
                }
            }
        });
    }

    private bool CanAttackTarget(Entity targetEntity, float3 myPos, float attackDistance)
    {
        Translation targetTranslation = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Translation>(targetEntity);
        if (math.distance(myPos, targetTranslation.Value) <= attackDistance)
            return true;

        return false;
    }
}
