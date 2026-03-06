using RPGFPS.Combat;
using UnityEngine;

namespace RPGFPS.AI
{
    public class SimpleEnemyAI : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 3.5f;
        [SerializeField] private float attackDistance = 2f;
        [SerializeField] private float attackCooldown = 0.8f;
        [SerializeField] private string enemyIdPrefix = "Enemy";
        [SerializeField] private HealthComponent health;

        private Transform target;
        private float nextAttack;

        private void Awake()
        {
            if (health == null)
            {
                health = GetComponent<HealthComponent>();
            }
        }

        public void SetTarget(Transform targetTransform)
        {
            target = targetTransform;
        }

        private void Update()
        {
            if (target == null || health == null || !health.IsAlive) return;

            var toTarget = target.position - transform.position;
            toTarget.y = 0f;
            var distance = toTarget.magnitude;

            if (distance > attackDistance)
            {
                transform.position += toTarget.normalized * (moveSpeed * Time.deltaTime);
                transform.forward = Vector3.Lerp(transform.forward, toTarget.normalized, 8f * Time.deltaTime);
                return;
            }

            if (Time.time < nextAttack) return;
            nextAttack = Time.time + attackCooldown;

            if (target.TryGetComponent<HealthComponent>(out var playerHealth))
            {
                playerHealth.TakeDamage(new DamageInfo(health.RuntimeStats.attack, $"{enemyIdPrefix}_{GetInstanceID()}", gameObject));
            }
        }
    }
}
