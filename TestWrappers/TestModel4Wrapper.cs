namespace TestWrappers
{
    using ModelWrapper;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using TestModels;

    public class TestModel4Wrapper(TestModel4 model) : ModelWrapperBase<TestModel4>(model)
    {
        public long TM4Property1
        {
            get => GetValue<long>();
            set => SetValue(value);
        }

        public bool TM4Property1IsChanged => GetIsChanged(nameof(TM4Property1));

        public long TM4Property1OriginalValue => GetOriginalValue<long>(nameof(TM4Property1));

        public char TM4Property2
        {
            get => GetValue<char>();
            set => SetValue(value);
        }

        public bool TM4Property2IsChanged => GetIsChanged(nameof(TM4Property2));

        public char TM4Property2OriginalValue => GetOriginalValue<char>(nameof(TM4Property2));

        public uint TM4Property3
        {
            get => GetValue<uint>();
            set => SetValue(value);
        }

        public bool TM4Property3IsChanged => GetIsChanged(nameof(TM4Property3));

        public uint TM4Property3OriginalValue => GetOriginalValue<uint>(nameof(TM4Property3));

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TM4Property2 is < 'a' or > 'e')
            {
                string propertyName = nameof(TM4Property2);
                yield return new ValidationResult($"{propertyName} must be between 'a' and 'e'.",
                    new[] { propertyName });
            }
        }
    }
}