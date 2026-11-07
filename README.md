# Skylight Toolkit

[![MelonLoader](https://img.shields.io/badge/MelonLoader-0.5.4-blueviolet.svg)](https://github.com/LavaGang/MelonLoader)
[![Platform](https://img.shields.io/badge/Platform-Windows-blue.svg)]()

**Skylight Toolkit** is a modification for the game Skylight, built on `MelonLoader v0.5.4`. The game is a first-person vertical platformer with procedurally generated levels, running on Unity 4.x (Win32).

The project was created as a tool for researching game mechanics, analyzing procedural generation, and expanding movement capabilities. The main focus is on modular architecture, clean code, and compatibility with older Unity versions.

You can download the game here: [Skylight](https://nition.itch.io/skylight).

## Architecture Features

- **Modular approach.** Each functional block is implemented as a separate `static` class with a clear lifecycle (`Initialize`, `Update`, `OnGUI`).
- **Unity 4.x compatibility.** Uses legacy APIs such as `Application.LoadLevel` instead of `SceneManager`, and `Object.FindObjectsOfType(typeof(T))` instead of generic methods.
- **Reliable cursor management.** A combined approach using a Harmony patch on `Screen.set_lockCursor` and a direct WinAPI `ShowCursor` call via `user32.dll` to guarantee cursor release when the UI is open.

## Modules

### Movement Module

Responsible for modifying character physics. Interacts with the `RobotController` class and the `CharacterMotorMovement` struct.

- **Snappy Movement.** Implemented via a Harmony Transpiler that modifies the `ApplyInputVelocityChange` method. Removes the smooth velocity change limit (`b.sqrMagnitude > num * num`), providing instant acceleration and stopping.
- **Speed Multiplier.** Multiplier for base speed (`maxForwardSpeed`, `maxSidewaysSpeed`, `maxBackwardsSpeed`).
- **Gravity Modifier.** Multiplier for gravity force.
- **Max Fall Speed.** Limits the maximum falling speed.
- **Jump Height.** Modifies jump height via a Harmony Postfix on the `DoJumpPad` method, which multiplies vertical velocity (`velocity.y`) by a given coefficient.
- **Module Toggle.** Allows fully disabling the module and reverting to original physics values.
- **Reset Movement.** Resets all movement parameters to default values.

### Speed Hack Module

Controls the global game time scale via `Time.timeScale`. Allows slowing down or speeding up gameplay. Correctly interacts with the Freecam Module, since the free camera uses `Time.realtimeSinceStartup`.

### Freecam Module

Implements a free camera for level exploration.

- On activation, disables player control scripts (`RobotController`, `FPSInputController`) and the original main camera.
- Creates a new camera object and copies parameters from the original one.
- Camera movement is independent of `Time.timeScale`, allowing it to be used together with Speed Hack.
- Supports accelerated flight while holding `LeftShift`.

### NoClip Module

Allows the character to pass through collisions.

- Disables the `CharacterController` component.
- Temporarily zeroes gravity in `CharacterMotorMovement` to prevent infinite falling.
- Adds manual character flight control with nearby trigger detection.

### Graphics Module

Provides tools for configuring rendering and performance.

- **Day Mode.** Modifies `RenderSettings.ambientLight` and `RenderSettings.fogDensity` to improve visibility.
- **FPS Cap.** Sets the frame rate limit via `Application.targetFrameRate`.
- **V-Sync.** Controls vertical synchronization via `QualitySettings.vSyncCount`.
- **Far Clip Plane.** Changes the draw distance for all active cameras in the scene.
- **Reset to Original.** Reverts all graphics settings to values saved during mod initialization.

### Seed Manager

A tool for controlling procedural level generation.

- Allows setting a fixed seed by assigning the static fields `GenerateLevel.useSameSeed` and `GenerateLevel.currentSeed`.
- Ensures deterministic generation of platforms and clouds when reloading the scene.
- Supports quick restart of the current scene to apply a new seed.

## Controls

### Global Keys

- `Insert` — Open/close the main mod window.
- `F3` — Toggle Freecam.
- `F4` — Toggle NoClip.
- `F6` — Open/close the Seed Manager panel.
- `R` — Reload the current scene (apply a new seed).

### Freecam Controls

- `W`, `A`, `S`, `D` — Move the camera.
- `Space` or `E` — Move up.
- `Q` or `LeftControl` — Move down.
- `LeftShift` — Accelerated flight.

### NoClip Controls

- `W`, `A`, `S`, `D` — Move the character.
- `Space` — Move up.
- `LeftControl` — Move down.
- `LeftShift` — Accelerated flight.

## Installation

1. Make sure `MelonLoader v0.5.4` is installed in the game folder.
2. Copy the `SkylightToolkit.dll` file into the `Mods` folder inside the game directory.
3. Launch the game. The MelonLoader console should display `=== Skylight Toolkit v1.0.0 Loaded ===`.
4. Press `Insert` to open the interface.

## Credits

Developed by **w3ntr**. The project is distributed for educational purposes.

## License

This project is fully open-source. You are free to take it, fork it, and build whatever you want on top of it.
