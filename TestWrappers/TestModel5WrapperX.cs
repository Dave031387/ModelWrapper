namespace TestWrappers
{
    using ModelWrapper;
    using System.Collections.Generic;
    using TestModels;

    public class TestModel5WrapperX : ModelWrapperBase<TestModel5>
    {
#pragma warning disable CS8618 // The InitializeCollectionProperties and InitializeComplexProperties methods guarantee that the properties aren't null.
        public TestModel5WrapperX(TestModel5 model) : base(model)
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

        public void InitializeCollectionProperty()
            => TM5Property2 = new(Model.TM5Property2.Select(e => new TestModel1Wrapper(e)));

        public void InitializeComplexProperty()
            => TM5Property4 = new(Model.TM5Property4);

        public void InitializePropertyAction()
            => RegisterPropertyAction(nameof(TM5Property1), TM5Property1Action);

        public new void RegisterCollection<TWrapper, TModel>(ChangeTrackingCollection<TWrapper> wrapperCollection, IList<TModel> modelCollection)
            where TWrapper : ModelWrapperBase<TModel>
            where TModel : class
            => base.RegisterCollection(wrapperCollection, modelCollection);

        public new void RegisterComplex<TModel>(ModelWrapperBase<TModel> wrapper)
            where TModel : class
            => base.RegisterComplex(wrapper);

        public new void RegisterPropertyAction(string propertyName, Action propertyAction)
            => base.RegisterPropertyAction(propertyName, propertyAction);

        public void TM5Property1Action()
        {
            TM5Property3 = TestModel5.TM5Property3ActionValue;
        }
    }
}