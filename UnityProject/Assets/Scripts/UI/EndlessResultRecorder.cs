using RPGFPS.Combat;
using RPGFPS.Core;
using UnityEngine;

namespace RPGFPS.UI
{
    public class EndlessResultRecorder : MonoBehaviour
    {
        private ScoreManager scoreManager;
        private GameModeController gameMode;

        public void Configure(ScoreManager score, GameModeController mode)
        {
            scoreManager = score;
            gameMode = mode;
        }

        public string BuildResultText()
        {
            var time = gameMode == null ? 0f : gameMode.ElapsedTime;
            var score = scoreManager == null ? 0 : scoreManager.CurrentScore;
            return $"Endless Result -> Time: {time:F1}s, Score: {score}";
        }
    }
}
