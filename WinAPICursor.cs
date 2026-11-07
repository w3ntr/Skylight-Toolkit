using MelonLoader;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SkylightToolkit
{
    public static class WinAPICursor
    {
        [DllImport("user32.dll")]
        private static extern int ShowCursor(bool bShow);

        private static bool isUnlocked = false;
        private static int callCount = 0;

        public static void UnlockCursor()
        {
            if (isUnlocked) return;
            for (int i = 0; i < 10; i++)
            {
                ShowCursor(true);
            }
            callCount = 10;
            Screen.lockCursor = false;
            Screen.showCursor = true;
            isUnlocked = true;
            MelonLogger.Msg("WinAPI: Cursor unlocked");
        }

        public static void LockCursor()
        {
            if (!isUnlocked) return;
            for (int i = 0; i < callCount; i++)
            {
                ShowCursor(false);
            }
            Screen.lockCursor = true;
            Screen.showCursor = false;
            callCount = 0;
            isUnlocked = false;
            MelonLogger.Msg("WinAPI: Cursor locked");
        }
    }
}