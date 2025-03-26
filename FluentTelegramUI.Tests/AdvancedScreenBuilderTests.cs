using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentTelegramUI.Builders;
using FluentTelegramUI.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Telegram.Bot;
using Xunit;

namespace FluentTelegramUI.Tests
{
    public class AdvancedScreenBuilderTests
    {
        [Fact]
        public void AddToggle_AddsToggleToScreen()
        {
            // Arrange
            var screen = new Screen { Title = "Test Screen" };
            
            // Act
            var toggle = new Toggle("Dark Mode", "toggle_dark_mode", true);
            screen.AddControl(toggle);
            
            // Assert
            screen.Controls.Should().ContainSingle();
            screen.Controls[0].Should().BeOfType<Toggle>();
            
            var toggleFromScreen = (Toggle)screen.Controls[0];
            toggleFromScreen.Label.Should().Be("Dark Mode");
            toggleFromScreen.CallbackData.Should().Be("toggle_dark_mode");
            toggleFromScreen.IsOn.Should().Be(true);
        }
        
        [Fact]
        public void AddImageCarousel_AddsCarouselToScreen()
        {
            // Arrange
            var screen = new Screen { Title = "Test Screen" };
            var imageUrls = new List<string> { "https://example.com/image1.jpg", "https://example.com/image2.jpg" };
            var captions = new List<string> { "Caption 1", "Caption 2" };
            
            // Act
            var carousel = new ImageCarousel(imageUrls, captions);
            screen.AddControl(carousel);
            
            // Assert
            screen.Controls.Should().ContainSingle();
            screen.Controls[0].Should().BeOfType<ImageCarousel>();
            
            var carouselFromScreen = (ImageCarousel)screen.Controls[0];
            carouselFromScreen.ImageUrls.Should().BeEquivalentTo(imageUrls);
            carouselFromScreen.Captions.Should().BeEquivalentTo(captions);
        }
        
        [Fact]
        public void AddProgressIndicator_AddsProgressToScreen()
        {
            // Arrange
            var screen = new Screen { Title = "Test Screen" };
            
            // Act
            var indicator = new ProgressIndicator("Download Progress", 75);
            screen.AddControl(indicator);
            
            // Assert
            screen.Controls.Should().ContainSingle();
            screen.Controls[0].Should().BeOfType<ProgressIndicator>();
            
            var indicatorFromScreen = (ProgressIndicator)screen.Controls[0];
            indicatorFromScreen.Label.Should().Be("Download Progress");
            indicatorFromScreen.Progress.Should().Be(75);
        }
        
        [Fact]
        public void AddAccordion_AddsAccordionToScreen()
        {
            // Arrange
            var screen = new Screen { Title = "Test Screen" };
            
            // Act
            var accordion = new Accordion("FAQ", "Frequently asked questions go here...", true);
            screen.AddControl(accordion);
            
            // Assert
            screen.Controls.Should().ContainSingle();
            screen.Controls[0].Should().BeOfType<Accordion>();
            
            var accordionFromScreen = (Accordion)screen.Controls[0];
            accordionFromScreen.Title.Should().Be("FAQ");
            accordionFromScreen.Content.Should().Be("Frequently asked questions go here...");
            accordionFromScreen.IsExpanded.Should().Be(true);
        }
        
        [Fact]
        public void AddRichText_AddsRichTextToScreen()
        {
            // Arrange
            var screen = new Screen { Title = "Test Screen" };
            
            // Act
            var richText = new RichText("This is rich text") 
            {
                IsBold = true,
                IsItalic = true,
                IsUnderlined = false,
                Alignment = TextAlignment.Center
            };
            screen.AddControl(richText);
            
            // Assert
            screen.Controls.Should().ContainSingle();
            screen.Controls[0].Should().BeOfType<RichText>();
            
            var richTextFromScreen = (RichText)screen.Controls[0];
            richTextFromScreen.Text.Should().Be("This is rich text");
            richTextFromScreen.IsBold.Should().Be(true);
            richTextFromScreen.IsItalic.Should().Be(true);
            richTextFromScreen.IsUnderlined.Should().Be(false);
            richTextFromScreen.Alignment.Should().Be(TextAlignment.Center);
        }
        
