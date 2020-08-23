using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Nebula.UI.Controls
{
    public class SimpleGrid : Grid
    {
        private GridLengthCollection _rows;
        private GridLengthCollection _columns;

        public SimpleGrid()
        {
        }

        public GridLengthCollection Rows
        {
            get => _rows;
            set
            {
                _rows = value;
                RowDefinitions.Clear();
                if (_rows == null)
                    return;
                foreach (GridLength length in _rows)
                    RowDefinitions.Add(new RowDefinition {Height = length});
            }
        }

        public GridLengthCollection Columns
        {
            get => _columns;
            set
            {
                _columns = value;
                ColumnDefinitions.Clear();
                if (_columns == null)
                    return;
                foreach (GridLength length in _columns)
                    ColumnDefinitions.Add(new ColumnDefinition {Width = length});
            }
        }
    }
}