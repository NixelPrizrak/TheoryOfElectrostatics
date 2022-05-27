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

namespace TheoryOfElectrostatics.Pages
{
    /// <summary>
    /// Логика взаимодействия для EditLectionPage.xaml
    /// </summary>
    public partial class EditLectionPage : Page
    {
        public EditLectionPage()
        {
            InitializeComponent();
            DataManager.CheckTempFolder();
            PathTextBox.Text = @"D:\Folder\TheoryOfElectrostatics\TheoryOfElectrostatics\bin\Debug\Lection1.html";

            RefreshLections();
        }

        private void RefreshLections()
        {
            LectionsComboBox.ItemsSource = null;
            LectionsComboBox.Items.Clear();

            List<string> themes = new List<string>();
            using (ZipFile zip = DataManager.OpenZip())
            {
                if (Directory.Exists(Properties.Settings.Default.TempPath))
                {
                    Directory.Delete(Properties.Settings.Default.TempPath, true);
                }
                DataManager.CheckTempFolder();

                foreach (var entry in zip.Entries)
                {
                    themes.Add(entry.FileName.Split('/')[0]);
                }
                themes = themes.Distinct().ToList();
                foreach (var theme in themes)
                {
                    zip.ExtractSelectedEntries("*", $"{theme}/Icon", Properties.Settings.Default.TempPath);
                }
            }

            if (themes.Count == 0)
            {
                LectionsComboBox.IsEnabled = false;
                LoadButton.IsEnabled = false;
                SelectPathButton.IsEnabled = false;
                LectionsComboBox.Items.Add("Пусто");
            }
            else
            {
                LectionsComboBox.IsEnabled = true;
                LoadButton.IsEnabled = true;
                SelectPathButton.IsEnabled = true;
                LectionsComboBox.ItemsSource = themes;
            }

            LectionsComboBox.SelectedIndex = 0;
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
                    string fileFolder = $"{LectionsComboBox.SelectedItem}.files";
                    //MessageBox.Show(Regex.Match(html, "[^\\\\]'(([^'<>?|\"]|(\\\\'))*[^\\\\])'").Value);

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
                    if (!document.QuerySelector("head").InnerHtml.Contains("utf-8"))
                    {
                        document.QuerySelector("head").InnerHtml += "<meta charset='utf-8'>";
                    }

                    byte[] htmlBytes = Encoding.Convert(Encoding.GetEncoding("windows-1251"), Encoding.UTF8, Encoding.GetEncoding("windows-1251").GetBytes(document.QuerySelector("html").OuterHtml));

                    using (ZipFile zip = DataManager.OpenZip())
                    {
                        zip.RemoveSelectedEntries("*", Path.Combine(LectionsComboBox.SelectedItem.ToString(), fileFolder));
                        zip.RemoveSelectedEntries("*", LectionsComboBox.SelectedItem.ToString());
                        zip.Save();
                    }

                    hrefs = hrefs.Distinct().ToList();
                    using (ZipFile zip = DataManager.OpenZip())
                    {
                        zip.AddEntry($"{LectionsComboBox.SelectedItem}/{LectionsComboBox.SelectedItem}.html", htmlBytes);
                        zip.AddFiles(hrefs, Path.Combine(LectionsComboBox.SelectedItem.ToString(), fileFolder));
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
            open.InitialDirectory = Path.GetPathRoot(Environment.CurrentDirectory);
            open.Filter = "HTML|*.html;*.htm";

            if (open.ShowDialog().Value)
            {
                PathTextBox.Text = open.FileName;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (ZipFile zip = DataManager.OpenZip())
                {
                    zip.AddDirectoryByName(ThemeTextBox.Text.Trim());
                    zip.Save();
                }

                ThemeTextBox.Text = "";
                MessageBox.Show("Тема добавлена.", "Информация");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            RefreshLections();
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            string folder = DataManager.SelectFolder();
            using (ZipFile zip = DataManager.OpenZip())
            {
                if (Directory.Exists(Path.Combine(folder, LectionsComboBox.SelectedItem.ToString())))
                {
                    if (MessageBox.Show("Папка с названием темы уже существует. Перезаписать?", "Информация", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                    {
                        return;
                    }
                    Directory.Delete(Path.Combine(folder, LectionsComboBox.SelectedItem.ToString()), true);
                }

                zip.ExtractSelectedEntries("*", LectionsComboBox.SelectedItem.ToString(), folder, ExtractExistingFileAction.OverwriteSilently);
                zip.ExtractSelectedEntries("*", Path.Combine(LectionsComboBox.SelectedItem.ToString(), $"{LectionsComboBox.SelectedItem}.files"), folder, ExtractExistingFileAction.OverwriteSilently);
            }

            MessageBox.Show("Выгрузка файлов темы завершена.", "Информация");
        }

        private void ChangeImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Все (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png|BMP(*.bmp)|*.bmp|JPEG(*.jpg)|*.jpg|PNG(*.png)|*.png";
            if (open.ShowDialog().Value)
            {
                using (ZipFile zip = DataManager.OpenZip())
                {
                    string folder = Path.Combine(LectionsComboBox.SelectedItem.ToString(), "Icon");
                    zip.RemoveSelectedEntries("*", folder);
                    zip.Save();
                    zip.AddFile(open.FileName, folder);
                    zip.Save();
                    zip.ExtractSelectedEntries("*", folder, Properties.Settings.Default.TempPath);
                }
                ChangeImage(LectionsComboBox.SelectedItem.ToString());
            }
        }

        private void ChangeImage(string theme)
        {
            string folder = $"{Properties.Settings.Default.TempPath}/{theme}/Icon";
            if (Directory.Exists(folder))
            {
                string image = Directory.GetFiles(folder).FirstOrDefault();
                if (image != null)
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(image);
                    bitmap.EndInit();
                    ThemeImage.Source = bitmap;
                    return;
                }
            }

            ThemeImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/NoImage.png"));
        }

        private void LectionsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LectionsComboBox.SelectedItem != null)
            {
                ChangeImage(LectionsComboBox.SelectedItem.ToString());
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (ZipFile zip = DataManager.OpenZip())
                {
                    Regex reg = new Regex($"^{LectionsComboBox.SelectedItem}");
                    var entries = zip.Entries.ToList();
                    foreach (var entry in entries)
                    {
                        if (reg.IsMatch(entry.FileName))
                        {
                            zip.RemoveEntry(entry);
                        }
                    }
                    zip.Save();
                }

                RefreshLections();
                MessageBox.Show("Тема удалена.", "Информация");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
