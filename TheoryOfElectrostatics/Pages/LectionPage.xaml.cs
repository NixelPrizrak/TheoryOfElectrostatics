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
using TheoryOfElectrostatics.Classes;

namespace TheoryOfElectrostatics.Pages
{
    /// <summary>
    /// Логика взаимодействия для Lection.xaml
    /// </summary>
    public partial class HTMLPage : Page
    {
        public HTMLPage(string theme,bool lection)
        {
            InitializeComponent();
            Directory.Delete(Properties.Settings.Default.TempPath, true);
            DataManager.CheckTempFolder();
            using (ZipFile zip = DataManager.OpenZip(DataManager.LectionsPath))
            {
                zip.ExtractSelectedEntries("*", theme, Properties.Settings.Default.TempPath);
                zip.ExtractSelectedEntries("*", $"{theme}/Lection.files", Properties.Settings.Default.TempPath);
            }
            string fileLection = Path.Combine(Properties.Settings.Default.TempPath, theme, $"Lection.html");

            if (File.Exists(fileLection))
            {
                LectionWebBrowser.Navigate(new Uri(fileLection));
            }
        }
    }
}
