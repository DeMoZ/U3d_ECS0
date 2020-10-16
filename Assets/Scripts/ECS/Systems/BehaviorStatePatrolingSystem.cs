using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class BehaviorStatePatrolingSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
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
           // Translation? closestTargetTranslation = null;

            FindTarget(ref closestTarget, ref closestTargetPos, myTeam, myPos, myNoticeTargetDistance);

            // if target found then chasing
            if (closestTarget != Entity.Null)
            {
                behaviourState.Value = ProjectEnums.BehaviourState.Chasing;

                PostUpdateCommands.AddComponent(entity, new ChaseTarget { Target = closestTarget });
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
}
