using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentTelegramUI.Models;
using Xunit;

namespace FluentTelegramUI.Tests
{
    public class AdvancedUIControlTests
    {
        [Fact]
        public void Toggle_Constructor_InitializesProperties()
        {
            // Arrange
            var label = "Dark Mode";
            var callbackData = "toggle_dark_mode";
            var isOn = true;
            
            // Act
            var toggle = new Toggle(label, callbackData, isOn);
            
            // Assert
            toggle.Id.Should().NotBeNullOrEmpty();
            toggle.Id.Length.Should().BeLessOrEqualTo(7);
            toggle.Label.Should().Be(label);
            toggle.CallbackData.Should().Be(callbackData);
            toggle.IsOn.Should().Be(isOn);
            toggle.OnText.Should().Be("ON");
            toggle.OffText.Should().Be("OFF");
            toggle.Style.Should().Be(FluentStyle.Default);
        }
        
        [Fact]
        public void Toggle_ToMessage_CreatesMessageWithCorrectText()
        {
            // Arrange
            var toggle = new Toggle("Dark Mode", "toggle_dark_mode", true);
            
            // Act
            var message = toggle.ToMessage();
            
            // Assert
            message.Should().NotBeNull();
            message.Text.Should().Be("Dark Mode: ON");
            message.Buttons.Should().ContainSingle();
            message.Buttons[0].Text.Should().Be("Turn OFF");
            message.Buttons[0].CallbackData.Should().Be("toggle_dark_mode:off");
            
            // Test with toggle off
            toggle.IsOn = false;
            message = toggle.ToMessage();
            message.Text.Should().Be("Dark Mode: OFF");
            message.Buttons[0].Text.Should().Be("Turn ON");
            message.Buttons[0].CallbackData.Should().Be("toggle_dark_mode:on");
        }
        
        [Fact]
        public void Toggle_CustomOnOffText_UsesCustomText()
        {
            // Arrange
            var toggle = new Toggle("Notifications", "toggle_notifications", true)
            {
                OnText = "ENABLED",
                OffText = "DISABLED"
            };
            
            // Act
            var message = toggle.ToMessage();
            
            // Assert
            message.Text.Should().Be("Notifications: ENABLED");
            message.Buttons[0].Text.Should().Be("Turn DISABLED");
            
            // Test with toggle off
            toggle.IsOn = false;
            message = toggle.ToMessage();
            message.Text.Should().Be("Notifications: DISABLED");
            message.Buttons[0].Text.Should().Be("Turn ENABLED");
        }
        
        [Fact]
        public void ImageCarousel_Constructor_InitializesProperties()
        {
            // Arrange
            var imageUrls = new List<string> 
            { 
                "https://example.com/image1.jpg", 
                "https://example.com/image2.jpg" 
            };
            var captions = new List<string> 
            { 
                "Image 1 Caption", 
                "Image 2 Caption" 
            };
            
            // Act
            var carousel = new ImageCarousel(imageUrls, captions);
            
            // Assert
            carousel.Id.Should().NotBeNullOrEmpty();
            carousel.Id.Length.Should().BeLessOrEqualTo(7);
            carousel.ImageUrls.Should().BeEquivalentTo(imageUrls);
            carousel.Captions.Should().BeEquivalentTo(captions);
            carousel.CurrentIndex.Should().Be(0);
            carousel.Style.Should().Be(FluentStyle.Default);
        }
        
        [Fact]
        public void ImageCarousel_Constructor_HandlesMissingCaptions()
        {
            // Arrange
            var imageUrls = new List<string> 
            { 
                "https://example.com/image1.jpg", 
                "https://example.com/image2.jpg",
                "https://example.com/image3.jpg"
            };
            
            // Act
            var carousel = new ImageCarousel(imageUrls);
            
            // Assert
            carousel.ImageUrls.Should().BeEquivalentTo(imageUrls);
            carousel.Captions.Should().HaveCount(imageUrls.Count);
            carousel.Captions.Should().AllBeEquivalentTo(string.Empty);
        }
        
