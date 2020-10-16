using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

// Only state machine changing
public class BehaviourSwitchSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        // for all with no valid target
        Entities.WithAll<ChaseTarget, BehaviourState>().ForEach((
            Entity entity,
            ref BehaviourState behaviourState,
            ref ChaseTarget chaseTarget) =>
        {
            if (!World.DefaultGameObjectInjectionWorld.EntityManager.Exists(chaseTarget.Target))
            {
                behaviourState.Value = ProjectEnums.BehaviourState.Patrolling;
                //PostUpdateCommands.RemoveComponent(entity, typeof(ChaseTarget));
                EntityManager.RemoveComponent(entity,typeof(ChaseTarget));
            }
        });

        // for all who has no target
        Entities.WithNone<ChaseTarget>().WithAll<BehaviourState>().ForEach((
            Entity entity,
            ref BehaviourState behaviourState,
            ref TeamTag teamTag,
            ref Translation tran,
            ref NoticeTagetDistance noticeTagetDistance
            ) =>
        {
            behaviourState.Value = ProjectEnums.BehaviourState.Patrolling;

            // find and select target
            float3 myPos = tran.Value;
            ProjectEnums.TeamTag myTeam = teamTag.Value;
            float myNoticeTargetDistance = noticeTagetDistance.Value;
            Entity closestTarget = Entity.Null;
            float3 closestTargetPos = float3.zero;

            FindTarget(ref closestTarget, ref closestTargetPos, myTeam, myPos, myNoticeTargetDistance);

            // if target found then chasing
            if (closestTarget != Entity.Null)
            {
                behaviourState.Value = ProjectEnums.BehaviourState.Chasing;

                PostUpdateCommands.AddComponent(entity, new ChaseTarget { Target = closestTarget });
            }
        });

        // for all with target
        Entities.WithAll<ChaseTarget, BehaviourState>().ForEach((
            Entity entity,
            ref Translation tran,
            ref ChaseTarget chaseTarget,
            ref TeamTag teamTag,
            ref BehaviourState behaviourState,
            ref AttackDistance attackDistance) =>
        {

            // if (!World.DefaultGameObjectInjectionWorld.EntityManager.Exists(chaseTarget.Target))
            if (chaseTarget.Target == Entity.Null)
            {
                behaviourState.Value = ProjectEnums.BehaviourState.Patrolling;
                // PostUpdateCommands.RemoveComponent(entity, typeof(ChaseTarget));
                EntityManager.RemoveComponent(entity, typeof(ChaseTarget));

            }
            else
            {
                float3 myPos = tran.Value;

                switch (behaviourState.Value)
                {
                    case ProjectEnums.BehaviourState.Chasing:
                        if (CanAttackTarget(chaseTarget.Target, myPos, attackDistance.Value))
                            behaviourState.Value = ProjectEnums.BehaviourState.Attack;
                        break;

                    case ProjectEnums.BehaviourState.Attack:
                        if (!CanAttackTarget(chaseTarget.Target, myPos, attackDistance.Value))
                            behaviourState.Value = ProjectEnums.BehaviourState.Chasing;
                        break;
                }
            }
        });

        //// for all with target
        //Entities.WithAll<ChaseTarget, BehaviourState>().ForEach((
        //    Entity entity,
        //    ref Translation tran,
        //    ref ChaseTarget chaseTarget,
        //    ref TeamTag teamTag,
        //    ref BehaviourState behaviourState,
        //    ref AttackDistance attackDistance) =>
        //{
        //    float3 myPos = tran.Value;

        //    switch (behaviourState.Value)
        //    {
        //        case ProjectEnums.BehaviourState.Chasing:
        //            if (CanAttackTarget1(chaseTarget.Target, myPos, attackDistance.Value))
        //                behaviourState.Value = ProjectEnums.BehaviourState.Attack;
        //            break;

        //        case ProjectEnums.BehaviourState.Attack:
        //            if (World.DefaultGameObjectInjectionWorld.EntityManager.Exists(chaseTarget.Target))
        //                if (!CanAttackTarget2(chaseTarget.Target, myPos, attackDistance.Value))
        //                    behaviourState.Value = ProjectEnums.BehaviourState.Chasing;
        //            break;
        //    }
        //});



        //Entities.WithAll<ChaseTarget,BehaviourState>().ForEach((
        //    ref Translation tran,
        //    ref ChaseTarget chaseTarget,
        //    ref TeamTag teamTag,
        //    ref BehaviourState behaviourState,
        //    ref AttackDistance attackDistance,
        //    ref NoticeTagetDistance noticeTagetDistance
        //    ) =>
        //{
        //    float3 myPos = tran.Value;
        //    ProjectEnums.TeamTag myTeam = teamTag.Value;
        //    float myNoticeTargetDistance = noticeTagetDistance.Value;

        //    switch (behaviourState.Value)
        //    {
        //        case ProjectEnums.BehaviourState.Patrolling:
        //            // find target
        //            Entity closestTarget = Entity.Null;
        //            float3 closestTargetPos = float3.zero;

        //            if (chaseTarget.Target == Entity.Null)
        //                FindTarget(ref closestTarget, ref closestTargetPos, myTeam, myPos, myNoticeTargetDistance);

        //            // if target found then chasing
        //            if (closestTarget != Entity.Null)
        //            {
        //                behaviourState.Value = ProjectEnums.BehaviourState.Chasing;
        //                chaseTarget.Target = closestTarget;
        //            }
        //            // if no target found then movement system will proceed with movement
        //            break;

        //        case ProjectEnums.BehaviourState.Chasing:
        //            // check if target exist, if no - browsing
        //            if (!World.DefaultGameObjectInjectionWorld.EntityManager.Exists(chaseTarget.Target))
        //            {
        //                behaviourState.Value = ProjectEnums.BehaviourState.Patrolling;
        //                chaseTarget.Target = Entity.Null;
        //            }
        //            else if (CanAttackTarget(chaseTarget.Target, myPos, attackDistance.Value))
        //                // check if can attack - then attack
        //                behaviourState.Value = ProjectEnums.BehaviourState.Attack;
        //            break;

        //        case ProjectEnums.BehaviourState.Attack:
        //            if (!World.DefaultGameObjectInjectionWorld.EntityManager.Exists(chaseTarget.Target))
        //            {
        //                behaviourState.Value = ProjectEnums.BehaviourState.Patrolling;
        //                chaseTarget.Target = Entity.Null;
        //            }
        //            else if (!CanAttackTarget(chaseTarget.Target, myPos, attackDistance.Value))
        //                behaviourState.Value = ProjectEnums.BehaviourState.Chasing;
        //            break;
        //    }
        //});
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
        Translation targetTranslation = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<Translation>(targetEntity);
        if (math.distance(myPos, targetTranslation.Value) <= attackDistance)
            return true;

        return false;
    }  

}
