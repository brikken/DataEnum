using DataEnum;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace WindowsApp.Main
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string TableDefinitionFilename { get; set; } = @"table-def02.json";
        public string TableDefinitionFilenameFull {
            get {
                return (new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "TableDefinitions", TableDefinitionFilename))).FullName;
            }
        }
        public ObservableCollection<TabDefItem> TableDefinitionItems { get; set; }

        public bool CanLoadTableDefinition()
        {
            return (new FileInfo(TableDefinitionFilenameFull)).Exists;
        }

        public void LoadTableDefinition()
        {
            var tableDefinition = TableDefinition.FromFile(TableDefinitionFilenameFull).Single();
            TableDefinitionItems = TabDefItem.FromTableDefinition(tableDefinition);
        }
    }

    class TabDefItem : Generic.TreeItemViewModel
    {
        public ObservableCollection<TabDefItem> Children { get; set; } = new ObservableCollection<TabDefItem>();
        public string Label { get; set; }
        public ICollectionView Items
        {
            get
            {
                return new CollectionViewSource() { Source = Children }.View;
            }
        }

        public static ObservableCollection<TabDefItem> FromTableDefinition(TableDefinition tableDefinition)
        {
            return new ObservableCollection<TabDefItem>(tableDefinition.members.Select(member => FromMember(member)));
        }

        public static TabDefItem FromMember(TableDefinition.Member member)
        {
            IEnumerable<TabDefItem> children = member.alternatives.Select(alt => FromAlternative(alt));
            if (member.signal)
            {
                children = children.Prepend(new TabDefSignal() { Label = member.signalFullname });
            }

            TabDefItem tabDefItem;
            if (member.dataType.Primitive)
            {
                tabDefItem = new TabDefPrimitive() { DataTypeName = member.dataType.name };
            }
            else
            {
                tabDefItem = new TabDefMember() { DataTypeName = member.dataType.name };
            }
            tabDefItem.Label = member.fullname;
            tabDefItem.Children = new ObservableCollection<TabDefItem>(children);
            return tabDefItem;
        }

        public static TabDefItem FromAlternative(TableDefinition.Alternative alternative)
        {
            return new TabDefAlternative()
            {
                Label = alternative.fullname,
                Children = new ObservableCollection<TabDefItem>(alternative.fields.Select(field => FromMember(field)))
            };
        }
    }

    class TabDefTyped : TabDefItem
    {
        public string DataTypeName { get; set; }
        public string LabelWithType
        {
            get
            {
                return Label + " (" + DataTypeName + ")";
            }
        }
    }
    class TabDefMember : TabDefTyped { }
    class TabDefAlternative : TabDefItem { }
    class TabDefSignal : TabDefItem { }
    class TabDefPrimitive : TabDefTyped { }
}
