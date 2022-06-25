using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TheoryOfElectrostatics.Classes;

namespace TheoryOfElectrostatics.Controls
{
    /// <summary>
    /// Логика взаимодействия для EditCheckAnswer.xaml
    /// </summary>
    public partial class EditVariantAnswer : UserControl
    {
        private string image;
        public bool Check
        {
            get
            {
                if (VisibleRadioButton)
                {
                    return MainRadioButton.IsChecked.Value;
                }
                else
                {
                    return MainCheckBox.IsChecked.Value;
                }
            }
            set => MainRadioButton.IsChecked = MainCheckBox.IsChecked = value;
        }
        public string Text { get => MainTextBox.Text; set => MainTextBox.Text = value; }
        public string Image
        {
            get => image; set
            {
                if (image != null)
                {
                    DataManager.RemoveImage(image);
                }
                ChangeImage(value);
                image = value;
            }
        }
        public bool VisibleRadioButton { get => MainRadioButton.IsVisible; set => MainRadioButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        public bool VisibleCheckBox { get => MainCheckBox.IsVisible; set => MainCheckBox.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }

        public EditVariantAnswer()
        {
            InitializeComponent();
            VisibleRadioButton = false;
            VisibleCheckBox = false;
            MainRadioButton.GroupName = "Question";
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
                if (image.Width > 100 || image.Height > 50)
                {
                    MainImage.Stretch = Stretch.Uniform;
                }
                else
                {
                    MainImage.Stretch = Stretch.None;
                }

                MainImage.Source = image;
                return;
            }

            MainImage.Stretch = Stretch.Uniform;
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
