using System.Collections.Generic;
using FluentAssertions;
using FluentTelegramUI.Models;
using Xunit;

namespace FluentTelegramUI.Tests
{
    public class StateMachineTests
    {
        [Fact]
        public void StateMachine_GetState_ReturnsDefaultWhenNotSet()
        {
            // Arrange
            var stateMachine = new StateMachine();
            long chatId = 123;
            string key = "testKey";
            
            // Act
            var result = stateMachine.GetState<string>(chatId, key);
            
            // Assert
            result.Should().BeNull();
        }
        
        [Fact]
        public void StateMachine_SetState_StoresValue()
        {
            // Arrange
            var stateMachine = new StateMachine();
            long chatId = 123;
            string key = "testKey";
            string value = "testValue";
            
            // Act
            stateMachine.SetState(chatId, key, value);
            var result = stateMachine.GetState<string>(chatId, key);
            
            // Assert
            result.Should().Be(value);
        }
        
        [Fact]
        public void StateMachine_RemoveState_RemovesValue()
        {
            // Arrange
            var stateMachine = new StateMachine();
            long chatId = 123;
            string key = "testKey";
            string value = "testValue";
            
            // Act
            stateMachine.SetState(chatId, key, value);
            bool removed = stateMachine.RemoveState(chatId, key);
            var result = stateMachine.GetState<string>(chatId, key);
            
            // Assert
            removed.Should().BeTrue();
            result.Should().BeNull();
        }
        
        [Fact]
        public void StateMachine_ClearState_RemovesAllValues()
        {
            // Arrange
            var stateMachine = new StateMachine();
            long chatId = 123;
            
            // Act
            stateMachine.SetState(chatId, "key1", "value1");
            stateMachine.SetState(chatId, "key2", "value2");
            stateMachine.ClearState(chatId);
            
            var result1 = stateMachine.GetState<string>(chatId, "key1");
            var result2 = stateMachine.GetState<string>(chatId, "key2");
            
            // Assert
            result1.Should().BeNull();
            result2.Should().BeNull();
        }
        
        [Fact]
        public void StateMachine_GetCurrentScreen_ReturnsNull_WhenNotSet()
        {
            // Arrange
            var stateMachine = new StateMachine();
            long chatId = 123;
            
            // Act
            var result = stateMachine.GetCurrentScreen(chatId);
            
            // Assert
            result.Should().BeNull();
        }
        
        [Fact]
        public void StateMachine_SetCurrentScreen_StoresScreenId()
        {
            // Arrange
            var stateMachine = new StateMachine();
            long chatId = 123;
            string screenId = "screen1";
            
            // Act
            stateMachine.SetCurrentScreen(chatId, screenId);
            var result = stateMachine.GetCurrentScreen(chatId);
            
            // Assert
            result.Should().Be(screenId);
        }
        
        [Fact]
        public void StateMachine_IsInState_ReturnsFalse_WhenStateNotSet()
        {
            // Arrange
            var stateMachine = new StateMachine();
            long chatId = 123;
            
            // Act
            var result = stateMachine.IsInState(chatId, "any_state");
            
            // Assert
            result.Should().BeFalse();
        }
        
        [Fact]
        public void StateMachine_SetState_SetsNamedState()
        {
            // Arrange
            var stateMachine = new StateMachine();
            long chatId = 123;
            string stateName = "waiting_for_input";
            
            // Act
            stateMachine.SetState(chatId, stateName);
            var isInState = stateMachine.IsInState(chatId, stateName);
            var stateValue = stateMachine.GetState<string>(chatId, "state");
            
            // Assert
            isInState.Should().BeTrue();
            stateValue.Should().Be(stateName);
        }
        
        [Fact]
        public void StateMachine_GetAllState_ReturnsAllStateForChat()
        {
            // Arrange
            var stateMachine = new StateMachine();
            long chatId = 123;
            
            // Act
            stateMachine.SetState(chatId, "key1", "value1");
            stateMachine.SetState(chatId, "key2", 42);
            var allState = stateMachine.GetAllState(chatId);
            
            // Assert
            allState.Should().ContainKey("key1");
            allState.Should().ContainKey("key2");
            allState["key1"].Should().Be("value1");
            allState["key2"].Should().Be(42);
        }
        
        [Fact]
        public void StateMachine_GetAllState_ReturnsEmptyDictionary_WhenNoState()
        {
            // Arrange
            var stateMachine = new StateMachine();
            long chatId = 123;
            
            // Act
            var allState = stateMachine.GetAllState(chatId);
            
            // Assert
            allState.Should().NotBeNull();
            allState.Should().BeEmpty();
        }
    }
} 