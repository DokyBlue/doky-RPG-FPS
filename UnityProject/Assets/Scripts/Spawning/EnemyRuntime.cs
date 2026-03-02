using RPGFPS.Combat;
using UnityEngine;

namespace RPGFPS.Spawning
{
    public class EnemyRuntime : MonoBehaviour
    {
        [SerializeField] private HealthComponent health;
        [SerializeField] private ScoreManager scoreManager;

        private int scoreValue = 1;
        private int enemyLevel;

        private void OnEnable()
        {
            health.OnDeath += HandleDeath;
        }

        private void OnDisable()
        {
            health.OnDeath -= HandleDeath;
        }

        public void Initialize(StatBlock enemyStats, int score, int level)
        {
            scoreValue = Mathf.Max(1, score);
            enemyLevel = level;
            health.OverrideStats(enemyStats);
            health.HealToFull();
        }

        private void HandleDeath(DamageInfo info)
        {
            scoreManager.AddScore(scoreValue);
            Destroy(gameObject);
        }

        public int GetEnemyLevel() => enemyLevel;
    }
}
