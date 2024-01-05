namespace TestWrappers
{
    using ModelWrapper;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;
    using TestModels;

    public class TestModel5Wrapper : ModelWrapperBase<TestModel5>
    {
#pragma warning disable CS8618 // The InitializeCollectionProperties and InitializeComplexProperties methods guarantee that the properties aren't null.
        public TestModel5Wrapper(TestModel5 model) : base(model)
#pragma warning restore CS8618 // The InitializeCollectionProperties and InitializeComplexProperties methods guarantee that the properties aren't null.
        {
        }

        public int TM5Property1
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public bool TM5Property1IsChanged => GetIsChanged(nameof(TM5Property1));

        public int TM5Property1OriginalValue => GetOriginalValue<int>(nameof(TM5Property1));

        public ChangeTrackingCollection<TestModel1Wrapper> TM5Property2 { get; private set; }

        public string TM5Property3
        {
            get => GetValue<string>()!;
            set => SetValue(value);
        }

        public bool TM5Property3IsChanged => GetIsChanged(nameof(TM5Property3));

        public string TM5Property3OriginalValue => GetOriginalValue<string>(nameof(TM5Property3))!;

        public TestModel2Wrapper TM5Property4 { get; private set; }

        public ChangeTrackingCollection<TestModel3Wrapper> TM5Property5 { get; private set; }

        public TestModel4Wrapper TM5Property6 { get; private set; }

        public new bool GetIsChanged(string propertyName) => base.GetIsChanged(propertyName);

        public new TValue? GetOriginalValue<TValue>(string propertyName) => base.GetOriginalValue<TValue?>(propertyName);

        public new TValue? GetValue<TValue>([CallerMemberName] string? propertyName = null)
            => base.GetValue<TValue>(propertyName);

        protected override void InitializeCollectionProperties()
        {
            ArgumentNullException.ThrowIfNull(Model.TM5Property2);

            TM5Property2 = new ChangeTrackingCollection<TestModel1Wrapper>(Model.TM5Property2.Select(e => new TestModel1Wrapper(e)));
            RegisterCollection(TM5Property2, Model.TM5Property2);

            ArgumentNullException.ThrowIfNull(Model.TM5Property5);

            TM5Property5 = new ChangeTrackingCollection<TestModel3Wrapper>(Model.TM5Property5.Select(e => new TestModel3Wrapper(e)));
            RegisterCollection(TM5Property5, Model.TM5Property5);
        }

        protected override void InitializeComplexProperties()
        {
            ArgumentNullException.ThrowIfNull(Model.TM5Property4);

            TM5Property4 = new TestModel2Wrapper(Model.TM5Property4);
            RegisterComplex(TM5Property4);

            ArgumentNullException.ThrowIfNull(Model.TM5Property6);

            TM5Property6 = new TestModel4Wrapper(Model.TM5Property6);
            RegisterComplex(TM5Property6);
        }

        public new void SetValue<TValue>(TValue? newValue, [CallerMemberName] string? propertyName = null)
            => base.SetValue(newValue, propertyName);

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TM5Property1 is < TestModel5.TM5Property1MinValue or > TestModel5.TM5Property1MaxValue)
            {
                string propertyName = nameof(TM5Property1);
                yield return new ValidationResult($"{propertyName} must be between 100 and 200.",
                    new[] { propertyName });
            }

            if (string.IsNullOrWhiteSpace(TM5Property3))
            {
                string propertyName = nameof(TM5Property3);
                yield return new ValidationResult($"{propertyName} can't be null or whitespace.",
                    new[] { propertyName });
            }
            else
            {
                if (TM5Property3 == "X" && TM5Property2.Count == 0)
                {
                    string propertyName1 = nameof(TM5Property2);
                    string propertyName2 = nameof(TM5Property3);
                    yield return new ValidationResult($"{propertyName1} must not be empty when {propertyName2} is \"X\".",
                        new[] { propertyName1, propertyName2 });
                }
            }
        }
    }
}