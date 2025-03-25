using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentTelegramUI.Models;
using Xunit;

namespace FluentTelegramUI.Tests
{
    public class UIControlTests
    {
        [Fact]
        public void UIControl_Abstract_HasRequiredProperties()
        {
            // Arrange & Act
            var type = typeof(UIControl);
            
            // Assert
            type.Should().BeAbstract();
            type.GetProperty("Id").Should().NotBeNull();
            type.GetProperty("Style").Should().NotBeNull();
            type.GetMethod("ToMessage").Should().NotBeNull();
        }
        
        [Fact]
        public void TextButton_Constructor_InitializesProperties()
        {
            // Arrange
            var text = "Test Button";
            var callbackData = "test_callback";
            
            // Act
            var button = new TextButton(text, callbackData);
            
            // Assert
            button.Id.Should().NotBeNullOrEmpty();
            button.Text.Should().Be(text);
            button.CallbackData.Should().Be(callbackData);
            button.Style.Should().Be(FluentStyle.Default);
        }
        
        [Fact]
        public void TextButton_ToMessage_CreatesMessageWithButton()
        {
            // Arrange
            var text = "Test Button";
            var callbackData = "test_callback";
            var button = new TextButton(text, callbackData);
            
            // Act
            var message = button.ToMessage();
            
            // Assert
            message.Should().NotBeNull();
            message.Text.Should().BeEmpty();
            message.Buttons.Should().ContainSingle();
            message.Buttons[0].Text.Should().Be(text);
            message.Buttons[0].CallbackData.Should().Be(callbackData);
        }
        
        [Fact]
        public void ButtonGroup_Constructor_InitializesProperties()
        {
            // Arrange
            var buttons = new List<Button>
            {
                new Button { Text = "Button 1", CallbackData = "callback1" },
                new Button { Text = "Button 2", CallbackData = "callback2" }
            };
            var buttonsPerRow = 2;
            
            // Act
            var buttonGroup = new ButtonGroup(buttons, buttonsPerRow);
            
            // Assert
            buttonGroup.Id.Should().NotBeNullOrEmpty();
            buttonGroup.Buttons.Should().HaveSameCount(buttons);
            buttonGroup.Buttons.Should().Contain(buttons);
            buttonGroup.ButtonsPerRow.Should().Be(buttonsPerRow);
            buttonGroup.Style.Should().Be(FluentStyle.Default);
        }
        
        [Fact]
        public void ButtonGroup_AddButton_AddsButtonToList()
        {
            // Arrange
            var buttonGroup = new ButtonGroup(new List<Button>());
            var button = new Button { Text = "Test Button", CallbackData = "test" };
            
            // Act
            buttonGroup.AddButton(button);
            
            // Assert
            buttonGroup.Buttons.Should().ContainSingle();
            buttonGroup.Buttons[0].Should().BeSameAs(button);
        }
        
        [Fact]
        public void ButtonGroup_ToMessage_CreatesMessageWithButtons()
        {
            // Arrange
            var buttons = new List<Button>
            {
                new Button { Text = "Button 1", CallbackData = "callback1" },
                new Button { Text = "Button 2", CallbackData = "callback2" }
            };
            var buttonsPerRow = 2;
            var buttonGroup = new ButtonGroup(buttons, buttonsPerRow);
            
            // Act
            var message = buttonGroup.ToMessage();
            
            // Assert
            message.Should().NotBeNull();
            message.Text.Should().BeEmpty();
            message.Buttons.Should().HaveSameCount(buttons);
            message.ButtonsPerRow.Should().Be(buttonsPerRow);
        }
        
        [Fact]
        public void TextInput_Constructor_InitializesProperties()
        {
            // Arrange
            var label = "Enter your name:";
            var placeholder = "John Doe";
            
            // Act
            var textInput = new TextInput(label, placeholder);
            
            // Assert
            textInput.Id.Should().NotBeNullOrEmpty();
            textInput.Label.Should().Be(label);
            textInput.Placeholder.Should().Be(placeholder);
            textInput.Style.Should().Be(FluentStyle.Default);
        }
        
        [Fact]
        public void TextInput_ToMessage_CreatesMessageWithLabelText()
        {
            // Arrange
            var label = "Enter your name:";
            var placeholder = "John Doe";
            var textInput = new TextInput(label, placeholder);
            
            // Act
            var message = textInput.ToMessage();
            
            // Assert
            message.Should().NotBeNull();
            message.Text.Should().Be(label);
            message.Buttons.Should().BeEmpty();
        }
    }
} 