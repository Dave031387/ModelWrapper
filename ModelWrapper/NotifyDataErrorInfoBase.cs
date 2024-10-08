﻿namespace ModelWrapper
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    /// <include file="docs.xml" path="docs/members[@name=&quot;notifydataerrorinfobase&quot;]/NotifyDataErrorInfoBase/*"/>
    public abstract class NotifyDataErrorInfoBase : ObservableBase, INotifyDataErrorInfo
    {
        /// <include file="docs.xml" path="docs/members[@name=&quot;notifydataerrorinfobase&quot;]/Errors/*"/>
        protected readonly Dictionary<string, List<string>> Errors;

        /// <include file="docs.xml" path="docs/members[@name=&quot;notifydataerrorinfobase&quot;]/Constructor/*"/>
        protected NotifyDataErrorInfoBase() => Errors = [];

        /// <include file="docs.xml" path="docs/members[@name=&quot;notifydataerrorinfobase&quot;]/ErrorsChanged/*"/>
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        /// <include file="docs.xml" path="docs/members[@name=&quot;notifydataerrorinfobase&quot;]/HasErrors/*"/>
        public bool HasErrors => Errors.Count > 0;

        /// <include file="docs.xml" path="docs/members[@name=&quot;notifydataerrorinfobase&quot;]/GetErrors/*"/>
        public IEnumerable GetErrors(string? propertyName) =>
            propertyName is not null && Errors.TryGetValue(propertyName, out List<string>? value) ? value : [];

        /// <include file="docs.xml" path="docs/members[@name=&quot;notifydataerrorinfobase&quot;]/ClearErrors/*"/>
        protected void ClearErrors()
        {
            foreach (string propertyName in Errors.Keys.ToList())
            {
                Errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }

        /// <include file="docs.xml" path="docs/members[@name=&quot;notifydataerrorinfobase&quot;]/OnErrorsChanged/*"/>
        protected virtual void OnErrorsChanged(string propertyName) =>
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }
}