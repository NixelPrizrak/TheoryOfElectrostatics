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

namespace TheoryOfElectrostatics
{
    /// <summary>
    /// Логика взаимодействия для AdministrationWindow.xaml
    /// </summary>
    public partial class AdministrationWindow : Window
    {
        public AdministrationWindow()
        {
            InitializeComponent();
            DataManager.AdministrationFrame = AdministrationFrame;
            DataManager.AdministrationFrame.Navigate(new Pages.EditLectionPage());
        }

        private void LectionsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TestsButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
