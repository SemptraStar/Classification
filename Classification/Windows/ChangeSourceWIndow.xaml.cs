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
    public partial class ChangeSourceWindow : Window
    {
        private readonly SQLClient _SQLClient;

        private int _SourceId;

        public ChangeSourceWindow()
        {
            InitializeComponent();
        }
        public ChangeSourceWindow(SQLClient client) : this()
        {
            _SQLClient = client;
        }
        public ChangeSourceWindow(SQLClient client, int sourceId) : this(client)
        {
            _SourceId = sourceId;
            FindSource();
        }

        private void FindSource()
        {
            var source = _SQLClient.FindSource(_SourceId);

            SourceNameTextBox.Text = (string)source["Name"];
            SourceAuthorTextBox.Text = (string)source["Author"];
            SourceYearTextBox.Text = ((int)source["Year"]).ToString();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ChangeSourceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _SQLClient.UpdateSource(
                        _SourceId,
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
