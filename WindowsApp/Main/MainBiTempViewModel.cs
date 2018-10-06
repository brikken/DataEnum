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
            Folder = Path.Combine(Directory.GetCurrentDirectory(), "TableDefinitions", "BiTemporal");
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
}
