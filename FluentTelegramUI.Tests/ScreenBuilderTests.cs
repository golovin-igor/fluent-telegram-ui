using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentTelegramUI.Builders;
using FluentTelegramUI.Models;
using Moq;
using Xunit;

namespace FluentTelegramUI.Tests
{
    public class ScreenBuilderTests
    {
        private readonly Mock<FluentTelegramBot> _botMock;
        private readonly ScreenBuilder _builder;
        
        public ScreenBuilderTests()
        {
            _botMock = new Mock<FluentTelegramBot>();
            _builder = new ScreenBuilder(_botMock.Object, "Test Screen");
        }
        
        [Fact]
        public void ScreenBuilder_Constructor_CreatesScreen()
        {
            // Act
            var screen = _builder.Build();
            
            // Assert
            screen.Should().NotBeNull();
            screen.Title.Should().Be("Test Screen");
            _botMock.Verify(b => b.RegisterScreen(It.IsAny<Screen>(), It.IsAny<bool>()), Times.Once);
        }
        
        [Fact]
        public void ScreenBuilder_WithContent_SetsTextContent()
        {
            // Arrange
            var text = "Screen content";
            var parseMarkdown = true;
            
            // Act
            _builder.WithContent(text, parseMarkdown);
            var screen = _builder.Build();
            
            // Assert
            screen.Content.Text.Should().Be(text);
            screen.Content.ParseMarkdown.Should().Be(parseMarkdown);
        }
        
        [Fact]
        public void ScreenBuilder_WithContent_SetsMessageContent()
        {
            // Arrange
            var message = new Message
            {
                Text = "Screen content",
                ParseMarkdown = true
            };
            
            // Act
            _builder.WithContent(message);
            var screen = _builder.Build();
            
            // Assert
            screen.Content.Should().BeSameAs(message);
        }
        
        [Fact]
        public void ScreenBuilder_AddButton_AddsButtonToContent()
        {
            // Arrange
            var text = "Button Text";
            var callbackData = "button_callback";
            
            // Act
            _builder.AddButton(text, callbackData);
            var screen = _builder.Build();
            
            // Assert
            screen.Content.Buttons.Should().ContainSingle();
            screen.Content.Buttons[0].Text.Should().Be(text);
            screen.Content.Buttons[0].CallbackData.Should().Be(callbackData);
        }
        
        [Fact]
        public void ScreenBuilder_AddTextButton_AddsButtonControl()
        {
            // Arrange
            var text = "Button Text";
            var callbackData = "button_callback";
            
            // Act
            _builder.AddTextButton(text, callbackData);
            var screen = _builder.Build();
            
            // Assert
            screen.Controls.Should().ContainSingle();
            screen.Controls[0].Should().BeOfType<TextButton>();
            var button = (TextButton)screen.Controls[0];
            button.Text.Should().Be(text);
            button.CallbackData.Should().Be(callbackData);
        }
        
        [Fact]
        public void ScreenBuilder_WithButtonsPerRow_SetsButtonsPerRow()
        {
            // Arrange
            var buttonsPerRow = 3;
            
            // Act
            _builder.WithButtonsPerRow(buttonsPerRow);
            var screen = _builder.Build();
            
            // Assert
            screen.Content.ButtonsPerRow.Should().Be(buttonsPerRow);
        }
        
        [Fact]
        public void ScreenBuilder_OnCallback_AddsCallbackHandler()
        {
            // Arrange
            var callbackData = "test_callback";
            Func<string, Task<bool>> handler = _ => Task.FromResult(true);
            
            // Act
            _builder.OnCallback(callbackData, handler);
            var screen = _builder.Build();
            
            // Assert
            screen.EventHandlers.Should().ContainKey(callbackData);
            screen.EventHandlers[callbackData].Should().BeSameAs(handler);
        }
        
        [Fact]
        public void ScreenBuilder_AddNavigationButton_AddsButtonWithScreenPrefix()
        {
            // Arrange
            var text = "Go to Settings";
            var targetScreenId = "settings-screen-id";
            
            // Act
            _builder.AddNavigationButton(text, targetScreenId);
            var screen = _builder.Build();
            
            // Assert
            screen.Content.Buttons.Should().ContainSingle();
            screen.Content.Buttons[0].Text.Should().Be(text);
            screen.Content.Buttons[0].CallbackData.Should().Be($"screen:{targetScreenId}");
        }
        
        [Fact]
        public void ScreenBuilder_WithParent_SetsParentScreen()
        {
            // Arrange
            var parentScreen = new Screen { Title = "Parent Screen" };
            
            // Act
            _builder.WithParent(parentScreen);
            var screen = _builder.Build();
            
            // Assert
            screen.ParentScreen.Should().BeSameAs(parentScreen);
        }
        
        [Fact]
        public void ScreenBuilder_AllowBack_SetsBackNavigationFlag()
        {
            // Act
            _builder.AllowBack(false);
            var screen = _builder.Build();
            
            // Assert
            screen.AllowBackNavigation.Should().BeFalse();
            
            // Act again with default (true)
            _builder.AllowBack();
            screen = _builder.Build();
            
            // Assert
            screen.AllowBackNavigation.Should().BeTrue();
        }
        
        [Fact]
        public void ScreenBuilder_WithBackButtonText_SetsBackButtonText()
        {
            // Arrange
            var text = "Return";
            
            // Act
            _builder.WithBackButtonText(text);
            var screen = _builder.Build();
            
            // Assert
            screen.BackButtonText.Should().Be(text);
        }
        
        [Fact]
        public void ScreenBuilder_AsMainScreen_SetsMainScreenFlag()
        {
            // Act
            _builder.AsMainScreen();
            var screen = _builder.Build();
            
            // Assert
            screen.IsMainScreen.Should().BeTrue();
            _botMock.Verify(b => b.SetMainScreen(It.IsAny<Screen>()), Times.Once);
        }
        
        [Fact]
        public void ScreenBuilder_MethodChaining_Works()
        {
            // Act
            var result = _builder
                .WithContent("Test content")
                .AddButton("Button 1", "callback1")
                .WithButtonsPerRow(2)
                .AllowBack()
                .WithBackButtonText("Go Back")
                .AsMainScreen();
            
            // Assert
            result.Should().BeSameAs(_builder);
        }
    }
} 