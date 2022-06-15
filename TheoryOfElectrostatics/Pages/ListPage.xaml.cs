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
using TheoryOfElectrostatics.Classes;

namespace TheoryOfElectrostatics.Pages
{
    /// <summary>
    /// Логика взаимодействия для Lections.xaml
    /// </summary>
    public partial class ListPage : Page
    {
        public ListPage(byte type)
        {
            InitializeComponent();
            List<string> lections = new List<string>();
            Dictionary<TextImage, string> themes = new Dictionary<TextImage, string>();
            string view = type == 0 ? "Лекция" : (type == 1 ? "Практика" : "Тест");

            using (ZipFile zip = DataManager.OpenZip(DataManager.DataPath))
            {
                foreach (var entry in zip.Entries)
                {
                    lections.Add(entry.FileName.Split('/')[0]);
                }
                lections = lections.Distinct().ToList();

                foreach (var lection in lections)
                {
                    var entry = zip.SelectEntries("*", $"{lection}/Icon").FirstOrDefault();
                    string image = null;
                    if (entry != null)
                    {
                        var pathArr = entry.FileName.Split('/');
                        image = pathArr[pathArr.Length - 1];
                    }

                    themes.Add(new TextImage() { Text = lection, Image = image }, view);
                }
            }

            DataListView.ItemsSource = themes;
        }

        private void ListViewItemBorderOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender as Border).DataContext is KeyValuePair<TextImage, string> theme)
            {
                switch (theme.Value)
                {
                    case "Лекция":
                        DataManager.DataFrame.Navigate(new Pages.HtmlPage(theme.Key.Text,true));
                        break;
                    case "Практика":
                        DataManager.DataFrame.Navigate(new Pages.HtmlPage(theme.Key.Text,false));
                        break;
                    case "Тест":
                        DataManager.CurrentTheme = theme.Key.Text;
                        DataManager.DataFrame.Navigate(new Pages.DescriptionTestPage());
                        break;
                    default:
                        break;
                }
            }
        }

        private void ImageTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            if (textBlock.DataContext is KeyValuePair<TextImage, string> theme)
            {
                string name = theme.Key.Image;
                Image imageControl = (textBlock.Parent as Grid).FindName("ItemImage") as Image;
                name = $"{theme.Key.Text}/Icon/{name}";
                var image = DataManager.GetImage(name);

                if (image != null)
                {
                    imageControl.Source = image;
                    return;
                }

                imageControl.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/NoImage.png"));
            }
        }
    }
}
