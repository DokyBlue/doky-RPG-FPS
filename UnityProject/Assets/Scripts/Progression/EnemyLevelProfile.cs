using RPGFPS.Combat;
using UnityEngine;

namespace RPGFPS.Progression
{
    [CreateAssetMenu(fileName = "EnemyLevelProfile", menuName = "RPGFPS/Enemy Level Profile")]
    public class EnemyLevelProfile : ScriptableObject
    {
        [Header("Base (Level 0)")]
        public StatBlock levelZeroStats = new(80f, 0f, 8f, 1.5f);
        public int levelZeroScore = 1;

        [Header("Per Level Multipliers")]
        [Range(1f, 3f)] public float hpMultiplier = 1.2f;
        [Range(1f, 3f)] public float armorMultiplier = 1.15f;
        [Range(1f, 3f)] public float attackMultiplier = 1.15f;
        [Range(1f, 3f)] public float scoreMultiplier = 1.25f;

        public StatBlock GetStats(int level)
        {
            if (level <= 0)
            {
                return levelZeroStats;
            }

            var hp = levelZeroStats.maxHp * Mathf.Pow(hpMultiplier, level);
            var armor = Mathf.Max(0f, levelZeroStats.armor + (Mathf.Pow(armorMultiplier, level) - 1f) * 2f);
            var attack = levelZeroStats.attack * Mathf.Pow(attackMultiplier, level);
            return new StatBlock(hp, armor, attack, levelZeroStats.fireRate);
        }

        public int GetScore(int level)
        {
            return Mathf.RoundToInt(levelZeroScore * Mathf.Pow(scoreMultiplier, level));
        }
    }
}
