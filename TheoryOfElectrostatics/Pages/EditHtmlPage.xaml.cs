using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Ionic.Zip;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для EditLectionPage.xaml
    /// </summary>
    public partial class EditHtmlPage : Page
    {
        private string type;

        public EditHtmlPage(bool lection)
        {
            InitializeComponent();

            type = lection ? "Lection" : "Practice";
            TitleTextBlock.Text = lection ? "Изменение лекции" : "Изменение практики";

            DataManager.CheckTempFolder();
            ChangeImage(DataManager.CurrentTheme);
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(PathTextBox.Text))
            {
                try
                {
                    List<string> hrefs = new List<string>();
                    string html = "";
                    using (StreamReader reader = new StreamReader(PathTextBox.Text))
                    {
                        html = reader.ReadToEnd();
                    }

                    FileInfo htmlPath = new FileInfo(PathTextBox.Text);
                    string fileFolder = $"{type}.files";

                    foreach (Match item in Regex.Matches(html, "[^\\\\]'(([^'<>?|\"]|(\\\\'))*[^\\\\])'"))
                    {
                        string filePath = "";
                        string name = item.Groups[1].Value;

                        if (File.Exists(name))
                        {
                            filePath = name;
                        }
                        else if (File.Exists($"{htmlPath.DirectoryName}/{name}"))
                        {
                            filePath = name;
                        }

                        if (filePath != "")
                        {
                            FileInfo file = new FileInfo(filePath);
                            html = Regex.Replace(html, name, $"{fileFolder}/{file.Name}");
                            hrefs.Add(file.FullName);
                        }
                    }
                    foreach (Match item in Regex.Matches(html, "[^\\\\]\"([^<>?|\"]+[^\\\\])\""))
                    {
                        string filePath = "";
                        string name = item.Groups[1].Value;

                        if (File.Exists(name))
                        {
                            filePath = name;
                        }
                        else if (File.Exists($"{htmlPath.DirectoryName}/{name}"))
                        {
                            filePath = name;
                        }

                        if (filePath != "")
                        {
                            FileInfo file = new FileInfo(filePath);
                            html = Regex.Replace(html, name, $"{fileFolder}/{file.Name}");
                            hrefs.Add(file.FullName);
                        }
                    }

                    var parser = new HtmlParser();
                    var document = parser.ParseDocument(html);

                    if (!document.QuerySelector("head").InnerHtml.Contains("IE=edge"))
                    {
                        document.QuerySelector("head").InnerHtml += "<meta http-equiv='X-UA-Compatible' content='IE=edge' />";
                    }
                    if (!document.QuerySelector("head").InnerHtml.Contains("windows-1251"))
                    {
                        document.QuerySelector("head").InnerHtml += "<meta charset='windows-1251'>";
                    }
                    document.QuerySelector("body").SetAttribute("style", "background-color:aliceblue");

                    html = document.QuerySelector("html").OuterHtml;

                    using (ZipFile zip = DataManager.OpenZip(DataManager.DataPath))
                    {
                        zip.RemoveSelectedEntries("*", Path.Combine(DataManager.CurrentTheme, fileFolder));
                        zip.RemoveSelectedEntries($"{type}.html", DataManager.CurrentTheme);
                        zip.Save();
                    }

                    hrefs = hrefs.Distinct().ToList();
                    using (ZipFile zip = DataManager.OpenZip(DataManager.DataPath))
                    {
                        zip.AddEntry($"{DataManager.CurrentTheme}/{type}.html", html);
                        zip.AddFiles(hrefs, Path.Combine(DataManager.CurrentTheme, fileFolder));
                        zip.Save();
                    }

                    MessageBox.Show("Файлы загружены.", "Информация");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Данного файла не существует.", "Информация");
            }
        }

        private void SelectPathButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "HTML|*.html;*.htm";

            if (open.ShowDialog().Value)
            {
                PathTextBox.Text = open.FileName;
            }
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            string folder = DataManager.SelectFolder();
            if (folder == null)
            {
                return;
            }

            using (ZipFile zip = DataManager.OpenZip(DataManager.DataPath))
            {
                if (Directory.Exists(Path.Combine(folder, DataManager.CurrentTheme)))
                {
                    if (MessageBox.Show("Папка с названием темы уже существует. Перезаписать?", "Информация", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    {
                        return;
                    }
                    Directory.Delete(Path.Combine(folder, DataManager.CurrentTheme), true);
                }

                zip.ExtractSelectedEntries("*", DataManager.CurrentTheme, folder, ExtractExistingFileAction.OverwriteSilently);
                zip.ExtractSelectedEntries("*", Path.Combine(DataManager.CurrentTheme, $"{type}.files"), folder, ExtractExistingFileAction.OverwriteSilently);
            }

            MessageBox.Show("Выгрузка файлов темы завершена.", "Информация");
        }

        private void ChangeImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Все (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png|BMP(*.bmp)|*.bmp|JPEG(*.jpg)|*.jpg|PNG(*.png)|*.png";
            if (open.ShowDialog().Value)
            {
                using (ZipFile zip = DataManager.OpenZip(DataManager.DataPath))
                {
                    string folder = Path.Combine(DataManager.CurrentTheme, "Icon");
                    zip.RemoveSelectedEntries("*", folder);
                    zip.Save();
                    zip.AddFile(open.FileName, folder);
                    zip.Save();
                }
                ChangeImage(DataManager.CurrentTheme);
            }
        }

        private void ChangeImage(string theme)
        {
            string image = null;
            using (ZipFile zip = DataManager.OpenZip(DataManager.DataPath))
            {
                Regex reg = new Regex($"^{theme}/Icon");
                foreach (var entry in zip.Entries)
                {
                    if (reg.IsMatch(entry.FileName))
                    {
                        image = entry.FileName;
                        break;
                    }
                }
            }

            if (image != null)
            {
                ThemeImage.Source = DataManager.GetImage(image);
                return;
            }

            ThemeImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/NoImage.png"));
        }
    }
}
