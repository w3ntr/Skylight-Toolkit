using MelonLoader;
using System;
using UnityEngine;
using Object = UnityEngine.Object;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace SkylightToolkit
{
    public static class FreecamModule
    {
        public static bool IsEnabled = false;
        public static float FlySpeed = 15f;
        public static float FastFlyMultiplier = 3f;
        public static float MouseSensitivity = 2f;

        private static KeyCode toggleKey = KeyCode.F3;
        private static GameObject freecamObj;
        private static Camera freecamComp;
        private static Camera originalMainCam;
        private static float savedTimeScale = 1f;
        private static float rotX = 0f;
        private static float rotY = 0f;
        private static float lastRealTime = 0f;

        private static MonoBehaviour robotControllerScript;
        private static MonoBehaviour fpsInputControllerScript;

        public static void Initialize()
        {
            MelonLogger.Msg("FreecamModule: Initialized (Toggle: F3)");
        }

        public static void Update()
        {
            if (Input.GetKeyDown(toggleKey)) ToggleFreecam();
            if (!IsEnabled) return;

            if (freecamObj == null || originalMainCam == null) { DisableFreecam(); return; }

            float currentRealTime = Time.realtimeSinceStartup;
            float realDeltaTime = currentRealTime - lastRealTime;
            lastRealTime = currentRealTime;

            if (realDeltaTime > 0.1f) realDeltaTime = 0.1f;
            if (realDeltaTime < 0f) realDeltaTime = 0f;

            rotX += Input.GetAxis("Mouse X") * MouseSensitivity * 2f;
            rotY -= Input.GetAxis("Mouse Y") * MouseSensitivity * 2f;
            rotY = Mathf.Clamp(rotY, -89f, 89f);

            freecamObj.transform.rotation = Quaternion.Euler(rotY, rotX, 0f);

            float speed = FlySpeed * (Input.GetKey(KeyCode.LeftShift) ? FastFlyMultiplier : 1f) * realDeltaTime;
            Vector3 move = Vector3.zero;

            if (Input.GetKey(KeyCode.W)) move += freecamObj.transform.forward;
            if (Input.GetKey(KeyCode.S)) move -= freecamObj.transform.forward;
            if (Input.GetKey(KeyCode.D)) move += freecamObj.transform.right;
            if (Input.GetKey(KeyCode.A)) move -= freecamObj.transform.right;
            if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Space)) move += Vector3.up;
            if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftControl)) move -= Vector3.up;

            freecamObj.transform.position += move * speed;
        }

        public static void ToggleFreecam()
        {
            if (!IsEnabled) EnableFreecam();
            else DisableFreecam();
        }

        private static void EnableFreecam()
        {
            originalMainCam = Camera.main;
            if (originalMainCam == null) { MelonLogger.Msg("FreecamModule: ERROR - No main camera!"); return; }

            IsEnabled = true;
            savedTimeScale = Time.timeScale;
            Time.timeScale = 0f; // Останавливаем игру, но Update работает через realtimeSinceStartup
            lastRealTime = Time.realtimeSinceStartup;

            freecamObj = new GameObject("SkylightFreecam");
            freecamComp = freecamObj.AddComponent<Camera>();
            freecamComp.CopyFrom(originalMainCam);

            freecamObj.transform.position = originalMainCam.transform.position;
            freecamObj.transform.rotation = originalMainCam.transform.rotation;
            rotX = freecamObj.transform.eulerAngles.y;
            rotY = freecamObj.transform.eulerAngles.x;

            originalMainCam.enabled = false;
            Screen.lockCursor = true;
            Screen.showCursor = false;

            DisablePlayerScripts();
            MelonLogger.Msg("FreecamModule: Enabled");
        }

        private static void DisablePlayerScripts()
        {
            var rc = Object.FindObjectsOfType(typeof(RobotController)) as RobotController[];
            if (rc != null && rc.Length > 0)
            {
                robotControllerScript = rc[0];
                robotControllerScript.enabled = false;
            }

            var fps = Object.FindObjectsOfType(typeof(MonoBehaviour)) as MonoBehaviour[];
            foreach (var mb in fps)
            {
                if (mb.GetType().Name == "FPSInputController")
                {
                    fpsInputControllerScript = mb;
                    fpsInputControllerScript.enabled = false;
                    break;
                }
            }
        }

        private static void DisableFreecam()
        {
            IsEnabled = false;
            Time.timeScale = savedTimeScale > 0f ? savedTimeScale : 1f;
            if (originalMainCam != null) originalMainCam.enabled = true;
            if (freecamObj != null) Object.Destroy(freecamObj);

            if (robotControllerScript != null) robotControllerScript.enabled = true;
            if (fpsInputControllerScript != null) fpsInputControllerScript.enabled = true;

            Screen.lockCursor = false;
            Screen.showCursor = true;
            MelonLogger.Msg("FreecamModule: Disabled");
        }
    }
}