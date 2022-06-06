using Microsoft.Win32;
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

namespace TheoryOfElectrostatics
{
    /// <summary>
    /// Логика взаимодействия для EditRadioAnswer.xaml
    /// </summary>
    public partial class EditRadioAnswer : UserControl
    {
        private string image;
        public bool Check { get => MainRadioButton.IsChecked.Value; set => MainRadioButton.IsChecked = value; }
        public string Text { get => MainTextBox.Text; set => MainTextBox.Text = value; }
        public string Image
        {
            get => image; set
            {
                ChangeImage(value);
                image = value;
            }
        }

        public EditRadioAnswer()
        {
            InitializeComponent();
        }

        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Все (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png|BMP(*.bmp)|*.bmp|JPEG(*.jpg)|*.jpg|PNG(*.png)|*.png";
            if (open.ShowDialog().Value)
            {
                Image = DataManager.LoadImage(open.FileName, DataManager.ImagesPath);
            }
        }

        private void ChangeImage(string name)
        {
            name = $"{DataManager.ImagesPath}/{name}";
            var image = DataManager.GetImage(name);

            if (image != null)
            {
                MainImage.Source = image;
                return;
            }

            MainImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/NoImage.png"));
        }

        private void DeleteImageButton_Click(object sender, RoutedEventArgs e)
        {
            Image = null;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.Parent is Panel panel)
            {
                panel.Children.Remove(this);
            }
        }
    }
}
