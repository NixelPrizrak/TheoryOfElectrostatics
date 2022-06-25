using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TheoryOfElectrostatics.Classes;

namespace TheoryOfElectrostatics
{
    /// <summary>
    /// Логика взаимодействия для AdministrationWindow.xaml
    /// </summary>
    public partial class AdministrationWindow : PatternWindow
    {
        private List<string> themes = new List<string>();

        public AdministrationWindow()
        {
            InitializeComponent();

            DataManager.Edit = true;

            DataManager.AdministrationFrame = AdministrationFrame;
            DataManager.AdministrationFrame.Navigate(new Pages.EditHtmlPage(true));

            using (ZipFile zip = DataManager.OpenZip(DataManager.DataPath))
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
            }

            if (themes.Count > 0)
            {
                ThemesListView.ItemsSource = themes;
                ThemesListView.SelectedIndex = 0;
            }
        }

        private void LectionsButton_Click(object sender, RoutedEventArgs e)
        {
            DataManager.Section = 0;
            DataManager.AdministrationFrame.Navigate(new Pages.EditHtmlPage(true));
        }

        private void PracticesButton_Click(object sender, RoutedEventArgs e)
        {
            DataManager.Section = 1;
            DataManager.AdministrationFrame.Navigate(new Pages.EditHtmlPage(false));
        }

        private void TestsButton_Click(object sender, RoutedEventArgs e)
        {
            DataManager.Section = 2;
            DataManager.AdministrationFrame.Navigate(new Pages.EditTestPage());
        }

        private void AddThemeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string theme = ThemeTextBox.Text.Trim();
                if (theme.Length < 4)
                {
                    MessageBox.Show("Название темы должно состоять из 3 символов и более.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (themes.Contains(theme))
                {
                    MessageBox.Show("Данная тема уже есть.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                using (ZipFile zip = DataManager.OpenZip(DataManager.DataPath))
                {
                    zip.AddEntry($"{theme}/Lection.html", "");
                    zip.AddEntry($"{theme}/Practice.html", "");
                    zip.AddEntry($"{theme}/Test.json", "");
                    zip.Save();
                }
                themes.Add(theme);
                ThemesListView.Items.Refresh();

                ThemesListView.SelectedIndex = ThemesListView.Items.Count - 1;
                ThemeTextBox.Text = "";
                MessageBox.Show("Тема добавлена.", "Информация");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteThemeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("При удалении темы будут удалены все загруженные файлы. Удалить?", "Информация", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    using (ZipFile zip = DataManager.OpenZip(DataManager.DataPath))
                    {
                        Regex reg = new Regex($"^{DataManager.CurrentTheme}");
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

                    themes.Remove(DataManager.CurrentTheme);
                    ThemesListView.Items.Refresh();
                    ThemesListView.SelectedIndex = 0;

                    MessageBox.Show("Тема удалена.", "Информация");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ThemesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemesListView.SelectedItem != null)
            {
                DataManager.CurrentTheme = ThemesListView.SelectedItem.ToString();

                switch (DataManager.Section)
                {
                    case 0:
                        AdministrationFrame.Navigate(new Pages.EditHtmlPage(true));
                        break;
                    case 1:
                        AdministrationFrame.Navigate(new Pages.EditHtmlPage(false));
                        break;
                    case 2:
                        AdministrationFrame.Navigate(new Pages.EditTestPage());
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
