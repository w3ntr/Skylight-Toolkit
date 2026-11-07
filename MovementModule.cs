using HarmonyLib;
using MelonLoader;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace SkylightToolkit
{
    public static class MovementModule
    {
        public static RobotController playerMotor;

        public static bool moduleEnabled = true; // ДОБАВЛЕНО
        public static bool snappyMovement = true;
        public static float speedMultiplier = 1f;
        public static float gravityModifier = 1f;
        public static float maxFallSpeed = 40f;
        public static float jumpHeight = 1f;

        private static float origMaxForward = 10f;
        private static float origMaxSideways = 10f;
        private static float origNaturalDecel = 1f;
        private static float origMaxAirAccel = 20f;
        private static float origGravity = 20f;
        private static float origMaxFall = 40f;

        private static HarmonyLib.Harmony harmony;
        private static HarmonyLib.Harmony jumpHarmony;

        public static void Initialize()
        {
            MelonLogger.Msg("MovementModule: Initialized");

            // Патчим ApplyInputVelocityChange для snappy movement
            var rcType = typeof(RobotController);
            var applyInputMethod = rcType.GetMethod("ApplyInputVelocityChange", BindingFlags.NonPublic | BindingFlags.Instance);

            if (applyInputMethod != null)
            {
                harmony = new HarmonyLib.Harmony("com.w3ntr.skylighttoolkit.movement");
                harmony.Patch(applyInputMethod, transpiler: new HarmonyMethod(typeof(MovementModule), nameof(ApplyInputVelocityChangeTranspiler)));
                MelonLogger.Msg("MovementModule: Patched ApplyInputVelocityChange!");
            }

            // Патчим DoJumpPad для jumpHeight
            var doJumpPadMethod = rcType.GetMethod("DoJumpPad", BindingFlags.Public | BindingFlags.Instance);
            if (doJumpPadMethod != null)
            {
                jumpHarmony = new HarmonyLib.Harmony("com.w3ntr.skylighttoolkit.jump");
                jumpHarmony.Patch(doJumpPadMethod, postfix: new HarmonyMethod(typeof(MovementModule), nameof(DoJumpPadPostfix)));
                MelonLogger.Msg("MovementModule: Patched DoJumpPad!");
            }
        }

        public static void Update()
        {
            if (playerMotor == null)
            {
                var motors = Object.FindObjectsOfType(typeof(RobotController)) as RobotController[];
                if (motors != null && motors.Length > 0)
                {
                    playerMotor = motors[0];
                    var m = playerMotor.movement;
                    origMaxForward = m.maxForwardSpeed;
                    origMaxSideways = m.maxSidewaysSpeed;
                    origNaturalDecel = m.naturalDeceleration;
                    origMaxAirAccel = m.maxAirAcceleration;
                    origGravity = m.gravity;
                    origMaxFall = m.maxFallSpeed;
                    MelonLogger.Msg("MovementModule: Found RobotController!");
                }
            }

            if (playerMotor != null && playerMotor.movement != null)
            {
                ApplyMods();
            }
        }

        private static void ApplyMods()
        {
            var m = playerMotor.movement; // ОДНО объявление!

            if (!moduleEnabled)
            {
                // ВОЗВРАЩАЕМ ОРИГИНАЛЬНЫЕ ЗНАЧЕНИЯ
                m.maxForwardSpeed = origMaxForward;
                m.maxSidewaysSpeed = origMaxSideways;
                m.maxBackwardsSpeed = origMaxForward;
                m.naturalDeceleration = origNaturalDecel;
                m.maxAirAcceleration = origMaxAirAccel;
                m.maxAirDeceleration = 50f;
                m.gravity = origGravity;
                m.maxFallSpeed = origMaxFall;
                return;
            }

            if (snappyMovement)
            {
                m.maxForwardSpeed = origMaxForward * 3f * speedMultiplier;
                m.maxSidewaysSpeed = origMaxSideways * 5f * speedMultiplier;
                m.maxBackwardsSpeed = origMaxForward * 3f * speedMultiplier;
                m.naturalDeceleration = 10000f;
                m.maxAirAcceleration = 10000f;
                m.maxAirDeceleration = 10000f;
            }
            else
            {
                m.maxForwardSpeed = origMaxForward * speedMultiplier;
                m.maxSidewaysSpeed = origMaxSideways * speedMultiplier;
                m.maxBackwardsSpeed = origMaxForward * speedMultiplier;
                m.naturalDeceleration = origNaturalDecel;
                m.maxAirAcceleration = origMaxAirAccel;
                m.maxAirDeceleration = 50f;
            }

            m.gravity = origGravity * gravityModifier;
            m.maxFallSpeed = maxFallSpeed;
        }

        // Postfix для DoJumpPad - умножаем velocity.y на jumpHeight
        private static void DoJumpPadPostfix(RobotController __instance, float jumpPower)
        {
            if (jumpHeight != 1f && __instance.movement != null)
            {
                __instance.movement.velocity.y *= jumpHeight;
                MelonLogger.Msg($"DoJumpPad Postfix: velocity.y *= {jumpHeight}");
            }
        }

        public static void ResetMovement()
        {
            moduleEnabled = true;
            speedMultiplier = 1f;
            gravityModifier = 1f;
            maxFallSpeed = 40f;
            jumpHeight = 1f;
            snappyMovement = true;
            MelonLogger.Msg("MovementModule: Reset to default values");
        }

        private static IEnumerable<CodeInstruction> ApplyInputVelocityChangeTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].opcode == OpCodes.Callvirt &&
                    codes[i].operand is MethodInfo mi &&
                    mi.Name == "get_sqrMagnitude")
                {
                    int skipTo = i + 1;
                    while (skipTo < codes.Count &&
                           codes[skipTo].opcode != OpCodes.Brfalse &&
                           codes[skipTo].opcode != OpCodes.Brtrue &&
                           codes[skipTo].opcode != OpCodes.Beq &&
                           codes[skipTo].opcode != OpCodes.Bne_Un)
                    {
                        skipTo++;
                    }

                    for (int j = i; j <= skipTo && j < codes.Count; j++)
                    {
                        codes[j] = new CodeInstruction(OpCodes.Nop);
                    }

                    break;
                }
            }

            return codes.AsEnumerable();
        }
    }
}