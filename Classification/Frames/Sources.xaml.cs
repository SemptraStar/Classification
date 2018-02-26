using System;
using System.Collections;
using System.Collections.Generic;
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

using Classification.Utility.SQL;
using System.Data.SqlClient;
using System.Data;

namespace Classification.Frames
{
    /// <summary>
    /// Логика взаимодействия для Sources.xaml
    /// </summary>
    public partial class Sources : Page
    {
        private readonly SQLClient _SQLClient;

        private int _SelectedSourceId;

        public static Sources Instance;

        public Sources()
        {
            InitializeComponent();
            Instance = this;
        }
        public Sources(SQLClient client) : this()
        {
            _SQLClient = client;

            DataTables.SourcesDataTable = new System.Data.DataTable();

            SelectSources();
        }

        public void SelectSources()
        {
            DataTables.SourcesDataTable.Clear();
            DataTables.SourcesDataTable = _SQLClient.SelectSources();

            SourcesDataGrid.ItemsSource = null;
            SourcesDataGrid.ItemsSource = DataTables.SourcesDataTable?.DefaultView;
        }
        private void SelectSourceDefinitions()
        {
            DataTable dataTable = _SQLClient.SelectSourceDefinitions(_SelectedSourceId);

            DefinitionsDataGrid.ItemsSource = null;
            DefinitionsDataGrid.ItemsSource = dataTable?.DefaultView;
        }
        
        private void SourcesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SourcesDataGrid.SelectedItems.Count == 1)
            {
                _SelectedSourceId = (int)((DataRowView)SourcesDataGrid.SelectedItem)["Id"];
                SelectSourceDefinitions();
            }
            else
            {
                DefinitionsDataGrid.ItemsSource = null;
                DefinitionsDataGrid.Items.Clear();
            }
        }


        private void AddSource_Click(object sender, RoutedEventArgs e)
        {
            var addSourceWindow = new Windows.AddSourceWindow(_SQLClient);
            addSourceWindow.Show();
        }
        private void ChangeSources_Click(object sender, RoutedEventArgs e)
        {
            if (SourcesDataGrid.SelectedItems.Count == 1)
            {
                int selectedSourceId = (int)((DataRowView)SourcesDataGrid.SelectedItem)["Id"];

                var changeSourceWindow = new Windows.ChangeSourceWindow(_SQLClient, selectedSourceId);
                changeSourceWindow.Show();
            }
                    
        }      
        private void DeleteSources_Click(object sender, RoutedEventArgs e)
        {
            if (SourcesDataGrid.SelectedItems.Count > 0)
            {
                if (MessageBox.Show(
                    $"Вы действительно желаете удалить " +
                    $"({SourcesDataGrid.SelectedItems.Count}) источник(а/ов)? " +
                    $"Это действие нельзя будет отменить.",
                    "Удаление источников",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    return;
                }

                foreach (var item in SourcesDataGrid.SelectedItems)
                {
                    _SQLClient.DeleteSource((int)((DataRowView)item)["Id"]);
                }

                SelectSources();
                SelectSourceDefinitions();
            }
        }
    }
}
