# Rotary — Roadmap

Phased plan for the project, each phase ending in something runnable. Durations are **rough effort estimates, not commitments or deadlines** — assumed part-time/hobby pace (a few sessions a week), with extra buffer on the earliest phases since this is a first project with native .NET GUI development. Expect these to shift once Phase 1 gives a real read on pace; revise in place rather than tracking slippage.

| Phase | Scope | Est. Effort |
|---|---|---|
| 0 — Finish the shell | Wire `FluentAvaloniaTheme`, minimal `NavigationView` shell with one dummy page. Get comfortable with Avalonia XAML binding + `CommunityToolkit.Mvvm` source generators. | 1–2 sessions |
| 1 — Vertical slice: send one request | Minimal `Rotary.Core` request model + `HttpExecutor`; minimal `Rotary.App` page (method/URL/send + plain response view). Proves the App↔Core boundary, async commands, error handling end-to-end. | 1–2 weeks |
| 2 — Collections & persistence | Resolve storage format (leaning flat JSON files, one per collection). Collection/Folder/Request models in Core, sidebar tree in the shell. | 1–2 weeks |
| 3 — Response viewer polish | Swap plain response view for `AvaloniaEdit` (syntax highlighting, virtualized scrolling). Add `ProgressRing` for in-flight requests. | 3–5 days |
| 4 — Import | cURL command parser, Postman v2.1 importer, Insomnia importer. | 1–2 weeks |
| 5 — Export | Export to Postman/Insomnia formats + Rotary's own format. | 3–5 days |
| 6 — Codegen | Request/response model boilerplate generation, starting with C# as the first target language. | 1–2 weeks |
| 7 — Polish & packaging | NativeAOT publish/trimming, app icon, settings, maybe an installer. | ~1 week |

## Notes

- Phase 1 is the highest-value phase to get right early — it's the smallest slice that exercises the whole stack (UI binding, async commands, Core/App boundary, HTTP), so problems there surface before more is built on top.
- Phases 4–6 (import/export/codegen) don't strictly have to happen in this order — cURL import is likely the fastest win and could move earlier if it's more motivating to build than collections persistence.
- See `PROGRESS.md` for what's actually been done and current blockers; this file is the forward-looking plan, that one is the running log.

## Backlog (unscheduled — revisit later)

- **Environments** — named variable sets (e.g. `{{baseUrl}}`) substituted into requests at send time, plus a UI for creating/editing/switching between them. Not yet assigned a phase.
- **Cookie jar** — explicit, user-visible cross-request cookie storage (view/edit/clear, per-domain scoping, expiry), replacing the current implicit behavior where `HttpRequestExecutor`'s shared `HttpClient` silently accumulates and resends cookies via its default `HttpClientHandler`/`CookieContainer`. Needs a model + UI, not just a field.
