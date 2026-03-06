using System.Collections.Generic;
using RPGFPS.Combat;
using UnityEngine;

namespace RPGFPS.Network
{
    public class CoopDamageTracker : MonoBehaviour
    {
        private readonly Dictionary<string, float> dealtByPlayer = new();
        private readonly Dictionary<string, float> takenByPlayer = new();
        private readonly Dictionary<string, int> killsByPlayer = new();

        public IReadOnlyDictionary<string, float> DealtByPlayer => dealtByPlayer;
        public IReadOnlyDictionary<string, float> TakenByPlayer => takenByPlayer;
        public IReadOnlyDictionary<string, int> KillsByPlayer => killsByPlayer;

        public void RegisterPlayer(string playerId)
        {
            if (!dealtByPlayer.ContainsKey(playerId)) dealtByPlayer[playerId] = 0f;
            if (!takenByPlayer.ContainsKey(playerId)) takenByPlayer[playerId] = 0f;
            if (!killsByPlayer.ContainsKey(playerId)) killsByPlayer[playerId] = 0;
        }

        public void RecordDamageDealt(string playerId, float value)
        {
            RegisterPlayer(playerId);
            dealtByPlayer[playerId] += Mathf.Max(0f, value);
        }

        public void RecordDamageTaken(string playerId, float value)
        {
            RegisterPlayer(playerId);
            takenByPlayer[playerId] += Mathf.Max(0f, value);
        }

        public void RecordKill(string playerId)
        {
            RegisterPlayer(playerId);
            killsByPlayer[playerId] += 1;
        }

        public void BindPlayerHealth(string playerId, HealthComponent health)
        {
            RegisterPlayer(playerId);
            health.OnDamaged += (_, finalDamage) => RecordDamageTaken(playerId, finalDamage);
        }
    }
}
