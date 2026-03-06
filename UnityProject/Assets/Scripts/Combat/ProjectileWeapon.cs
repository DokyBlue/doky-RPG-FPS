using RPGFPS.Network;
using UnityEngine;

namespace RPGFPS.Combat
{
    public class ProjectileWeapon : MonoBehaviour
    {
        [SerializeField] private float range = 80f;
        [SerializeField] private LayerMask hitMask = ~0;
        [SerializeField] private CoopDamageTracker damageTracker;

        private string ownerId;
        private Camera viewCamera;
        private HealthComponent ownerHealth;
        private float nextShotTime;

        public void Configure(string playerId, Camera cameraRef, HealthComponent health)
        {
            ownerId = playerId;
            viewCamera = cameraRef;
            ownerHealth = health;
        }

        public void SetTracker(CoopDamageTracker tracker)
        {
            damageTracker = tracker;
        }

        public void TryFire()
        {
            if (viewCamera == null || ownerHealth == null) return;
            var fireInterval = 1f / Mathf.Max(0.2f, ownerHealth.RuntimeStats.fireRate);
            if (Time.time < nextShotTime) return;
            nextShotTime = Time.time + fireInterval;

            if (!Physics.Raycast(viewCamera.transform.position, viewCamera.transform.forward, out var hit, range, hitMask)) return;
            if (!hit.collider.TryGetComponent<HealthComponent>(out var target)) return;

            var damage = target.TakeDamage(new DamageInfo(ownerHealth.RuntimeStats.attack, ownerId, gameObject));
            if (!string.IsNullOrEmpty(ownerId) && damageTracker != null)
            {
                damageTracker.RecordDamageDealt(ownerId, damage);
            }
        }
    }
}
