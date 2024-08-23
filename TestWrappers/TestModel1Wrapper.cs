namespace TestWrappers
{
    using ModelWrapper;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using TestModels;

    public class TestModel1Wrapper(TestModel1 model) : ModelWrapperBase<TestModel1>(model)
    {
        public int TM1Property1
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public bool TM1Property1IsChanged => GetIsChanged(nameof(TM1Property1));

        public int TM1Property1OriginalValue => GetOriginalValue<int>(nameof(TM1Property1));

        public string TM1Property2
        {
            get => GetValue<string>()!;
            set => SetValue(value);
        }

        public bool TM1Property2IsChanged => GetIsChanged(nameof(TM1Property2));

        public string TM1Property2OriginalValue => GetOriginalValue<string>(nameof(TM1Property2))!;

        public DateTime? TM1Property3
        {
            get => GetValue<DateTime?>();
            set => SetValue(value);
        }

        public bool TM1Property3IsChanged => GetIsChanged(nameof(TM1Property3));

        public DateTime? TM1Property3OriginalValue => GetOriginalValue<DateTime?>(nameof(TM1Property3));

        public bool TM1Property4
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        public bool TM1Property4IsChanged => GetIsChanged(nameof(TM1Property4));

        public bool TM1Property4OriginalValue => GetOriginalValue<bool>(nameof(TM1Property4));

        public new bool GetIsChanged(string propertyName) => base.GetIsChanged(propertyName);

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TM1Property1 is < TestModel1.TM1Property1MinValue or > TestModel1.TM1Property1MaxValue)
            {
                string propertyName = nameof(TM1Property1);
                yield return new ValidationResult($"{propertyName} must be between 1 and 10.",
                    new[] { propertyName });
            }

            if (string.IsNullOrWhiteSpace(TM1Property2))
            {
                string propertyName = nameof(TM1Property2);
                yield return new ValidationResult($"{propertyName} can't be null or whitespace.",
                    new[] { propertyName });
            }

            if (TM1Property3 is null)
            {
                if (TM1Property4)
                {
                    string propertyName1 = nameof(TM1Property3);
                    string propertyName2 = nameof(TM1Property4);
                    yield return new ValidationResult($"{propertyName1} must not be null when {propertyName2} is true.",
                        new[] { propertyName1, propertyName2 });
                }
            }
            else
            {
                if (TM1Property3 < TestModel1.TM1Property3MinValue || TM1Property3 > TestModel1.TM1Property3MaxValue)
                {
                    string propertyName = nameof(TM1Property3);
                    string date1 = TestModel1.TM1Property3MinValue.ToString("d");
                    string date2 = TestModel1.TM1Property3MaxValue.ToString("d");
                    yield return new ValidationResult($"{propertyName} must be between {date1} and {date2}.",
                        new[] { propertyName });
                }
            }
        }
    }
}