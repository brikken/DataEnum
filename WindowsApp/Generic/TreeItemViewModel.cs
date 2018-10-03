using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
        ObservableCollection<ItemProperty> Properties { get; }
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

    [DoNotNotify]
    abstract class TreeItemViewModel : INotifyPropertyChanged, ITreeItemViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        public ObservableCollection<ItemProperty> Properties
        {
            get
            {
                return new ObservableCollection<ItemProperty>(
                    GetType()
                        .GetProperties(System.Reflection.BindingFlags.Public)
                        .OrderBy(prop => prop.Name)
                        .Select(prop => (ItemProperty)new ItemProperty() { Name = prop.Name, Value = prop.GetValue(this).ToString() })
                        .ToList()
                );
            }
        }
    }
}
