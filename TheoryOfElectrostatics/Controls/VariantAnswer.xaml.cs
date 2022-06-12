using System;
using System.Windows;
using System.Windows.Controls;

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

        public void ViewImage(string image)
        {
            if (image != "" && image != null)
            {
                MainImage.Visibility = Visibility.Visible;

            }
            else
            {
                MainImage.Visibility = Visibility.Collapsed;
            }
        }
    }
}
