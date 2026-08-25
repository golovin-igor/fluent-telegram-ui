using FluentTelegramUI;
using FluentTelegramUI.Models;

var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN")
    ?? throw new InvalidOperationException("Set TELEGRAM_BOT_TOKEN.");

const string pitchHtml =
    """
    <h1>Build your next Telegram bot with fluent-telegram-ui</h1>
    <p>Fluent C# screens, buttons, and native Rich Messages — without a Mini App.</p>
    <h2>Why this library</h2>
    <ul>
    <li>Screen-based navigation with back handling</li>
    <li>Fluent builders for messages, screens, and controls</li>
    <li>Interactive toggles, carousels, ratings, and accordions</li>
    <li><code>sendRichMessage</code> from the same API you already use</li>
    </ul>
    <h2>Getting started</h2>
    <pre>dotnet add package FluentTelegramUI</pre>
    <table bordered striped>
    <tr><th>Task</th><th>Screen builder</th><th>Raw Telegram.Bot</th></tr>
    <tr><td>Menu + callbacks</td><td>AddScreen + AddButton</td><td>Hand-roll markup and routing</td></tr>
    <tr><td>Formatted reply</td><td>WithRichHtml / WithRichMarkdown</td><td>Build InputRichMessage yourself</td></tr>
    </table>
    <details>
    <summary>More</summary>
    <p>Source and docs: <a href="https://github.com/golovin-igor/fluent-telegram-ui">GitHub</a> ·
    <a href="https://golovin-igor.github.io/fluent-telegram-ui/">Documentation</a></p>
    </details>
    """;

var bot = new TelegramBotBuilder()
    .WithToken(token)
    .WithFluentUI(FluentStyle.Modern)
    .AddScreen("fluent-telegram-ui", builder => builder
        .WithId("pitch")
        .WithRichHtml(pitchHtml)
        .AddUrlButton("GitHub", "https://github.com/golovin-igor/fluent-telegram-ui")
        .AddUrlButton("NuGet", "https://www.nuget.org/packages/FluentTelegramUI")
        .WithButtonsPerRow(2), isMainScreen: true)
    .WithAutoStartReceiving()
    .Build();

Console.WriteLine("RichMessageBot is running. Send /start to see the pitch. Press Enter to stop.");
Console.ReadLine();
bot.StopReceiving();
