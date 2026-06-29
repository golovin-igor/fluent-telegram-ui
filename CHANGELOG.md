# Changelog

## 0.2.0

- Target .NET 10 and Telegram.Bot 22.x
- Fix default handler wiring in `TelegramBotBuilder`
- Fix duplicate callback answers and per-chat control state
- Refresh screens with `EditMessageText` instead of delete/resend
- Add `AddFluentTelegramUI()` DI extension and hosted polling service
- Add typed `FluentCallbackContext`, wildcard callback patterns, and `IStateStore`
- Apply `FluentStyle` templates during screen rendering
- Exclude examples from NuGet package; add `samples/` project

## 0.1.0

- Initial alpha release with screen builder, state machine, and advanced UI controls
