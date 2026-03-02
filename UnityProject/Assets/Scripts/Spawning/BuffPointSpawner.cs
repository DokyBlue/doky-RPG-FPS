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

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer < spawnInterval)
            {
                return;
            }

            timer = 0f;
            SpawnBuffPoint();
        }

        private void SpawnBuffPoint()
        {
            if (buffPickupPrefab == null || spawnPoints.Count == 0)
            {
                return;
            }

            var point = spawnPoints[Random.Range(0, spawnPoints.Count)];
            Instantiate(buffPickupPrefab, point.position, point.rotation);
        }
    }

    public class BuffPickup : MonoBehaviour
    {
        [SerializeField] private StatBlock buff = new(20f, 1f, 3f, 0.5f);

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<HealthComponent>(out var health))
            {
                return;
            }

            var newStats = health.RuntimeStats + buff;
            health.OverrideStats(newStats);
            health.HealToFull();
            Destroy(gameObject);
        }
    }
}
