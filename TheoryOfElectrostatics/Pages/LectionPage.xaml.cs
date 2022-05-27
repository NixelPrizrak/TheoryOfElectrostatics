using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Ionic.Zip;
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

namespace TheoryOfElectrostatics.Pages
{
    /// <summary>
    /// Логика взаимодействия для Lection.xaml
    /// </summary>
    public partial class LectionPage : Page
    {
        public LectionPage(string lection)
        {
            InitializeComponent();
            Directory.Delete(Properties.Settings.Default.TempPath, true);
            DataManager.CheckTempFolder();
            using (ZipFile zip = DataManager.OpenZip())
            {
                zip.ExtractSelectedEntries("*", lection, Properties.Settings.Default.TempPath);
                zip.ExtractSelectedEntries("*", $"{lection}/{lection}.files", Properties.Settings.Default.TempPath);
            }
            string fileLection = Path.Combine(Properties.Settings.Default.TempPath, lection, $"{lection}.html");

            if (File.Exists(fileLection))
            {
                LectionWebBrowser.Navigate(new Uri(fileLection));
            }
        }
    }
}
