using System.Collections.Generic;
using RPGFPS.Combat;
using RPGFPS.Core;
using RPGFPS.Equipment;
using RPGFPS.Network;
using RPGFPS.Player;
using RPGFPS.Spawning;
using RPGFPS.UI;
using UnityEngine;

namespace RPGFPS.Gameplay
{
    public class PrimitiveGameBootstrap : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AutoCreate()
        {
            if (Object.FindObjectOfType<PrimitiveGameBootstrap>() != null) return;
            new GameObject("PrimitiveGameBootstrap").AddComponent<PrimitiveGameBootstrap>();
        }

        private void Awake()
        {
            BuildScene();
        }

        private void BuildScene()
        {
            var systems = new GameObject("GameSystems");
            var score = systems.AddComponent<ScoreManager>();
            var tracker = systems.AddComponent<CoopDamageTracker>();
            var director = systems.AddComponent<EnemyDirector>();
            var mode = systems.AddComponent<GameModeController>();
            var hud = systems.AddComponent<SimpleHudPresenter>();
            var endlessRecorder = systems.AddComponent<EndlessResultRecorder>();

            var player = CreatePlayer("P1", new Vector3(0f, 1f, 0f), Color.cyan);
            var teammate = CreateDummyTeammate("P2", new Vector3(3f, 1f, 0f));

            tracker.BindPlayerHealth("P1", player.GetComponent<HealthComponent>());
            tracker.BindPlayerHealth("P2", teammate.GetComponent<HealthComponent>());
            player.GetComponentInChildren<ProjectileWeapon>().SetTracker(tracker);
            teammate.GetComponentInChildren<ProjectileWeapon>().SetTracker(tracker);

            var loadout = player.AddComponent<EquipmentLoadout>();
            loadout.Configure(player.GetComponent<HealthComponent>());
            loadout.RecalculateStats();

            CreateArena();

            var spawner = new GameObject("EnemySpawner").AddComponent<EnemySpawner>();
            spawner.Configure(director, score, tracker, player.transform);

            var buffSpawner = new GameObject("BuffPointSpawner").AddComponent<BuffPointSpawner>();
            buffSpawner.Configure(CreateBuffPrefab(), CreateBuffPoints());

            mode.Configure(GameMode.Endless, director, score);
            endlessRecorder.Configure(score, mode);
            hud.Configure(score, tracker, mode, player.GetComponent<HealthComponent>());
        }

        private GameObject CreatePlayer(string id, Vector3 position, Color color)
        {
            var go = new GameObject(id);
            go.transform.position = position;
            var cc = go.AddComponent<CharacterController>();
            cc.height = 1.8f;
            cc.radius = 0.35f;

            var visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.transform.SetParent(go.transform);
            visual.transform.localPosition = Vector3.zero;
            visual.GetComponent<Renderer>().material.color = color;

            var health = go.AddComponent<HealthComponent>();

            var cameraGo = new GameObject("Main Camera");
            cameraGo.tag = "MainCamera";
            cameraGo.transform.SetParent(go.transform);
            cameraGo.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            var cam = cameraGo.AddComponent<Camera>();

            var weapon = new GameObject("Weapon").AddComponent<ProjectileWeapon>();
            weapon.transform.SetParent(go.transform);
            weapon.transform.localPosition = new Vector3(0.3f, 1.4f, 0.5f);

            var controller = go.AddComponent<FpsPlayerController>();
            controller.Configure(id, cam, weapon, health);
            return go;
        }

        private GameObject CreateDummyTeammate(string id, Vector3 position)
        {
            var go = new GameObject(id);
            go.transform.position = position;

            var health = go.AddComponent<HealthComponent>();
            var visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.transform.SetParent(go.transform);
            visual.GetComponent<Renderer>().material.color = Color.green;

            var dummyCamera = new GameObject("DummyCam").AddComponent<Camera>();
            dummyCamera.enabled = false;
            dummyCamera.transform.SetParent(go.transform);
            dummyCamera.transform.localPosition = new Vector3(0f, 1.6f, 0f);

            var dummyWeapon = new GameObject("DummyWeapon").AddComponent<ProjectileWeapon>();
            dummyWeapon.transform.SetParent(go.transform);
            dummyWeapon.Configure(id, dummyCamera, health);

            return go;
        }

        private static GameObject CreateBuffPrefab()
        {
            var pickup = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pickup.name = "BuffPickupPrefab";
            pickup.transform.localScale = Vector3.one * 0.8f;
            pickup.GetComponent<Renderer>().material.color = Color.yellow;
            var col = pickup.GetComponent<SphereCollider>();
            col.isTrigger = true;
            pickup.AddComponent<BuffPickup>();
            pickup.SetActive(false);
            return pickup;
        }

        private static List<Transform> CreateBuffPoints()
        {
            var points = new List<Transform>();
            var positions = new[]
            {
                new Vector3(10f, 1f, 10f), new Vector3(-10f, 1f, 10f), new Vector3(10f, 1f, -10f), new Vector3(-10f, 1f, -10f)
            };
            foreach (var p in positions)
            {
                var point = new GameObject("BuffPoint");
                point.transform.position = p;
                points.Add(point.transform);
            }

            return points;
        }

        private static void CreateArena()
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(6f, 1f, 6f);
            ground.GetComponent<Renderer>().material.color = new Color(0.2f, 0.2f, 0.2f);

            var lightGo = new GameObject("Directional Light");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }
    }
}
