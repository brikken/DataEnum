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
    class InvalidTableDefinitionFileException : Exception
    {
        public IList<string> TableDefinitionNames { get; set; } = new List<string>();
    }

    class MainViewModel : INotifyPropertyChanged
    {

        private const string DefaultTabDefFolder = "TableDefinitions";

        public event PropertyChangedEventHandler PropertyChanged;

        public string SQL { get; private set; }
        public string TableDefinitionFilename { get; set; } = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), DefaultTabDefFolder, @"table-def02.json")).FullName;
        public string TableDefinitionsFolder {
            get {
                return Path.Combine(Directory.GetCurrentDirectory(), DefaultTabDefFolder);
            }
        }
        public ObservableCollection<TabDefItem> TableDefinitionItems { get; set; }

        public bool CanLoadTableDefinition()
        {
            return new FileInfo(TableDefinitionFilename).Exists;
        }

        public void LoadTableDefinition()
        {
            ICollection<TableDefinition> tabDefs = new Collection<TableDefinition>();
            TableDefinition tableDefinition;
            try
            {
                tabDefs = TableDefinition.FromFile(TableDefinitionFilename);
                tableDefinition = tabDefs.Single();
            }
            catch (InvalidOperationException e)
            {
                InvalidTableDefinitionFileException invalidTableDefinitionFileException = new InvalidTableDefinitionFileException() { TableDefinitionNames = tabDefs.Select(td => td.name).ToList() };
                throw invalidTableDefinitionFileException;
            }
            TableDefinitionItems = TabDefItem.FromTableDefinition(tableDefinition);
            SQL = tableDefinition.GetSQL();
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
