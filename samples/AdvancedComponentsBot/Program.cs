using FluentTelegramUI;
using FluentTelegramUI.Builders;
using FluentTelegramUI.Models;

var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN")
    ?? throw new InvalidOperationException("Set TELEGRAM_BOT_TOKEN.");

var bot = new TelegramBotBuilder()
    .WithToken(token)
    .WithFluentUI(FluentStyle.Modern)
    .AddScreen("Advanced UI Components", ConfigureMenuScreen, isMainScreen: true)
    .AddScreen("Toggle Example", ConfigureToggleScreen)
    .AddScreen("Carousel Example", ConfigureCarouselScreen)
    .AddScreen("Progress Example", ConfigureProgressScreen)
    .AddScreen("Accordion Example", ConfigureAccordionScreen)
    .AddScreen("Rich Text Example", ConfigureRichTextScreen)
    .AddScreen("Rating Example", ConfigureRatingScreen)
    .WithAutoStartReceiving()
    .Build();

Console.WriteLine("AdvancedComponentsBot is running. Press Enter to stop.");
Console.ReadLine();
bot.StopReceiving();

static void ConfigureMenuScreen(ScreenBuilder screen)
{
    screen.WithId("menu")
        .WithContent("Welcome to the Advanced UI Components demo!\n\nSelect a component:")
        .AddNavigationButton("Toggle Switch", "toggle")
        .AddNavigationButton("Image Carousel", "carousel")
        .AddNavigationButton("Progress Indicator", "progress")
        .AddNavigationButton("Accordion", "accordion")
        .AddNavigationButton("Rich Text", "rich-text")
        .AddNavigationButton("Rating", "rating")
        .WithButtonsPerRow(2);
}

static void ConfigureToggleScreen(ScreenBuilder screen)
{
    screen.WithId("toggle")
        .WithContent("Toggle Switch Example")
        .AddRichText("Toggles let users switch features on and off", isBold: true)
        .AddToggle("Dark Mode", "dark-mode", false)
        .AddToggle("Notifications", "notifications", true)
        .AddNavigationButton("Back to Menu", "menu");
}

static void ConfigureCarouselScreen(ScreenBuilder screen)
{
    screen.WithId("carousel")
        .WithContent("Image Carousel Example")
        .AddRichText("Carousels let users browse through multiple images", isBold: true)
        .AddImageCarousel(
        [
            "https://images.unsplash.com/photo-1470071459604-3b5ec3a7fe05",
            "https://images.unsplash.com/photo-1447752875215-b2761acb3c5d",
            "https://images.unsplash.com/photo-1472214103451-9374bd1c798e"
        ],
        [
            "Sunset over the mountains",
            "Forest tree",
            "Calm lake"
        ])
        .AddNavigationButton("Back to Menu", "menu");
}

static void ConfigureProgressScreen(ScreenBuilder screen)
{
    screen.WithId("progress")
        .WithContent("Progress Indicator Example")
        .AddRichText("Progress indicators show completion status", isBold: true)
        .AddProgressIndicator("Download", 25)
        .AddProgressIndicator("Upload", 50)
        .AddProgressIndicator("Processing", 75)
        .AddProgressIndicator("Completed", 100)
        .AddNavigationButton("Back to Menu", "menu");
}

static void ConfigureAccordionScreen(ScreenBuilder screen)
{
    screen.WithId("accordion")
        .WithContent("Accordion Example")
        .AddRichText("Accordions hide and show content sections", isBold: true)
        .AddAccordion("FAQ", "1. How do I create a bot?\n2. How do I add commands?", false)
        .AddAccordion("Features", "• Fluent interface\n• Modern UI components\n• Screen navigation", false)
        .AddNavigationButton("Back to Menu", "menu");
}

static void ConfigureRichTextScreen(ScreenBuilder screen)
{
    screen.WithId("rich-text")
        .WithContent("Rich Text Example")
        .AddRichText("Bold text", isBold: true)
        .AddRichText("Italic text", isItalic: true)
        .AddRichText("Underlined text", isUnderlined: true)
        .AddRichText("Center aligned", alignment: TextAlignment.Center)
        .AddNavigationButton("Back to Menu", "menu");
}

static void ConfigureRatingScreen(ScreenBuilder screen)
{
    screen.WithId("rating")
        .WithContent("Rating Example")
        .AddRichText("Ratings let users provide feedback", isBold: true)
        .AddRating("Rate our service", "service-rating", 0)
        .AddRating("Rate our bot", "bot-rating", 4)
        .AddNavigationButton("Back to Menu", "menu");
}
