using System;
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
using System.Windows.Shapes;

using Classification.Utility.SQL;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Classification.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddClassificatonWindow.xaml
    /// </summary>
    public partial class AddSourceWindow : Window
    {
        private readonly SQLClient _SQLClient;

        public AddSourceWindow()
        {
            InitializeComponent();
        }

        public AddSourceWindow(SQLClient client) : this()
        {
            _SQLClient = client;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void AddSourceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _SQLClient.InsertSource(
                        SourceNameTextBox.Text,
                        SourceAuthorTextBox.Text,
                        int.Parse(SourceYearTextBox.Text)
                    );

                Frames.Sources.Instance.SelectSources();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }

            Frames.Properties.Instance.RefreshDataGrid();
        }
    }
}
