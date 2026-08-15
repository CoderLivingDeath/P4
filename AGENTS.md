# AGENTS.md

Unity 6 (6000.7.0a4, alpha) 2D URP game. Zenject DI. Single `master` branch; origin is `https://github.com/CoderLivingDeath/P4.git`.

## Tooling

- No CLI build/lint/test path — verification happens in the Unity Editor. Root `*.csproj`/`P4.slnx` are Unity-generated and gitignored; don't build or edit them.
- `Library/`, `Temp/`, `Logs/`, `UserSettings/` are gitignored — never commit or reference them.
- `.meta` files are Unity-generated and tracked — never hand-edit them; a new asset needs its `.meta` (regenerate by importing in the Editor).
- `Assets/Plugins/` contains vendored libraries (Zenject full source, UniTask, TextMesh Pro). Treat as third-party; don't refactor inside. Tweening is via **LitMotion** (git dependency), not DOTween.

## Architecture

- DI wiring lives in Zenject installers: `Assets/Resources/ProjectContext.prefab` (project context) → `Assets/Project/Scripts/DI/ProjectInstaller.cs` (currently empty); scene installer is `DI/GameplayInstaller.cs`. Bind new services in installers — no `FindObjectOfType`/singletons.
- Project code lives in `Assets/Project/Scripts/` as plain `Assembly-CSharp` classes with **no namespaces**. Follow that.
- Main scene: `Assets/Project/Scenes/Gameplay.unity` (only scene in build settings). UI sliders `Slider_Master`/`Slider_Music`/`Slider_Sounds` live there.
- Input uses the new Input System package (`InputActionAsset`, `InputActionReference`).

## Audio system (hard-won)

- `AudioService` (MonoBehaviour, `[DefaultExecutionOrder(-100)]`): creates the sound pool in `Awake`, but **loads volumes from PlayerPrefs in `Start`** and applies them to `MainMixer.mixer` via exposed params `MasterVolume`/`MusicVolume`/`SoundVolume`. Do NOT move loading to `Awake` — the mixer's start snapshot re-applies its baked values after `Awake`, clobbering `SetFloat`.
- `AudioSlidersManager`: only mirrors service values onto sliders with `SetValueWithoutNotify`, and pushes user changes back to the service. Never set `slider.value` during init — it fires `onValueChanged` and resets the saved PlayerPrefs.
- Volumes persist in PlayerPrefs keys `MasterVolume`/`MusicVolume`/`SoundVolume`.

## Packages

- `Packages/manifest.json` pulls git deps (LitMotion, `com.besty.unity-skills`) plus URP/2D/Cinemachine/InputSystem/UGUI. Don't add packages without updating the manifest.