        [Fact]
        public void ImageCarousel_ToMessage_CreatesMessageWithCorrectButtons()
        {
            // Arrange
            var imageUrls = new List<string> 
            { 
                "https://example.com/image1.jpg", 
                "https://example.com/image2.jpg",
                "https://example.com/image3.jpg"
            };
            var captions = new List<string> 
            { 
                "Image 1 Caption", 
                "Image 2 Caption",
                "Image 3 Caption"
            };
            var carousel = new ImageCarousel(imageUrls, captions);
            
            // Act - First image (should only have Next button and info)
            var message = carousel.ToMessage();
            
            // Assert
            message.Should().NotBeNull();
            message.Text.Should().Be("Image 1 Caption");
            message.ImageUrl.Should().Be("https://example.com/image1.jpg");
            message.Buttons.Should().HaveCount(2); // Info and Next
            message.Buttons[0].Text.Should().Be("1/3");
            message.Buttons[1].Text.Should().Be("Next ▶️");
            message.ButtonsPerRow.Should().Be(3);
            
            // Act - Middle image (should have Prev, Info, and Next buttons)
            carousel.CurrentIndex = 1;
            message = carousel.ToMessage();
            
            // Assert
            message.Text.Should().Be("Image 2 Caption");
            message.ImageUrl.Should().Be("https://example.com/image2.jpg");
            message.Buttons.Should().HaveCount(3); // Prev, Info, and Next
            message.Buttons[0].Text.Should().Be("◀️ Prev");
            message.Buttons[1].Text.Should().Be("2/3");
            message.Buttons[2].Text.Should().Be("Next ▶️");
            
            // Act - Last image (should only have Prev button and info)
            carousel.CurrentIndex = 2;
            message = carousel.ToMessage();
            
            // Assert
            message.Text.Should().Be("Image 3 Caption");
            message.ImageUrl.Should().Be("https://example.com/image3.jpg");
            message.Buttons.Should().HaveCount(2); // Prev and Info
            message.Buttons[0].Text.Should().Be("◀️ Prev");
            message.Buttons[1].Text.Should().Be("3/3");
        }
        
        [Fact]
        public void ProgressIndicator_Constructor_InitializesProperties()
        {
            // Arrange
            var label = "Download Progress";
            var progress = 75;
            
            // Act
            var indicator = new ProgressIndicator(label, progress);
            
            // Assert
            indicator.Id.Should().NotBeNullOrEmpty();
            indicator.Id.Length.Should().BeLessOrEqualTo(7);
            indicator.Label.Should().Be(label);
            indicator.Progress.Should().Be(progress);
            indicator.ShowPercentage.Should().BeTrue();
            indicator.FilledChar.Should().Be("█");
            indicator.EmptyChar.Should().Be("░");
            indicator.Width.Should().Be(10);
            indicator.Style.Should().Be(FluentStyle.Default);
        }
        
        [Fact]
        public void ProgressIndicator_ToMessage_CreatesCorrectProgressBar()
        {
            // Arrange
            var indicator = new ProgressIndicator("Download Progress", 50);
            
            // Act
            var message = indicator.ToMessage();
            
            // Assert
            message.Should().NotBeNull();
            message.Text.Should().Be("Download Progress: █████░░░░░ 50%");
            
            // Test with different progress values
            indicator.Progress = 0;
            message = indicator.ToMessage();
            message.Text.Should().Be("Download Progress: ░░░░░░░░░░ 0%");
            
            indicator.Progress = 100;
            message = indicator.ToMessage();
            message.Text.Should().Be("Download Progress: ██████████ 100%");
            
            // Test without percentage
            indicator.ShowPercentage = false;
            message = indicator.ToMessage();
            message.Text.Should().Be("Download Progress: ██████████");
        }
        
        [Fact]
        public void ProgressIndicator_CustomChars_UsesCustomChars()
        {
            // Arrange
            var indicator = new ProgressIndicator("Upload", 30)
            {
                FilledChar = "■",
                EmptyChar = "□",
                Width = 5
            };
            
            // Act
            var message = indicator.ToMessage();
            
            // Assert
            message.Should().NotBeNull();
            message.Text.Should().Be("Upload: ■■□□□ 30%");
        }
        
        [Fact]
        public void Accordion_Constructor_InitializesProperties()
        {
            // Arrange
            var title = "FAQ";
            var content = "Frequently asked questions go here...";
            var isExpanded = true;
            
            // Act
            var accordion = new Accordion(title, content, isExpanded);
            
            // Assert
            accordion.Id.Should().NotBeNullOrEmpty();
            accordion.Id.Length.Should().BeLessOrEqualTo(7);
            accordion.Title.Should().Be(title);
            accordion.Content.Should().Be(content);
            accordion.IsExpanded.Should().Be(isExpanded);
            accordion.Style.Should().Be(FluentStyle.Default);
        }
        
        [Fact]
        public void Accordion_ToMessage_CreatesCorrectExpandedCollapsedState()
        {
            // Arrange
            var accordion = new Accordion("FAQ", "1. How do I use this?\n2. What is this for?", true);
            
            // Act - Expanded state
            var message = accordion.ToMessage();
            
            // Assert
            message.Should().NotBeNull();
            message.Text.Should().Be("*FAQ*\n\n1. How do I use this?\n2. What is this for?");
            message.ParseMarkdown.Should().BeTrue();
            message.Buttons.Should().ContainSingle();
            message.Buttons[0].Text.Should().Be("▼ Collapse");
            message.Buttons[0].CallbackData.Should().Be($"accordion:{accordion.Id}:collapse");
            
            // Act - Collapsed state
            accordion.IsExpanded = false;
            message = accordion.ToMessage();
            
            // Assert
            message.Text.Should().Be("*FAQ*");
            message.Buttons[0].Text.Should().Be("▶ Expand");
            message.Buttons[0].CallbackData.Should().Be($"accordion:{accordion.Id}:expand");
        }
        
