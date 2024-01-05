namespace ModelWrapper
{
    using TestModels;
    using TestWrappers;

    public enum CollectionType
    {
        ContainedItems,
        AddedItems,
        ModifiedItems,
        RemovedItems
    }

    public class ChangeTrackingCollectionTests
    {
        private readonly TestModel1 _invalid1 = new();
        private readonly TestModel1 _invalid2 = new();
        private readonly TestModel1 _valid1 = new();
        private readonly TestModel1 _valid2 = new();
        private readonly TestModel1 _valid3 = new();
        private readonly TestModel1 _valid4 = new();
        private int _collectionAddedItemsCount;
        private bool _collectionIsChanged;
        private bool _collectionIsValid;
        private int _collectionModifiedItemsCount;
        private int _collectionRemovedItemsCount;
        private bool _parentHasErrors;
        private bool _parentIsChanged;
        private bool _parentIsValid;

        public ChangeTrackingCollectionTests() => ResetTestModels();

        [Fact]
        public void AcceptChanges_CollectionContainsInvalidItems_DoesNothing()
        {
            // Arrange
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(true, _valid1, _valid2, _valid3);
            TestModel1Wrapper unmodifiedItem1 = parentWrapper.TM5Property2[0];
            TestModel1Wrapper unmodifiedItem2 = parentWrapper.TM5Property2[1];
            TestModel1Wrapper modifiedItem = parentWrapper.TM5Property2[2];
            modifiedItem.TM1Property3 = null;
            SaveOriginalState(parentWrapper);

            // Act
            parentWrapper.AcceptChanges();

            // Assert
            AssertOriginalState(parentWrapper);
            AssertCollectionItems(parentWrapper, CollectionType.ModifiedItems, modifiedItem);
            AssertCollectionItems(parentWrapper, CollectionType.ContainedItems, unmodifiedItem1, unmodifiedItem2, modifiedItem);
            modifiedItem.TM1Property3
                .Should()
                .BeNull();
        }

        [Fact]
        public void AcceptChanges_CollectionContainsValidAddedItems_AcceptsChanges()
        {
            // Arrange
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(false, _valid1);
            parentWrapper.TM5Property2.Add(new(_valid2));
            parentWrapper.TM5Property2.Add(new(_valid3));
            TestModel1Wrapper unmodifiedItem = parentWrapper.TM5Property2[0];
            TestModel1Wrapper modifiedItem1 = parentWrapper.TM5Property2[1];
            TestModel1Wrapper modifiedItem2 = parentWrapper.TM5Property2[2];

            // Act
            parentWrapper.AcceptChanges();

            // Assert
            AssertParentWrapperProperties(parentWrapper, false, true, false);
            AssertCollectionProperties(parentWrapper, 0, 0, 0, false, true);
            AssertCollectionItems(parentWrapper, CollectionType.ContainedItems, unmodifiedItem, modifiedItem1, modifiedItem2);
        }

        [Fact]
        public void AcceptChanges_CollectionContainsValidModifiedItems_AcceptsChanges()
        {
            // Arrange
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(true, _invalid1, _valid2, _valid3);
            TestModel1Wrapper modifiedItem1 = parentWrapper.TM5Property2[0];
            TestModel1Wrapper modifiedItem2 = parentWrapper.TM5Property2[1];
            TestModel1Wrapper unmodifiedItem = parentWrapper.TM5Property2[2];
            int modifiedProperty1Value = 6;
            string modifiedProperty2Value = "modified";
            modifiedItem1.TM1Property1 = modifiedProperty1Value;
            modifiedItem2.TM1Property2 = modifiedProperty2Value;

            // Act
            parentWrapper.AcceptChanges();

            // Assert
            AssertParentWrapperProperties(parentWrapper, false, true, false);
            AssertCollectionProperties(parentWrapper, 0, 0, 0, false, true);
            AssertCollectionItems(parentWrapper, CollectionType.ContainedItems, modifiedItem1, modifiedItem2, unmodifiedItem);
            modifiedItem1.TM1Property1
                .Should()
                .Be(modifiedProperty1Value);
            modifiedItem2.TM1Property2
                .Should()
                .Be(modifiedProperty2Value);
        }

        [Fact]
        public void AcceptChanges_CollectionIsValidAndHasAddedModifiedAndRemovedItems_AcceptsChanges()
        {
            // Arrange
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(false, _invalid1, _valid2, _valid3);
            TestModel1Wrapper removedItem = parentWrapper.TM5Property2[0];
            TestModel1Wrapper unmodifiedItem = parentWrapper.TM5Property2[1];
            TestModel1Wrapper modifiedItem = parentWrapper.TM5Property2[2];
            TestModel1Wrapper addedItem = new(_valid1);
            parentWrapper.TM5Property2.Remove(removedItem);
            parentWrapper.TM5Property2.Add(addedItem);
            string modifiedProperty2Value = "modified";
            modifiedItem.TM1Property2 = modifiedProperty2Value;

            // Act
            parentWrapper.AcceptChanges();

            // Assert
            AssertParentWrapperProperties(parentWrapper, false, true, false);
            AssertCollectionProperties(parentWrapper, 0, 0, 0, false, true);
            AssertCollectionItems(parentWrapper, CollectionType.ContainedItems, unmodifiedItem, modifiedItem, addedItem);
            modifiedItem.TM1Property2
                .Should()
                .Be(modifiedProperty2Value);
        }

        [Fact]
        public void AcceptChanges_CollectionIsValidAndHasRemovedItems_AcceptsChanges()
        {
            // Arrange
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(true, _valid1, _invalid2, _valid3);
            TestModel1Wrapper unmodifiedItem = parentWrapper.TM5Property2[0];
            TestModel1Wrapper removedItem1 = parentWrapper.TM5Property2[1];
            TestModel1Wrapper removedItem2 = parentWrapper.TM5Property2[2];
            parentWrapper.TM5Property2.Remove(removedItem1);
            parentWrapper.TM5Property2.Remove(removedItem2);

            // Act
            parentWrapper.AcceptChanges();

            // Assert
            AssertParentWrapperProperties(parentWrapper, false, true, false);
            AssertCollectionProperties(parentWrapper, 0, 0, 0, false, true);
            AssertCollectionItems(parentWrapper, CollectionType.ContainedItems, unmodifiedItem);
        }

        [Fact]
        public void AddedItems_AddAndThenRemoveInvalidItemFromCollection_ShouldRevertToOriginalState()
        {
            // Arrange
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(true, _valid1);
            SaveOriginalState(parentWrapper);
            TestModel1Wrapper unmodifiedItem = parentWrapper.TM5Property2[0];
            TestModel1Wrapper invalidItem = new(_invalid2);
            parentWrapper.TM5Property2.Add(invalidItem);

            // Act
            parentWrapper.TM5Property2.Remove(invalidItem);

            // Assert
            AssertOriginalState(parentWrapper);
            AssertCollectionItems(parentWrapper, CollectionType.ContainedItems, unmodifiedItem);
        }

        [Fact]
        public void AddedItems_AddInvalidItemsToCollection_UpdatesAddedItems()
        {
            // Arrange
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(true, _valid1);
            TestModel1Wrapper unmodifiedItem = parentWrapper.TM5Property2[0];
            TestModel1Wrapper invalidItem = new(_invalid2);
            TestModel1Wrapper validItem = new(_valid3);

            // Act
            parentWrapper.TM5Property2.Add(invalidItem);
            parentWrapper.TM5Property2.Add(validItem);

            // Assert
            AssertParentWrapperProperties(parentWrapper, true, false, false);
            AssertCollectionProperties(parentWrapper, 2, 0, 0, true, false);
            AssertCollectionItems(parentWrapper, CollectionType.AddedItems, invalidItem, validItem);
            AssertCollectionItems(parentWrapper, CollectionType.ContainedItems, unmodifiedItem, invalidItem, validItem);
        }

        [Fact]
        public void AddedItems_AddValidItemsToCollection_UpdatesAddedItems()
        {
            // Arrange
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(true, _valid1);
            TestModel1Wrapper unmodifiedItem = parentWrapper.TM5Property2[0];
            TestModel1Wrapper validItem1 = new(_valid2);
            TestModel1Wrapper validItem2 = new(_valid3);

            // Act
            parentWrapper.TM5Property2.Add(validItem1);
            parentWrapper.TM5Property2.Add(validItem2);

            // Assert
            AssertParentWrapperProperties(parentWrapper, true, true, false);
            AssertCollectionProperties(parentWrapper, 2, 0, 0, true, true);
            AssertCollectionItems(parentWrapper, CollectionType.AddedItems, validItem1, validItem2);
            AssertCollectionItems(parentWrapper, CollectionType.ContainedItems, unmodifiedItem, validItem1, validItem2);
        }

        [Fact]
        public void ChangeTrackingCollection_ConstructUsingEmptyList_InitializesProperties()
        {
            // Arrange/Act
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(false);

            // Assert
            AssertParentWrapperProperties(parentWrapper, false, true, false);
            parentWrapper.TM5Property2
                .Should()
                .NotBeNull();
            parentWrapper.TM5Property2
                .Should()
                .BeEmpty();
            AssertCollectionProperties(parentWrapper, 0, 0, 0, false, true);
        }

        [Fact]
        public void ChangeTrackingCollection_ConstructUsingListOfValidItems_InitializesProperties()
        {
            // Arrange/Act
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(true, _valid1, _valid2, _valid3);

            // Assert
            AssertParentWrapperProperties(parentWrapper, false, true, false);
            parentWrapper.TM5Property2
                .Should()
                .HaveCount(3);
            AssertCollectionProperties(parentWrapper, 0, 0, 0, false, true);
        }

        [Fact]
        public void ChangeTrackingCollection_ConstructWithListContainingInvalidItems_InitializesProperties()
        {
            // Arrange/Act
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(false, _valid1, _invalid2, _valid3);

            // Assert
            AssertParentWrapperProperties(parentWrapper, false, false, false);
            AssertCollectionProperties(parentWrapper, 0, 0, 0, false, false);
            parentWrapper.TM5Property2
                .Should()
                .HaveCount(3);
        }

        [Fact]
        public void ModifiedItems_MakeInvalidChangesAndThenReverseThem_ShouldRevertToOriginalState()
        {
            // Arrange
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(false, _valid1, _valid2, _valid3);
            SaveOriginalState(parentWrapper);
            TestModel1Wrapper modifiedModel = parentWrapper.TM5Property2[1];
            DateTime? originalValue = modifiedModel.TM1Property3;
            modifiedModel.TM1Property3 = new(1887, 7, 4);

            // Act
            modifiedModel.TM1Property3 = originalValue;

            // Assert
            AssertOriginalState(parentWrapper);
            modifiedModel.TM1Property3
                .Should()
                .Be(originalValue);
        }

        [Fact]
        public void ModifiedItems_MakeInvalidChangesToAValidItem_UpdatesModifiedItems()
        {
            // Arrange
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(false, _valid1, _valid2, _valid3);
            TestModel1Wrapper modifiedItem = parentWrapper.TM5Property2[1];
            TestModel1Wrapper unmodifiedItem1 = parentWrapper.TM5Property2[0];
            TestModel1Wrapper unmodifiedItem2 = parentWrapper.TM5Property2[2];
            DateTime? modifiedProperty3Value = new(1887, 7, 4);

            // Act
            modifiedItem.TM1Property3 = modifiedProperty3Value;

            // Assert
            AssertParentWrapperProperties(parentWrapper, true, false, false);
            AssertCollectionProperties(parentWrapper, 0, 1, 0, true, false);
            AssertCollectionItems(parentWrapper, CollectionType.ModifiedItems, modifiedItem);
            AssertCollectionItems(parentWrapper, CollectionType.ContainedItems, unmodifiedItem1, modifiedItem, unmodifiedItem2);
            modifiedItem.TM1Property3
                .Should()
                .Be(modifiedProperty3Value);
        }

        [Fact]
        public void ModifiedItems_MakeValidChangesAndThenReverseThem_ShouldRevertToOriginalState()
        {
            // Arrange
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(false, _invalid1, _valid2, _valid3);
            SaveOriginalState(parentWrapper);
            TestModel1Wrapper modifiedItem = parentWrapper.TM5Property2[0];
            int originalValue = modifiedItem.TM1Property1;
            modifiedItem.TM1Property1 = 2;

            // Act
            modifiedItem.TM1Property1 = originalValue;

            // Assert
            AssertOriginalState(parentWrapper);
            modifiedItem.TM1Property1
                .Should()
                .Be(originalValue);
        }

        [Fact]
        public void ModifiedItems_MakeValidChangesToAnInvalidItem_UpdatesModifiedItems()
        {
            // Arrange
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(false, _invalid2, _valid2, _valid3);
            TestModel1Wrapper modifiedItem = parentWrapper.TM5Property2[0];
            string modifiedProperty2Value = "valid value";
            DateTime modifiedProperty3Value = new(1987, 12, 12);

            // Act
            modifiedItem.TM1Property2 = modifiedProperty2Value;
            modifiedItem.TM1Property3 = modifiedProperty3Value;

            // Assert
            AssertParentWrapperProperties(parentWrapper, true, true, false);
            AssertCollectionProperties(parentWrapper, 0, 1, 0, true, true);
            AssertCollectionItems(parentWrapper, CollectionType.ModifiedItems, modifiedItem);
            modifiedItem.TM1Property2
                .Should()
                .Be(modifiedProperty2Value);
            modifiedItem.TM1Property3
                .Should()
                .Be(modifiedProperty3Value);
        }

        [Fact]
        public void ModifiedItems_ModifyMultipleItems_UpdatesModifiedItems()
        {
            // Arrange
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(false, _valid1, _valid2, _valid3);
            TestModel1Wrapper modifiedItem1 = parentWrapper.TM5Property2[2];
            TestModel1Wrapper modifiedItem2 = parentWrapper.TM5Property2[1];
            string modifiedProperty2Value = "modified";
            bool modifiedProperty4Value = true;

            // Act
            modifiedItem1.TM1Property2 = modifiedProperty2Value;
            modifiedItem2.TM1Property4 = modifiedProperty4Value;

            // Assert
            AssertParentWrapperProperties(parentWrapper, true, true, false);
            AssertCollectionProperties(parentWrapper, 0, 2, 0, true, true);
            AssertCollectionItems(parentWrapper, CollectionType.ModifiedItems, modifiedItem1, modifiedItem2);
            modifiedItem1.TM1Property2
                .Should()
                .Be(modifiedProperty2Value);
            modifiedItem2.TM1Property4
                .Should()
                .Be(modifiedProperty4Value);
        }

        [Fact]
        public void RejectChanges_CollectionItemsHaveBeenAdded_RejectsChanges()
        {
            // Arrange
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(false, _valid1);
            SaveOriginalState(parentWrapper);
            TestModel1Wrapper unmodifiedItem = parentWrapper.TM5Property2[0];
            TestModel1Wrapper addedItem1 = new(_valid2);
            TestModel1Wrapper addedItem2 = new(_valid3);
            parentWrapper.TM5Property2.Add(addedItem1);
            parentWrapper.TM5Property2.Add(addedItem2);

            // Act
            parentWrapper.RejectChanges();

            // Assert
            AssertOriginalState(parentWrapper);
            AssertCollectionItems(parentWrapper, CollectionType.ContainedItems, unmodifiedItem);
        }

        [Fact]
        public void RejectChanges_CollectionItemsHaveBeenAddedModifiedAndRemoved_RejectsChanges()
        {
            // Arrange
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(false, _valid1, _valid2, _valid3);
            SaveOriginalState(parentWrapper);
            TestModel1Wrapper unmodifiedItem = parentWrapper.TM5Property2[0];
            TestModel1Wrapper removedItem = parentWrapper.TM5Property2[1];
            TestModel1Wrapper modifiedItem = parentWrapper.TM5Property2[2];
            TestModel1Wrapper addedItem = new(_valid4);
            DateTime? originalProperty3Value = modifiedItem.TM1Property3;
            modifiedItem.TM1Property3 = null;
            parentWrapper.TM5Property2.Add(addedItem);
            parentWrapper.TM5Property2.Remove(removedItem);

            // Act
            parentWrapper.RejectChanges();

            // Assert
            AssertOriginalState(parentWrapper);
            AssertCollectionItems(parentWrapper, CollectionType.ContainedItems, unmodifiedItem, removedItem, modifiedItem);
            modifiedItem.TM1Property3
                .Should()
                .Be(originalProperty3Value);
        }

        [Fact]
        public void RejectChanges_CollectionItemsHaveBeenModified_RejectsChanges()
        {
            // Arrange
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(true, _valid1, _valid2, _valid3);
            SaveOriginalState(parentWrapper);
            TestModel1Wrapper modifiedItem1 = parentWrapper.TM5Property2[0];
            TestModel1Wrapper unmodifiedItem = parentWrapper.TM5Property2[1];
            TestModel1Wrapper modifiedItem2 = parentWrapper.TM5Property2[2];
            string originalProperty2Value1 = modifiedItem1.TM1Property2;
            string originalProperty2Value2 = modifiedItem2.TM1Property2;
            modifiedItem1.TM1Property2 = "modified1";
            modifiedItem2.TM1Property2 = "modified2";

            // Act
            parentWrapper.RejectChanges();

            // Assert
            AssertOriginalState(parentWrapper);
            AssertCollectionItems(parentWrapper, CollectionType.ContainedItems, modifiedItem1, unmodifiedItem, modifiedItem2);
            modifiedItem1.TM1Property2
                .Should()
                .Be(originalProperty2Value1);
            modifiedItem2.TM1Property2
                .Should()
                .Be(originalProperty2Value2);
        }

        [Fact]
        public void RejectChanges_CollectionItemsHaveBeenRemoved_RejectsChanges()
        {
            // Arrange
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(false, _valid1, _valid2, _valid3);
            SaveOriginalState(parentWrapper);
            TestModel1Wrapper removedItem1 = parentWrapper.TM5Property2[0];
            TestModel1Wrapper removedItem2 = parentWrapper.TM5Property2[2];
            TestModel1Wrapper unmodifiedItem = parentWrapper.TM5Property2[1];
            DateTime? originalProperty3Value = removedItem2.TM1Property3;
            removedItem2.TM1Property3 = null;
            parentWrapper.TM5Property2.Remove(removedItem1);
            parentWrapper.TM5Property2.Remove(removedItem2);

            // Act
            parentWrapper.RejectChanges();

            // Assert
            AssertOriginalState(parentWrapper);
            AssertCollectionItems(parentWrapper.TM5Property2, removedItem1, removedItem2, unmodifiedItem);
            removedItem2.TM1Property3
                .Should()
                .Be(originalProperty3Value);
        }

        [Fact]
        public void RejectChanges_NothingChangedInTheCollection_DoesNothing()
        {
            // Arrange
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(false, _valid1, _invalid2, _valid3);
            SaveOriginalState(parentWrapper);
            TestModel1Wrapper unmodifiedItem1 = parentWrapper.TM5Property2[0];
            TestModel1Wrapper unmodifiedItem2 = parentWrapper.TM5Property2[1];
            TestModel1Wrapper unmodifiedItem3 = parentWrapper.TM5Property2[2];

            // Act
            parentWrapper.RejectChanges();

            // Assert
            AssertOriginalState(parentWrapper);
            AssertCollectionItems(parentWrapper, CollectionType.ContainedItems, unmodifiedItem1, unmodifiedItem2, unmodifiedItem3);
        }

        [Fact]
        public void RemovedItems_MakeInvalidChangesToAnItemAndThenRemoveIt_UpdatesRemovedItems()
        {
            // Arrange
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(true, _valid1, _valid2, _valid3);
            TestModel1Wrapper unmodifiedItem1 = parentWrapper.TM5Property2[0];
            TestModel1Wrapper unmodifiedItem2 = parentWrapper.TM5Property2[1];
            TestModel1Wrapper removedItem = parentWrapper.TM5Property2[2];
            removedItem.TM1Property3 = null;

            // Act
            parentWrapper.TM5Property2.Remove(removedItem);

            // Assert
            AssertParentWrapperProperties(parentWrapper, true, true, false);
            AssertCollectionProperties(parentWrapper, 0, 0, 1, true, true);
            AssertCollectionItems(parentWrapper, CollectionType.RemovedItems, removedItem);
            AssertCollectionItems(parentWrapper, CollectionType.ContainedItems, unmodifiedItem1, unmodifiedItem2);
        }

        [Fact]
        public void RemovedItems_RemoveInvalidItem_UpdatesRemovedItems()
        {
            // Arrange
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(true, _valid1, _invalid2, _valid3, _valid4);
            TestModel1Wrapper unmodifiedItem1 = parentWrapper.TM5Property2[0];
            TestModel1Wrapper unmodifiedItem2 = parentWrapper.TM5Property2[2];
            TestModel1Wrapper unmodifiedItem3 = parentWrapper.TM5Property2[3];
            TestModel1Wrapper removedItem = parentWrapper.TM5Property2[1];

            // Act
            parentWrapper.TM5Property2.Remove(removedItem);

            // Assert
            AssertParentWrapperProperties(parentWrapper, true, true, false);
            AssertCollectionProperties(parentWrapper, 0, 0, 1, true, true);
            AssertCollectionItems(parentWrapper, CollectionType.RemovedItems, removedItem);
            AssertCollectionItems(parentWrapper, CollectionType.ContainedItems, unmodifiedItem1, unmodifiedItem2, unmodifiedItem3);
        }

        [Fact]
        public void RemovedItems_RemoveMultipleItems_UpdatesRemovedItems()
        {
            // Arrange
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(true, _valid1, _invalid2, _valid3, _valid4);
            TestModel1Wrapper unmodifiedItem1 = parentWrapper.TM5Property2[2];
            TestModel1Wrapper unmodifiedItem2 = parentWrapper.TM5Property2[3];
            TestModel1Wrapper removedItem1 = parentWrapper.TM5Property2[1];
            TestModel1Wrapper removedItem2 = parentWrapper.TM5Property2[0];

            // Act
            parentWrapper.TM5Property2.Remove(removedItem1);
            parentWrapper.TM5Property2.Remove(removedItem2);

            // Assert
            AssertParentWrapperProperties(parentWrapper, true, true, false);
            AssertCollectionProperties(parentWrapper, 0, 0, 2, true, true);
            AssertCollectionItems(parentWrapper, CollectionType.RemovedItems, removedItem1, removedItem2);
            AssertCollectionItems(parentWrapper, CollectionType.ContainedItems, unmodifiedItem1, unmodifiedItem2);
        }

        [Fact]
        public void RemovedItems_RemoveValidItem_UpdatesRemovedItems()
        {
            // Arrange
            TestModel5Wrapper parentWrapper = GetTestModel5Wrapper(true, _valid1, _invalid2, _valid3);
            TestModel1Wrapper removedItem = parentWrapper.TM5Property2![0];
            TestModel1Wrapper unmodifiedItem1 = parentWrapper.TM5Property2[1];
            TestModel1Wrapper unmodifiedItem2 = parentWrapper.TM5Property2[2];

            // Act
            parentWrapper.TM5Property2.Remove(removedItem);

            // Assert
            AssertParentWrapperProperties(parentWrapper, true, false, false);
            AssertCollectionProperties(parentWrapper, 0, 0, 1, true, false);
            AssertCollectionItems(parentWrapper, CollectionType.RemovedItems, removedItem);
            AssertCollectionItems(parentWrapper, CollectionType.ContainedItems, unmodifiedItem1, unmodifiedItem2);
        }

        private static void AssertCollectionItems(IEnumerable<TestModel1Wrapper> actual, params TestModel1Wrapper[] expectedItems)
        {
            if (expectedItems.Length == 0)
            {
                actual
                    .Should()
                    .BeEmpty();
            }
            else
            {
                List<TestModel1Wrapper> expected = new();

                foreach (TestModel1Wrapper item in expectedItems)
                {
                    expected.Add(item);
                }

                actual
                    .Should()
                    .HaveCount(expected.Count);
                actual
                    .Should()
                    .BeEquivalentTo(expected);
            }
        }

        private static void AssertCollectionItems(TestModel5Wrapper parentWrapper, CollectionType type, params TestModel1Wrapper[] expectedItems)
        {
            IEnumerable<TestModel1Wrapper> collection;

            switch (type)
            {
                case CollectionType.ContainedItems:
                    collection = parentWrapper.TM5Property2;
                    break;

                case CollectionType.AddedItems:
                    collection = parentWrapper.TM5Property2.AddedItems;
                    break;

                case CollectionType.ModifiedItems:
                    collection = parentWrapper.TM5Property2.ModifiedItems;
                    break;

                case CollectionType.RemovedItems:
                    collection = parentWrapper.TM5Property2.RemovedItems;
                    break;

                default:
                    Assert.Fail("Unknown collection type passed into AssertCollectionItems");
                    return;
            }

            AssertCollectionItems(collection, expectedItems);
        }

        private static void AssertCollectionProperties(
            TestModel5Wrapper parentWrapper,
            int addedItemsCount,
            int modifiedItemsCount,
            int removedItemsCount,
            bool isChanged,
            bool isValid)
        {
            ChangeTrackingCollection<TestModel1Wrapper> collection = parentWrapper.TM5Property2;

            ValidateItemCount(collection.AddedItems, addedItemsCount);
            ValidateItemCount(collection.ModifiedItems, modifiedItemsCount);
            ValidateItemCount(collection.RemovedItems, removedItemsCount);

            collection.IsChanged
                .Should()
                .Be(isChanged);
            collection.IsValid
                .Should()
                .Be(isValid);
        }

        private static void AssertParentWrapperProperties(TestModel5Wrapper parentWrapper, bool isChanged, bool isValid, bool hasErrors)
        {
            parentWrapper.IsChanged
                .Should()
                .Be(isChanged);
            parentWrapper.IsValid
                .Should()
                .Be(isValid);
            parentWrapper.HasErrors
                .Should()
                .Be(hasErrors);
        }

        private static TestModel5Wrapper GetTestModel5Wrapper(bool isX, params TestModel1[] collectionItems)
        {
            string property3Value = isX ? "X" : "test model";

            TestModel5 testModel5 = new()
            {
                TM5Property1 = 100,
                TM5Property2 = collectionItems.ToList(),
                TM5Property3 = property3Value,
                TM5Property4 = new(),
                TM5Property5 = new(),
                TM5Property6 = new() { TM4Property2 = 'a' }
            };

            return new TestModel5Wrapper(testModel5);
        }

        private static void SetModelProperties(
            TestModel1 model,
            int property1Value,
            string property2Value,
            DateTime? property3Value,
            bool property4Value)
        {
            model.TM1Property1 = property1Value;
            model.TM1Property2 = property2Value;
            model.TM1Property3 = property3Value;
            model.TM1Property4 = property4Value;
        }

        private static void ValidateItemCount(IList<TestModel1Wrapper> collection, int count)
        {
            collection
                .Should()
                .NotBeNull();

            if (count > 0)
            {
                collection
                    .Should()
                    .HaveCount(count);
            }
            else
            {
                collection
                    .Should()
                    .BeEmpty();
            }
        }

        private void AssertOriginalState(TestModel5Wrapper parentWrapper)
        {
            AssertParentWrapperProperties(parentWrapper, _parentIsChanged, _parentIsValid, _parentHasErrors);
            AssertCollectionProperties(parentWrapper, _collectionAddedItemsCount, _collectionModifiedItemsCount, _collectionRemovedItemsCount, _collectionIsChanged, _collectionIsValid);
        }

        private void ResetTestModels()
        {
            SetModelProperties(_invalid1, 11, "test", new(1987, 3, 13), false);
            SetModelProperties(_invalid2, 5, "", null, true);
            SetModelProperties(_valid1, 7, "valid1", null, false);
            SetModelProperties(_valid2, 10, "valid2", new(1955, 12, 22), false);
            SetModelProperties(_valid3, 1, "valid3", new(1979, 11, 13), true);
            SetModelProperties(_valid4, 9, "valid4", new(1988, 6, 4), true);
        }

        private void SaveOriginalState(TestModel5Wrapper parentWrapper)
        {
            _parentIsChanged = parentWrapper.IsChanged;
            _parentIsValid = parentWrapper.IsValid;
            _parentHasErrors = parentWrapper.HasErrors;

            ChangeTrackingCollection<TestModel1Wrapper> collection = parentWrapper.TM5Property2;
            _collectionAddedItemsCount = collection.AddedItems.Count;
            _collectionModifiedItemsCount = collection.ModifiedItems.Count;
            _collectionRemovedItemsCount = collection.RemovedItems.Count;
            _collectionIsChanged = collection.IsChanged;
            _collectionIsValid = collection.IsValid;
        }
    }
}