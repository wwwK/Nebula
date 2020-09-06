using System;
using System.Runtime.CompilerServices;

namespace Nebula.Core.UI.Content.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DataProperty : Attribute
    {
        public DataProperty(Type controlType, string headerKey = "", string placeholderKey = "", string descriptionKey = "",
                            string toolTipKey = "", string dependencyProperty = "", object tag = null,
                            [CallerLineNumber] int order = 0, params object[] args)
        {
            ControlType = controlType;
            DependencyProperty = dependencyProperty;
            Order = order;
            HeaderKey = headerKey;
            PlaceholderKey = placeholderKey;
            DescriptionKey = descriptionKey;
            ToolTipKey = toolTipKey;
            Tag = tag;
            Params = args;
        }

        public Type     ControlType        { get; }
        public string   DependencyProperty { get; }
        public int      Order              { get; }
        public string   HeaderKey          { get; }
        public string   PlaceholderKey     { get; }
        public string   DescriptionKey     { get; }
        public string   ToolTipKey         { get; }
        public object   Tag                { get; }
        public object[] Params             { get; }
    }
}