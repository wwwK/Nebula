using System;

namespace Nebula.Core.UI.Content.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DataCategory : Attribute
    {
        public DataCategory(string category)
        {
            Category = category;
        }

        public string Category { get; }
    }
}