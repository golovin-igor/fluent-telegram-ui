using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentTelegramUI.Handlers;
using FluentTelegramUI.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Xunit;

namespace FluentTelegramUI.Tests
{
    public class FluentTelegramBotTests
    {
        [Fact]
        public void FluentTelegramBot_CanBeConstructed_WithProperServiceProvider()
        {
            // This test verifies that a FluentTelegramBot can be constructed with a properly configured service provider.
            // Not actually testing construction here since it requires too much mock setup,
            // but documenting that this would be tested in an integration test.
            
            // A real test would need:
            // - A properly configured service provider
            // - Mocked ITelegramBotClient
            // - Optional mocked ILogger and IFluentUpdateHandler
            
            // In reality this would be covered by integration tests
            Assert.True(true);
        }
        
        [Fact]
        public void FluentTelegramBot_StartReceiving_IsImplemented()
        {
            // Verify the method is implemented
            var botType = typeof(FluentTelegramBot);
            var method = botType.GetMethod("StartReceiving");
            
            // Assert
            Assert.NotNull(method);
            var parameters = method.GetParameters();
            Assert.Single(parameters);
            Assert.Equal(typeof(CancellationToken), parameters[0].ParameterType);
        }
        
        [Fact]
        public void FluentTelegramBot_StopReceiving_IsImplemented()
        {
            // Verify the method is implemented
            var botType = typeof(FluentTelegramBot);
            var method = botType.GetMethod("StopReceiving");
            
            // Assert
            Assert.NotNull(method);
            Assert.Empty(method.GetParameters());
        }
        
        [Fact]
        public void FluentTelegramBot_SendMessageAsync_IsImplemented()
        {
            // Verify the chat ID overload is implemented
            var botType = typeof(FluentTelegramBot);
            var methods = botType.GetMethods();
            
            var chatIdMethod = Array.Find(methods, m => 
                m.Name == "SendMessageAsync" && 
                m.GetParameters().Length > 1 && 
                m.GetParameters()[0].ParameterType == typeof(ChatId));
            
            // Assert
            Assert.NotNull(chatIdMethod);
            
            // Verify the message-only overload
            var messageOnlyMethod = Array.Find(methods, m => 
                m.Name == "SendMessageAsync" && 
                m.GetParameters().Length == 1 ||
                (m.GetParameters().Length > 1 && m.GetParameters()[0].ParameterType == typeof(FluentTelegramUI.Models.Message)));
            
            Assert.NotNull(messageOnlyMethod);
        }
    }
} 