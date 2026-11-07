using MelonLoader;
using UnityEngine;

namespace SkylightToolkit
{
    public static class GraphicsModule
    {
        public static bool dayModeEnabled = false;
        public static float ambientBrightness = 0.5f;
        public static float fogDensity = 0.02f;
        public static int fpsCap = -1;
        public static bool vsyncEnabled = true;
        public static float farClipPlane = 1000f;

        private static Color originalAmbient;
        private static float originalFogDensity;
        private static bool originalFogEnabled;
        private static float originalFarClip;
        private static Camera mainCam;
        private static bool valuesSaved = false;

        public static void Initialize()
        {
            if (!valuesSaved)
            {
                originalAmbient = RenderSettings.ambientLight;
                originalFogDensity = RenderSettings.fogDensity;
                originalFogEnabled = RenderSettings.fog;

                mainCam = Camera.main;
                if (mainCam != null)
                {
                    originalFarClip = mainCam.farClipPlane;
                    farClipPlane = originalFarClip; // Устанавливаем текущее значение
                    MelonLogger.Msg($"GraphicsModule: Saved original FarClip: {originalFarClip}");
                }
                else
                {
                    originalFarClip = 1000f; // Дефолтное значение
                    farClipPlane = 1000f;
                    MelonLogger.Msg("GraphicsModule: Camera.main not found, using default FarClip: 1000");
                }

                valuesSaved = true;
            }

            MelonLogger.Msg("GraphicsModule: Initialized");
        }

        public static void Update()
        {
            // Применяем каждый кадр
            if (dayModeEnabled)
            {
                RenderSettings.ambientLight = new Color(ambientBrightness, ambientBrightness, ambientBrightness);
                RenderSettings.fogDensity = fogDensity;
                RenderSettings.fog = true;
            }

            Application.targetFrameRate = fpsCap;
            QualitySettings.vSyncCount = vsyncEnabled ? 1 : 0;

            // Применяем FarClip ТОЛЬКО если значение > 0
            if (farClipPlane > 0f)
            {
                Camera[] allCams = Object.FindObjectsOfType(typeof(Camera)) as Camera[];
                if (allCams != null)
                {
                    foreach (var cam in allCams)
                    {
                        if (cam != null && cam.farClipPlane != farClipPlane)
                        {
                            cam.farClipPlane = farClipPlane;
                        }
                    }
                }
            }
        }

        public static void ToggleDayMode()
        {
            dayModeEnabled = !dayModeEnabled;
            if (!dayModeEnabled)
            {
                // Восстанавливаем ОРИГИНАЛЬНЫЕ значения
                RenderSettings.ambientLight = originalAmbient;
                RenderSettings.fogDensity = originalFogDensity;
                RenderSettings.fog = originalFogEnabled;
            }
            MelonLogger.Msg($"GraphicsModule: Day Mode {(dayModeEnabled ? "ON" : "OFF")}");
        }

        public static void ResetToOriginal()
        {
            dayModeEnabled = false;
            ambientBrightness = 0.5f;
            fogDensity = 0.02f;
            fpsCap = -1;
            vsyncEnabled = true;
            farClipPlane = originalFarClip;
            
            RenderSettings.ambientLight = originalAmbient;
            RenderSettings.fogDensity = originalFogDensity;
            RenderSettings.fog = originalFogEnabled;
            Application.targetFrameRate = -1;
            QualitySettings.vSyncCount = 1;
            
            Camera[] allCams = Object.FindObjectsOfType(typeof(Camera)) as Camera[];
            if (allCams != null)
            {
                foreach (var cam in allCams)
                {
                    if (cam != null)
                    {
                        cam.farClipPlane = originalFarClip;
                    }
                }
            }
            
            MelonLogger.Msg("GraphicsModule: Reset to original settings");
        }
    }
}