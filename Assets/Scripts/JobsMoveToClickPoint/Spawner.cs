using UnityEngine;

namespace JobsMoveToClickPoint
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private int _numberOfBots;

        private BotBehaviour _botBehaviour;

        private void Start()
        {
            IBotFactory botFactory = new BotFactory();

            _botBehaviour = new BotBehaviour(_numberOfBots, botFactory);

            Clicker clicker = new Clicker(SetTargetPosition);
        }

        private void Update()
        {
            _botBehaviour.Update();
        }

        private void SetTargetPosition(Vector3 point) =>
            _botBehaviour.TargetPosition = point;

        void OnDrawGizmos()
        {
            if (_botBehaviour == null) return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_botBehaviour.TargetPosition, 0.5f);
        }
    }
}