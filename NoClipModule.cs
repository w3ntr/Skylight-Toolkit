using MelonLoader;
using UnityEngine;

namespace SkylightToolkit
{
    public static class NoClipModule
    {
        public static bool IsEnabled = false;
        public static float FlySpeed = 15f;
        public static float FastFlyMultiplier = 3f;

        private static CharacterController controller;
        private static Rigidbody rb;
        private static bool wasKinematic;
        private static Vector3 lastPosition;
        private static Collider[] nearbyTriggers;

        public static void Initialize()
        {
            MelonLogger.Msg("NoClipModule: Initialized (F4 to toggle)");
        }

        public static void Update()
        {
            if (Input.GetKeyDown(KeyCode.F4)) ToggleNoClip();

            if (IsEnabled && MovementModule.playerMotor != null)
            {
                if (controller == null)
                {
                    controller = MovementModule.playerMotor.GetComponent<CharacterController>();
                    rb = MovementModule.playerMotor.GetComponent<Rigidbody>();
                    if (rb != null) wasKinematic = rb.isKinematic;
                }

                // Отключаем коллизии
                if (controller != null) controller.enabled = false;
                if (rb != null) rb.isKinematic = true;

                // Ручное управление полётом
                float speed = FlySpeed * (Input.GetKey(KeyCode.LeftShift) ? FastFlyMultiplier : 1f) * Time.deltaTime;
                Vector3 move = Vector3.zero;

                if (Input.GetKey(KeyCode.W)) move += MovementModule.playerMotor.transform.forward;
                if (Input.GetKey(KeyCode.S)) move -= MovementModule.playerMotor.transform.forward;
                if (Input.GetKey(KeyCode.D)) move += MovementModule.playerMotor.transform.right;
                if (Input.GetKey(KeyCode.A)) move -= MovementModule.playerMotor.transform.right;
                if (Input.GetKey(KeyCode.Space)) move += Vector3.up;
                if (Input.GetKey(KeyCode.LeftControl)) move -= Vector3.up;

                lastPosition = MovementModule.playerMotor.transform.position;
                MovementModule.playerMotor.transform.position += move * speed;

                // Проверяем триггеры вручную
                CheckTriggers();
            }
            else if (!IsEnabled && controller != null)
            {
                // Восстанавливаем
                controller.enabled = true;
                if (rb != null) rb.isKinematic = wasKinematic;

                // Возвращаем гравитацию
                if (MovementModule.playerMotor != null)
                {
                    MovementModule.playerMotor.movement.gravity = 20f * MovementModule.gravityModifier;
                }
            }
        }

        private static void CheckTriggers()
        {
            // Ищем все триггеры рядом с игроком
            nearbyTriggers = Physics.OverlapSphere(MovementModule.playerMotor.transform.position, 2f);

            foreach (var trigger in nearbyTriggers)
            {
                if (trigger.isTrigger)
                {
                    // Вызываем OnTriggerEnter вручную
                    var scripts = trigger.GetComponents<MonoBehaviour>();
                    foreach (var script in scripts)
                    {
                        script.SendMessage("OnTriggerEnter", MovementModule.playerMotor.gameObject, SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
        }

        public static void ToggleNoClip()
        {
            IsEnabled = !IsEnabled;
            MelonLogger.Msg($"NoClipModule: {(IsEnabled ? "ENABLED" : "DISABLED")}");
        }
    }
}