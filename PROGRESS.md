# Rotary — Progress Log

Running record of what's been worked on, plus open issues/blockers. Newest entries at top. This is a working log, not user-facing docs — keep entries short.

## Known Issues / Blockers

- No test project in the solution yet.
- Collection persistence (SQLite) exists, but there's no UI yet to create/rename/delete collections, folders, or requests, no drag-reorder, and selecting a tree node doesn't load it into the request builder — the sidebar tree only ever shows what's already in `rotary.db`, which today is nothing. This is most of what's left before the app is actually usable, and it's all UI-side work.
- `Avalonia.Controls.TreeDataGrid` (the virtualized tree control originally planned for the collection sidebar) is gated behind Avalonia's Accelerate commercial licensing for this project's Avalonia version (12.1.1) — the free Community tier needs a license key + telemetry opt-in we haven't set up, and the last unlicensed version only targets Avalonia 11 core. Sidebar currently uses the base `TreeView` control instead; revisit if tree size/virtualization becomes a real problem, or once licensing is sorted.
- `HttpRequestExecutor` reads the whole response body as a string; no handling yet for binary responses or very large bodies.

## Log

### 2026-08-21 (1)

- Phase 2 storage format decided: SQLite (single `rotary.db` file via `Microsoft.Data.Sqlite` + raw SQL via `Dapper`/`Dapper.AOT`), not flat JSON files — one centralized file is easier to protect/back up than many scattered ones, and querying an index (`SELECT Id, Name FROM Collections`) is simpler and less fragile than hand-maintaining a separate manifest file. `Collections`/`Environments`/`EnvironmentVariables`/`Cookies` tables are all created up front (`Rotary.Core/Data/Schema.cs`) so the backlog features are a repository away rather than a schema migration when they get built; only `Collections` has a repository/service today.
- Added `ICollectionService`/`CollectionService` (`Rotary.Core/Collections`) as the public persistence entry point. `RotaryDatabase`/`CollectionsRepository`/`ICollectionsRepository` are `internal` so nothing outside `Rotary.Core` depends on SQLite/Dapper directly — callers only ever see `ICollectionService`.
- `CollectionDefinition`/`CollectionNodeDefinition` (`Folder`/`Request`, sealed hierarchy) round-trip through a JSON blob column via `System.Text.Json`. Needed `[JsonPolymorphic]`/`[JsonDerivedType]` on `AuthDefinition` and `CollectionNodeDefinition` (so deserialization picks the right concrete subtype), a custom `HttpMethodJsonConverter` (`HttpMethod` has no built-in STJ support), and a source-generated `RotaryJsonContext` to keep serialization reflection-free/AOT-safe.
- Hit a real Dapper.AOT + System.Text.Json source-generator interaction bug: a Dapper query parameter sourced from `var x = JsonSerializer.Serialize(..., RotaryJsonContext.Default.X)` failed to compile, because source generators don't see each other's generated output within the same compilation pass, so Dapper.AOT's interceptor generator couldn't resolve the local's type. Fixed by declaring that local with an explicit `string` type instead of `var`.
- Wired a collapsible collection-tree sidebar into `MainWindow.axaml`/`MainViewModel`: loads the collection index at startup, lazily fetches each collection's full tree via `CollectionService` on first expand (`CollectionTreeNodeViewModel`, driven by a two-way `IsExpanded` binding). Originally built against `Avalonia.Controls.TreeDataGrid`, reverted to base `TreeView` after hitting the licensing wall above (see blockers).

### 2026-08-19 (4)

- Phase 1 (vertical slice) done: request/response panels wired into the `FANavigationView` content area as a horizontal split (`GridSplitter` between request pane and response pane, user-resizable).
- Request pane: method dropdown, URL box, Send button, Headers tab (editable add/remove rows), Body tab (content-type + raw text body). Response pane: status/duration line, error text (failed requests only), Body tab (read-only), Headers tab (flattened list).
- `MainViewModel` now holds the form state and a `SendCommand` that builds a `RequestDefinition`, calls `HttpRequestExecutor` from `Rotary.Core`, and maps `RequestResult.Completed`/`Failed` back onto the response fields. New `HeaderRow` model (`Rotary.App/Models`) backs the editable header lists.
- Manually verified end-to-end against a live endpoint — request sends, response body/headers/status/duration display correctly, error path (bad URL) surfaces the failure message.
- Reviewed and fixed several bugs in the `Rotary.Core` HTTP layer the user wrote independently: null-headers crash, `HttpClient.BaseAddress` throwing on a second request, headers mutating the shared/reused `HttpClient` instead of the per-request message, the `Failed` branch being unreachable (no try/catch around `SendAsync`), and the response body accidentally being read from the request's content instead of the response's. Also swapped `DateTime.UtcNow` timing for `Stopwatch`, and changed `RequestResult.Completed.Headers` from the un-mockable `HttpResponseHeaders` type to a flattened `IList<KeyValuePair<string,string>>` (now also merging `response.Content.Headers`, which was previously missed entirely).

### 2026-08-19 (3)
- Made the initial commit (`88b3c65`) covering the full project scaffold to date.
- Fixed the commit-msg hook: `commitlint.net` targets net8.0 and this machine only has net10 runtimes installed, so it failed to launch. `.husky/commit-msg` now exports `DOTNET_ROLL_FORWARD=LatestMajor` before invoking husky. (Note: the tool manifest's `rollForward` field in `dotnet-tools.json` only accepts `true`/`false` and controls tool-version pinning, not runtime resolution — that's not the right knob for this problem.)
- Talked through `Rotary.Core`'s request/response shape before implementation: `RequestDefinition`/`RequestResult` keep the envelope (method, headers, status, timing) strongly typed while treating the body as opaque text; headers modeled as an ordered list (not a dictionary) to preserve duplicates; `RequestResult` planned as a sealed record hierarchy (`Completed` vs `Failed`) rather than nullable fields, to keep "got a response" and "couldn't get one" distinct. User is implementing this part themselves.

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
