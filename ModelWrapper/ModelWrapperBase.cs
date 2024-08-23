namespace ModelWrapper
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/ModelWrapperBase/*" />
    public abstract class ModelWrapperBase<T> : NotifyDataErrorInfoBase, IModelWrapper<T>, IValidatingTrackingObject, IValidatableObject
        where T : class
    {
        internal readonly Dictionary<string, Action> _propertyActions;
        internal readonly List<IValidatingTrackingObject> _trackingObjects;
        private const string IsChangedSuffix = "IsChanged";
        private readonly Dictionary<string, object?> _originalValues;

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/Constructor/*" />
        public ModelWrapperBase(T model)
        {
            ArgumentNullException.ThrowIfNull(model, nameof(model));
            Model = model;
            _originalValues = [];
            _propertyActions = [];
            _trackingObjects = [];
            InitializeComplexProperties();
            InitializeCollectionProperties();
            InitializePropertyActions();
            Validate();
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/IsChanged/*" />
        public bool IsChanged => _originalValues.Count > 0 || _trackingObjects.Any(t => t.IsChanged);

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/IsValid/*" />
        public bool IsValid => !HasErrors && _trackingObjects.All(r => r.IsValid);

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/Model/*" />
        public T Model
        {
            get;
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/AcceptChanges/*" />
        public void AcceptChanges()
        {
            if (IsChanged && IsValid)
            {
                _originalValues.Clear();

                foreach (IRevertibleChangeTracking trackingObject in _trackingObjects)
                {
                    trackingObject.AcceptChanges();
                }

                OnPropertyChanged(string.Empty);
            }
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/RejectChanges/*" />
        public void RejectChanges()
        {
            if (IsChanged)
            {
                foreach (KeyValuePair<string, object?> originalValueEntry in _originalValues)
                {
                    PropertyInfo? property = typeof(T).GetProperty(originalValueEntry.Key);
                    property?.SetValue(Model, originalValueEntry.Value);
                }

                _originalValues.Clear();

                foreach (IRevertibleChangeTracking trackingObject in _trackingObjects)
                {
                    trackingObject.RejectChanges();
                }

                Validate();
                OnPropertyChanged(string.Empty);
            }
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/Validate1/*" />
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/GetIsChanged/*" />
        protected bool GetIsChanged(string propertyName) => _originalValues.ContainsKey(propertyName);

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/GetOriginalValue/*" />
        protected TValue? GetOriginalValue<TValue>(string propertyName)
        {
            if (_originalValues.TryGetValue(propertyName, out object? value))
            {
                return value is null ? default : (TValue?)value;
            }

            return GetValue<TValue>(propertyName);
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/GetValue/*" />
        protected TValue? GetValue<TValue>([CallerMemberName] string? propertyName = null)
        {
            PropertyInfo propertyInfo = GetPropertyInfo(propertyName);
            TValue? value = (TValue?)propertyInfo.GetValue(Model);
            return value is null ? default : value;
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/InitializeCollectionProperties/*" />
        protected virtual void InitializeCollectionProperties()
        {
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/InitializeComplexProperties/*" />
        protected virtual void InitializeComplexProperties()
        {
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/InitializePropertyActions/*" />
        protected virtual void InitializePropertyActions()
        {
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/RegisterCollection/*" />
        protected void RegisterCollection<TWrapper, TModel>(ChangeTrackingCollection<TWrapper> wrapperCollection, IList<TModel> modelCollection)
            where TWrapper : ModelWrapperBase<TModel>
            where TModel : class
        {
            ArgumentNullException.ThrowIfNull(modelCollection, nameof(modelCollection));
            ArgumentNullException.ThrowIfNull(wrapperCollection, nameof(wrapperCollection));

            if (modelCollection.Count != wrapperCollection.Count)
            {
                throw new ArgumentException("RegisterCollection: The model collection and wrapper collection must contain the same number of elements.");
            }

            wrapperCollection.CollectionChanged += (s, e) =>
            {
                modelCollection.Clear();

                foreach (TWrapper wrapper in wrapperCollection)
                {
                    modelCollection.Add(wrapper.Model);
                }

                Validate();
            };
            RegisterTrackingObject(wrapperCollection);
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/RegisterComplex/*" />
        protected void RegisterComplex<TModel>(ModelWrapperBase<TModel> wrapper)
            where TModel : class
        {
            ArgumentNullException.ThrowIfNull(wrapper, nameof(wrapper));

            RegisterTrackingObject(wrapper);
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/RegisterPropertyAction/*" />
        protected void RegisterPropertyAction(string propertyName, Action propertyAction)
        {
            // GetPropertyInfo is called to verify that a valid property name was passed into the
            // method. An exception will be thrown if the property name is invalid.
            _ = GetPropertyInfo(propertyName);
            ArgumentNullException.ThrowIfNull(propertyAction, nameof(propertyAction));

            if (!_propertyActions.TryAdd(propertyName, propertyAction))
            {
                _propertyActions[propertyName] = propertyAction;
            }
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/SetValue/*" />
        protected void SetValue<TValue>(TValue? newValue, [CallerMemberName] string? propertyName = null)
        {
            PropertyInfo propertyInfo = GetPropertyInfo(propertyName);
            object? currentValue = propertyInfo.GetValue(Model);

            if (!Equals(currentValue, newValue))
            {
                UpdateOriginalValue(currentValue, newValue, propertyName!);
                propertyInfo.SetValue(Model, newValue);

                if (_propertyActions.TryGetValue(propertyName!, out Action? value))
                {
                    value?.Invoke();
                }

                Validate();
                OnPropertyChanged(propertyName);
                OnPropertyChanged(propertyName + IsChangedSuffix);
            }
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/Validate2/*" />
        protected void Validate()
        {
            ClearErrors();

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

            OnPropertyChanged(nameof(IsValid));
        }

        private PropertyInfo GetPropertyInfo(string? propertyName)
        {
            ArgumentNullException.ThrowIfNull(propertyName, nameof(propertyName));
            ArgumentNullException.ThrowIfNull(Model, nameof(Model));

            PropertyInfo? propertyInfo = Model.GetType().GetProperty(propertyName);

            if (propertyInfo is null)
            {
                string msg = $"The property \"{propertyName}\" is not a member of \"{Model.GetType().Name}\".";
                throw new ArgumentException(msg);
            }

            return propertyInfo;
        }

        private void RegisterTrackingObject(IValidatingTrackingObject trackingObject)
        {
            if (!_trackingObjects.Contains(trackingObject))
            {
                _trackingObjects.Add(trackingObject);
                trackingObject.PropertyChanged += TrackingObjectPropertyChanged;
            }
        }

        private void TrackingObjectPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsChanged))
            {
                OnPropertyChanged(nameof(IsChanged));
            }
            else if (e.PropertyName == nameof(IsValid))
            {
                OnPropertyChanged(nameof(IsValid));
            }
        }

        private void UpdateOriginalValue(object? currentValue, object? newValue, string propertyName)
        {
            if (!_originalValues.TryGetValue(propertyName, out object? value))
            {
                _originalValues.Add(propertyName, currentValue);
                OnPropertyChanged(nameof(IsChanged));
            }
            else if (Equals(newValue, value))
            {
                _ = _originalValues.Remove(propertyName);
                OnPropertyChanged(nameof(IsChanged));
            }
        }
    }
}