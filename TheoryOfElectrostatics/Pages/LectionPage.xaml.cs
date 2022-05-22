using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
    /// Логика взаимодействия для Lection.xaml
    /// </summary>
    public partial class LectionPage : Page
    {
        public LectionPage(int lection)
        {
            InitializeComponent();
            LectionWebBrowser.Navigate($@"{Environment.CurrentDirectory}/Lection2.html");
        }
    }
}
