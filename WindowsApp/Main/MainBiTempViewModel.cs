using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using DataBiTemporal.Parser;

namespace WindowsApp.Main
{
    class MainBiTempViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string folder;
        public string Folder
        {
            get
            {
                return folder;
            }
            set
            {
                folder = value;
                Files = new ObservableCollection<FileInfo>(new DirectoryInfo(Folder).GetFiles());
            }
        }

        public ObservableCollection<FileInfo> Files { get; private set; } = new ObservableCollection<FileInfo>();

        public MainBiTempViewModel()
        {
        }
    }

    class FileToTokensConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = (value as FileInfo).FullName;
            string code = File.ReadAllText(path);
            return new List<IToken>(LexerHelpers.GetTokens(code));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class FileToChannelsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = (value as FileInfo).FullName;
            string code = File.ReadAllText(path);
            return LexerHelpers.GetChannels(code);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class TokenToPropertiesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value
                .GetType()
                .GetProperties()
                .Select(prop => new { Property = prop.Name, Value = prop.GetValue(value).ToString() })
                .OrderBy(prop => prop.Property)
                .ToList();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class StripLineBreaksConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value as string;
            return text?.Replace("\n", " ").Replace("\r", "");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class FileToRootContextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = (value as FileInfo).FullName;
            string code = File.ReadAllText(path);
            return new List<IParseTree>() { ParserHelpers.GetRootContext(code) };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class ContextToChildContextsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var context = value as IParseTree;
            var children = new List<IParseTree>();
            foreach (var child in Trees.GetDescendants(context))
            {
                if (child.Parent == context)
                {
                    children.Add(child);
                }
            }
            return children;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class ContextToPropertiesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return new List<object>();
            }

            var properties = value
                .GetType()
                .GetProperties()
                .Select(prop => new {
                    Type = "Property",
                    prop.Name,
                    Value = prop.PropertyType.GetInterfaces().Count(i => i.Name == "IEnumerable") > 0
                        ? string.Join(", ", (prop.GetValue(value) as IEnumerable<object>)?.Select(subprop => subprop.ToString()))
                        : prop.GetValue(value)?.ToString()
                })
                .ToList();
            var fields = value
                .GetType()
                .GetFields()
                .Select(field => new {
                    Type = "Field",
                    field.Name,
                    Value = field.FieldType.GetInterfaces().Count(i => i.Name == "IEnumerable") > 0
                        ? string.Join(", ", (field.GetValue(value) as IEnumerable<object>)?.Select(subfield => subfield.ToString()))
                        : field.GetValue(value)?.ToString()
                })
                .ToList();
            return properties.Concat(fields).OrderBy(mem => mem.Name);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
