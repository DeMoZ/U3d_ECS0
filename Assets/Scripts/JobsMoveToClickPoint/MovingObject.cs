using UnityEngine;

namespace JobsMoveToClickPoint
{
    public class MovingObject
    {
        public readonly Transform Transform;
        public readonly Vector3 Velocity;

        public MovingObject(Transform transform, Vector3 velocity)
        {
            Transform = transform;
            Velocity = velocity;
        }
    }
}