using System;
using UnityEngine;

namespace RPGFPS.Combat
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private StatBlock baseStats = new(100f, 0f, 10f, 1.2f);

        public StatBlock RuntimeStats { get; private set; }
        public float CurrentHp { get; private set; }
        public bool IsAlive => CurrentHp > 0f;

        public event Action<float, float> OnHealthChanged;
        public event Action<DamageInfo, float> OnDamaged;
        public event Action<DamageInfo> OnDeath;

        private void Awake()
        {
            RuntimeStats = baseStats;
            CurrentHp = RuntimeStats.maxHp;
        }

        public void OverrideStats(StatBlock newStats)
        {
            RuntimeStats = newStats;
            CurrentHp = Mathf.Min(CurrentHp, RuntimeStats.maxHp);
            OnHealthChanged?.Invoke(CurrentHp, RuntimeStats.maxHp);
        }

        public void HealToFull()
        {
            CurrentHp = RuntimeStats.maxHp;
            OnHealthChanged?.Invoke(CurrentHp, RuntimeStats.maxHp);
        }

        public float TakeDamage(DamageInfo info)
        {
            if (!IsAlive)
            {
                return 0f;
            }

            var finalDamage = Mathf.Max(1f, info.rawDamage - RuntimeStats.armor);
            CurrentHp = Mathf.Max(0f, CurrentHp - finalDamage);
            OnDamaged?.Invoke(info, finalDamage);
            OnHealthChanged?.Invoke(CurrentHp, RuntimeStats.maxHp);

            if (!IsAlive)
            {
                OnDeath?.Invoke(info);
            }

            return finalDamage;
        }
    }

    public readonly struct DamageInfo
    {
        public readonly float rawDamage;
        public readonly string sourceId;
        public readonly GameObject sourceObject;

        public DamageInfo(float rawDamage, string sourceId, GameObject sourceObject)
        {
            this.rawDamage = rawDamage;
            this.sourceId = sourceId;
            this.sourceObject = sourceObject;
        }
    }
}
