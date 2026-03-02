using RPGFPS.Combat;
using UnityEngine;

namespace RPGFPS.UI
{
    public class EndlessResultRecorder : MonoBehaviour
    {
        [SerializeField] private ScoreManager scoreManager;

        private float survivalTime;

        private void Update()
        {
            survivalTime += Time.deltaTime;
        }

        public string BuildResultText()
        {
            return $"Endless Result -> Time: {survivalTime:F1}s, Score: {scoreManager.CurrentScore}";
        }
    }
}
