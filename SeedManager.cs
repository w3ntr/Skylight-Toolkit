using MelonLoader;
using UnityEngine;

namespace SkylightToolkit
{
    public static class SeedManager
    {
        public static bool IsMenuOpen = false;
        public static int currentSeed = 0;
        public static bool isEnabled = false;
        public static string seedInput = "";
        private static KeyCode toggleKey = KeyCode.F6;
        private static KeyCode reloadKey = KeyCode.R;

        public static void Initialize()
        {
            MelonLogger.Msg("SeedManager: Initialized (F6 - Toggle Menu, R - Reload Scene)");

            // В Unity 4.x нет SceneManager, используем Application.loadedLevel
            MelonLogger.Msg($"SeedManager: Current scene index: {Application.loadedLevel}");
            MelonLogger.Msg($"SeedManager: Current scene name: {Application.loadedLevelName}");
        }

        public static void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                IsMenuOpen = !IsMenuOpen;
                if (IsMenuOpen) WinAPICursor.UnlockCursor();
                else WinAPICursor.LockCursor();
            }

            // Перезапуск сцены по R
            if (Input.GetKeyDown(reloadKey))
            {
                ReloadScene();
            }
        }

        private static void ReloadScene()
        {
            // В Unity 4.x используем Application.LoadLevel
            int currentLevel = Application.loadedLevel;
            MelonLogger.Msg($"SeedManager: Reloading scene {currentLevel} ({Application.loadedLevelName})...");
            Application.LoadLevel(currentLevel);
        }

        public static void LoadMainMenu()
        {
            // Загружаем сцену 0 (обычно главное меню)
            MelonLogger.Msg("SeedManager: Loading scene 0 (main menu)...");
            Application.LoadLevel(0);
        }

        public static void ApplySeed()
        {
            if (int.TryParse(seedInput, out int newSeed))
            {
                currentSeed = newSeed;
                isEnabled = true;
                GenerateLevel.useSameSeed = true;
                GenerateLevel.currentSeed = currentSeed;
                MelonLogger.Msg($"SeedManager: Seed set to {currentSeed}. Reload scene (R) to apply!");
            }
        }
    }
}