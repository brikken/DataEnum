using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindowsApp.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadTableDefinition_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = viewModel.CanLoadTableDefinition();
        }

        private void LoadTableDefinition_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            viewModel.LoadTableDefinition();
        }

        private void ExpandAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ExpandAllNodes(tvTableDefinition.Items);
        }

        private void ExpandAllNodes(ICollectionView itemCol)
        {
            foreach (var item in itemCol)
            {
                var treeItem = item as TabDefItem;
                if (treeItem != null)
                {
                    ExpandAllNodes(treeItem.Items);
                    treeItem.IsExpanded = true;
                }
            }
        }
    }

    class Commands
    {
        public static readonly RoutedUICommand LoadTableDefinition = new RoutedUICommand(
            "Load table definition from file",
            "LoadTableDefinition",
            typeof(MainWindow),
            new InputGestureCollection() { new KeyGesture(Key.L, ModifierKeys.Control) }
            );

        public static readonly RoutedUICommand ExpandAll = new RoutedUICommand(
            "Expand all nodes",
            "ExpandAll",
            typeof(MainWindow),
            new InputGestureCollection() { new KeyGesture(Key.X, ModifierKeys.Alt | ModifierKeys.Control) }
            );
    }
}