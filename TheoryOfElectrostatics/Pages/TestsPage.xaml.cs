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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TheoryOfElectrostatics.Pages
{
    /// <summary>
    /// Логика взаимодействия для Tests.xaml
    /// </summary>
    public partial class TestsPage : Page
    {
        public TestsPage()
        {
            InitializeComponent();
        }

        private void ListViewItemBorderOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            int lection = DataListView.Items.IndexOf(((sender as Border).TemplatedParent as ListViewItem).DataContext);
            switch (lection)
            {
                case 0:
                    DataManager.IdTest = 1;
                    DataManager.DataFrame.Navigate(new Pages.DescriptionTestPage());
                    break;
                default:
                    break;
            }
        }
    }
}
