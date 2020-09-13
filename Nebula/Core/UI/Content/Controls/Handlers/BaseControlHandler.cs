using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using ModernWpf.Controls.Primitives;
using Nebula.Core.UI.Content.Attributes;

namespace Nebula.Core.UI.Content.Controls.Handlers
{
    public class BaseControlHandler : IControlCacheHandler
    {
        private static readonly BindingFlags FieldBindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.Default;

        public Type ControlType { get; } = null;

        public void HandleControl(FrameworkElement element, PropertyInfo propertyInfo, DataCategory dataCategory, DataProperty dataProperty,
                                  ref DependencyProperty dependencyProperty)
        {
            FrameworkElement bindableElement = element is IDataControlsContainer container ? container.GetBindableElement() : element;
            bindableElement.Tag = dataProperty.Tag;
            if (!string.IsNullOrWhiteSpace(dataProperty.ToolTipKey))
                bindableElement.ToolTip = NebulaClient.GetLocString(dataProperty.ToolTipKey);
            if (bindableElement is Control control)
            {
                ControlHelper.SetHeader(control, NebulaClient.GetLocString(dataProperty.HeaderKey));
                ControlHelper.SetPlaceholderText(control, NebulaClient.GetLocString(dataProperty.PlaceholderKey));
                ControlHelper.SetDescription(control, NebulaClient.GetLocString(dataProperty.DescriptionKey));
            }

            if (string.IsNullOrWhiteSpace(dataProperty.DependencyProperty))
                return;

            dependencyProperty =
                bindableElement.GetType().GetField(dataProperty.DependencyProperty, FieldBindingFlags)?.GetValue(bindableElement) as DependencyProperty;
        }
    }
}