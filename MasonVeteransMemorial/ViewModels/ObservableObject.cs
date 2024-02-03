using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MasonVeteransMemorial.ViewModels
{
    public class ObservableObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <returns><c>true</c>, if property was set, <c>false</c> otherwise.</returns>
        /// <param name="backingStore">Backing store.</param>
        /// <param name="value">Value.</param>
        /// <param name="propertyName">Property name.</param>
        /// <param name="onChanged">On changed.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected virtual bool SetProperty<T>(
            ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            NotifyDependantProperties(propertyName);
        }

        protected virtual void RaiseProperyChanged([CallerMemberName]string propertyName = "") => OnPropertyChanged(propertyName);

        private void NotifyDependantProperties(string propertyName)
        {
            foreach (var dep in deps)
            {
                if (dep.Value?.Contains(propertyName) == true)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(dep.Key));
                }
            }
        }

        Dictionary<string, string[]> deps = new Dictionary<string, string[]>();
        protected void ComputedProperty<TSource>(string computedProperty, params string[] dependantProps)
        {
            deps.Remove(computedProperty);
            deps.Add(computedProperty, dependantProps);
        }

    }
}
