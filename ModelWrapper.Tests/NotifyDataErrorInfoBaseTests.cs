namespace ModelWrapper
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public static class Messages
    {
        public const string Message1 = "Property1 must be between 1 and 10.";
        public const string Message1x3 = "Property1 must be 10 when Property3 is true.";
        public const string Message2x3 = "Property2 must not be null when Property3 is true.";
        public const string Message4 = "Property4 must not be null or empty.";
    }

    public class NotifyDataErrorInfoBaseTests
    {
        private const string Property1 = "Property1";
        private const string Property2 = "Property2";
        private const string Property3 = "Property3";
        private const string Property4 = "Property4";

        [Fact]
        public void ClearErrors_ErrorsListContainsMultipleErrors_ClearsAllErrorsFromTheList()
        {
            // Arrange
            TestModel model = GetTestModel(0, null, true, "");

            model.Validate();

            // Act
            model.Clear();

            // Assert
            model.HasErrors
                .Should()
                .BeFalse();
            AssertValidationErrors(model, Property1);
            AssertValidationErrors(model, Property2);
            AssertValidationErrors(model, Property3);
            AssertValidationErrors(model, Property4);
        }

        [Fact]
        public void Validate_ModelContainsMultipleErrors_ReturnsAllErrorMessages()
        {
            // Arrange
            TestModel model = GetTestModel(0, null, true, "");

            // Act
            model.Validate();

            // Assert
            model.HasErrors
                .Should()
                .BeTrue();
            AssertValidationErrors(model, Property1, Messages.Message1, Messages.Message1x3);
            AssertValidationErrors(model, Property2, Messages.Message2x3);
            AssertValidationErrors(model, Property3, Messages.Message1x3, Messages.Message2x3);
            AssertValidationErrors(model, Property4, Messages.Message4);
        }

        [Fact]
        public void Validate_ModelContainsOneError_ReturnsErrorMessage()
        {
            // Arrange
            TestModel model = GetTestModel(11, "test", false, "valid");

            // Act
            model.Validate();

            // Assert
            model.HasErrors
                .Should()
                .BeTrue();
            AssertValidationErrors(model, Property1, Messages.Message1);
            AssertValidationErrors(model, Property2);
            AssertValidationErrors(model, Property3);
            AssertValidationErrors(model, Property4);
        }

        [Fact]
        public void Validate_NoValidationErrors_ErrorsListShouldBeEmpty()
        {
            // Arrange
            TestModel model = GetTestModel(10, "test", true, "valid");

            // Act
            model.Validate();

            // Assert
            model.HasErrors
                .Should()
                .BeFalse();
            AssertValidationErrors(model, Property1);
            AssertValidationErrors(model, Property2);
            AssertValidationErrors(model, Property3);
            AssertValidationErrors(model, Property4);
        }

            private static void AssertValidationErrors(TestModel model, string propertyName, params string[] expectedErrorMessages)
        {
            List<string> errors = (model.GetErrors(propertyName) as List<string>)!;

            errors
                .Should()
                .NotBeNull();

            int expectedErrorCount = expectedErrorMessages.Length;

            if (expectedErrorCount < 1)
            {
                errors
                    .Should()
                    .BeEmpty();
            }
            else
            {
                errors
                    .Should()
                    .HaveCount(expectedErrorCount);
                errors
                    .Should()
                    .BeEquivalentTo(expectedErrorMessages);
            }
        }

        private static TestModel GetTestModel(int property1, string? property2, bool property3, string property4)
        {
            return new()
            {
                Property1 = property1,
                Property2 = property2,
                Property3 = property3,
                Property4 = property4
            };
        }
    }

    public class TestModel : NotifyDataErrorInfoBase, IValidatableObject
    {
        public int Property1 { get; set; }

        public string? Property2 { get; set; }

        public bool Property3 { get; set; }

        public string Property4 { get; set; } = string.Empty;

        public void Clear() => ClearErrors();

        public void Validate()
        {
            List<ValidationResult> results = [];
            ValidationContext context = new(this);
            Validator.TryValidateObject(this, context, results);

            if (results.Count > 0)
            {
                List<string> propertyNames = results.SelectMany(r => r.MemberNames).Distinct().ToList();

                foreach (string propertyName in propertyNames)
                {
                    Errors[propertyName] = results
                        .Where(r => r.MemberNames.Contains(propertyName))
                        .Select(r => r.ErrorMessage)
                        .Distinct()
                        .ToList()!;
                    OnErrorsChanged(propertyName);
                }
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Property1 is < 1 or > 10)
            {
                yield return new ValidationResult(Messages.Message1, new[] { nameof(Property1) });
            }

            if (string.IsNullOrWhiteSpace(Property4))
            {
                yield return new ValidationResult(Messages.Message4, new[] { nameof(Property4) });
            }

            if (Property3)
            {
                if (Property1 is not 10)
                {
                    yield return new ValidationResult(Messages.Message1x3, new[] { nameof(Property1), nameof(Property3) });
                }

                if (Property2 is null)
                {
                    yield return new ValidationResult(Messages.Message2x3, new[] { nameof(Property2), nameof(Property3) });
                }
            }
        }
    }
}