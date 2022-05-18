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
    /// Логика взаимодействия для Lections.xaml
    /// </summary>
    public partial class LectionsPage : Page
    {
        public LectionsPage()
        {
            InitializeComponent();
        }

        private void ListViewItemBorderOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            int lection = DataListView.Items.IndexOf(((sender as Border).TemplatedParent as ListViewItem).DataContext);
            switch (lection)
            {
                case 0:
                    DataManager.DataFrame.Navigate(new Pages.LectionPage(1));
                    break;
                default:
                    break;
            }
        }
    }
}
