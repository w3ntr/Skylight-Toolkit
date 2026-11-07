using MelonLoader;
using System;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace SkylightToolkit
{
    public static class WindowManager
    {
        private enum Tab { Movement, Speed, Freecam, NoClip, Graphics, Seed }

        public static bool IsMenuOpen => showWindow;
        private static KeyCode toggleKey = KeyCode.Insert;
        private static bool showWindow = false;
        private static Rect windowRect = new Rect(20, 20, 350, 420);
        private static Tab activeTab = Tab.Movement;

        public static void Initialize()
        {
            MelonLogger.Msg("WindowManager: Initialized (Toggle: Insert)");
        }

        public static void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                showWindow = !showWindow;
                if (showWindow) WinAPICursor.UnlockCursor();
                else WinAPICursor.LockCursor();
            }
        }

        public static void OnGUI()
        {
            if (!showWindow) return;
            windowRect = GUILayout.Window(1, windowRect, DrawWindow, "Skylight Toolkit");
        }

        private static void DrawWindow(int windowID)
        {
            GUILayout.BeginHorizontal();
            GUI.color = activeTab == Tab.Movement ? Color.cyan : Color.white;
            if (GUILayout.Button("Move", GUILayout.ExpandWidth(true))) activeTab = Tab.Movement;
            GUI.color = activeTab == Tab.Speed ? Color.cyan : Color.white;
            if (GUILayout.Button("Speed", GUILayout.ExpandWidth(true))) activeTab = Tab.Speed;
            GUI.color = activeTab == Tab.Freecam ? Color.cyan : Color.white;
            if (GUILayout.Button("Cam", GUILayout.ExpandWidth(true))) activeTab = Tab.Freecam;
            GUI.color = activeTab == Tab.NoClip ? Color.cyan : Color.white;
            if (GUILayout.Button("NoClip", GUILayout.ExpandWidth(true))) activeTab = Tab.NoClip;
            GUI.color = activeTab == Tab.Graphics ? Color.cyan : Color.white;
            if (GUILayout.Button("Gfx", GUILayout.ExpandWidth(true))) activeTab = Tab.Graphics;
            GUI.color = activeTab == Tab.Seed ? Color.cyan : Color.white;
            if (GUILayout.Button("Seed", GUILayout.ExpandWidth(true))) activeTab = Tab.Seed;
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            switch (activeTab)
            {
                case Tab.Movement: DrawMovementTab(); break;
                case Tab.Speed: DrawSpeedTab(); break;
                case Tab.Freecam: DrawFreecamTab(); break;
                case Tab.NoClip: DrawNoClipTab(); break;
                case Tab.Graphics: DrawGraphicsTab(); break;
                case Tab.Seed: DrawSeedTab(); break;
            }
            GUI.DragWindow(new Rect(0, 0, windowRect.width, 25));
        }

        private static void DrawMovementTab()
        {
            GUILayout.Label("=== Movement ===");

            GUILayout.BeginHorizontal();
            bool enabled = GUILayout.Toggle(MovementModule.moduleEnabled, "Enable Module", GUILayout.Width(120));
            if (enabled != MovementModule.moduleEnabled)
            {
                MovementModule.moduleEnabled = enabled;
            }
            GUILayout.EndHorizontal();

            GUILayout.Label($"Speed Multiplier: {MovementModule.speedMultiplier:F1}x");
            MovementModule.speedMultiplier = GUILayout.HorizontalSlider(MovementModule.speedMultiplier, 1f, 5f);

            GUILayout.Label($"Gravity: {MovementModule.gravityModifier:F1}x");
            MovementModule.gravityModifier = GUILayout.HorizontalSlider(MovementModule.gravityModifier, 0.1f, 3f);

            GUILayout.Label($"Max Fall Speed: {MovementModule.maxFallSpeed:F0}");
            MovementModule.maxFallSpeed = GUILayout.HorizontalSlider(MovementModule.maxFallSpeed, 10f, 200f);

            GUILayout.Label($"Jump Height: {MovementModule.jumpHeight:F1}x");
            MovementModule.jumpHeight = GUILayout.HorizontalSlider(MovementModule.jumpHeight, 0.5f, 3f);

            GUILayout.Space(10);
            if (GUILayout.Button("Reset Movement"))
            {
                MovementModule.ResetMovement();
            }

            GUILayout.Space(5);
            GUILayout.Label("--- Info ---");
            GUILayout.Label("Snappy: Instant acceleration/deceleration");
            GUILayout.Label("Higher speed = faster climbing");
        }

        private static void DrawSpeedTab()
        {
            GUILayout.Label("=== Speed Hack ===");
            GUILayout.BeginHorizontal();
            bool enabled = GUILayout.Toggle(SpeedHackModule.isEnabled, "Enable", GUILayout.Width(100));
            if (enabled != SpeedHackModule.isEnabled) SpeedHackModule.isEnabled = enabled;
            GUILayout.EndHorizontal();

            GUILayout.Label($"Time Scale: {SpeedHackModule.timeScale:F2}x");
            SpeedHackModule.timeScale = GUILayout.HorizontalSlider(SpeedHackModule.timeScale, 0.1f, 5f);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("0.5x")) { SpeedHackModule.timeScale = 0.5f; SpeedHackModule.isEnabled = true; }
            if (GUILayout.Button("2.0x")) { SpeedHackModule.timeScale = 2f; SpeedHackModule.isEnabled = true; }
            if (GUILayout.Button("Reset")) { SpeedHackModule.timeScale = 1f; SpeedHackModule.isEnabled = false; }
            GUILayout.EndHorizontal();
        }

        private static void DrawFreecamTab()
        {
            GUILayout.Label("=== Freecam (F3) ===");
            GUILayout.Label($"Status: {(FreecamModule.IsEnabled ? "ENABLED" : "DISABLED")}");
            if (GUILayout.Button(FreecamModule.IsEnabled ? "Disable" : "Enable")) FreecamModule.ToggleFreecam();

            GUILayout.Label($"Fly Speed: {FreecamModule.FlySpeed:F1}");
            FreecamModule.FlySpeed = GUILayout.HorizontalSlider(FreecamModule.FlySpeed, 1f, 200f); // Увеличили до 200

            GUILayout.Label($"Sensitivity: {FreecamModule.MouseSensitivity:F1}");
            FreecamModule.MouseSensitivity = GUILayout.HorizontalSlider(FreecamModule.MouseSensitivity, 0.5f, 10f);
        }

        private static void DrawNoClipTab()
        {
            GUILayout.Label("=== NoClip (F4) ===");
            GUILayout.Label($"Status: {(NoClipModule.IsEnabled ? "ENABLED" : "DISABLED")}");
            if (GUILayout.Button(NoClipModule.IsEnabled ? "Disable" : "Enable")) NoClipModule.ToggleNoClip();

            GUILayout.Label($"Fly Speed: {NoClipModule.FlySpeed:F1}");
            NoClipModule.FlySpeed = GUILayout.HorizontalSlider(NoClipModule.FlySpeed, 1f, 100f);

            GUILayout.Space(5);
            GUILayout.Label("--- Info ---");
            GUILayout.Label("Fly through platforms");
            GUILayout.Label("WASD - Move | Space/Ctrl - Up/Down");
        }

        private static void DrawGraphicsTab()
        {
            GUILayout.Label("=== Graphics ===");
            GUILayout.BeginHorizontal();
            bool day = GUILayout.Toggle(GraphicsModule.dayModeEnabled, "Day Mode", GUILayout.Width(100));
            if (day != GraphicsModule.dayModeEnabled) GraphicsModule.ToggleDayMode();
            GUILayout.EndHorizontal();

            GUILayout.Label($"Ambient: {GraphicsModule.ambientBrightness:F2}");
            GraphicsModule.ambientBrightness = GUILayout.HorizontalSlider(GraphicsModule.ambientBrightness, 0f, 2f);

            GUILayout.Label($"Fog Density: {GraphicsModule.fogDensity:F3}");
            GraphicsModule.fogDensity = GUILayout.HorizontalSlider(GraphicsModule.fogDensity, 0f, 0.1f);

            GUILayout.Space(10);
            GUILayout.Label("--- Performance ---");

            GUILayout.BeginHorizontal();
            bool vsync = GUILayout.Toggle(GraphicsModule.vsyncEnabled, "V-Sync", GUILayout.Width(100));
            if (vsync != GraphicsModule.vsyncEnabled) GraphicsModule.vsyncEnabled = vsync;
            GUILayout.EndHorizontal();

            GUILayout.Label($"FPS Cap: {(GraphicsModule.fpsCap == -1 ? "Unlimited" : GraphicsModule.fpsCap.ToString())}");
            int currentCap = GraphicsModule.fpsCap == -1 ? 720 : GraphicsModule.fpsCap;
            int newCap = (int)GUILayout.HorizontalSlider(currentCap, 15, 2000);
            if (newCap != currentCap) GraphicsModule.fpsCap = (newCap == 2000) ? -1 : newCap;

            GUILayout.Label($"Far Clip: {GraphicsModule.farClipPlane:F0}m");
            GraphicsModule.farClipPlane = GUILayout.HorizontalSlider(GraphicsModule.farClipPlane, 100f, 5000f);

            GUILayout.Space(10);
            if (GUILayout.Button("Reset to Original"))
            {
                GraphicsModule.ResetToOriginal();
            }
        }

        private static void DrawSeedTab()
        {
            GUILayout.Label("=== Seed Manager (F6) ===");
            GUILayout.Label($"Current: {GenerateLevel.currentSeed}");
            GUILayout.Label("Enter seed:");
            SeedManager.seedInput = GUILayout.TextField(SeedManager.seedInput);

            if (GUILayout.Button("Apply Seed"))
            {
                SeedManager.ApplySeed();
            }
            GUILayout.Label("Reload scene to apply new seed!");
        }
    }
}