using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TheoryOfElectrostatics.Classes;

namespace TheoryOfElectrostatics.Controls
{
    /// <summary>
    /// Логика взаимодействия для CheckAnswer.xaml
    /// </summary>
    public partial class VariantAnswer : UserControl
    {
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
        public string Text
        {
            get => MainTextBlock.Text; set
            {
                if (value != "" && value != null)
                {
                    MainTextBlock.Visibility = Visibility.Visible;
                }
                else
                {
                    MainTextBlock.Visibility = Visibility.Collapsed;
                }
                MainTextBlock.Text = value;
            }
        }
        public bool VisibleRadioButton { get => MainRadioButton.IsVisible; set => MainRadioButton.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        public bool VisibleCheckBox { get => MainCheckBox.IsVisible; set => MainCheckBox.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }

        public VariantAnswer()
        {
            InitializeComponent();
            VisibleRadioButton = false;
            VisibleCheckBox = false;
            MainRadioButton.GroupName = "Question";
        }

        public void ViewImage(string name)
        {
            if (name != "" && name != null)
            {
                MainImage.Visibility = Visibility.Visible;

                name = $"{DataManager.ImagesPath}/{name}";
                var image = DataManager.GetImage(name);
                if (image.Width > 400 || image.Height > 140)
                {
                    MainImage.Stretch = Stretch.Uniform;
                }
                else
                {
                    MainImage.Stretch = Stretch.None;
                }

                if (image != null)
                {
                    MainImage.Source = image;
                    return;
                }

                MainImage.Visibility = Visibility.Collapsed;
            }
            else
            {
                MainImage.Visibility = Visibility.Collapsed;
            }
        }
    }
}
