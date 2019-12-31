using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FaExportService
{
    public class CCsvExport<T> : IExporter where T : class
    {
        private readonly string _delimiter;

        private readonly IEnumerable<T> _items;

        private readonly PropertyInfo[] _properties;

        public CCsvExport(IEnumerable<T> items, string delimiter = ",")
        {
            _items = items;
            _delimiter = delimiter;
            _properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        public void ExportToFile(string path)
        {
            File.WriteAllText(path, ExportToString());
        }

        public string ExportToString()
        {
            StringBuilder sb = new StringBuilder();
            
            string header = String.Join(_delimiter, _properties.Select(f => f.Name).ToArray());
            sb.AppendLine(header);

            foreach (var item in _items)
                sb.AppendLine(ExportItem(item));

            return sb.ToString();
        }

        private string ExportItem(T item)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var property in _properties)
            {
                if (sb.Length > 0)
                    sb.Append(_delimiter);
                var value = property.GetValue(item);
                if (value != null)
                    sb.Append(value.ToString());
            }
            return sb.ToString();
        }
    }
}
