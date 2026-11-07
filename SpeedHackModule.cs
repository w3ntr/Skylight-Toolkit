using MelonLoader;
using UnityEngine;

namespace SkylightToolkit
{
    public static class SpeedHackModule
    {
        public static bool isEnabled = false;
        public static float timeScale = 1f;

        public static void Initialize()
        {
            MelonLogger.Msg("SpeedHackModule: Initialized");
        }

        public static void Update()
        {
            if (isEnabled)
            {
                Time.timeScale = timeScale;
            }
            else if (Time.timeScale != 1f && !FreecamModule.IsEnabled)
            {
                Time.timeScale = 1f;
            }
        }
    }
}