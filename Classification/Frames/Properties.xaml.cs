﻿using Classification.Utility.SQL;
using System.Data;
using System.Windows;
using System.Windows.Controls;

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

            InitializeTables();
        }

        public void RefreshDataGrid()
        {
            DataTables.PropertiesDataTable.Clear();
            DataTables.PropertiesDataTable = _SQLClient.SelectProperties();

            PropertiesDataGrid.ItemsSource = null;
            PropertiesDataGrid.ItemsSource = DataTables.PropertiesDataTable.DefaultView;
        }

        private void SelectPropertyArousedConcepts(int propertyId)
        {
            DataTables.PropertyArousedConceptsDataTable.Clear();

            DataTables.PropertyArousedConceptsDataTable = 
                _SQLClient.SelectPropertyArousedConcepts(propertyId);

            PropertyArousedConceptsDataGrid.ItemsSource = null;
            PropertyArousedConceptsDataGrid.ItemsSource = DataTables.PropertyArousedConceptsDataTable.DefaultView;
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
                    "Вы действительно желаете удалить " +
                    string.Format("({0}) свойств(о/а)? ", PropertiesDataGrid.SelectedItems.Count) +
                    "Это действие нельзя будет отменить.", 
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

        private void PropertiesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PropertiesDataGrid.SelectedItems.Count == 1)
            {
                SelectPropertyArousedConcepts((int)((DataRowView)PropertiesDataGrid.SelectedItem)["Id"]);
            }
            else
            {
                DataTables.PropertyArousedConceptsDataTable.Clear();

                PropertyArousedConceptsDataGrid.ItemsSource = null;
            }
        }

        private void InitializeTables()
        {
            DataTables.PropertiesDataTable = new DataTable();

            DataTables.PropertyArousedConceptsDataTable = new DataTable();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeTables();

            RefreshDataGrid();           
        }
    }
}
