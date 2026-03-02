using System;
using RPGFPS.Combat;
using RPGFPS.Spawning;
using UnityEngine;

namespace RPGFPS.Core
{
    public enum GameMode
    {
        StageClear,
        Endless
    }

    public class GameModeController : MonoBehaviour
    {
        [Header("Mode")]
        [SerializeField] private GameMode gameMode = GameMode.StageClear;

        [Header("Stage Clear Conditions")]
        [SerializeField] private float surviveSecondsToWin = 300f;
        [SerializeField] private int scoreToWin = 120;

        [Header("Endless")]
        [SerializeField] private float endlessBuffInterval = 60f;

        [Header("References")]
        [SerializeField] private EnemyDirector enemyDirector;
        [SerializeField] private ScoreManager scoreManager;

        private float elapsed;
        private bool isGameEnded;

        public event Action<string> OnGameWin;

        private void Update()
        {
            if (isGameEnded)
            {
                return;
            }

            elapsed += Time.deltaTime;

            if (gameMode == GameMode.StageClear)
            {
                EvaluateStageClearWin();
                return;
            }

            EvaluateEndlessScaling();
        }

        private void EvaluateStageClearWin()
        {
            if (elapsed >= surviveSecondsToWin || scoreManager.CurrentScore >= scoreToWin)
            {
                isGameEnded = true;
                OnGameWin?.Invoke($"Victory! Time: {elapsed:F1}s, Score: {scoreManager.CurrentScore}");
            }
        }

        private void EvaluateEndlessScaling()
        {
            if (elapsed < endlessBuffInterval)
            {
                return;
            }

            var waveCount = Mathf.FloorToInt(elapsed / endlessBuffInterval);
            enemyDirector.ApplyTimedEndlessBuff(waveCount);
        }
    }
}
