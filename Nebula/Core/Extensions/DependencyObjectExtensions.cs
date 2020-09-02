using System.Windows;
using System.Windows.Controls;

namespace Nebula.Core.Extensions
{
    public static class DependencyObjectExtensions
    {
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