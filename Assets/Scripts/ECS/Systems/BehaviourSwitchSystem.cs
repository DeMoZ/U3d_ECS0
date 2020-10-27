using Unity.Mathematics;
using Unity.Transforms;
using Unity.Entities;

public class BehaviourSwitchSystem : ComponentSystem
{
    private float _noticeTargetDistance = 10f;
    protected override void OnUpdate()
    {
        Entities.WithAll<BehaviourStatePatrolling>().ForEach((
            Entity entity,
            ref Translation translation,
            ref TeamTag teamTag) =>
        {
            float3 myPos = translation.Value;
            ProjectEnums.TeamTag myTeam = teamTag.Value;
            Entity closestTarget = Entity.Null;
            float3 closestTargetPos = float3.zero;

            FindTarget(ref closestTarget, ref closestTargetPos, myTeam, myPos, _noticeTargetDistance);

            // if target found then chasing
            if (closestTarget != Entity.Null)
            {
                EntityManager.RemoveComponent(entity, typeof(BehaviourStatePatrolling));
                EntityManager.AddComponentData(entity, new BehaviourStateChasing { ChaseTarget = closestTarget });
            }
        });

        Entities.WithAll<BehaviourStateChasing>().ForEach((
            Entity entity,
            ref Translation translation,
            ref BehaviourStateChasing behaviourStateChasing,
            ref AttackDistance attackDistance) =>
        {
            Entity targetEntity = behaviourStateChasing.ChaseTarget;
            float3 myPos = translation.Value;
            float myAttackDistance = attackDistance.Value;

            //if (targetEntity == Entity.Null)
            if (!EntityManager.HasComponent(targetEntity, typeof(Translation)))
            {
                EntityManager.RemoveComponent(entity, typeof(BehaviourStateChasing));
                EntityManager.AddComponent(entity, typeof(BehaviourStatePatrolling));
            }
            else
            {
                if (CanAttackTarget(targetEntity, myPos, myAttackDistance))
                {
                    EntityManager.RemoveComponent(entity, typeof(BehaviourStateChasing));
                    EntityManager.AddComponentData(entity, new BehaviourStateAttacking { AttackTarget = targetEntity });
                }
            }
        });

        Entities.WithAll<BehaviourStateAttacking>().ForEach((
            Entity entity,
            ref Translation translation,
            ref BehaviourStateAttacking behaviourStateAttacking,
            ref AttackDistance attackDistance
            ) =>
        {
            Entity targetEntity = behaviourStateAttacking.AttackTarget;
            float3 myPos = translation.Value;
            float myAttackDistance = attackDistance.Value;

            //if (targetEntity == Entity.Null)
            if (!EntityManager.HasComponent(targetEntity, typeof(Translation)))
            {
                EntityManager.RemoveComponent(entity, typeof(BehaviourStateChasing));
                EntityManager.AddComponent(entity, typeof(BehaviourStatePatrolling));
            }
            else
            {
                if (!CanAttackTarget(targetEntity, myPos, myAttackDistance))
                {
                    EntityManager.RemoveComponent(entity, typeof(BehaviourStateAttacking));
                    EntityManager.AddComponentData(entity, new BehaviourStateChasing { ChaseTarget = targetEntity });
                }
            }
        });
    }

    private void FindTarget(ref Entity target, ref float3 position, ProjectEnums.TeamTag myTeam, float3 myPos, float myNoticeTargetDistance)
    {
        Entity closestTarget = Entity.Null;
        float3 closestTargetPos = float3.zero;

        Entities.WithAll<TeamTag>().ForEach((
            Entity targetEntity,
            ref Translation targetTran,
            ref TeamTag targetTeamTag
            ) =>
        {
            if (myTeam != targetTeamTag.Value)
            {
                if (closestTarget == Entity.Null
                && math.distance(myPos, targetTran.Value) < myNoticeTargetDistance
                )
                {
                    closestTarget = targetEntity;
                    closestTargetPos = targetTran.Value;
                }

                if (math.distance(myPos, targetTran.Value) < math.distance(myPos, closestTargetPos)
                      && math.distance(myPos, targetTran.Value) < myNoticeTargetDistance
                      )
                {
                    closestTarget = targetEntity;
                    closestTargetPos = targetTran.Value;
                }

            }
        });

        target = closestTarget;
        position = closestTargetPos;
    }

    private bool CanAttackTarget(Entity targetEntity, float3 myPos, float attackDistance)
    {
        if (EntityManager.HasComponent(targetEntity, typeof(Translation)))
        {
            Translation targetTranslation = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Translation>(targetEntity);
            if (math.distance(myPos, targetTranslation.Value) <= attackDistance)
                return true;
        }

        return false;
    }
}
