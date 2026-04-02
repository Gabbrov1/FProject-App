using System;
using CodeIndex.App;
using Xunit;
using FluentAssertions;

namespace CodeIndex.App.Tests;

public class pageLoaderTests
{
    [Fact]
    public void HandlePythonError_Should_()
    {
        // Arrange
        var sut = new pageLoader();

        // Act
        sut.HandlePythonError(1, "test");

        // Assert
        // Add your assertions here
    }
}