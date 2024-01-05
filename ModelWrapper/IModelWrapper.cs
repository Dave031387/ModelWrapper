namespace ModelWrapper
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/IModelWrapper/*"/>
    public interface IModelWrapper<T> where T : class
    {
        /// <include file="docs.xml" path="docs/members[@name=&quot;observablebase&quot;]/PropertyChanged/*"/>
        event PropertyChangedEventHandler? PropertyChanged;

        /// <include file="docs.xml" path="docs/members[@name=&quot;notifydataerrorinfobase&quot;]/HasErrors/*"/>
        bool HasErrors { get; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/IsChanged/*"/>
        bool IsChanged { get; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/IsValid/*"/>
        bool IsValid { get; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/Model/*"/>
        T Model { get; }

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/AcceptChanges/*"/>
        void AcceptChanges();

        /// <include file="docs.xml" path="docs/members[@name=&quot;notifydataerrorinfobase&quot;]/GetErrors/*"/>
        IEnumerable GetErrors(string? propertyName);

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/RejectChanges/*"/>
        void RejectChanges();

        /// <include file="docs.xml" path="docs/members[@name=&quot;modelwrapperbase&quot;]/Validate1/*"/>
        IEnumerable<ValidationResult> Validate(ValidationContext validationContext);
    }
}