using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class EnemyTurnSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<TurnTimer>().ForEach((ref Translation trans, ref Rotation rot, ref MoveForward moveForward, ref TurnTimer turnTimer) =>
        {
            turnTimer.TimerCounter -= Time.DeltaTime;
            // on timer out change rotationt
            if (turnTimer.TimerCounter <= 0)
            {
                turnTimer.TimerCounter = SharedMethods.MakeRandom(turnTimer.TimeRange, turnTimer.TimeRange + "timer");

                rot.Value = quaternion.Euler(math.mul(rot.Value, new float3(0, SharedMethods.MakeRandom(-90, 90, "rotat"), 0)));
            }
        });
    }
}