        [Fact]
        public void AddRating_AddsRatingToScreen()
        {
            // Arrange
            var screen = new Screen { Title = "Test Screen" };
            
            // Act
            var rating = new Rating("Rate our service", "rate_service", 4);
            screen.AddControl(rating);
            
            // Assert
            screen.Controls.Should().ContainSingle();
            screen.Controls[0].Should().BeOfType<Rating>();
            
            var ratingFromScreen = (Rating)screen.Controls[0];
            ratingFromScreen.Label.Should().Be("Rate our service");
            ratingFromScreen.CallbackDataPrefix.Should().Be("rate_service");
            ratingFromScreen.Value.Should().Be(4);
        }
        
        [Fact]
        public void MultipleControls_WorkCorrectly()
        {
            // Arrange
            var screen = new Screen { Title = "Test Screen" };
            
            // Act
            screen.AddControl(new Toggle("Dark Mode", "dark_mode", false));
            screen.AddControl(new ProgressIndicator("Download", 50));
            screen.AddControl(new RichText("This is important") { IsBold = true });
            screen.AddControl(new Accordion("Help Section", "Help content goes here", false));
            screen.AddControl(new Rating("Rate Us", "rate_app", 5));
            
            // Assert
            screen.Controls.Should().HaveCount(5);
            screen.Controls[0].Should().BeOfType<Toggle>();
            screen.Controls[1].Should().BeOfType<ProgressIndicator>();
            screen.Controls[2].Should().BeOfType<RichText>();
            screen.Controls[3].Should().BeOfType<Accordion>();
            screen.Controls[4].Should().BeOfType<Rating>();
        }
        
        [Fact]
        public void EventHandlers_CorrectlyAddedForToggle()
        {
            // Arrange
            var screen = new Screen { Title = "Test Screen" };
            var toggleId = "dark_mode";
            
            // Act - manually add the event handlers that would be added by WithToggleHandler
            screen.OnCallback($"{toggleId}:on", async (data, state) => {
                return true;
            });
            
            screen.OnCallback($"{toggleId}:off", async (data, state) => {
                return true;
            });
            
            // Assert
            screen.EventHandlers.Should().ContainKeys($"{toggleId}:on", $"{toggleId}:off");
        }
        
        [Fact]
        public void EventHandlers_CorrectlyAddedForCarousel()
        {
            // Arrange
            var screen = new Screen { Title = "Test Screen" };
            var carouselId = "image_gallery";
            
            // Act - manually add the event handlers that would be added by WithCarouselHandler
            screen.OnCallback($"carousel:{carouselId}:prev", async (data, state) => {
                return true;
            });
            
            screen.OnCallback($"carousel:{carouselId}:next", async (data, state) => {
                return true;
            });
            
            // Assert
            screen.EventHandlers.Should().ContainKeys($"carousel:{carouselId}:prev", $"carousel:{carouselId}:next");
        }
        
        [Fact]
        public void EventHandlers_CorrectlyAddedForAccordion()
        {
            // Arrange
            var screen = new Screen { Title = "Test Screen" };
            var accordionId = "faq_section";
            
            // Act - manually add the event handlers that would be added by WithAccordionHandler
            screen.OnCallback($"accordion:{accordionId}:expand", async (data, state) => {
                return true;
            });
            
            screen.OnCallback($"accordion:{accordionId}:collapse", async (data, state) => {
                return true;
            });
            
            // Assert
            screen.EventHandlers.Should().ContainKeys($"accordion:{accordionId}:expand", $"accordion:{accordionId}:collapse");
        }
    }
} 