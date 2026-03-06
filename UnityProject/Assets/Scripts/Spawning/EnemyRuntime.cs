using RPGFPS.Combat;
using RPGFPS.Network;
using UnityEngine;

namespace RPGFPS.Spawning
{
    public class EnemyRuntime : MonoBehaviour
    {
        [SerializeField] private HealthComponent health;

        private ScoreManager scoreManager;
        private CoopDamageTracker damageTracker;
        private int scoreValue = 1;
        private int enemyLevel;

        private void Awake()
        {
            if (health == null)
            {
                health = GetComponent<HealthComponent>();
            }
        }

        private void OnEnable()
        {
            if (health != null)
            {
                health.OnDeath += HandleDeath;
            }
        }

        private void OnDisable()
        {
            if (health != null)
            {
                health.OnDeath -= HandleDeath;
            }
        }

        public void Initialize(StatBlock enemyStats, int score, int level, ScoreManager scoreRef, CoopDamageTracker tracker)
        {
            scoreManager = scoreRef;
            damageTracker = tracker;
            scoreValue = Mathf.Max(1, score);
            enemyLevel = level;
            health.OverrideStats(enemyStats);
            health.HealToFull();
        }

        private void HandleDeath(DamageInfo info)
        {
            scoreManager.AddScore(scoreValue);
            if (!string.IsNullOrEmpty(info.sourceId))
            {
                damageTracker.RecordKill(info.sourceId);
            }

            Destroy(gameObject);
        }

        public int GetEnemyLevel() => enemyLevel;
    }
}
