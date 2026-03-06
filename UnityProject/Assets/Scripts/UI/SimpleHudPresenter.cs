using System.Text;
using RPGFPS.Combat;
using RPGFPS.Core;
using RPGFPS.Network;
using UnityEngine;

namespace RPGFPS.UI
{
    public class SimpleHudPresenter : MonoBehaviour
    {
        private ScoreManager scoreManager;
        private CoopDamageTracker tracker;
        private GameModeController gameMode;
        private HealthComponent playerHealth;
        private readonly StringBuilder sb = new();

        public void Configure(ScoreManager score, CoopDamageTracker damageTracker, GameModeController mode, HealthComponent health)
        {
            scoreManager = score;
            tracker = damageTracker;
            gameMode = mode;
            playerHealth = health;
        }

        private void OnGUI()
        {
            if (scoreManager == null || tracker == null || gameMode == null || playerHealth == null) return;

            sb.Clear();
            sb.AppendLine($"HP: {playerHealth.CurrentHp:F0}/{playerHealth.RuntimeStats.maxHp:F0}  Armor: {playerHealth.RuntimeStats.armor:F1}  FireRate: {playerHealth.RuntimeStats.fireRate:F1}");
            sb.AppendLine($"Score: {scoreManager.CurrentScore}  Time: {gameMode.ElapsedTime:F1}s  EnemyLv: {FindObjectOfType<Spawning.EnemyDirector>().CurrentLevel}");
            sb.AppendLine("=== Coop PvE 统计（造成/承伤/击杀）===");
            foreach (var kv in tracker.DealtByPlayer)
            {
                var pid = kv.Key;
                var dealt = kv.Value;
                var taken = tracker.TakenByPlayer.ContainsKey(pid) ? tracker.TakenByPlayer[pid] : 0f;
                var kills = tracker.KillsByPlayer.ContainsKey(pid) ? tracker.KillsByPlayer[pid] : 0;
                sb.AppendLine($"{pid}: {dealt:F0} / {taken:F0} / {kills}");
            }

            GUI.Label(new Rect(20, 20, 900, 300), sb.ToString());
            GUI.Label(new Rect(Screen.width * 0.5f - 8f, Screen.height * 0.5f - 10f, 30f, 30f), "+");
        }
    }
}
