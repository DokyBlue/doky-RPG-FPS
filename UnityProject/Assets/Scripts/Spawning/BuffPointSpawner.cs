using System.Collections.Generic;
using RPGFPS.Combat;
using UnityEngine;

namespace RPGFPS.Spawning
{
    public class BuffPointSpawner : MonoBehaviour
    {
        [SerializeField] private List<Transform> spawnPoints = new();
        [SerializeField] private GameObject buffPickupPrefab;
        [SerializeField] private float spawnInterval = 20f;

        private float timer;

        public void Configure(GameObject prefab, List<Transform> points)
        {
            buffPickupPrefab = prefab;
            spawnPoints = points;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer < spawnInterval) return;
            timer = 0f;
            SpawnBuffPoint();
        }

        private void SpawnBuffPoint()
        {
            if (buffPickupPrefab == null || spawnPoints.Count == 0) return;
            var point = spawnPoints[Random.Range(0, spawnPoints.Count)];
            var obj = Instantiate(buffPickupPrefab, point.position, point.rotation);
            obj.SetActive(true);
        }
    }

    public class BuffPickup : MonoBehaviour
    {
        [SerializeField] private float maxHpUp = 20f;
        [SerializeField] private float armorUp = 1f;
        [SerializeField] private float attackUp = 3f;
        [SerializeField] private float fireRateUp = 0.5f;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<HealthComponent>(out var health)) return;

            var buff = new StatBlock(maxHpUp, armorUp, attackUp, fireRateUp);
            var newStats = health.RuntimeStats + buff;
            health.OverrideStats(newStats);
            health.HealToFull();
            Destroy(gameObject);
        }
    }
}
