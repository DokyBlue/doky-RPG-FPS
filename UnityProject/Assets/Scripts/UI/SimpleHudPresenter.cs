using System.Text;
using RPGFPS.Combat;
using RPGFPS.Core;
using RPGFPS.Network;
using RPGFPS.Spawning;
using UnityEngine;

namespace RPGFPS.UI
{
    public class SimpleHudPresenter : MonoBehaviour
    {
        private ScoreManager scoreManager;
        private CoopDamageTracker tracker;
        private GameModeController gameMode;
        private HealthComponent playerHealth;
        private EnemyDirector enemyDirector;

        private readonly StringBuilder sb = new();
        private GUIStyle panelStyle;
        private GUIStyle titleStyle;
        private GUIStyle bodyStyle;
        private GUIStyle miniStyle;
        private Texture2D panelTexture;
        private Texture2D hpBackground;
        private Texture2D hpForeground;

        public void Configure(ScoreManager score, CoopDamageTracker damageTracker, GameModeController mode, HealthComponent health)
        {
            scoreManager = score;
            tracker = damageTracker;
            gameMode = mode;
            playerHealth = health;
            enemyDirector = FindObjectOfType<EnemyDirector>();
        }

        private void OnGUI()
        {
            if (scoreManager == null || tracker == null || gameMode == null || playerHealth == null) return;
            EnsureStyles();

            DrawMainPanel();
            DrawCoopStatsPanel();
            DrawCrosshair();
        }

        private void DrawMainPanel()
        {
            var hpRatio = Mathf.Clamp01(playerHealth.CurrentHp / Mathf.Max(1f, playerHealth.RuntimeStats.maxHp));
            var panelRect = new Rect(20f, 20f, 460f, 138f);
            GUI.Box(panelRect, GUIContent.none, panelStyle);

            GUI.Label(new Rect(34f, 30f, 430f, 24f), "战斗面板", titleStyle);
            GUI.Label(new Rect(34f, 56f, 420f, 24f), $"分数 {scoreManager.CurrentScore}    时间 {gameMode.ElapsedTime:F1}s", bodyStyle);
            GUI.Label(new Rect(34f, 82f, 420f, 24f), $"护甲 {playerHealth.RuntimeStats.armor:F1}    射速 {playerHealth.RuntimeStats.fireRate:F1}", bodyStyle);
            GUI.Label(new Rect(34f, 108f, 420f, 24f), $"敌人等级 {(enemyDirector != null ? enemyDirector.CurrentLevel : 1)}", bodyStyle);

            var hpBarBgRect = new Rect(240f, 108f, 220f, 12f);
            GUI.DrawTexture(hpBarBgRect, hpBackground);
            GUI.DrawTexture(new Rect(hpBarBgRect.x, hpBarBgRect.y, hpBarBgRect.width * hpRatio, hpBarBgRect.height), hpForeground);
            GUI.Label(new Rect(244f, 90f, 210f, 20f), $"HP {playerHealth.CurrentHp:F0}/{playerHealth.RuntimeStats.maxHp:F0}", miniStyle);
        }

        private void DrawCoopStatsPanel()
        {
            var panelRect = new Rect(20f, 170f, 420f, 210f);
            GUI.Box(panelRect, GUIContent.none, panelStyle);
            GUI.Label(new Rect(34f, 180f, 390f, 22f), "Co-op PvE 数据（造成 / 承伤 / 击杀）", titleStyle);

            sb.Clear();
            foreach (var kv in tracker.DealtByPlayer)
            {
                var playerId = kv.Key;
                var dealt = kv.Value;
                var taken = tracker.TakenByPlayer.TryGetValue(playerId, out var takenDamage) ? takenDamage : 0f;
                var kills = tracker.KillsByPlayer.TryGetValue(playerId, out var killCount) ? killCount : 0;
                sb.AppendLine($"{playerId,-4}  {dealt,6:F0} / {taken,6:F0} / {kills,3}");
            }

            GUI.Label(new Rect(34f, 208f, 370f, 164f), sb.ToString(), bodyStyle);
        }

        private static void DrawCrosshair()
        {
            var cx = Screen.width * 0.5f;
            var cy = Screen.height * 0.5f;
            DrawRect(new Rect(cx - 1f, cy - 10f, 2f, 20f), new Color(1f, 1f, 1f, 0.9f));
            DrawRect(new Rect(cx - 10f, cy - 1f, 20f, 2f), new Color(1f, 1f, 1f, 0.9f));
            DrawRect(new Rect(cx - 2f, cy - 2f, 4f, 4f), new Color(1f, 0.8f, 0.3f, 0.95f));
        }

        private void EnsureStyles()
        {
            if (panelStyle != null) return;

            panelTexture = MakeTex(new Color(0.06f, 0.08f, 0.12f, 0.78f));
            hpBackground = MakeTex(new Color(0.2f, 0.25f, 0.3f, 0.95f));
            hpForeground = MakeTex(new Color(0.36f, 0.88f, 0.5f, 0.95f));

            panelStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { background = panelTexture },
                border = new RectOffset(8, 8, 8, 8)
            };
            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.95f, 0.97f, 1f) }
            };
            bodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                richText = true,
                normal = { textColor = new Color(0.84f, 0.89f, 0.96f) }
            };
            miniStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = new Color(0.86f, 0.94f, 0.92f) }
            };
        }

        private static Texture2D MakeTex(Color color)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }

        private static void DrawRect(Rect rect, Color color)
        {
            var old = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, Texture2D.whiteTexture);
            GUI.color = old;
        }

        private void OnDestroy()
        {
            if (panelTexture != null) Destroy(panelTexture);
            if (hpBackground != null) Destroy(hpBackground);
            if (hpForeground != null) Destroy(hpForeground);
        }
    }
}
