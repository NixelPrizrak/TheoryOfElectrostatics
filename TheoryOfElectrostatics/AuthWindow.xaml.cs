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
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : PatternWindow
    {
        public AuthWindow()
        {
            InitializeComponent();
            ButtonsVisible = false;
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            if (AuthTextBox.Text == "12345")
            {
                new AdministrationWindow().ShowDialog();
                this.Close();
            }
            else
            {
                AuthTextBox.Background = new SolidColorBrush(Color.FromArgb(255, 255, 171, 171));
            }
        }

        private void AuthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AuthTextBox.Background = Brushes.White;
        }
    }
}
