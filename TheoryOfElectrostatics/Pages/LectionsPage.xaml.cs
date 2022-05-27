using Ionic.Zip;
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
            List<string> lections = new List<string>();
            List<Theme> themes = new List<Theme>();
            using (ZipFile zip = DataManager.OpenZip())
            {
                if (Directory.Exists(Properties.Settings.Default.TempPath))
                {
                    Directory.Delete(Properties.Settings.Default.TempPath, true);
                }
                DataManager.CheckTempFolder();

                foreach (var entry in zip.Entries)
                {
                    lections.Add(entry.FileName.Split('/')[0]);
                }
                lections = lections.Distinct().ToList();

                foreach (var lection in lections)
                {
                    zip.ExtractSelectedEntries("*", $"{lection}/Icon", Properties.Settings.Default.TempPath);
                    string folder = Path.Combine(Properties.Settings.Default.TempPath, lection, "Icon");
                    string image = null;
                    if (Directory.Exists(folder))
                    {
                        image = Directory.GetFiles(folder).FirstOrDefault();
                    }
                    if (image == null)
                    {
                        image = "pack://application:,,,/Resources/NoImage.png";
                    }

                    themes.Add(new Theme() { Name = lection, Image = image });
                }
            }

            DataListView.ItemsSource = themes;
        }

        private void ListViewItemBorderOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            string lection = (((sender as Border).TemplatedParent as ListViewItem).DataContext as Theme).Name;
            DataManager.DataFrame.Navigate(new Pages.LectionPage(lection));
        }
    }
}
