using System.Collections.Generic;
using RPGFPS.Combat;
using UnityEngine;

namespace RPGFPS.Network
{
    public class CoopDamageTracker : MonoBehaviour
    {
        private readonly Dictionary<string, float> dealtByPlayer = new();
        private readonly Dictionary<string, float> takenByPlayer = new();

        public IReadOnlyDictionary<string, float> DealtByPlayer => dealtByPlayer;
        public IReadOnlyDictionary<string, float> TakenByPlayer => takenByPlayer;

        public void RecordDamageDealt(string playerId, float value)
        {
            if (!dealtByPlayer.ContainsKey(playerId))
            {
                dealtByPlayer[playerId] = 0f;
            }

            dealtByPlayer[playerId] += Mathf.Max(0f, value);
        }

        public void RecordDamageTaken(string playerId, float value)
        {
            if (!takenByPlayer.ContainsKey(playerId))
            {
                takenByPlayer[playerId] = 0f;
            }

            takenByPlayer[playerId] += Mathf.Max(0f, value);
        }

        public void BindPlayerHealth(string playerId, HealthComponent health)
        {
            health.OnDeath += _ => RecordDamageTaken(playerId, health.RuntimeStats.maxHp);
        }
    }
}
