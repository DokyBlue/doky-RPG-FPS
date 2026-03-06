using RPGFPS.Combat;
using RPGFPS.Progression;
using UnityEngine;

namespace RPGFPS.Spawning
{
    public class EnemyDirector : MonoBehaviour
    {
        [SerializeField] private EnemyLevelProfile enemyProfile;
        [SerializeField] private int stageLevel;
        [SerializeField] private float endlessHpScaleStep = 0.1f;
        [SerializeField] private float endlessArmorStep = 0.5f;
        [SerializeField] private float endlessAttackStep = 0.1f;

        public int CurrentLevel => stageLevel;

        private void Awake()
        {
            if (enemyProfile == null)
            {
                enemyProfile = ScriptableObject.CreateInstance<EnemyLevelProfile>();
            }
        }

        public void NextStage() => stageLevel += 1;

        public void ApplyTimedEndlessBuff(int minuteCount)
        {
            stageLevel = Mathf.Max(stageLevel, minuteCount);
        }

        public void ConfigureEnemy(EnemyRuntime enemyRuntime, ScoreManager scoreManager, Network.CoopDamageTracker tracker)
        {
            var stats = enemyProfile.GetStats(stageLevel);
            stats.maxHp *= 1f + stageLevel * endlessHpScaleStep;
            stats.armor += stageLevel * endlessArmorStep;
            stats.attack *= 1f + stageLevel * endlessAttackStep;
            enemyRuntime.Initialize(stats, enemyProfile.GetScore(stageLevel), stageLevel, scoreManager, tracker);
        }
    }
}
