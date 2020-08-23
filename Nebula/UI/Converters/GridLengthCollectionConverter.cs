using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using Nebula.UI.Controls;

namespace Nebula.UI.Converters
{
    public class GridLengthCollectionConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string s)
                return ParseString(s, culture);
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is GridLengthCollection collection)
                return ToString(collection, culture);
            return base.ConvertTo(context, culture, value, destinationType);
        }

        private string ToString(GridLengthCollection value, CultureInfo culture)
        {
            GridLengthConverter converter = new GridLengthConverter();
            return string.Join(",", value.Select(v => converter.ConvertToString(v)));
        }

        private GridLengthCollection ParseString(string s, CultureInfo culture)
        {
            GridLengthConverter converter = new GridLengthConverter();
            IEnumerable<GridLength> lengths = s.Split(',').Select(p => (GridLength) converter.ConvertFromString(p.Trim()));
            return new GridLengthCollection(lengths.ToArray());
        }
    }
}