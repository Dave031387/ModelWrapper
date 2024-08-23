namespace TestWrappers
{
    using ModelWrapper;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using TestModels;

    public class TestModel2Wrapper(TestModel2 model) : ModelWrapperBase<TestModel2>(model)
    {
        public int TM2Property1
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public bool TM2Property1IsChanged => GetIsChanged(nameof(TM2Property1));

        public int TM2Property1OriginalValue => GetOriginalValue<int>(nameof(TM2Property1));

        public Guid TM2Property2
        {
            get => GetValue<Guid>();
            set => SetValue(value);
        }

        public bool TM2Property2IsChanged => GetIsChanged(nameof(TM2Property2));

        public Guid TM2Property2OriginalValue => GetOriginalValue<Guid>(nameof(TM2Property2));

        public float? TM2Property3
        {
            get => GetValue<float?>();
            set => SetValue(value);
        }

        public bool TM2Property3IsChanged => GetIsChanged(nameof(TM2Property3));

        public float? TM2Property3OriginalValue => GetOriginalValue<float?>(nameof(TM2Property3));

        public new bool GetIsChanged(string propertyName) => base.GetIsChanged(propertyName);

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            string propertyName = nameof(TM2Property1);
            if (TM2Property1 < TestModel2.TM2Property1MinValue)
            {
                yield return new ValidationResult($"{propertyName} must not be less than zero.", new[] { propertyName });
            }
        }
    }
}