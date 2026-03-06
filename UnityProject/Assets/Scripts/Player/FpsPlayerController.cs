using RPGFPS.Combat;
using UnityEngine;

namespace RPGFPS.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class FpsPlayerController : MonoBehaviour
    {
        [SerializeField] private string playerId = "P1";
        [SerializeField] private float moveSpeed = 7f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private ProjectileWeapon weapon;
        [SerializeField] private HealthComponent health;

        private CharacterController controller;
        private Vector3 velocity;
        private float pitch;

        public string PlayerId => playerId;

        public void Configure(string id, Camera cam, ProjectileWeapon weaponComponent, HealthComponent healthComponent)
        {
            playerId = id;
            playerCamera = cam;
            weapon = weaponComponent;
            health = healthComponent;
            weapon.Configure(playerId, playerCamera, health);
        }

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (playerCamera == null || weapon == null || health == null) return;

            var move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
            controller.Move(move * (moveSpeed * Time.deltaTime));

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            var mouseX = Input.GetAxis("Mouse X") * 2f;
            var mouseY = Input.GetAxis("Mouse Y") * 2f;
            transform.Rotate(Vector3.up * mouseX);
            pitch = Mathf.Clamp(pitch - mouseY, -75f, 75f);
            playerCamera.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);

            if (Input.GetMouseButton(0)) weapon.TryFire();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}
