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
    /// Логика взаимодействия для HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : PatternWindow
    {
        public HelpWindow()
        {
            InitializeComponent();
            ResizeMode = ResizeMode.NoResize;
            ButtonsVisible = false;
        }

        private void AdministrationButton_Click(object sender, RoutedEventArgs e)
        {
            new AuthWindow().ShowDialog();
        }
    }
}
