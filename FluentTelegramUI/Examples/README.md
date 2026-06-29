# Examples (legacy)

The C# examples that previously lived here have been migrated to runnable projects under [`samples/`](../../samples/):

| Former example | Sample project |
|----------------|----------------|
| `SimpleBot.cs` | [`SimpleScreenBot`](../../samples/SimpleScreenBot/) |
| `AdvancedUIComponentsExample.cs` | [`AdvancedComponentsBot`](../../samples/AdvancedComponentsBot/) |
| Screen / state / context patterns | [`HostedServiceBot`](../../samples/HostedServiceBot/), [`WebhookBot`](../../samples/WebhookBot/) |
| Localization | [`LocalizedScreenBot`](../../samples/LocalizedScreenBot/) |

These files are excluded from the NuGet package compile (`Compile Remove="Examples\**\*.cs"`). Refer to the sample projects for up-to-date .NET 10 and Telegram.Bot 22 APIs.
