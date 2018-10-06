using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WindowsApp.Generic
{
    public interface ITreeItemViewModel : IItemViewModel
    {
        bool IsExpanded { get; set; }
    }

    public interface IItemViewModel
    {
        bool IsSelected { get; set; }
        ObservableCollection<IItemProperty> Properties { get; }
    }

    public interface IItemProperty
    {
        string Name { get; }
        string Value { get; }
    }

    public class ItemProperty : IItemProperty
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public abstract class ItemViewModel : INotifyPropertyChanged, IItemViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<IItemProperty> Properties
        {
            get
            {
                return new ObservableCollection<IItemProperty>(
                    GetType()
                        .GetProperties()
                        .Where(prop => prop.Name != "Properties")
                        .OrderBy(prop => prop.Name)
                        .Select(prop => new ItemProperty() { Name = prop.Name, Value = prop.GetValue(this).ToString() })
                        .ToList()
                );
            }
        }

        bool _isSelected = false;
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }
    }

    [DoNotNotify]
    abstract class TreeItemViewModel : ItemViewModel, INotifyPropertyChanged, ITreeItemViewModel
    {
        bool _isExpanded = false;
        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    OnPropertyChanged("IsExpanded");
                }
            }
        }

        public Type Type
        {
            get
            {
                return GetType();
            }
        }
    }
}
