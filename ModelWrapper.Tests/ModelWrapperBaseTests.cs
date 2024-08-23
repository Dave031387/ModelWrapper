namespace ModelWrapper
{
    using System;
    using TestModels;
    using TestWrappers;

    public class ModelWrapperBaseTests
    {
        private const int TM1Property1InvalidValue = TestModel1.TM1Property1MinValue - 2;
        private const int TM1Property1ModifiedValue = TestModel1.TM1Property1MaxValue;
        private const int TM1Property1ValidValue = TestModel1.TM1Property1MinValue;
        private const int TM2Property1InvalidValue = TestModel2.TM2Property1MinValue - 1;
        private const int TM2Property1ModifiedValue = TestModel2.TM2Property1MinValue + 100;
        private const int TM2Property1ValidValue = TestModel2.TM2Property1MinValue;
        private const int TM5Property1InvalidValue = TestModel5.TM5Property1MinValue - 1;
        private const int TM5Property1ModifiedValue = TestModel5.TM5Property1MaxValue;
        private const int TM5Property1ValidValue = TestModel5.TM5Property1MinValue;
        private const string TM5Property3InvalidValue = "";
        private const string TM5Property3ModifiedValue = "Dave";
        private const string TM5Property3ValidValue = "Bob";
        private bool _collectionIsChanged;
        private bool _collectionIsValid;
        private int _collectionOriginalValue;
        private int _collectionPropertyValue;
        private bool _complexHasErrors;
        private bool _complexIsChanged;
        private bool _complexIsValid;
        private int _complexOriginalValue;
        private int _complexPropertyValue;
        private bool _hasErrors;
        private bool _isChanged;
        private bool _isValid;
        private int _simpleOriginalValue;
        private int _simplePropertyValue;

        [Fact]
        public void AcceptChanges_CollectionPropertiesAreModifiedAndContainInvalidValues_DoesNothing()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();
            int newValue1 = TM5Property1ModifiedValue;
            int newValue2 = TM1Property1InvalidValue;
            SetPropertyValues(wrapper, newValue1, 0, newValue2);
            SaveOriginalState(wrapper);

            // Act
            wrapper.AcceptChanges();

            // Assert
            AssertOriginalState(wrapper);
        }

        [Fact]
        public void AcceptChanges_ComplexPropertiesAreModifiedAndContainInvalidValues_DoesNothing()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();
            int newValue1 = TM5Property1ModifiedValue;
            int newValue2 = TM2Property1InvalidValue;
            SetPropertyValues(wrapper, newValue1, newValue2, 0);
            SaveOriginalState(wrapper);

            // Act
            wrapper.AcceptChanges();

            // Assert
            AssertOriginalState(wrapper);
        }

        [Fact]
        public void AcceptChanges_PropertiesAreModifiedAndContainValidValues_AcceptsChanges()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();
            SaveOriginalState(wrapper);
            int newValue1 = TM5Property1ModifiedValue;
            int newValue2 = TM2Property1ModifiedValue;
            int newValue3 = TM1Property1ModifiedValue;
            SetPropertyValues(wrapper, newValue1, newValue2, newValue3);

            // Act
            wrapper.AcceptChanges();

            // Assert
            AssertOriginalState(wrapper, false);
            AssertOriginalValues(wrapper, newValue1, newValue2, newValue3);
            AssertPropertyValues(wrapper, newValue1, newValue2, newValue3);
        }

        [Fact]
        public void AcceptChanges_PropertiesAreNotModifiedAndContainInvalidValues_DoesNothing()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5(false);
            SaveOriginalState(wrapper);

            // Act
            wrapper.AcceptChanges();

            // Assert
            AssertOriginalState(wrapper);
        }

        [Fact]
        public void AcceptChanges_PropertiesAreNotModifiedAndContainValidValues_DoesNothing()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();
            SaveOriginalState(wrapper);

            // Act
            wrapper.AcceptChanges();

            // Assert
            AssertOriginalState(wrapper);
        }

        [Fact]
        public void AcceptChanges_SimplePropertiesAreModifiedAndContainInvalidValues_DoesNothing()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();
            int newValue1 = TM5Property1InvalidValue;
            int newValue2 = TM2Property1ModifiedValue;
            SetPropertyValues(wrapper, newValue1, newValue2, 0);
            SaveOriginalState(wrapper);

            // Act
            wrapper.AcceptChanges();

            // Assert
            AssertOriginalState(wrapper);
        }

        [Fact]
        public void GetIsChanged_PropertyIsChanged_ReturnsTrue()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();
            wrapper.TM5Property1 = TM5Property1ModifiedValue;

            // Act
            bool isChanged = wrapper.GetIsChanged(nameof(wrapper.TM5Property1));

            // Assert
            isChanged
                .Should()
                .BeTrue();
        }

        [Fact]
        public void GetIsChanged_PropertyIsChangedAndThenChangedBack_ReturnsFalse()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();
            wrapper.TM5Property1 = TM5Property1ModifiedValue;
            wrapper.TM5Property1 = TM5Property1ValidValue;

            // Act
            bool isChanged = wrapper.GetIsChanged(nameof(wrapper.TM5Property1));

            // Assert
            isChanged
                .Should()
                .BeFalse();
        }

        [Fact]
        public void GetIsChanged_PropertyIsNotChanged_ReturnsFalse()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Act
            bool isChanged = wrapper.GetIsChanged(nameof(wrapper.TM5Property1));

            // Assert
            isChanged
                .Should()
                .BeFalse();
        }

        [Theory()]
        [InlineData("")]
        [InlineData("Invalid")]
        public void GetIsChanged_PropertyNameIsEmptyOrInvalid_ReturnsFalse(string propertyName)
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Act
            bool isChanged = wrapper.GetIsChanged(propertyName!);

            // Assert
            isChanged
                .Should()
                .BeFalse();
        }

        [Fact]
        public void GetIsChanged_PropertyNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();
            Action action = () => wrapper.GetIsChanged(null!);
            string expectedMessage = "Value cannot be null. (Parameter 'key')";

            // Act/Assert
            action
                .Should()
                .Throw<ArgumentNullException>()
                .WithMessage(expectedMessage);
        }

        [Theory()]
        [InlineData("")]
        [InlineData("Invalid")]
        public void GetOriginalValue_ParameterNameIsEmptyOrInvalid_ThrowsArgumentException(string propertyName)
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();
            Action action = () => wrapper.GetOriginalValue<string>(propertyName);
            string expectedMessage = $"The property \"{propertyName}\" is not a member of \"TestModel5\".";

            // Act/Assert
            action
                .Should()
                .Throw<ArgumentException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public void GetOriginalValue_ParameterNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();
            Action action = () => wrapper.GetOriginalValue<string>(null!);
            string expectedMessage = "Value cannot be null. (Parameter 'key')";

            // Act/Assert
            action
                .Should()
                .Throw<ArgumentNullException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public void GetOriginalValue_PropertyIsChanged_ReturnsOriginalValue()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();
            wrapper.TM5Property1 = TM5Property1ModifiedValue;

            // Act
            int originalValue = wrapper.GetOriginalValue<int>(nameof(wrapper.TM5Property1));

            // Assert
            originalValue
                .Should()
                .Be(TM5Property1ValidValue);
        }

        [Fact]
        public void GetOriginalValue_PropertyIsChangedAndThenChangedBack_ReturnsOriginalValue()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();
            wrapper.TM5Property1 = TM5Property1ModifiedValue;
            wrapper.TM5Property1 = TM5Property1ValidValue;

            // Act
            int originalValue = wrapper.GetOriginalValue<int>(nameof(wrapper.TM5Property1));

            // Assert
            originalValue
                .Should()
                .Be(TM5Property1ValidValue);
        }

        [Fact]
        public void GetOriginalValue_PropertyIsChangedTwice_ReturnsOriginalValue()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();
            wrapper.TM5Property1 = TM5Property1ModifiedValue;
            wrapper.TM5Property1 = TM5Property1ValidValue + 2;

            // Act
            int originalValue = wrapper.GetOriginalValue<int>(nameof(wrapper.TM5Property1));

            // Assert
            originalValue
                .Should()
                .Be(TM5Property1ValidValue);
        }

        [Fact]
        public void GetOriginalValue_PropertyIsNotChanged_ReturnsOriginalValue()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Act
            int originalValue = wrapper.GetOriginalValue<int>(nameof(wrapper.TM5Property1));

            // Assert
            originalValue
                .Should()
                .Be(TM5Property1ValidValue);
        }

        [Fact]
        public void GetValue_ParameterNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();
            Action action = () => wrapper.GetValue<string>(null);
            string expectedMessage = "Value cannot be null. (Parameter 'propertyName')";

            // Act/Assert
            action
                .Should()
                .Throw<ArgumentNullException>()
                .WithMessage(expectedMessage);
        }

        [Theory()]
        [InlineData("")]
        [InlineData("Invalid")]
        public void GetValue_PropertyNameIsEmptyOrInvalid_ThrowsArgumentException(string propertyName)
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();
            Action action = () => wrapper.GetValue<string>(propertyName);
            string expectedMessage = $"The property \"{propertyName}\" is not a member of \"TestModel5\".";

            // Act/Assert
            action
                .Should()
                .Throw<ArgumentException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public void GetValue_ValidPropertyName_ReturnsPropertyValue()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Act
            string propertyValue = wrapper.GetValue<string>(nameof(wrapper.TM5Property3))!;

            // Assert
            propertyValue
                .Should()
                .Be(TM5Property3ValidValue);
        }

        [Fact]
        public void HasErrors_ConstructWrapperUsingInvalidModel_ReturnsTrue()
        {
            // Arrange/Act
            TestModel5Wrapper wrapper = GetTestWrapper5(false);

            // Assert
            wrapper.HasErrors
                .Should()
                .BeTrue();
        }

        [Fact]
        public void HasErrors_ConstructWrapperUsingValidModel_ReturnsFalse()
        {
            // Arrange/Act
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Assert
            wrapper.HasErrors
                .Should()
                .BeFalse();
        }

        [Fact]
        public void HasErrors_SetCollectionPropertyToInvalidValue_ReturnsFalse()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Act
            wrapper.TM5Property2[0].TM1Property1 = TM1Property1InvalidValue;

            // Assert
            wrapper.TM5Property2[0].HasErrors
                .Should()
                .BeTrue();
            wrapper.HasErrors
                .Should()
                .BeFalse();
        }

        [Fact]
        public void HasErrors_SetComplexPropertyToInvalidValue_ReturnsFalse()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Act
            wrapper.TM5Property4.TM2Property1 = TM2Property1InvalidValue;

            // Assert
            wrapper.TM5Property4.HasErrors
                .Should()
                .BeTrue();
            wrapper.HasErrors
                .Should()
                .BeFalse();
        }

        [Fact]
        public void HasErrors_SetSimplePropertyToInvalidValue_ReturnsTrue()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Act
            wrapper.TM5Property1 = TM5Property1InvalidValue;

            // Assert
            wrapper.HasErrors
                .Should()
                .BeTrue();
        }

        [Fact]
        public void HasErrors_SetSimplePropertyToInvalidValueAndThenValidValue_ReturnsFalse()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Act
            wrapper.TM5Property1 = TM5Property1InvalidValue;
            wrapper.TM5Property1 = TM5Property1ModifiedValue;

            // Assert
            wrapper.HasErrors
                .Should()
                .BeFalse();
        }

        [Fact]
        public void IsChanged_CollectionItemPropertyIsChanged_ReturnsTrue()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Act
            wrapper.TM5Property2[0].TM1Property1 = TM1Property1ModifiedValue;

            // Assert
            wrapper.IsChanged
                .Should()
                .BeTrue();
        }

        [Fact]
        public void IsChanged_CollectionItemPropertyIsChangedAndThenChangedBack_ReturnsFalse()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Act
            wrapper.TM5Property2[0].TM1Property1 = TM1Property1ModifiedValue;
            wrapper.TM5Property2[0].TM1Property1 = TM1Property1ValidValue;

            // Assert
            wrapper.IsChanged
                .Should()
                .BeFalse();
        }

        [Fact]
        public void IsChanged_ComplexPropertyIsChanged_ReturnsTrue()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Act
            wrapper.TM5Property4.TM2Property1 = TM2Property1ModifiedValue;

            // Assert
            wrapper.IsChanged
                .Should()
                .BeTrue();
        }

        [Fact]
        public void IsChanged_ComplexPropertyIsChangedAndThenChangedBack_ReturnsFalse()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Act
            wrapper.TM5Property4.TM2Property1 = TM2Property1ModifiedValue;
            wrapper.TM5Property4.TM2Property1 = TM2Property1ValidValue;

            // Assert
            wrapper.IsChanged
                .Should()
                .BeFalse();
        }

        [Fact]
        public void IsChanged_NewPropertyValueSameAsOldValue_ReturnsFalse()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Act
            wrapper.TM5Property3 = TM5Property3ValidValue;

            // Assert
            wrapper.IsChanged
                .Should()
                .BeFalse();
        }

        [Fact]
        public void IsChanged_NoChangesMade_ReturnsFalse()
        {
            // Arrange/Act
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Assert
            wrapper.IsChanged
                .Should()
                .BeFalse();
        }

        [Fact]
        public void IsChanged_SimplePropertyIsChanged_ReturnsTrue()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Act
            wrapper.TM5Property3 = TM5Property3ModifiedValue;

            // Assert
            wrapper.IsChanged
                .Should()
                .BeTrue();
        }

        [Fact]
        public void IsChanged_SimplePropertyIsChangedAndThenChangedBack_ReturnsFalse()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Act
            wrapper.TM5Property3 = TM5Property3ModifiedValue;
            wrapper.TM5Property3 = TM5Property3ValidValue;

            // Assert
            wrapper.IsChanged
                .Should()
                .BeFalse();
        }

        [Fact]
        public void IsChanged_TwoPropertiesChangedAndThenOneIsChangedBack_ReturnsTrue()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Act
            wrapper.TM5Property3 = TM5Property3ModifiedValue;
            wrapper.TM5Property4.TM2Property1 = TM2Property1ModifiedValue;
            wrapper.TM5Property3 = TM5Property3ValidValue;

            // Assert
            wrapper.IsChanged
                .Should()
                .BeTrue();
        }

        [Fact]
        public void IsValid_ConstructWrapperUsingInvalidModel_ReturnsFalse()
        {
            // Arrange/Act
            TestModel5Wrapper wrapper = GetTestWrapper5(false);

            // Assert
            wrapper.TM5Property2[0].IsValid
                .Should()
                .BeFalse();
            wrapper.TM5Property4.IsValid
                .Should()
                .BeFalse();
            wrapper.IsValid
                .Should()
                .BeFalse();
        }

        [Fact]
        public void IsValid_ConstructWrapperUsingValidModel_ReturnsTrue()
        {
            // Arrange/Act
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Assert
            wrapper.TM5Property2[0].IsValid
                .Should()
                .BeTrue();
            wrapper.TM5Property4.IsValid
                .Should()
                .BeTrue();
            wrapper.IsValid
                .Should()
                .BeTrue();
        }

        [Fact]
        public void IsValid_SetCollectionPropertyToInvalidValue_ReturnsFalse()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Act
            wrapper.TM5Property2[0].TM1Property1 = TM1Property1InvalidValue;

            // Assert
            wrapper.TM5Property2[0].IsValid
                .Should()
                .BeFalse();
            wrapper.IsValid
                .Should()
                .BeFalse();
        }

        [Fact]
        public void IsValid_SetCollectionPropertyToInvalidValueAndBackToValidValue_ReturnsTrue()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Act
            wrapper.TM5Property2[0].TM1Property1 = TM1Property1InvalidValue;
            wrapper.TM5Property2[0].TM1Property1 = TM1Property1ValidValue;

            // Assert
            wrapper.TM5Property2[0].IsValid
                .Should()
                .BeTrue();
            wrapper.IsValid
                .Should()
                .BeTrue();
        }

        [Fact]
        public void IsValid_SetComplexPropertyToInvalidValue_ReturnsFalse()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Act
            wrapper.TM5Property4.TM2Property1 = TM2Property1InvalidValue;

            // Assert
            wrapper.TM5Property4.IsValid
                .Should()
                .BeFalse();
            wrapper.IsValid
                .Should()
                .BeFalse();
        }

        [Fact]
        public void IsValid_SetComplexPropertyToInvalidValueAndBackToValidValue_ReturnsTrue()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Act
            wrapper.TM5Property4.TM2Property1 = TM2Property1InvalidValue;
            wrapper.TM5Property4.TM2Property1 = TM2Property1ModifiedValue;

            // Assert
            wrapper.TM5Property4.IsValid
                .Should()
                .BeTrue();
            wrapper.IsValid
                .Should()
                .BeTrue();
        }

        [Fact]
        public void IsValid_SetSimplePropertyToInvalidValue_ReturnsFalse()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Act
            wrapper.TM5Property3 = TM5Property3InvalidValue;

            // Assert
            wrapper.IsValid
                .Should()
                .BeFalse();
        }

        [Fact]
        public void IsValid_SetSimplePropertyToInvalidValueAndBackToValidValue_ReturnsTrue()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Act
            wrapper.TM5Property3 = TM5Property3InvalidValue;
            wrapper.TM5Property3 = TM5Property3ModifiedValue;

            // Assert
            wrapper.IsValid
                .Should()
                .BeTrue();
        }

        [Fact]
        public void Model_ConstructUsingInvalidModel_ReturnsModelPassedIntoConstructor()
        {
            // Arrange
            TestModel5 model = GetTestModel5(false);

            // Act
            TestModel5Wrapper wrapper = new(model);

            // Assert
            wrapper.Model
                .Should()
                .BeSameAs(model);
        }

        [Fact]
        public void Model_ConstructUsingValidModel_ReturnsModelPassedIntoConstructor()
        {
            // Arrange
            TestModel5 model = GetTestModel5();

            // Act
            TestModel5Wrapper wrapper = new(model);

            // Assert
            wrapper.Model
                .Should()
                .BeSameAs(model);
        }

        [Fact]
        public void Model_PropertiesAreModified_ReturnsModelWithModifiedProperties()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();
            wrapper.TM5Property1 = TM5Property1ModifiedValue;
            wrapper.TM5Property3 = TM5Property3ModifiedValue;

            // Act
            TestModel5 model = wrapper.Model;

            // Assert
            model.TM5Property1
                .Should()
                .Be(TM5Property1ModifiedValue);
            model.TM5Property3
                .Should()
                .Be(TM5Property3ModifiedValue);
        }

        [Fact]
        public void ModelWrapperBase_InvalidModelPassedToConstructor_InitializesStateCorrectly()
        {
            // Arrange
            TestModel5 model = GetTestModel5(false);

            // Act
            TestModel5Wrapper wrapper = new(model);

            // Assert
            AssertState(
                wrapper,
                true,
                false,
                false,
                false,
                false,
                true,
                false,
                false);
        }

        [Fact]
        public void ModelWrapperBase_NullModelPassedToConstructor_ThrowsArgumentNullException()
        {
            // Arrange
            string testFailedMessage = "The test should have thrown an ArgumentNullException, but didn't throw anything.";
            string expectedMessage = "Value cannot be null. (Parameter 'model')";

            // Act/Assert
            try
            {
                TestModel5Wrapper wrapper = new(null!);
                Assert.Fail(testFailedMessage);
            }
            catch (Exception exception)
            {
                exception
                    .Should()
                    .BeOfType<ArgumentNullException>();

                string actualMessage = exception.Message;

                actualMessage
                    .Should()
                    .Be(expectedMessage);
            }
        }

        [Fact]
        public void ModelWrapperBase_ValidModelPassedToConstructor_InitializesStateCorrectly()
        {
            // Arrange
            TestModel5 model = GetTestModel5();

            // Act
            TestModel5Wrapper wrapper = new(model);

            // Assert
            AssertState(
                wrapper,
                false,
                false,
                true,
                false,
                true,
                false,
                false,
                true);
        }

        [Fact]
        public void RegisterCollection_NullModelCollection_ThrowsArgumentNullException()
        {
            // Arrange
            TestModel5WrapperX wrapper = GetTestWrapper5X();
            wrapper.InitializeCollectionProperty();
            ChangeTrackingCollection<TestModel1Wrapper> collection = wrapper.TM5Property2;
            Action action = () =>
            {
                wrapper.RegisterCollection(collection, (List<TestModel1>)null!);
            };
            string expectedMessage = "Value cannot be null. (Parameter 'modelCollection')";

            // Act/Assert
            action
                .Should()
                .Throw<ArgumentNullException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public void RegisterCollection_NullWrapperCollection_ThrowsArgumentNullException()
        {
            // Arrange
            TestModel5WrapperX wrapper = GetTestWrapper5X();
            Action action = () =>
            {
                wrapper.RegisterCollection(
                    (ChangeTrackingCollection<TestModel1Wrapper>)null!, wrapper.Model.TM5Property2);
            };
            string expectedMessage = "Value cannot be null. (Parameter 'wrapperCollection')";

            // Act/Assert
            action
                .Should()
                .Throw<ArgumentNullException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public void RegisterCollection_ValidCollection_CollectionIsAddedToTrackingObjectsList()
        {
            // Arrange
            TestModel5WrapperX wrapper = GetTestWrapper5X();
            wrapper.InitializeCollectionProperty();
            ChangeTrackingCollection<TestModel1Wrapper> collection = wrapper.TM5Property2;

            // Act
            wrapper.RegisterCollection(collection, wrapper.Model.TM5Property2);

            // Assert
            wrapper._trackingObjects
                .Should()
                .ContainSingle();
            wrapper._trackingObjects
                .Should()
                .HaveElementAt(0, collection);
        }

        [Fact]
        public void RegisterCollection_WrapperAndModelCollectionsHaveDifferentCounts_ThrowsArgumentException()
        {
            // Arrange
            TestModel5WrapperX wrapper = GetTestWrapper5X();
            wrapper.InitializeCollectionProperty();
            ChangeTrackingCollection<TestModel1Wrapper> collection = wrapper.TM5Property2;
            wrapper.Model.TM5Property2.Clear();
            Action action = () =>
            {
                wrapper.RegisterCollection(collection, wrapper.Model.TM5Property2);
            };
            string expectedMessage = "RegisterCollection: The model collection and wrapper collection must contain the same number of elements.";

            // Act/Assert
            action
                .Should()
                .Throw<ArgumentException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public void RegisterComplex_ComplexPropertyIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            TestModel5WrapperX wrapper = GetTestWrapper5X();
            Action action = () =>
            {
                wrapper.RegisterComplex((ModelWrapperBase<TestModel2>)null!);
            };
            string expectedMessage = "Value cannot be null. (Parameter 'wrapper')";

            // Act/Assert
            action
                .Should()
                .Throw<ArgumentNullException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public void RegisterComplex_ValidPropertyValue_AddsObjectToTrackingList()
        {
            // Arrange
            TestModel5WrapperX wrapper = GetTestWrapper5X();
            wrapper.InitializeComplexProperty();
            TestModel2Wrapper complex = wrapper.TM5Property4;

            // Act
            wrapper.RegisterComplex(complex);

            // Assert
            wrapper._trackingObjects
                .Should()
                .ContainSingle();
            wrapper._trackingObjects
                .Should()
                .HaveElementAt(0, complex);
        }

        [Fact]
        public void RegisterPropertyAction_ActionIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            TestModel5WrapperX wrapper = GetTestWrapper5X();
            Action action = () => { wrapper.RegisterPropertyAction(nameof(wrapper.TM5Property1), null!); };
            string expectedMessage = "Value cannot be null. (Parameter 'propertyAction')";

            // Act/Assert
            action
                .Should()
                .Throw<ArgumentNullException>()
                .WithMessage(expectedMessage);
        }

        [Theory()]
        [InlineData("")]
        [InlineData("Invalid")]
        public void RegisterPropertyAction_PropertyNameIsEmptyOrInvalid_ThrowsArgumentException(string propertyName)
        {
            // Arrange
            TestModel5WrapperX wrapper = GetTestWrapper5X();
            Action action = () => { wrapper.RegisterPropertyAction(propertyName, wrapper.TM5Property1Action); };
            string expectedMessage = $"The property \"{propertyName}\" is not a member of \"TestModel5\".";

            // Act/Assert
            action
                .Should()
                .Throw<ArgumentException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public void RegisterPropertyAction_PropertyNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            TestModel5WrapperX wrapper = GetTestWrapper5X();
            Action action = () => { wrapper.RegisterPropertyAction(null!, wrapper.TM5Property1Action); };
            string expectedMessage = "Value cannot be null. (Parameter 'propertyName')";

            // Act/Assert
            action
                .Should()
                .Throw<ArgumentNullException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public void RegisterPropertyAction_ValidPropertyNameAndAction_RegistersThePropertyAction()
        {
            // Arrange
            TestModel5WrapperX wrapper = GetTestWrapper5X();
            string propertyName = nameof(wrapper.TM5Property1);
            Action action = wrapper.TM5Property1Action;
            KeyValuePair<string, Action> expectedEntry = new(propertyName, action);

            // Act
            wrapper.RegisterPropertyAction(propertyName, action);

            // Assert
            wrapper._propertyActions
                .Should()
                .HaveElementAt(0, expectedEntry);
        }

        [Fact]
        public void RejectChanges_CollectionPropertyIsChanged_RejectsChanges()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();
            SaveOriginalState(wrapper);
            wrapper.TM5Property2[0].TM1Property1 = TM1Property1ValidValue + 1;

            // Act
            wrapper.RejectChanges();

            // Assert
            AssertOriginalState(wrapper);
        }

        [Fact]
        public void RejectChanges_ComplexPropertyChanged_RejectsChanges()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();
            SaveOriginalState(wrapper);
            wrapper.TM5Property4.TM2Property1 = TM2Property1ModifiedValue;

            // Act
            wrapper.RejectChanges();

            // Assert
            AssertOriginalState(wrapper);
        }

        [Fact]
        public void RejectChanges_NoChangesMade_DoesNothing()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();
            SaveOriginalState(wrapper);

            // Act
            wrapper.RejectChanges();

            // Assert
            AssertOriginalState(wrapper);
        }

        [Fact]
        public void RejectChanges_SimplePropertyChanged_RejectsChanges()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();
            SaveOriginalState(wrapper);
            wrapper.TM5Property1 = TM5Property1ModifiedValue;

            // Act
            wrapper.RejectChanges();

            // Assert
            AssertOriginalState(wrapper);
        }

        [Fact]
        public void SetValue_NewValueDiffersFromOldValue_SetsPropertyToNewValue()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();

            // Act
            wrapper.SetValue(TM5Property1ModifiedValue, nameof(wrapper.TM5Property1));

            // Assert
            wrapper.Model.TM5Property1
                .Should()
                .Be(TM5Property1ModifiedValue);
        }

        [Fact]
        public void SetValue_NewValueSameAsOldValue_DoesNothing()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();
            SaveOriginalState(wrapper);

            // Action
            wrapper.SetValue(TM5Property1ValidValue, nameof(wrapper.TM5Property1));

            // Assert
            AssertOriginalState(wrapper);
        }

        [Theory()]
        [InlineData("")]
        [InlineData("Invalid")]
        public void SetValue_ParameterNameIsEmptyOrInvalid_ThrowsArgumentException(string propertyName)
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();
            Action action = () => { wrapper.SetValue(1, propertyName); };
            string expectedMessage = $"The property \"{propertyName}\" is not a member of \"TestModel5\".";

            // Act/Assert
            action
                .Should()
                .Throw<ArgumentException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public void SetValue_ParameterNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            TestModel5Wrapper wrapper = GetTestWrapper5();
            Action action = () => { wrapper.SetValue(1, null); };
            string expectedMessage = "Value cannot be null. (Parameter 'propertyName')";

            // Act/Assert
            action
                .Should()
                .Throw<ArgumentNullException>()
                .WithMessage(expectedMessage);
        }

        [Fact]
        public void SetValue_PropertyHasAction_ActionIsPerformed()
        {
            // Arrange
            TestModel5WrapperX wrapper = GetTestWrapper5X();
            wrapper.InitializePropertyAction();

            // Act
            wrapper.TM5Property1 = TM5Property1ModifiedValue;

            // Assert
            wrapper.TM5Property3
                .Should()
                .Be(TestModel5.TM5Property3ActionValue);
        }

        private static void AssertOriginalValues(
            TestModel5Wrapper wrapper,
            int simplePropertyValue,
            int complexPropertyValue,
            int collectionPropertyValue)
        {
            wrapper.TM5Property1OriginalValue
                .Should()
                .Be(simplePropertyValue);
            wrapper.TM5Property4.TM2Property1OriginalValue
                .Should()
                .Be(complexPropertyValue);
            wrapper.TM5Property2[0].TM1Property1OriginalValue
                .Should()
                .Be(collectionPropertyValue);
        }

        private static void AssertPropertyValues(
            TestModel5Wrapper wrapper,
            int simplePropertyValue,
            int complexPropertyValue,
            int collectionPropertyValue)
        {
            wrapper.TM5Property1
                .Should()
                .Be(simplePropertyValue);
            wrapper.TM5Property4.TM2Property1
                .Should()
                .Be(complexPropertyValue);
            wrapper.TM5Property2[0].TM1Property1
                .Should()
                .Be(collectionPropertyValue);
        }

        private static void AssertState(
            TestModel5Wrapper wrapper,
            bool wrapperHasErrors,
            bool wrapperIsChanged,
            bool wrapperIsValid,
            bool collectionIsChanged,
            bool collectionIsValid,
            bool complexHasErrors,
            bool complexIsChanged,
            bool complexIsValid)
        {
            wrapper.HasErrors
                .Should()
                .Be(wrapperHasErrors);
            wrapper.IsChanged
                .Should()
                .Be(wrapperIsChanged);
            wrapper.IsValid
                .Should()
                .Be(wrapperIsValid);
            wrapper.TM5Property2.IsChanged
                .Should()
                .Be(collectionIsChanged);
            wrapper.TM5Property2.IsValid
                .Should()
                .Be(collectionIsValid);
            wrapper.TM5Property4.HasErrors
                .Should()
                .Be(complexHasErrors);
            wrapper.TM5Property4.IsChanged
                .Should()
                .Be(complexIsChanged);
            wrapper.TM5Property4.IsValid
                .Should()
                .Be(complexIsValid);
        }

        private static TestModel1 GetTestModel1(bool isValid = true)
        {
            int property1Value = isValid ? TM1Property1ValidValue : TM1Property1InvalidValue;

            return new()
            {
                TM1Property1 = property1Value,
                TM1Property2 = "XYZ",
                TM1Property3 = new(1955, 12, 22),
                TM1Property4 = true
            };
        }

        private static TestModel2 GetTestModel2(bool isValid = true)
        {
            int property1Value = isValid ? TM2Property1ValidValue : TM2Property1InvalidValue;

            return new()
            {
                TM2Property1 = property1Value,
                TM2Property2 = Guid.NewGuid(),
                TM2Property3 = 3.14159F
            };
        }

        private static TestModel3 GetTestModel3()
        {
            return new()
            {
                TM3Property1 = "ABCD",
                TM3Property2 = 1234.5678M
            };
        }

        private static TestModel4 GetTestModel4()
        {
            return new()
            {
                TM4Property1 = 1234567890L,
                TM4Property2 = 'b',
                TM4Property3 = 4U
            };
        }

        private static TestModel5 GetTestModel5(bool isValid = true)
        {
            TestModel1 testModel1 = GetTestModel1(isValid);
            TestModel2 testModel2 = GetTestModel2(isValid);
            TestModel3 testModel3 = GetTestModel3();
            TestModel4 testModel4 = GetTestModel4();

            return GetTestModel5(testModel1, testModel2, testModel3, testModel4, isValid);
        }

        private static TestModel5 GetTestModel5(
            TestModel1 testModel1,
            TestModel2 testModel2,
            TestModel3 testModel3,
            TestModel4 testModel4,
            bool isValid)
        {
            int property1Value = isValid ? TM5Property1ValidValue : TM5Property1InvalidValue;
            string property3Value = isValid ? TM5Property3ValidValue : TM5Property3InvalidValue;

            return new()
            {
                TM5Property1 = property1Value,
                TM5Property2 = [testModel1],
                TM5Property3 = property3Value,
                TM5Property4 = testModel2,
                TM5Property5 = [testModel3],
                TM5Property6 = testModel4
            };
        }

        private static TestModel5Wrapper GetTestWrapper5(bool isValid = true)
        {
            TestModel5 testModel = GetTestModel5(isValid);
            return new(testModel);
        }

        private static TestModel5WrapperX GetTestWrapper5X()
        {
            TestModel5 model = GetTestModel5();
            return new(model);
        }

        private static void SetPropertyValues(
            TestModel5Wrapper wrapper,
            int simplePropertyValue,
            int complexPropertyValue,
            int collectionPropertyValue)
        {
            if (simplePropertyValue != 0)
            {
                wrapper.TM5Property1 = simplePropertyValue;
            }

            if (complexPropertyValue != 0)
            {
                wrapper.TM5Property4.TM2Property1 = complexPropertyValue;
            }

            if (collectionPropertyValue != 0)
            {
                wrapper.TM5Property2[0].TM1Property1 = collectionPropertyValue;
            }
        }

        private void AssertOriginalState(TestModel5Wrapper wrapper, bool assertProperties = true)
        {
            AssertState(wrapper,
                _hasErrors,
                _isChanged,
                _isValid,
                _collectionIsChanged,
                _collectionIsValid,
                _complexHasErrors,
                _complexIsChanged,
                _complexIsValid);

            if (assertProperties)
            {
                AssertOriginalValues(wrapper, _simpleOriginalValue, _complexOriginalValue, _collectionOriginalValue);
                AssertPropertyValues(wrapper, _simplePropertyValue, _complexPropertyValue, _collectionPropertyValue);
            }
        }

        private void SaveOriginalState(TestModel5Wrapper wrapper)
        {
            _hasErrors = wrapper.HasErrors;
            _isChanged = wrapper.IsChanged;
            _isValid = wrapper.IsValid;
            _collectionIsChanged = wrapper.TM5Property2.IsChanged;
            _collectionIsValid = wrapper.TM5Property2.IsValid;
            _complexHasErrors = wrapper.TM5Property4.HasErrors;
            _complexIsChanged = wrapper.TM5Property4.IsChanged;
            _complexIsValid = wrapper.TM5Property4.IsValid;
            _simpleOriginalValue = wrapper.TM5Property1OriginalValue;
            _simplePropertyValue = wrapper.Model.TM5Property1;
            _complexOriginalValue = wrapper.TM5Property4.TM2Property1OriginalValue;
            _complexPropertyValue = wrapper.Model.TM5Property4.TM2Property1;
            _collectionOriginalValue = wrapper.TM5Property2[0].TM1Property1OriginalValue;
            _collectionPropertyValue = wrapper.Model.TM5Property2[0].TM1Property1;
        }
    }
}