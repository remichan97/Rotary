# Rotary — Progress Log

Running record of what's been worked on, plus open issues/blockers. Newest entries at top. This is a working log, not user-facing docs — keep entries short.

## Known Issues / Blockers

- `Rotary.Core` is still an empty stub (`Class1.cs`) — no HTTP execution, collection model, import/export, or codegen logic exists yet.
- No test project in the solution yet.
- Open design decision: storage format for Rotary's own collection format (flat JSON files vs SQLite vs one-file-per-collection) — needed before import/export and codegen can be built against a real model.
- NavigationView shell currently has one static "Home" menu item with no page-switching logic — fine for now, will need real navigation (Frame + per-page ViewModels, or a simple ContentControl swap) once more than one page exists.

## Log

### 2026-08-19 (2)
- Phase 0 (shell) done: `App.axaml` now uses `FluentAvaloniaTheme` instead of the stock `FluentTheme`; `MainWindow.axaml` has a minimal `FANavigationView` shell (one "Home" item, content bound to the existing `Greeting` property). Build verified clean, app launches without runtime errors.
- Learned FluentAvaloniaUI 3.0.2 prefixes its WinUI-ported controls with `FA` (`FANavigationView`, `FASymbolIcon`, etc.) — differs from what the official docs site showed; noted in `CLAUDE.md` so this doesn't need rediscovering.
- Added `ROADMAP.md` (phased plan, non-committed effort estimates) alongside this log.

### 2026-08-19 (1)
- Decided on stack: C#/Avalonia (native UI, no web-rendering stack — ruled out Electron and also Tauri/Wails-style webview shells) with a `Rotary.Core` class library kept UI-agnostic, referenced by the `Rotary.App` Avalonia project.
- Named the project **Rotary**.
- Scaffolded solution via Visual Studio: Avalonia MVVM Application template, .NET 10, CommunityToolkit.Mvvm, Avalonia ViewLocator removed (reflection-based view resolution is trim/AOT-unfriendly).
- Added FluentAvaloniaUI (WinUI-ported controls), `FluentAvalonia.ProgressRing`, and `FluentIcons.Avalonia.Fluent` packages to `Rotary.App`.
- Set up git hooks via Husky.Net: pre-commit runs csharpier format + build; commit-msg enforces Conventional Commits.
- Created `CLAUDE.md` documenting architecture, commands, and conventions for future sessions.
