using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Nebula.Core.UI.Content.Attributes;

namespace Nebula.Core.UI.Content.Controls.Handlers
{
    public class TextBoxHandler : IControlCacheHandler
    {
        public Type ControlType { get; } = typeof(TextBox);

        public void HandleControl(FrameworkElement element, PropertyInfo propertyInfo, DataCategory dataCategory, DataProperty dataProperty,
                                  ref DependencyProperty dependencyProperty)
        {
            dependencyProperty = TextBox.TextProperty;
        }
    }
}