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
    public partial class Properties : Page
    {
        public static Properties Instance;

        private readonly SQLClient _SQLClient;

        public Properties()
        {
            InitializeComponent();

            Instance = this;
        }

        public Properties(SQLClient client) : this()
        {
            _SQLClient = client;

            DataTables.PropertiesDataTable = new System.Data.DataTable();
            RefreshDataGrid();
        }

        public void RefreshDataGrid()
        {
            DataTables.PropertiesDataTable.Clear();
            DataTables.PropertiesDataTable = _SQLClient.SelectProperties();

            PropertiesDataGrid.ItemsSource = null;
            PropertiesDataGrid.ItemsSource = DataTables.PropertiesDataTable?.DefaultView;
        }

        private void AddProperty_Click(object sender, RoutedEventArgs e)
        {
            var addProperyWindow = new Windows.AddPropertyWindow(_SQLClient);
            addProperyWindow.Show();
        }

        private void ChangeConcepts_Click(object sender, RoutedEventArgs e)
        {
            if (PropertiesDataGrid.SelectedItems.Count == 1)
            {
                int selectedPropertyId = (int)((DataRowView)PropertiesDataGrid.SelectedItem)["Id"];

                var changeProperyWindow = new Windows.ChangePropertyWindow(_SQLClient, selectedPropertyId);
                changeProperyWindow.Show();
            }          
        }

        private void DeleteProperty_Click(object sender, RoutedEventArgs e)
        {
            if (PropertiesDataGrid.SelectedItems.Count > 0)
            {
                if (MessageBox.Show(
                    $"Вы действительно желаете удалить " +
                    $"({PropertiesDataGrid.SelectedItems.Count}) свойств(о/а)? " +
                    $"Это действие нельзя будет отменить.", 
                    "Удаление свойств", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    return;
                }

                foreach (var item in PropertiesDataGrid.SelectedItems)
                {
                    _SQLClient.DeleteProperty((int)((DataRowView)item)["Id"]);                            
                }

                RefreshDataGrid();
            }
        }
    }
}
