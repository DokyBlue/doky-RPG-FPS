using RPGFPS.Network;
using System.Collections;
using UnityEngine;

namespace RPGFPS.Combat
{
    public class ProjectileWeapon : MonoBehaviour
    {
        [Header("Ballistics")]
        [SerializeField] private float range = 80f;
        [SerializeField] private float projectileSpeed = 110f;
        [SerializeField] private float projectileGravity = -14f;
        [SerializeField] private float projectileMaxLifetime = 1.5f;
        [SerializeField] private float projectileRadius = 0.06f;
        [SerializeField] private Transform muzzlePoint;

        [Header("VFX")]
        [SerializeField] private float tracerWidth = 0.03f;
        [SerializeField] private Color tracerColor = new Color(1f, 0.85f, 0.35f, 1f);
        [SerializeField] private float tracerFadeDuration = 0.06f;
        [SerializeField] private float hitFxLifetime = 0.9f;

        [Header("Hit Detection")]
        [SerializeField] private LayerMask hitMask = ~0;
        [SerializeField] private CoopDamageTracker damageTracker;

        private string ownerId;
        private Camera viewCamera;
        private HealthComponent ownerHealth;
        private float nextShotTime;
        private Material tracerMaterial;
        private Material hitFxMaterial;

        private void Awake()
        {
            tracerMaterial = new Material(FindShader("Sprites/Default", "Unlit/Color", "Standard"));
            tracerMaterial.color = tracerColor;

            hitFxMaterial = new Material(FindShader("Particles/Standard Unlit", "Standard", "Unlit/Color"));
            hitFxMaterial.color = new Color(1f, 0.65f, 0.2f, 1f);
        }

        public void Configure(string playerId, Camera cameraRef, HealthComponent health)
        {
            ownerId = playerId;
            viewCamera = cameraRef;
            ownerHealth = health;
        }

        public void SetTracker(CoopDamageTracker tracker)
        {
            damageTracker = tracker;
        }

        public void TryFire()
        {
            if (viewCamera == null || ownerHealth == null) return;
            var fireInterval = 1f / Mathf.Max(0.2f, ownerHealth.RuntimeStats.fireRate);
            if (Time.time < nextShotTime) return;
            nextShotTime = Time.time + fireInterval;

            var origin = muzzlePoint != null ? muzzlePoint.position : viewCamera.transform.position;
            var direction = viewCamera.transform.forward.normalized;
            StartCoroutine(SimulateProjectile(origin, direction, ownerHealth.RuntimeStats.attack));
        }

        private IEnumerator SimulateProjectile(Vector3 origin, Vector3 direction, float attack)
        {
            var currentPos = origin;
            var velocity = direction * projectileSpeed;
            var tracer = CreateTracer(origin);
            var timeAlive = 0f;
            var traveledDistance = 0f;

            while (timeAlive < projectileMaxLifetime && traveledDistance < range)
            {
                var dt = Mathf.Max(0.001f, Time.deltaTime);
                var nextPos = currentPos + velocity * dt + Vector3.up * (0.5f * projectileGravity * dt * dt);
                velocity += Vector3.up * (projectileGravity * dt);

                var segment = nextPos - currentPos;
                var segmentDistance = segment.magnitude;
                if (segmentDistance > 0f)
                {
                    var hit = Physics.SphereCast(currentPos, projectileRadius, segment.normalized, out var hitInfo, segmentDistance, hitMask, QueryTriggerInteraction.Ignore);
                    if (hit)
                    {
                        UpdateTracer(tracer, origin, hitInfo.point);
                        ApplyHit(hitInfo, attack);
                        SpawnHitEffect(hitInfo.point, hitInfo.normal);
                        yield return new WaitForSeconds(tracerFadeDuration);
                        Destroy(tracer.gameObject);
                        yield break;
                    }
                }

                currentPos = nextPos;
                traveledDistance += segmentDistance;
                timeAlive += dt;
                UpdateTracer(tracer, origin, currentPos);
                yield return null;
            }

            UpdateTracer(tracer, origin, currentPos);
            yield return new WaitForSeconds(tracerFadeDuration);
            Destroy(tracer.gameObject);
        }

        private void ApplyHit(RaycastHit hit, float attack)
        {
            if (!hit.collider.TryGetComponent<HealthComponent>(out var target) || target == ownerHealth) return;

            var damage = target.TakeDamage(new DamageInfo(attack, ownerId, gameObject));
            if (!string.IsNullOrEmpty(ownerId) && damageTracker != null)
            {
                damageTracker.RecordDamageDealt(ownerId, damage);
            }
        }

        private LineRenderer CreateTracer(Vector3 origin)
        {
            var tracerGo = new GameObject("ProjectileTracer");
            var lineRenderer = tracerGo.AddComponent<LineRenderer>();
            lineRenderer.material = tracerMaterial;
            lineRenderer.widthMultiplier = tracerWidth;
            lineRenderer.positionCount = 2;
            lineRenderer.numCapVertices = 4;
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
            lineRenderer.useWorldSpace = true;
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, origin);
            return lineRenderer;
        }

        private static void UpdateTracer(LineRenderer tracer, Vector3 start, Vector3 end)
        {
            tracer.SetPosition(0, start);
            tracer.SetPosition(1, end);
        }

        private Shader FindShader(params string[] shaderNames)
        {
            foreach (var shaderName in shaderNames)
            {
                var shader = Shader.Find(shaderName);
                if (shader != null) return shader;
            }

            return Shader.Find("Standard");
        }

        private void SpawnHitEffect(Vector3 hitPoint, Vector3 normal)
        {
            var fxGo = new GameObject("HitFx");
            fxGo.transform.position = hitPoint + normal * 0.02f;
            fxGo.transform.rotation = Quaternion.LookRotation(normal);

            var ps = fxGo.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.duration = 0.2f;
            main.startLifetime = new ParticleSystem.MinMaxCurve(0.08f, 0.18f);
            main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 6f);
            main.startSize = new ParticleSystem.MinMaxCurve(0.03f, 0.07f);
            main.maxParticles = 32;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.gravityModifier = 0.35f;

            var emission = ps.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 12, 18) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.radius = 0.05f;
            shape.angle = 35f;

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.material = hitFxMaterial;

            ps.Play();
            Destroy(fxGo, hitFxLifetime);
        }

        private void OnDestroy()
        {
            if (tracerMaterial != null) Destroy(tracerMaterial);
            if (hitFxMaterial != null) Destroy(hitFxMaterial);
        }
    }
}
