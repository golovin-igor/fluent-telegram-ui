# Contributing to FluentTelegramUI

Thank you for your interest in contributing to FluentTelegramUI! This document outlines the process for contributing to the project and provides guidelines to help you get started.

## Getting Started

### Prerequisites

- .NET 6.0 SDK or later
- A Telegram Bot Token for testing (from [@BotFather](https://t.me/botfather))
- Git installed on your local machine
- An IDE with C# support (Visual Studio, VS Code with C# extension, Rider, etc.)

### Setting Up the Development Environment

1. Fork the repository on GitHub
2. Clone your fork locally:
   ```
   git clone https://github.com/YOUR-USERNAME/fluent-telegram-ui.git
   ```
3. Add the upstream repository as a remote:
   ```
   git remote add upstream https://github.com/ORIGINAL-OWNER/fluent-telegram-ui.git
   ```
4. Build the project to ensure everything works:
   ```
   dotnet build
   ```
5. Run the tests to verify your setup:
   ```
   dotnet test
   ```

## Development Workflow

1. Create a new branch for your feature or bug fix:
   ```
   git checkout -b feature/your-feature-name
   ```
   or
   ```
   git checkout -b fix/issue-description
   ```

2. Make your changes, following the coding conventions outlined below

3. Add tests for your changes

4. Run the tests to ensure everything passes:
   ```
   dotnet test
   ```

5. Commit your changes with a clear, descriptive commit message:
   ```
   git commit -m "Add feature: your feature description"
   ```

6. Push your branch to your fork:
   ```
   git push origin feature/your-feature-name
   ```

7. Create a Pull Request from your branch to the main repository

## Coding Conventions

### Style Guidelines

- Follow C# naming conventions:
  - PascalCase for class names, method names, and properties
  - camelCase for local variables and parameters
  - _camelCase for private fields
  - ALL_CAPS for constants
- Use meaningful, descriptive names for classes, methods, and variables
- Add XML documentation comments to all public APIs
- Keep methods short and focused on a single responsibility
- Prefer async/await over callbacks when working with asynchronous code

### Project Structure

- Place new functionality in the appropriate namespace:
  - `FluentTelegramUI` - Core functionality
  - `FluentTelegramUI.Models` - Data models
  - `FluentTelegramUI.Handlers` - Event handlers
  - `FluentTelegramUI.Builders` - Builder pattern components
  - `FluentTelegramUI.Examples` - Example code

### Testing

- Write unit tests for all new functionality
- Maintain existing tests when modifying code
- Test both success and failure paths
- Use descriptive test method names that explain what the test is checking
- Keep test classes organized by feature or component

## Pull Request Process

1. Update the README.md with details of any new features, changes to the API, or configuration changes
2. Update the example code if your changes affect how users would implement features
3. Make sure your code adheres to the existing style and conventions
4. Ensure all tests pass successfully
5. The PR should be reviewed by at least one core team member before being merged
6. If you're addressing an existing issue, reference it in your PR description with the issue number

## Context Parameters

When working with callbacks or handlers, always use the context parameters approach. This helps access important information like chatId and username:

```csharp
screen.OnCallback("button_click", async (data, context) => 
{
    // Extract data from context
    long chatId = (long)context["chatId"];
    string username = (string)context["username"];
    
    // Use the extracted data
    await bot.NavigateToScreenAsync(chatId, anotherScreen.Id);
    return true;
});
```

## Finding an Issue to Work On

If you're looking to contribute but don't have a specific feature or bug in mind, check the [Issues](https://github.com/ORIGINAL-OWNER/fluent-telegram-ui/issues) section of the repository. Issues labeled with "good first issue" are a great place to start!

## Questions?

If you have questions about contributing, open an issue with the "question" label, and we'll be happy to help! 