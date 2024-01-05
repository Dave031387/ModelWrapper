namespace ModelWrapper
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <include file="docs.xml" path="docs/members[@name=&quot;observablebase&quot;]/ObservableBase/*"/>
    public abstract class ObservableBase : INotifyPropertyChanged
    {
        /// <include file="docs.xml" path="docs/members[@name=&quot;observablebase&quot;]/PropertyChanged/*"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <include file="docs.xml" path="docs/members[@name=&quot;observablebase&quot;]/OnPropertyChanged/*"/>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}