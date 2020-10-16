using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class BehaviorStateAttackSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<ChaseTarget, BehaviourState>().ForEach((
            Entity entity,
            ref Translation tran,
            ref ChaseTarget chaseTarget,
            ref TeamTag teamTag,
            ref BehaviourState behaviourState,
            ref AttackDistance attackDistance) =>
        {

            // if (!World.DefaultGameObjectInjectionWorld.EntityManager.Exists(chaseTarget.Target))
            if (behaviourState.Value == ProjectEnums.BehaviourState.Attack && chaseTarget.Target != Entity.Null)
            {
                float3 myPos = tran.Value;

                if (!CanAttackTarget(chaseTarget.Target, myPos, attackDistance.Value))
                    behaviourState.Value = ProjectEnums.BehaviourState.Chasing;

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
