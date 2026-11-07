using MelonLoader;
using System.Reflection;

[assembly: MelonInfo(typeof(SkylightToolkit.Main), "Skylight Toolkit", "1.0.0", "w3ntr")]
[assembly: MelonGame("Parsec Productions", "Skylight")]

namespace SkylightToolkit
{
    public class Main : MelonMod
    {
        public override void OnApplicationStart()
        {
            MelonLogger.Msg("=== Skylight Toolkit v1.0.0 Loaded ===");
            CursorPatch.Initialize();
            WindowManager.Initialize();
            MovementModule.Initialize();
            SpeedHackModule.Initialize();
            FreecamModule.Initialize();
            NoClipModule.Initialize();
            GraphicsModule.Initialize();
            SeedManager.Initialize();
            MelonLogger.Msg("All modules initialized successfully!");
        }

        public override void OnApplicationQuit()
        {
            CursorPatch.Cleanup();
        }

        public override void OnUpdate()
        {
            WindowManager.Update();
            MovementModule.Update();
            SpeedHackModule.Update();
            FreecamModule.Update();
            NoClipModule.Update();
            GraphicsModule.Update();
            SeedManager.Update();
        }

        public override void OnGUI()
        {
            WindowManager.OnGUI();
        }
    }
}