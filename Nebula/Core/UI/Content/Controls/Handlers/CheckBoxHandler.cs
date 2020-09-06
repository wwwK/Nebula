using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Nebula.Core.UI.Content.Attributes;

namespace Nebula.Core.UI.Content.Controls.Handlers
{
    public class CheckBoxHandler : IControlCacheHandler
    {
        public Type ControlType { get; } = typeof(CheckBox);

        public void HandleControl(FrameworkElement element, PropertyInfo propertyInfo, DataCategory dataCategory, DataProperty dataProperty,
                                  ref DependencyProperty dependencyProperty)
        {
            dependencyProperty = ToggleButton.IsCheckedProperty;
            if (element is CheckBox checkBox)
                checkBox.Content = NebulaClient.GetLocString(dataProperty.HeaderKey);
        }
    }
}