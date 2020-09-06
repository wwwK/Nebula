using System;
using System.Reflection;
using System.Windows;
using Nebula.Core.UI.Content.Attributes;

namespace Nebula.Core.UI.Content.Controls
{
    public interface IControlCacheHandler
    {
        public Type ControlType { get; }

        public void HandleControl(FrameworkElement element, PropertyInfo propertyInfo, DataCategory dataCategory, DataProperty dataProperty,
                                  ref DependencyProperty dependencyProperty);
    }
}