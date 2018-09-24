using DataEnum;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    class TabDefItem
    {
        public ObservableCollection<TabDefItem> Children { get; set; }
        public string Label { get; set; }

        public static ObservableCollection<TabDefItem> FromTableDefinition(TableDefinition tableDefinition)
        {
            return new ObservableCollection<TabDefItem>(tableDefinition.members.Select(member => FromMember(member)));
        }

        public static TabDefItem FromMember(TableDefinition.Member member)
        {
            IEnumerable<TabDefItem> children = member.alternatives.Select(alt => FromAlternative(alt));
            if (member.signal)
            {
                children = children.Prepend(new TabDefItem() { Label = member.signalFullname });
            }

            TabDefItem tabDefItem;
            if (member.dataType.Primitive)
            {
                tabDefItem = new TabDefPrimitive();
            }
            else
            {
                tabDefItem = new TabDefMember();
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

    class TabDefMember : TabDefItem { }
    class TabDefAlternative : TabDefItem { }
    class TabDefSignal : TabDefItem { }
    class TabDefPrimitive : TabDefItem { }
}
