using System;
using UnityEngine;

namespace RPGFPS.Combat
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private StatBlock baseStats = new(100f, 0f, 10f, 2f);

        public StatBlock RuntimeStats { get; private set; }
        public float CurrentHp { get; private set; }

        public event Action<float, float> OnHealthChanged;
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

        public void TakeDamage(DamageInfo info)
        {
            if (CurrentHp <= 0f)
            {
                return;
            }

            var reduced = Mathf.Max(1f, info.rawDamage - RuntimeStats.armor);
            CurrentHp = Mathf.Max(0f, CurrentHp - reduced);
            OnHealthChanged?.Invoke(CurrentHp, RuntimeStats.maxHp);

            if (CurrentHp <= 0f)
            {
                OnDeath?.Invoke(info);
            }
        }
    }

    public readonly struct DamageInfo
    {
        public readonly float rawDamage;
        public readonly GameObject source;
        public readonly bool countedAsHit;

        public DamageInfo(float rawDamage, GameObject source, bool countedAsHit = true)
        {
            this.rawDamage = rawDamage;
            this.source = source;
            this.countedAsHit = countedAsHit;
        }
    }
}
