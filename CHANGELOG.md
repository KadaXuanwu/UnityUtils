# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.9] - 2026-09-02

### Changed
- `EventBus<T>.Raise` no longer allocates. It used to build a fresh `HashSet` of every binding on
  each call - roughly 330 bytes at 16 listeners, 2.4 KB at 64 - which is exactly the per-frame
  garbage that shows up as a hitch under Unity's non-generational collector. The snapshot is now a
  pooled `List`, returned in a `finally`. A raise nested inside a handler rents its own buffer, so
  re-entrancy needs no special case.

### Fixed
- Unregistering during a raise now takes effect immediately. Because the old snapshot was taken up
  front, a binding removed by an earlier handler was still invoked for that event - so a handler
  that destroyed an object whose teardown unregisters it would then run on the dead object. C#
  multicast delegates defer in the same way, but that is the wrong default here. Registering
  during a raise still takes effect on the next one.

## [1.1.8] - 2026-09-02

### Added
- EditMode tests for the EventBus, in `Tests/Editor`. They declare their event types inside the
  test assembly, which is the case the 1.1.7 discovery bug could never see. A consuming project
  has to list `"testables": ["com.kadaxuanwu.utils"]` in its manifest for them to run.

### Fixed
- `Singleton` used the deprecated `FindFirstObjectByType`. `FindAnyObjectByType` is what a
  singleton actually wants: it does not depend on instance ID ordering, and is cheaper.

## [1.1.7] - 2026-09-02

### Fixed
- EventBus listeners were never cleared. `EventBusUtil.InitializeAllBuses` created each bus type
  and then returned an empty list, so `ClearAllBuses` had nothing to iterate and bindings survived
  play mode still holding references to destroyed objects.
- Event types declared inside an asmdef were invisible to the EventBus. Discovery ran through
  `PredefinedAssemblyUtil`, which by design only searches Assembly-CSharp and
  Assembly-CSharp-firstpass. Buses now register themselves on first use, so any assembly works.
- `using UnityEditor;` was unguarded in the runtime assembly, where it does not compile for a
  player build.

### Changed
- `EventBus<T>.Clear()` is public and no longer has to be reached by reflection.
- `EventBusUtil.EventTypes` and `EventBusTypes` are read-only, and list the buses actually in use
  instead of every IEvent type a scan happened to find.

## [1.1.6] - 2025-12-26

### Changed
- Editor assembly now only compiles for the Unity Editor
- Corrected typo

## [1.1.5] - 2025-12-26

### Added
- DontDestroyOnLoad component to keep objects persistant across scenes

## [1.1.4] - 2025-12-26

### Changed
- Renamed PlayerRefs to CharacterRefs
- Renamed menu names for creating scriptabled objects

## [1.1.3] - 2025-12-23

### Added
- Getters inside InputManager for key binding

### Changed
- Renamed InputManager.Instance to InputManager.S

## [1.1.2] - 2025-12-07

- Added PlayerRefs for global access to specific references from the Player

## [1.1.1] - 2025-12-07

### Changed
- Renaming of internal files and cleanup

## [1.1.0] - 2025-12-06

### Added
- Physics helper
- Input system manager
- First person character controller with modifier system
- Jump, Crouch, Run, Sliding, Landing modifiers
- Input abstraction layer
- ScriptableObject-based configuration

## [1.0.0] - 2025-10-22

### Added
- Initial release