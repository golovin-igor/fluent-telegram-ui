using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FluentTelegramUI.Models;
using Xunit;

namespace FluentTelegramUI.Tests
{
    public class ScreenTests
    {
        [Fact]
        public void Screen_Constructor_InitializesProperties()
        {
            // Act
            var screen = new Screen();
            
            // Assert
            screen.Id.Should().NotBeNullOrEmpty();
            screen.Id.Length.Should().BeLessOrEqualTo(7);
            screen.Title.Should().BeEmpty();
            screen.Content.Should().NotBeNull();
            screen.Controls.Should().NotBeNull().And.BeEmpty();
            screen.EventHandlers.Should().NotBeNull().And.BeEmpty();
            screen.ParentScreen.Should().BeNull();
            screen.AllowBackNavigation.Should().BeTrue();
            screen.IsMainScreen.Should().BeFalse();
            screen.BackButtonText.Should().Be("⬅️ Back");
        }
        
        [Fact]
        public void Screen_OnCallback_AddsEventHandler()
        {
            // Arrange
            var screen = new Screen();
            Func<string, Dictionary<string, object>, Task<bool>> handler = (_, _) => Task.FromResult(true);
            
            // Act
            screen.OnCallback("test_callback", handler);
            
            // Assert
            screen.EventHandlers.Should().ContainKey("test_callback");
            screen.EventHandlers["test_callback"].Should().BeSameAs(handler);
        }
        
        [Fact]
        public async Task Screen_OnCallback_HandlerReceivesContext()
        {
            // Arrange
            var screen = new Screen();
            var contextPassedToHandler = new Dictionary<string, object>();
            
            // Set up callback that captures the context
            screen.OnCallback("test_context", async (data, context) => {
                contextPassedToHandler = context;
                return true;
            });
            
            // Create a test context
            var testContext = new Dictionary<string, object>
            {
                { "chatId", 123456789L },
                { "userId", 987654321L },
                { "username", "testuser" },
                { "firstName", "Test" },
                { "lastName", "User" }
            };
            
            // Act
            var result = await screen.EventHandlers["test_context"].Invoke("test_context", testContext);
            
            // Assert
            result.Should().BeTrue();
            contextPassedToHandler.Should().NotBeNull().And.HaveCount(testContext.Count);
            contextPassedToHandler["chatId"].Should().Be(123456789L);
            contextPassedToHandler["userId"].Should().Be(987654321L);
            contextPassedToHandler["username"].Should().Be("testuser");
            contextPassedToHandler["firstName"].Should().Be("Test");
            contextPassedToHandler["lastName"].Should().Be("User");
        }
        
        [Fact]
        public void Screen_OnTextInput_HandlerReceivesContext()
        {
            // Arrange
            var screen = new Screen();
            Dictionary<string, object> capturedContext = null;
            
            // Set up text input handler
            screen.OnTextInput("test_state", (text, context) => {
                capturedContext = context;
                return Task.FromResult(true);
            });
            
            // Create a test context
            var testContext = new Dictionary<string, object>
            {
                { "chatId", 123456789L },
                { "userId", 987654321L },
                { "messageId", 55555 },
                { "message", new Telegram.Bot.Types.Message() }
            };
            
            // Act
            var handler = screen.EventHandlers["text_input:test_state"];
            var handlerTask = handler.Invoke("test input", testContext);
            
            // Assert
            screen.EventHandlers.Should().ContainKey("text_input:test_state");
            capturedContext.Should().BeSameAs(testContext);
            capturedContext["chatId"].Should().Be(123456789L);
            capturedContext["userId"].Should().Be(987654321L);
            capturedContext["messageId"].Should().Be(55555);
            capturedContext["message"].Should().BeOfType<Telegram.Bot.Types.Message>();
        }
        
        [Fact]
        public void Screen_AddControl_AddsControlToList()
        {
            // Arrange
            var screen = new Screen();
            var control = new TextButton("Test Button", "test_callback");
            
            // Act
            screen.AddControl(control);
            
            // Assert
            screen.Controls.Should().ContainSingle();
            screen.Controls[0].Should().BeSameAs(control);
        }
        
        [Fact]
        public void Screen_AddControls_AddsMultipleControlsToList()
        {
            // Arrange
            var screen = new Screen();
            var controls = new List<UIControl>
            {
                new TextButton("Button 1", "callback1"),
                new TextButton("Button 2", "callback2")
            };
            
            // Act
            screen.AddControls(controls);
            
            // Assert
            screen.Controls.Should().HaveCount(2);
            screen.Controls[0].Should().BeSameAs(controls[0]);
            screen.Controls[1].Should().BeSameAs(controls[1]);
        }
        
        [Fact]
        public void Screen_WithContent_SetsContentMessage()
        {
            // Arrange
            var screen = new Screen();
            var message = new Message { Text = "Test Content" };
            
            // Act
            screen.WithContent(message);
            
            // Assert
            screen.Content.Should().BeSameAs(message);
            screen.Content.Text.Should().Be("Test Content");
        }
        
        [Fact]
        public void Screen_WithParent_SetsParentScreen()
        {
            // Arrange
            var screen = new Screen();
            var parentScreen = new Screen { Title = "Parent Screen" };
            
            // Act
            screen.WithParent(parentScreen);
            
            // Assert
            screen.ParentScreen.Should().BeSameAs(parentScreen);
        }
        
        [Fact]
        public void Screen_AllowBack_SetsBackNavigationFlag()
        {
            // Arrange
            var screen = new Screen();
            
            // Act
            screen.AllowBack(false);
            
            // Assert
            screen.AllowBackNavigation.Should().BeFalse();
            
            // Act again with the default value
            screen.AllowBack();
            
            // Assert
            screen.AllowBackNavigation.Should().BeTrue();
        }
        
        [Fact]
        public void Screen_WithBackButtonText_SetsBackButtonText()
        {
            // Arrange
            var screen = new Screen();
            var customText = "Go Back";
            
            // Act
            screen.WithBackButtonText(customText);
            
            // Assert
            screen.BackButtonText.Should().Be(customText);
        }
        
        [Fact]
        public void Screen_AsMainScreen_SetsMainScreenFlag()
        {
            // Arrange
            var screen = new Screen();
            
            // Act
            screen.AsMainScreen();
            
            // Assert
            screen.IsMainScreen.Should().BeTrue();
            
            // Act again with false
            screen.AsMainScreen(false);
            
            // Assert
            screen.IsMainScreen.Should().BeFalse();
        }
        
        [Fact]
        public void Screen_MethodChaining_Works()
        {
            // Arrange
            var screen = new Screen();
            var parentScreen = new Screen();
            var message = new Message();
            Func<string, Dictionary<string, object>, Task<bool>> handler = (_, _) => Task.FromResult(true);
            
            // Act
            var result = screen
                .WithContent(message)
                .WithParent(parentScreen)
                .AllowBack(true)
                .WithBackButtonText("Back")
                .AsMainScreen(true)
                .OnCallback("test", handler);
            
            // Assert
            result.Should().BeSameAs(screen);
        }
    }
} 