using HarmonyLib;
using MelonLoader;
using System.Reflection;
using UnityEngine;

namespace SkylightToolkit
{
    public static class CursorPatch
    {
        private static HarmonyLib.Harmony harmony;
        private static bool isPatched = false;
        private static bool isInsidePatch = false;

        public static void Initialize()
        {
            if (isPatched) return;
            harmony = new HarmonyLib.Harmony("com.w3ntr.skylighttoolkit.cursor");
            MethodInfo targetMethod = AccessTools.Method(typeof(Screen), "set_lockCursor");
            if (targetMethod != null)
            {
                MethodInfo prefixMethod = typeof(CursorPatch).GetMethod(nameof(LockCursorPrefix), BindingFlags.Static | BindingFlags.NonPublic);
                harmony.Patch(targetMethod, prefix: new HarmonyMethod(prefixMethod));
                MelonLogger.Msg("CursorPatch: Successfully patched Screen.set_lockCursor!");
            }
            isPatched = true;
        }

        private static bool LockCursorPrefix(ref bool value)
        {
            if (isInsidePatch) return true;
            if (WindowManager.IsMenuOpen && value == true)
            {
                isInsidePatch = true;
                value = false;
                isInsidePatch = false;
                return false;
            }
            return true;
        }

        public static void Cleanup()
        {
            if (harmony != null)
            {
                harmony.UnpatchSelf();
                isPatched = false;
            }
        }
    }
}