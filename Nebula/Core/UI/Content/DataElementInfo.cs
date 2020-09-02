using System.Windows;

namespace Nebula.Core.UI.Content
{
    public class DataElementInfo
    {
        public DataElementInfo(FrameworkElement frameworkElement, DependencyProperty dependencyProperty, string propertyName = null)
        {
            Element = frameworkElement;
            DependencyProperty = dependencyProperty;
            PropertyName = propertyName;
        }

        public FrameworkElement   Element            { get; }
        public DependencyProperty DependencyProperty { get; }
        public string             PropertyName       { get; }
    }
}