using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentTelegramUI.Builders;
using FluentTelegramUI.Models;
using Moq;
using Telegram.Bot;
using Xunit;

namespace FluentTelegramUI.Tests
{
    public class AdvancedScreenBuilderTests
    {
        private readonly FluentTelegramBot _mockBot;
        
        public AdvancedScreenBuilderTests()
        {
            // Set up mock bot for testing
            var mockBotClient = new Mock<ITelegramBotClient>();
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(ITelegramBotClient)))
                .Returns(mockBotClient.Object);
            
            _mockBot = new FluentTelegramBot(mockServiceProvider.Object);
        }
        
        [Fact]
        public void AddToggle_AddsToggleToScreen()
        {
            // Arrange
            var builder = new ScreenBuilder(_mockBot, "Test Screen");
            var label = "Dark Mode";
            var callbackData = "toggle_dark_mode";
            var isOn = true;
            
            // Act
            builder.AddToggle(label, callbackData, isOn);
            var screen = builder.Build();
            
            // Assert
            screen.Controls.Should().ContainSingle();
            screen.Controls[0].Should().BeOfType<Toggle>();
            
            var toggle = (Toggle)screen.Controls[0];
            toggle.Label.Should().Be(label);
            toggle.CallbackData.Should().Be(callbackData);
            toggle.IsOn.Should().Be(isOn);
        }
        
        [Fact]
        public void WithToggleHandler_AddsCorrectHandlersForToggle()
        {
            // Arrange
            var builder = new ScreenBuilder(_mockBot, "Test Screen");
            var toggleId = "dark_mode";
            
            // Act
            builder.WithToggleHandler(toggleId);
            var screen = builder.Build();
            
            // Assert
            screen.EventHandlers.Should().ContainKeys($"{toggleId}:on", $"{toggleId}:off");
        }
        
        [Fact]
        public void AddImageCarousel_AddsCarouselToScreen()
        {
            // Arrange
            var builder = new ScreenBuilder(_mockBot, "Test Screen");
            var imageUrls = new List<string> { "https://example.com/image1.jpg", "https://example.com/image2.jpg" };
            var captions = new List<string> { "Caption 1", "Caption 2" };
            
            // Act
            builder.AddImageCarousel(imageUrls, captions);
            var screen = builder.Build();
            
            // Assert
            screen.Controls.Should().ContainSingle();
            screen.Controls[0].Should().BeOfType<ImageCarousel>();
            
            var carousel = (ImageCarousel)screen.Controls[0];
            carousel.ImageUrls.Should().BeEquivalentTo(imageUrls);
            carousel.Captions.Should().BeEquivalentTo(captions);
        }
        
        [Fact]
        public void WithCarouselHandler_AddsCorrectHandlersForCarousel()
        {
            // Arrange
            var builder = new ScreenBuilder(_mockBot, "Test Screen");
            var carouselId = "image_gallery";
            
            // Act
            builder.WithCarouselHandler(carouselId);
            var screen = builder.Build();
            
            // Assert
            screen.EventHandlers.Should().ContainKeys($"carousel:{carouselId}:prev", $"carousel:{carouselId}:next");
        }
        
        [Fact]
        public void AddProgressIndicator_AddsProgressToScreen()
        {
            // Arrange
            var builder = new ScreenBuilder(_mockBot, "Test Screen");
            var label = "Download Progress";
            var progress = 75;
            
            // Act
            builder.AddProgressIndicator(label, progress);
            var screen = builder.Build();
            
            // Assert
            screen.Controls.Should().ContainSingle();
            screen.Controls[0].Should().BeOfType<ProgressIndicator>();
            
            var indicator = (ProgressIndicator)screen.Controls[0];
            indicator.Label.Should().Be(label);
            indicator.Progress.Should().Be(progress);
        }
        
        [Fact]
        public void AddAccordion_AddsAccordionToScreen()
        {
            // Arrange
            var builder = new ScreenBuilder(_mockBot, "Test Screen");
            var title = "FAQ";
            var content = "Frequently asked questions go here...";
            var isExpanded = true;
            
            // Act
            builder.AddAccordion(title, content, isExpanded);
            var screen = builder.Build();
            
            // Assert
            screen.Controls.Should().ContainSingle();
            screen.Controls[0].Should().BeOfType<Accordion>();
            
            var accordion = (Accordion)screen.Controls[0];
            accordion.Title.Should().Be(title);
            accordion.Content.Should().Be(content);
            accordion.IsExpanded.Should().Be(isExpanded);
        }
        
        [Fact]
        public void WithAccordionHandler_AddsCorrectHandlersForAccordion()
        {
            // Arrange
            var builder = new ScreenBuilder(_mockBot, "Test Screen");
            var accordionId = "faq_section";
            
            // Act
            builder.WithAccordionHandler(accordionId);
            var screen = builder.Build();
            
            // Assert
            screen.EventHandlers.Should().ContainKeys($"accordion:{accordionId}:expand", $"accordion:{accordionId}:collapse");
        }
        
        [Fact]
        public void AddRichText_AddsRichTextToScreen()
        {
            // Arrange
            var builder = new ScreenBuilder(_mockBot, "Test Screen");
            var text = "This is rich text";
            var isBold = true;
            var isItalic = true;
            var isUnderlined = false;
            var alignment = TextAlignment.Center;
            
            // Act
            builder.AddRichText(text, isBold, isItalic, isUnderlined, alignment);
            var screen = builder.Build();
            
            // Assert
            screen.Controls.Should().ContainSingle();
            screen.Controls[0].Should().BeOfType<RichText>();
            
            var richText = (RichText)screen.Controls[0];
            richText.Text.Should().Be(text);
            richText.IsBold.Should().Be(isBold);
            richText.IsItalic.Should().Be(isItalic);
            richText.IsUnderlined.Should().Be(isUnderlined);
            richText.Alignment.Should().Be(alignment);
        }
        
        [Fact]
        public void AddRating_AddsRatingToScreen()
        {
            // Arrange
            var builder = new ScreenBuilder(_mockBot, "Test Screen");
            var label = "Rate our service";
            var callbackDataPrefix = "rate_service";
            var initialValue = 4;
            
            // Act
            builder.AddRating(label, callbackDataPrefix, initialValue);
            var screen = builder.Build();
            
            // Assert
            screen.Controls.Should().ContainSingle();
            screen.Controls[0].Should().BeOfType<Rating>();
            
            var rating = (Rating)screen.Controls[0];
            rating.Label.Should().Be(label);
            rating.CallbackDataPrefix.Should().Be(callbackDataPrefix);
            rating.Value.Should().Be(initialValue);
        }
        
        [Fact]
        public void FluentChainingOfMethods_WorksAsExpected()
        {
            // Arrange & Act
            var builder = new ScreenBuilder(_mockBot, "Test Screen");
            builder
                .WithContent("This is a test screen with multiple components")
                .AddToggle("Dark Mode", "dark_mode", false)
                .WithToggleHandler("dark_mode")
                .AddProgressIndicator("Download", 50)
                .AddRichText("This is important", true)
                .AddAccordion("Help Section", "Help content goes here", false)
                .WithAccordionHandler("help_section")
                .AddRating("Rate Us", "rate_app", 5);
            
            var screen = builder.Build();
            
            // Assert
            screen.Title.Should().Be("Test Screen");
            screen.Content.Text.Should().Be("This is a test screen with multiple components");
            screen.Controls.Should().HaveCount(5);
            screen.Controls[0].Should().BeOfType<Toggle>();
            screen.Controls[1].Should().BeOfType<ProgressIndicator>();
            screen.Controls[2].Should().BeOfType<RichText>();
            screen.Controls[3].Should().BeOfType<Accordion>();
            screen.Controls[4].Should().BeOfType<Rating>();
        }
    }
} 