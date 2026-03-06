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
        [SerializeField] private GameMode gameMode = GameMode.StageClear;
        [SerializeField] private float surviveSecondsToWin = 180f;
        [SerializeField] private int scoreToWin = 120;
        [SerializeField] private float endlessBuffInterval = 60f;
        [SerializeField] private EnemyDirector enemyDirector;
        [SerializeField] private ScoreManager scoreManager;

        public float ElapsedTime { get; private set; }
        public bool IsGameEnded { get; private set; }

        public event Action<string> OnGameWin;

        public void Configure(GameMode mode, EnemyDirector director, ScoreManager score)
        {
            gameMode = mode;
            enemyDirector = director;
            scoreManager = score;
        }

        private void Update()
        {
            if (IsGameEnded) return;

            ElapsedTime += Time.deltaTime;

            if (gameMode == GameMode.StageClear)
            {
                if (ElapsedTime >= surviveSecondsToWin || scoreManager.CurrentScore >= scoreToWin)
                {
                    IsGameEnded = true;
                    OnGameWin?.Invoke($"通关成功！时间 {ElapsedTime:F1}s，分数 {scoreManager.CurrentScore}");
                }
                return;
            }

            var waveCount = Mathf.FloorToInt(ElapsedTime / endlessBuffInterval);
            enemyDirector.ApplyTimedEndlessBuff(waveCount);
        }
    }
}
