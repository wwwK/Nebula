using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Nebula.Core.UI.Content.Attributes;
using Nebula.Core.UI.Content.Controls;
using Nebula.Core.UI.Content.Controls.Handlers;

namespace Nebula.Core.UI.Content
{
    public class DataContentCache
    {
        private static readonly BindingFlags PropertyBindingFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty;

        private static readonly Dictionary<Type, IControlCacheHandler> ControlCacheHandlers = new Dictionary<Type, IControlCacheHandler>();
        private static readonly BaseControlHandler                     BaseControlHandler   = new BaseControlHandler();

        static DataContentCache()
        {
            RegisterControlHandler(new TextBoxHandler());
            RegisterControlHandler(new CheckBoxHandler());
            RegisterControlHandler(new NumberBoxHandler());
        }

        public DataContentCache(Type type)
        {
            DataContentType = type;
        }

        public Type DataContentType { get; }

        private List<DataElementInfo> Elements { get; } = new List<DataElementInfo>();

        public IEnumerable<FrameworkElement> GetFrameworkElements()
        {
            foreach (DataElementInfo elementInfo in Elements)
                yield return elementInfo.Element;
        }

        private void AddElement(FrameworkElement element, DependencyProperty dependencyProperty = null, PropertyInfo propertyInfo = null)
        {
            Elements.Add(new DataElementInfo(element, dependencyProperty, propertyInfo?.Name));
        }

        public void PrepareFor(BaseDataContent dataContent)
        {
            foreach (DataElementInfo dataElementInfo in Elements)
            {
                if (dataElementInfo.DependencyProperty == null)
                    continue;
                FrameworkElement bindableElement = dataElementInfo.Element;
                if (dataElementInfo.Element is IDataControlsContainer dataControlsContainer)
                    bindableElement = dataControlsContainer.GetBindableElement();
                Binding binding = new Binding
                {
                    Path = new PropertyPath(dataElementInfo.PropertyName),
                    Source = dataContent
                };
                bindableElement.SetBinding(dataElementInfo.DependencyProperty, binding);
            }
        }

        public static void RegisterControlHandler(IControlCacheHandler cacheHandler)
        {
            if (ControlCacheHandlers.ContainsKey(cacheHandler.ControlType))
                return;
            ControlCacheHandlers.Add(cacheHandler.ControlType, cacheHandler);
        }

        public static DataContentCache BuildCache<T>()
        {
            return BuildCache(typeof(T));
        }

        public static DataContentCache BuildCache(Type type)
        {
            DataContentCache dataContentCache = new DataContentCache(type);
            Type frameworkType = typeof(FrameworkElement);
            foreach (PropertyInfo propertyInfo in type.GetProperties(PropertyBindingFlags).OrderBy(p => p.GetCustomAttribute<DataProperty>()?.Order))
            {
                DataProperty dataProperty = propertyInfo.GetCustomAttribute<DataProperty>();
                if (dataProperty == null || !frameworkType.IsAssignableFrom(dataProperty.ControlType))
                    continue;
                ConstructorInfo ctor = dataProperty.ControlType.GetConstructor(new[] {typeof(object[])});
                FrameworkElement frameworkElement;
                if (ctor == null)
                    frameworkElement = (FrameworkElement) Activator.CreateInstance(dataProperty.ControlType);
                else
                    frameworkElement = (FrameworkElement) Activator.CreateInstance(dataProperty.ControlType, dataProperty.Params);
                if (frameworkElement == null)
                    continue;
                DataCategory dataCategory = propertyInfo.GetCustomAttribute<DataCategory>();
                if (dataCategory != null)
                    dataContentCache.AddElement(new TextBlock {Text = NebulaClient.GetLocString(dataCategory.Category), FontSize = 24});
                DependencyProperty dependencyProperty = null;
                BaseControlHandler.HandleControl(frameworkElement, propertyInfo, dataCategory, dataProperty, ref dependencyProperty);
                if (ControlCacheHandlers.ContainsKey(dataProperty.ControlType))
                    ControlCacheHandlers[dataProperty.ControlType].HandleControl(frameworkElement, propertyInfo, dataCategory, dataProperty, ref dependencyProperty);
                dataContentCache.AddElement(frameworkElement, dependencyProperty, propertyInfo);
            }

            return dataContentCache;
        }
    }
}