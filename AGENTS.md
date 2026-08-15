# AGENTS.md

Unity 6 (6000.7.0a4, alpha) 2D URP game. Zenject DI. Single `master` branch; origin is `https://github.com/CoderLivingDeath/P4.git`.

## Tooling

- **Unity Skills REST server is the verification/automation path**: the `com.besty.unity-skills` package exposes a local REST server on `localhost:8090`–`8100`. Full protocol (modes, grant flow, compile status, scene/asset/component skills) lives in `.opencode/skills/unity-skills/SKILL.md`; Python helper at `.opencode/skills/unity-skills/scripts/unity_skills.py`. Currently `auto` mode — writes execute directly (chat-level confirm still expected for scene/asset mutations). All asset/scene mutations while the editor is open go through these REST skills.
- **Unity CLI (`unity.exe`, bound; `Library/UnitySkills/cli_config.json`)** is the second automation path for driving the **already-open editor**: the project has the Unity Pipeline package (`com.unity.pipeline 0.5.0-exp.1`) with its HTTP server on `127.0.0.1:7800`, so `unity command <cmd> --project-path "D:\repos\P4" -- <args>` works (143 commands: `create_asset`, `set_serialized_field`, `find_gameobjects`, `get_scene_hierarchy`, `get_serialized_fields`, …). List them with `unity command --project-path "D:\repos\P4" --query <term> --detail full --format json`; help for the CLI is in `.opencode/skills/unity-skills/skills/unity-cli/SKILL.md` (load it via the `unity-cli` skill). `unity run`/`build` are **off** (`cliRun`/`cliBuild: false`) and `run` needs the editor closed — don't use them. When the editor is closed use the CLI `open`/`test` path instead.
- Compile check after editing scripts: a script edit triggers Domain Reload (server briefly answers `503`), then `GET /compile/status` reports `lastCompilation` success + exact errors/warnings. Do this after every script change.
- Root `*.csproj`/`P4.slnx` are Unity-generated and gitignored — don't build or edit them. `Library/`, `Temp/`, `Logs/`, `UserSettings/` are gitignored — never commit or reference them.
- `.meta` files are Unity-generated and tracked — never hand-edit; a new asset gets its `.meta` after the Editor imports it (`asset_refresh` if it lags).
- `Assets/Plugins/` is vendored third-party (Zenject, UniTask, TextMesh Pro) — don't refactor. LitMotion (git dep) source for verifying API names lives in `Library/PackageCache/com.annulusgames.lit-motion@*/Runtime/`.
- `.opencode/` (opencode config + unity-skills) is tracked; `__pycache__/` inside it is gitignored — never commit Python bytecode.
- **Skills for this repo** live in `.opencode/skills/unity-skills/skills/` (e.g. `unity-cli`, `scriptableobject`, `component`, `asset`, `scene`) and load via the `skill` tool (e.g. `unity-cli`, `unity-skills`). Full skill index: `.opencode/skills/unity-skills/skills/SKILL.md`. The `unity-cli` skill is where the CLI usage protocol is documented — load it before driving the editor via `unity command`.

## Architecture

- DI wiring in Zenject installers: `Assets/Resources/ProjectContext.prefab` → `Assets/Project/Scripts/DI/ProjectInstaller.cs` (empty); scene installer `DI/GameplayInstaller.cs` (empty). Bind services in installers — no `FindObjectOfType`/singletons; scene components are wired via `[SerializeField]` (e.g. `ResourcesOverviewBehaviour` → `ResourcesManager` on `[SCENE]`).
- Project code lives in `Assets/Project/Scripts/` as plain `Assembly-CSharp` classes with **no namespaces**. Follow that.
- Main scene: `Assets/Project/Scenes/Gameplay.unity` (only scene in build settings). Hunt UI: `MainCanvas` → `ResourcesBar` (TMP `Food`/`Eggs`); `HuntCanvas` → `StartHuntButton`/`Slider`/`HutnCounter`. Input uses the new Input System package (`InputActionReference`).

## Gameplay components (Common/)

- `ResourcesManager`: food/eggs never drop below 0 — **every mutation flows through `SetFood`/`SetEggs`** (`Mathf.Max(0, …)`, also clamps inspector edits via `OnValidate`). Fires C# events `FoodChanged`/`EggsChanged` (`Action<int>`); subscribe in `OnEnable`, unsubscribe in `OnDisable`. Don't add setters that bypass `SetFood`/`SetEggs`.
- `ResourcesOverviewBehaviour`: subscribes to those events and writes TMP text via `string.Format`.
- `HuntController`: button-triggered countdown (slider 1→0, TMP counter); UnityEvents `_onHuntStart`/`_onHuntStep`/`_onHuntEnd`/`_onHuntSuccess`/`_onHuntFail` (success roll `Random.value <= _successChance`).
- `HuntResultOverviewBehaviour`: public `OnHuntSuccess()`/`OnHuntFail()` wired to HuntController UnityEvents in the scene (result text + separate dino text).
- `CanvasGroupFader`: public `FadeIn()`/`FadeOut()` (for inspector UnityEvents) tweening CanvasGroup alpha via LitMotion; cancels in-flight fade on repeat/`OnDisable`.
- `CameraController`: moves in integer screen-steps from `_originX` (captured in `Awake`); destinations snap to the grid so an interrupted tween never leaves the camera off-screen; range clamps `_minStep`/`_maxStep` (defaults `int.MinValue`/`int.MaxValue`).
- `DinoBrainBehaviour`: random LitMotion moves within a `BoxCollider2D` + Z-axis rocking while moving.

## LitMotion gotchas

- Tween via `LMotion.Create(from, to, duration).WithEase(Ease.X).WithOnComplete(cb).BindTo...`; keep the `MotionHandle` and `Cancel()` it on repeat/`OnDisable` — stale handles leave tweens "derailed".
- Binding names differ from intuition: `BindToAlpha(CanvasGroup)`, `BindToEulerAnglesZ(Transform)`, `BindToPosition(Transform)`. Verify exact names in the package-cache source before writing.
- `WithLoops(-1, LoopType.Yoyo)` = infinite loop; `WithLoops(0, …)` does **not** mean infinite.

## UI text formatting

- Inspector-serialized format strings use `string.Format` placeholders: `"Food: {0}"`, **not** `{f}` — `{f}` throws `FormatException` (burned already). Scene-serialized values (`ResourcesBar._foodFormat`/`_eggsFormat`) must keep the `{0}` style.

## Audio system (hard-won)

- `AudioService` (MonoBehaviour, `[DefaultExecutionOrder(-100)]`): creates the sound pool in `Awake`, but **loads volumes from PlayerPrefs in `Start`** and applies them to `MainMixer.mixer` via exposed params `MasterVolume`/`MusicVolume`/`SoundVolume`. Do NOT move loading to `Awake` — the mixer's start snapshot re-applies its baked values after `Awake`, clobbering `SetFloat`.
- `AudioSlidersManager`: only mirrors service values onto sliders with `SetValueWithoutNotify`, and pushes user changes back to the service. Never set `slider.value` during init — it fires `onValueChanged` and resets the saved PlayerPrefs.
- Volumes persist in PlayerPrefs keys `MasterVolume`/`MusicVolume`/`SoundVolume`.

## Packages

- `Packages/manifest.json` pulls git deps (LitMotion, `com.besty.unity-skills`) plus URP/2D/Cinemachine/InputSystem/UGUI. Don't add packages without updating the manifest.
