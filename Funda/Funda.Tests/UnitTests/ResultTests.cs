using FluentAssertions;
using Funda.Domain.Models;

namespace Funda.Tests.UnitTests
{
    public class ResultTests
    {
        #region Non-Generic Result Tests

        [Fact]
        public void Success_WithNoWarnings_ShouldBeSuccess_And_HaveEmptyLists()
        {
            // Act
            var result = Result.Success();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Errors.Should().BeEmpty();
            result.Warnings.Should().BeEmpty();
        }

        [Fact]
        public void Success_WithWarnings_ShouldBeSuccess_And_ContainWarnings()
        {
            // Arrange
            var warnings = new[] { "Warning 1", "Warning 2" };

            // Act
            var result = Result.Success(warnings);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Errors.Should().BeEmpty();
            result.Warnings.Should().BeEquivalentTo("Warning 1", "Warning 2");
        }

        [Fact]
        public void Failure_WithSingleError_ShouldBeFailure_And_ContainSingleError()
        {
            // Act
            var result = Result.Failure("Something went wrong");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle().Which.Should().Be("Something went wrong");
            result.Warnings.Should().BeEmpty();
        }

        [Fact]
        public void Failure_WithMultipleErrorsAndWarnings_ShouldBeFailure_And_ContainAll()
        {
            // Arrange
            var errors = new List<string> { "Error A", "Error B" };
            var warnings = new List<string> { "Warning X" };

            // Act
            var result = Result.Failure(errors, warnings);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().HaveCount(2).And.ContainInOrder("Error A", "Error B");
            result.Warnings.Should().ContainSingle().Which.Should().Be("Warning X");
        }

        #endregion

        #region Generic Result<T> Tests

        [Fact]
        public void GenericSuccess_WithValue_ShouldBeSuccess_And_ContainData()
        {
            // Arrange
            var testData = new TestUser(Id: 1, Name: "Alice");

            // Act
            var result = Result<TestUser>.Success(testData);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().BeEquivalentTo(testData);
            result.Errors.Should().BeEmpty();
            result.Warnings.Should().BeEmpty();
        }

        [Fact]
        public void GenericSuccess_WithWarnings_ShouldBeSuccess_And_ContainDataAndWarnings()
        {
            // Arrange
            var warnings = new[] { "Low storage warning" };

            // Act
            var result = Result<string>.Success("Data Payload", warnings);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().Be("Data Payload");
            result.Warnings.Should().ContainSingle().Which.Should().Be("Low storage warning");
        }

        [Fact]
        public void GenericFailure_WithSingleError_ShouldBeFailure_And_HaveDefaultData()
        {
            // Act
            var result = Result<int>.Failure("Database timeout");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().Be(0); // default(int)
            result.Errors.Should().ContainSingle().Which.Should().Be("Database timeout");
        }

        [Fact]
        public void GenericFailure_WithMultipleErrorsAndWarnings_ShouldBeFailure_And_HaveDefaultData()
        {
            // Arrange
            var errors = new[] { "Validation failed", "Unauthorized" };
            var warnings = new[] { "Deprecation notice" };

            // Act
            var result = Result<object>.Failure(errors, warnings);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeNull(); // default(object)
            result.Errors.Should().HaveCount(2).And.Contain("Validation failed");
            result.Warnings.Should().ContainSingle().Which.Should().Be("Deprecation notice");
        }

        #endregion

        // Helper record for testing complex data types
        private record TestUser(int Id, string Name);
    }
}
