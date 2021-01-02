using Unity.Collections;
using UnityEngine;

namespace JobsMoveToClickPoint
{
    public class BotFactory : IBotFactory
    {
        private const string BotPath = "BotPrefabs/EcsEnemyYbotForJobs";
        private const string BulletPath = "BotPrefabs/ECSBulletForJobs";

        private const float GroundHeight = 1f;
        private const float SpawnAreaRadius = 5f;

        private readonly GameObject _botPrefab;

        public BotFactory() =>
            _botPrefab = Resources.Load(BotPath) as GameObject;
            //_botPrefab = Resources.Load(BulletPath) as GameObject;

        public Transform[] GenerateBots(int amount)
        {
            var transforms = new Transform[amount];

            for (var i = 0; i < amount; i++)
            {
                transforms[i] = GameObject.Instantiate(_botPrefab).transform;
                transforms[i].rotation = RandomRotation();
                transforms[i].position = RandomPosition();
            }

            return transforms;
        }

        private Vector3 RandomPosition()
        {
            var random = Random.insideUnitCircle * SpawnAreaRadius;
            return new Vector3(random.x, GroundHeight, random.y);
        }

        private Quaternion RandomRotation()
        {
            var random = Random.Range(0f, 360f);
            return Quaternion.Euler(new Vector3(0, random, 0));
        }
    }
}