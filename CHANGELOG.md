# Changelog

## 0.2.0

- Target .NET 10 and Telegram.Bot 22.x
- Fix default handler wiring in `TelegramBotBuilder`
- Fix duplicate callback answers and per-chat control state
- Refresh screens with `EditMessageText` instead of delete/resend
- Add `AddFluentTelegramUI()` DI extension and hosted polling service
- Add typed `FluentCallbackContext`, wildcard callback patterns, and `IStateStore`
- Apply `FluentStyle` templates during screen rendering
- Exclude examples from NuGet package; add `samples/` projects (polling, DI host, webhook)
- Add `configureBot` overload to `AddFluentTelegramUI()` for screen registration at startup
- Add `WebhookSecretToken` option for webhook validation
- Source Link symbol packages, GitHub Pages docs workflow, NuGet release workflow
- CI collects code coverage and builds all samples
- Localize screen titles and content via resource keys and per-chat culture
- Migrate legacy Examples into runnable samples; add integration tests for Build()

## 0.1.0

- Initial alpha release with screen builder, state machine, and advanced UI controls
