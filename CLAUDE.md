# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

See `ROADMAP.md` for the phased project plan (rough effort estimates, not deadlines) and `PROGRESS.md` for the running work log and known issues/blockers — check both for current state, and add a `PROGRESS.md` entry for notable work in a session.

## Project

Rotary is a lightweight, native desktop API client (Postman/Insomnia alternative) — organize requests into collections, import/export Postman/Insomnia/cURL, and generate request/response model boilerplate. The explicit design goal is to avoid *any* web-rendering stack (ruled out Electron, and also ruled out Tauri/Wails-style webview shells) in favor of a fully native UI, so no web frontend framework, HTML, or embedded browser control belongs anywhere in this repo.

## Commands

- Build: `dotnet build`
- Run the app: `dotnet run --project Rotary.App`
- Restore local tools (csharpier, husky, commitlint.net): `dotnet tool restore`
- Format manually: `dotnet csharpier format .` (also runs automatically on staged files via the pre-commit hook)
- No test project exists yet — when one is added, record its run command here.

## Architecture

Two-project solution, split so core logic stays UI-agnostic and testable:

- **Rotary.Core** — target for all UI-independent logic: HTTP request execution, the collection/request data model, Postman/Insomnia/cURL import-export, and request/response model codegen. Currently a stub; this is where most feature work lands.
- **Rotary.App** — Avalonia 12 desktop UI (net10.0, `WinExe`), referencing `Rotary.Core`. MVVM via `CommunityToolkit.Mvvm` (source-generator based, not ReactiveUI).

Non-obvious decisions worth knowing before touching the UI project:

- **Avalonia's default ViewLocator was intentionally removed** (reflection-based view resolution is trim/AOT-unfriendly). Views are wired explicitly rather than resolved by ViewModel-name convention — `App.axaml.cs` sets `MainWindow`'s `DataContext` directly, and any new View/ViewModel pairs need an explicit `DataTemplate` in `App.axaml` rather than relying on naming convention.
- **UI toolkit is FluentAvaloniaUI**, not the stock `Avalonia.Themes.Fluent` alone — it adds WinUI-ported controls (`NavigationView`, `InfoBar`, `TeachingTip`, `ContentDialog`, `TabView`, etc.). The planned app shell uses `NavigationView` (collections sidebar + request/response content area).
- `FluentAvalonia.ProgressRing` and `FluentIcons.Avalonia.Fluent` are separate add-on packages layered on top of FluentAvaloniaUI (neither ships in the core FluentAvaloniaUI package) — use these for loading indicators and iconography respectively rather than adding alternatives.
- .NET target is **net10.0** (current LTS); NativeAOT-friendly patterns are preferred throughout given the goal of a small, fast-starting native binary.
- **FluentAvaloniaUI 3.0.2's WinUI-ported control classes carry an `FA` prefix** (`FANavigationView`, `FANavigationViewItem`, `FASymbolIcon`, `FASymbolIconSource`, `FAContentDialog`, `FATaskDialog`, etc.) — the official docs site at the time of writing showed unprefixed names (`NavigationView`, `SymbolIcon`), which don't resolve against this version. When adding a new FluentAvalonia control, check the actual class name in the installed package (`grep` the DLL, or IntelliSense) rather than trusting docs/search results literally.

## Git hooks (Husky.Net)

Configured in `.husky/task-runner.json`, installed via `dotnet-tools.json`:

- **pre-commit**: runs `csharpier format` on staged files, then `dotnet build --no-restore`.
- **commit-msg**: enforces Conventional Commits via `commit-lint.net`, config in `.husky/commit-lint.json` — allowed types are `feat, fix, refactor, build, chore, style, test, docs, perf, revert`, max subject length 90, no scopes.

## Committing

- Write commit messages as Conventional Commits (`type: subject`, using the types listed above) — the commit-msg hook rejects anything else.
- When Claude Code creates a commit in this repo, append a `Co-Authored-By: Claude <noreply@anthropic.com>` trailer.
