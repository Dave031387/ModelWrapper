namespace TestWrappers
{
    using ModelWrapper;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using TestModels;

    public class TestModel3Wrapper(TestModel3 model) : ModelWrapperBase<TestModel3>(model)
    {
        public string? TM3Property1
        {
            get => GetValue<string?>();
            set => SetValue(value);
        }

        public bool TM3Property1IsChanged => GetIsChanged(nameof(TM3Property1));

        public string? TM3Property1OriginalValue => GetOriginalValue<string?>(nameof(TM3Property1));

        public decimal TM3Property2
        {
            get => GetValue<decimal>();
            set => SetValue(value);
        }

        public bool TM3Property2IsChanged => GetIsChanged(nameof(TM3Property2));

        public decimal TM3Property2OriginalValue => GetOriginalValue<decimal>(nameof(TM3Property2));

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TM3Property1 is null && TM3Property2 is > 1000M)
            {
                string propertyName1 = nameof(TM3Property1);
                string propertyName2 = nameof(TM3Property2);
                yield return new ValidationResult($"{propertyName1} must not be null when {propertyName2} is over 1,000.",
                    new[] { propertyName1, propertyName2 });
            }

            if (TM3Property2 is < 0 or > 10000M)
            {
                string propertyName = nameof(TM3Property2);
                yield return new ValidationResult($"{propertyName} must be between 0 and 10,000.",
                    new[] { propertyName });
            }
        }
    }
}