using Xunit;
using FluentAssertions;

namespace FluentTelegramUI.Tests;

public class UnitTests
{
    [Fact]
    public void Test_Setup_ShouldWork()
    {
        // Arrange
        var expected = true;

        // Act
        var actual = true;

        // Assert
        actual.Should().Be(expected);
    }
} 