using System;
using System.Reflection;
using System.Windows;
using ModernWpf.Controls;
using Nebula.Core.UI.Content.Attributes;

namespace Nebula.Core.UI.Content.Controls.Handlers
{
    public class NumberBoxHandler : IControlCacheHandler
    {
        public Type ControlType { get; } = typeof(NumberBox);

        public void HandleControl(FrameworkElement element, PropertyInfo propertyInfo, DataCategory dataCategory, DataProperty dataProperty,
                                  ref DependencyProperty dependencyProperty)
        {
            dependencyProperty = NumberBox.ValueProperty;
            if (element is NumberBox numberBox)
            {
                if (dataProperty.Params.Length != 3)
                    return;
                int buttonPlacement = (int) dataProperty.Params[0];
                int minValue = (int) dataProperty.Params[1];
                int maxValue = (int) dataProperty.Params[2];
                numberBox.Minimum = minValue;
                numberBox.Maximum = maxValue;
                numberBox.SpinButtonPlacementMode = buttonPlacement switch
                {
                    0 => NumberBoxSpinButtonPlacementMode.Compact,
                    1 => NumberBoxSpinButtonPlacementMode.Inline,
                    _ => NumberBoxSpinButtonPlacementMode.Hidden
                };
            }
        }
    }
}