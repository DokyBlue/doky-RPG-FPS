using RPGFPS.AI;
using RPGFPS.Combat;
using RPGFPS.Network;
using UnityEngine;

namespace RPGFPS.Spawning
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private EnemyDirector enemyDirector;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private CoopDamageTracker damageTracker;
        [SerializeField] private Transform target;
        [SerializeField] private float spawnInterval = 1.8f;
        [SerializeField] private float spawnRadius = 35f;

        private float spawnTimer;

        public void Configure(EnemyDirector director, ScoreManager score, CoopDamageTracker tracker, Transform targetTransform)
        {
            enemyDirector = director;
            scoreManager = score;
            damageTracker = tracker;
            target = targetTransform;
        }

        private void Update()
        {
            if (enemyDirector == null || scoreManager == null || target == null) return;

            spawnTimer += Time.deltaTime;
            if (spawnTimer < spawnInterval) return;
            spawnTimer = 0f;
            SpawnEnemy();
        }

        private void SpawnEnemy()
        {
            var randomPos2D = Random.insideUnitCircle.normalized * spawnRadius;
            var pos = new Vector3(randomPos2D.x, 1f, randomPos2D.y);

            var enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            enemy.name = "Enemy";
            enemy.transform.position = pos;
            enemy.transform.localScale = new Vector3(1f, 2f, 1f);
            enemy.GetComponent<Renderer>().material.color = new Color(0.8f, 0.2f, 0.2f);

            var rigid = enemy.AddComponent<Rigidbody>();
            rigid.isKinematic = true;
            rigid.useGravity = false;

            var health = enemy.AddComponent<HealthComponent>();
            var ai = enemy.AddComponent<SimpleEnemyAI>();
            ai.SetTarget(target);

            var runtime = enemy.AddComponent<EnemyRuntime>();
            enemyDirector.ConfigureEnemy(runtime, scoreManager, damageTracker);
        }
    }
}
