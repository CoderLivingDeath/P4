# AGENTS.md

Unity 6 (6000.7.0a4) 2D URP game. Zenject DI. Russian-speaking owner. Single `master` branch on a public GitHub remote.

## Tooling

- No CLI build/lint/test path — verification happens in the Unity Editor. Do not "build" via shell; compiled root `*.csproj`/`P4.slnx` are Unity-generated and gitignored.
- `Library/`, `Temp/`, `Logs/`, `UserSettings/` are gitignored — never commit or reference them.
- `.meta` files are Unity-generated and tracked — never create/edit/delete them by hand; add them for any new asset.
- `Assets/Plugins/` contains vendored libraries (Zenject full source, UniTask, DOTween, TextMesh Pro). Treat as third-party; don't refactor inside.

## Architecture

- DI wiring lives in Zenject installers: `Assets/Resources/ProjectContext.prefab` (project context) with `Assets/Project/Scripts/DI/ProjectInstaller.cs` (project-wide, currently empty) and `GameplayInstaller.cs` (scene). Bind new services in installers — do not use `FindObjectOfType`/singletons.
- Project scripts live in `Assets/Project/Scripts/` as plain `Assembly-CSharp` classes, **no namespaces**. Follow that.
- Main scene: `Assets/Project/Scenes/Gameplay.unity` (the old `Assets/Scenes/` path is gone). UI sliders `Slider_Master/Slider_Music/Slider_Sounds` live there.
- Input uses the new Input System package (`InputManager`, `InputActionAsset`).

## Audio system (hard-won)

- `AudioService` (MonoBehaviour, `[DefaultExecutionOrder(-100)]`): loads volumes from PlayerPrefs in `Start` and applies them to the `MainMixer` via exposed params `MasterVolume`/`MusicVolume`/`SoundVolume` (asset: `Assets/Settings/Audio/MainMixer.mixer`). Do NOT move this to `Awake` — the mixer's start snapshot re-applies its baked values (incl. `MusicVolume: -10.2dB`) after `Awake`, clobbering `SetFloat`.
- `AudioSlidersManager`: only mirrors service values onto sliders with `SetValueWithoutNotify`, and pushes changes to the service. Never set `slider.value` during init — it fires `onValueChanged` and resets the saved PlayerPrefs.
- Volumes are persisted in PlayerPrefs keys `MasterVolume`/`MusicVolume`/`SoundVolume`.

## Packages

- `Packages/manifest.json` pulls git deps (LitMotion, `com.besty.unity-skills`) plus URP/2D/Cinemachine/InputSystem/UGUI. Don't drop in unlisted packages without updating the manifest.
