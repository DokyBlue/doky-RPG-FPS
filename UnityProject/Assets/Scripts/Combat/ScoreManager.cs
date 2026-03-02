using System;
using UnityEngine;

namespace RPGFPS.Combat
{
    public class ScoreManager : MonoBehaviour
    {
        public int CurrentScore { get; private set; }

        public event Action<int> OnScoreChanged;

        public void AddScore(int value)
        {
            CurrentScore += Mathf.Max(0, value);
            OnScoreChanged?.Invoke(CurrentScore);
        }
    }
}