        [Fact]
        public void RichText_Constructor_InitializesProperties()
        {
            // Arrange
            var text = "This is rich text";
            
            // Act
            var richText = new RichText(text);
            
            // Assert
            richText.Id.Should().NotBeNullOrEmpty();
            richText.Id.Length.Should().BeLessOrEqualTo(7);
            richText.Text.Should().Be(text);
            richText.IsBold.Should().BeFalse();
            richText.IsItalic.Should().BeFalse();
            richText.IsUnderlined.Should().BeFalse();
            richText.Alignment.Should().Be(TextAlignment.Left);
            richText.Style.Should().Be(FluentStyle.Default);
        }
        
        [Fact]
        public void RichText_ToMessage_AppliesCorrectFormatting()
        {
            // Arrange
            var richText = new RichText("This is rich text");
            
            // Act - No formatting
            var message = richText.ToMessage();
            
            // Assert
            message.Should().NotBeNull();
            message.Text.Should().Be("This is rich text");
            message.ParseMarkdown.Should().BeTrue();
            
            // Act - Bold formatting
            richText.IsBold = true;
            message = richText.ToMessage();
            
            // Assert
            message.Text.Should().Be("*This is rich text*");
            
            // Act - Bold and italic formatting
            richText.IsItalic = true;
            message = richText.ToMessage();
            
            // Assert
            message.Text.Should().Be("*_This is rich text_*");
            
            // Act - Bold, italic, and underlined formatting
            richText.IsUnderlined = true;
            message = richText.ToMessage();
            
            // Assert
            message.Text.Should().Be("*_This is rich text_*");
        }
        
        [Fact]
        public void RichText_ToMessage_AppliesCorrectAlignment()
        {
            // Arrange
            var richText = new RichText("Line 1\nLine 2");
            
            // Act - Default (left) alignment
            var message = richText.ToMessage();
            
            // Assert
            message.Should().NotBeNull();
            message.Text.Should().Be("Line 1\nLine 2");
            
            // Act - Center alignment
            richText.Alignment = TextAlignment.Center;
            message = richText.ToMessage();
            
            // Assert
            message.Text.Should().Be("      Line 1      \n      Line 2      ");
            
            // Act - Right alignment
            richText.Alignment = TextAlignment.Right;
            message = richText.ToMessage();
            
            // Assert
            message.Text.Should().Be("                Line 1\n                Line 2");
        }
        
        [Fact]
        public void Rating_Constructor_InitializesProperties()
        {
            // Arrange
            var label = "Rate our service";
            var callbackDataPrefix = "rate_service";
            var initialValue = 4;
            
            // Act
            var rating = new Rating(label, callbackDataPrefix, initialValue);
            
            // Assert
            rating.Id.Should().NotBeNullOrEmpty();
            rating.Id.Length.Should().BeLessOrEqualTo(7);
            rating.Label.Should().Be(label);
            rating.CallbackDataPrefix.Should().Be(callbackDataPrefix);
            rating.Value.Should().Be(initialValue);
            rating.Style.Should().Be(FluentStyle.Default);
        }
        
        [Fact]
        public void Rating_Constructor_ClampValueTo0To5()
        {
            // Arrange & Act
            var ratingHigh = new Rating("Rate too high", "rate_high", 10);
            var ratingLow = new Rating("Rate too low", "rate_low", -5);
            
            // Assert
            ratingHigh.Value.Should().Be(5);
            ratingLow.Value.Should().Be(0);
        }
        
        [Fact]
        public void Rating_ToMessage_CreatesCorrectStarDisplay()
        {
            // Arrange
            var rating = new Rating("Rate our service", "rate_service", 3);
            
            // Act
            var message = rating.ToMessage();
            
            // Assert
            message.Should().NotBeNull();
            message.Text.Should().Be("Rate our service: ⭐⭐⭐");
            message.Buttons.Should().HaveCount(5);
            for (int i = 0; i < 5; i++)
            {
                message.Buttons[i].Text.Should().Be((i + 1).ToString());
                message.Buttons[i].CallbackData.Should().Be($"rate_service:{i + 1}");
            }
            message.ButtonsPerRow.Should().Be(5);
            
            // Test with zero rating
            rating.Value = 0;
            message = rating.ToMessage();
            message.Text.Should().Be("Rate our service: Not rated");
            
            // Test with maximum rating
            rating.Value = 5;
            message = rating.ToMessage();
            message.Text.Should().Be("Rate our service: ⭐⭐⭐⭐⭐");
        }
    }
} 