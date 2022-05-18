using System;
using System.Collections.Generic;
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

namespace TheoryOfElectrostatics.Pages
{
    /// <summary>
    /// Логика взаимодействия для EditLectionPage.xaml
    /// </summary>
    public partial class EditLectionPage : Page
    {
        public EditLectionPage()
        {
            InitializeComponent();
            LectionsComboBox.ItemsSource = new string[] { "Электростатика", "Цепи постоянного тока" };
            LectionsComboBox.SelectedIndex = 0;
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(PathTextBox.Text))
            {
                try
                {
                    string fileName = $@"{Environment.CurrentDirectory}\{LectionsComboBox.SelectedValue}.html";
                    File.Delete(fileName);
                    File.Move(PathTextBox.Text, fileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SelectPathButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
