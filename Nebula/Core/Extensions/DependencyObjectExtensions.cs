using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nebula.Core.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static T GetChildOfType<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null)
                return null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                T result = child as T ?? child.GetChildOfType<T>();
                if (result != null)
                    return result;
            }

            return null;
        }

        public static void RemoveChild(this DependencyObject parent, UIElement child)
        {
            switch (parent)
            {
                case Panel panel:
                    panel.Children.Remove(child);
                    return;
                case Decorator decorator:
                {
                    if (decorator.Child == child)
                        decorator.Child = null;
                    return;
                }
                case ContentPresenter contentPresenter:
                {
                    if (Equals(contentPresenter.Content, child))
                        contentPresenter.Content = null;
                    return;
                }
                case ContentControl contentControl:
                {
                    if (Equals(contentControl.Content, child))
                        contentControl.Content = null;
                    return;
                }
            }
        }
    }
}