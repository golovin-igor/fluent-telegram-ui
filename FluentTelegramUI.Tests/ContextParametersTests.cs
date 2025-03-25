using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FluentTelegramUI.Models;
using Xunit;
using Telegram.Bot.Types;

namespace FluentTelegramUI.Tests
{
    public class ContextParametersTests
    {
        [Fact]
        public async Task Callback_HandlerReceivesContext()
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
                { "lastName", "User" },
                { "messageId", 55555 },
                { "callbackQuery", new CallbackQuery() }
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
            contextPassedToHandler["messageId"].Should().Be(55555);
            contextPassedToHandler["callbackQuery"].Should().BeOfType<CallbackQuery>();
        }
        
        [Fact]
        public async Task TextInput_HandlerReceivesContext()
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
                { "username", "testuser" },
                { "firstName", "Test" },
                { "lastName", "User" },
                { "messageId", 55555 },
                { "message", new Telegram.Bot.Types.Message() }
            };
            
            // Act
            var handler = screen.EventHandlers["text_input:test_state"];
            var handlerResult = await handler.Invoke("test input", testContext);
            
            // Assert
            handlerResult.Should().BeTrue();
            capturedContext.Should().NotBeNull();
            capturedContext.Should().HaveCount(testContext.Count);
            capturedContext["chatId"].Should().Be(123456789L);
            capturedContext["userId"].Should().Be(987654321L);
            capturedContext["username"].Should().Be("testuser");
            capturedContext["firstName"].Should().Be("Test");
            capturedContext["lastName"].Should().Be("User");
            capturedContext["messageId"].Should().Be(55555);
            capturedContext["message"].Should().BeOfType<Telegram.Bot.Types.Message>();
        }
        
        [Fact]
        public void ContextParameters_InCallbacks_CanBeAccessed()
        {
            // Arrange
            var screen = new Screen();
            var extractedValues = new Dictionary<string, object>();
            
            // Set up a callback that extracts values from context
            screen.OnCallback("extract_values", (data, context) => {
                // Extract values as they would be in real code
                long chatId = (long)context["chatId"];
                long userId = (long)context["userId"];
                string username = (string)context["username"];
                string firstName = (string)context["firstName"];
                int messageId = (int)context["messageId"];
                
                // Store extracted values for assertion
                extractedValues["chatId"] = chatId;
                extractedValues["userId"] = userId;
                extractedValues["username"] = username;
                extractedValues["firstName"] = firstName;
                extractedValues["messageId"] = messageId;
                
                return Task.FromResult(true);
            });
            
            // Create a context with various values
            var context = new Dictionary<string, object>
            {
                { "chatId", 12345L },
                { "userId", 54321L },
                { "username", "johndoe" },
                { "firstName", "John" },
                { "lastName", "Doe" },
                { "messageId", 9876 },
                { "callbackQuery", new CallbackQuery() }
            };
            
            // Act
            screen.EventHandlers["extract_values"].Invoke("extract_values", context);
            
            // Assert
            extractedValues["chatId"].Should().Be(12345L);
            extractedValues["userId"].Should().Be(54321L);
            extractedValues["username"].Should().Be("johndoe");
            extractedValues["firstName"].Should().Be("John");
            extractedValues["messageId"].Should().Be(9876);
        }
    }
} 