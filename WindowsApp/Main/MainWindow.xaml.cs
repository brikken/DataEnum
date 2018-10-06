using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
            try
            {
                viewModel.LoadTableDefinition();
            }
            catch (InvalidTableDefinitionFileException ex)
            {
                string text = $"The table definition file is invalid. It must contain exactly one table definition.\nTable definitions in file:\n{string.Join("\n", ex.TableDefinitionNames)}";
                MessageBox.Show(text, "Invalid file", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void SelectFile_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.AddExtension = true;
            ofd.DefaultExt = "json";
            ofd.Filter = "JSON|*.json|All files|*.*";
            ofd.FilterIndex = 0;
            ofd.InitialDirectory = viewModel.TableDefinitionsFolder;
            ofd.Multiselect = false;
            ofd.Title = "Select file with table definition";
            ofd.ValidateNames = true;
            if (ofd.ShowDialog() ?? false)
            {
                viewModel.TableDefinitionFilename = ofd.FileName;
            }
        }
    }

    class Commands
    {
        public static readonly RoutedUICommand ExpandAll = new RoutedUICommand(
            "Expand all nodes",
            "ExpandAll",
            typeof(MainWindow),
            new InputGestureCollection() { new KeyGesture(Key.X, ModifierKeys.Alt | ModifierKeys.Control) }
            );

        public static readonly RoutedUICommand LoadTableDefinition = new RoutedUICommand(
            "Load table definition from file",
            "LoadTableDefinition",
            typeof(MainWindow),
            new InputGestureCollection() { new KeyGesture(Key.L, ModifierKeys.Control) }
            );

        public static readonly RoutedUICommand SelectFile = new RoutedUICommand(
            "Select file",
            "SelectFile",
            typeof(MainWindow),
            new InputGestureCollection() { new KeyGesture(Key.S, ModifierKeys.Alt) }
            );
    }
}