using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace TheoryOfElectrostatics.Controls
{
    /// <summary>
    /// Логика взаимодействия для EditMultiAnswers.xaml
    /// </summary>
    public partial class EditMultiAnswers : UserControl
    {
        public ObservableCollection<Answer> FirstAnswers { get; set; }
        public ObservableCollection<Answer> SecondAnswers { get; set; }
        public ObservableCollection<ComparionsAnswer> ComparionsAnswers { get; set; }
        private bool noDelete = true;

        public EditMultiAnswers()
        {
            InitializeComponent();
            FirstAnswers = new ObservableCollection<Answer>();
            SecondAnswers = new ObservableCollection<Answer>();
            ComparionsAnswers = new ObservableCollection<ComparionsAnswer>();
            FirstListView.ItemsSource = FirstAnswers;
            SecondListView.ItemsSource = SecondAnswers;
            ComparisonsListView.ItemsSource = ComparionsAnswers;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            noDelete = false;
            Answer answer = (sender as Button).DataContext as Answer;
            DataManager.RemoveImage(answer.Image);

            if ((sender as Control).Name == "FirstDeleteItemButton")
            {
                int id = FirstAnswers.IndexOf(answer);
                FirstAnswers.Remove(answer);

                foreach (var comparionAnswer in ComparionsAnswers)
                {
                    if (comparionAnswer != ComparionsAnswers[id])
                    {
                        foreach (var variant in ComparionsAnswers[id].SelectedVariants)
                        {
                            comparionAnswer.Variants.Add(variant);
                        }
                    }
                }

                ComparionsAnswers.RemoveAt(id);
            }
            else
            {
                foreach (var comparisonAnswer in ComparionsAnswers)
                {
                    comparisonAnswer.Variants.Remove(SecondAnswers.Count);
                    comparisonAnswer.SelectedVariants.Remove(SecondAnswers.Count);
                }

                SecondAnswers.Remove(answer);
            }
            noDelete = true;
        }

        private void AddAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            noDelete = false;
            if ((sender as Control).Name == "FirstAddItemButton")
            {
                if (FirstAnswers.Count < 5)
                {
                    FirstAnswers.Add(new Answer());

                    ComparionsAnswer answer = new ComparionsAnswer();
                    answer.Variants = new ObservableCollection<int>();
                    answer.SelectedVariants = new ObservableCollection<int>();

                    if (ComparionsAnswers.Count > 0)
                    {
                        foreach (var variant in ComparionsAnswers[0].Variants)
                        {
                            answer.Variants.Add(variant);
                        }
                        foreach (var variant in ComparionsAnswers[0].SelectedVariants)
                        {
                            answer.Variants.Remove(variant);
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= SecondAnswers.Count; i++)
                        {
                            answer.Variants.Add(i);
                        }
                    }

                    ComparionsAnswers.Add(answer);
                }
            }
            else
            {
                if (SecondAnswers.Count < 10)
                {
                    SecondAnswers.Add(new Answer());
                    foreach (var answer in ComparionsAnswers)
                    {
                        answer.Variants.Add(SecondAnswers.Count);
                    }
                }
            }
            noDelete = true;
        }

        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Все (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png|BMP(*.bmp)|*.bmp|JPEG(*.jpg)|*.jpg|PNG(*.png)|*.png";
            if (open.ShowDialog().Value)
            {
                Grid item = (sender as Button).Parent as Grid;
                TextBox name = item.FindName("ImageTextBox") as TextBox;

                DataManager.RemoveImage(name.Text);
                name.Text = DataManager.LoadImage(open.FileName, DataManager.ImagesPath);
            }
        }

        private void DeleteImageButton_Click(object sender, RoutedEventArgs e)
        {
            Grid item = (sender as Button).Parent as Grid;
            TextBox name = item.FindName("ImageTextBox") as TextBox;
            DataManager.RemoveImage(name.Text);
            name.Text = null;
        }
        private void ChangeImage(string name, Image imageControl)
        {
            name = $"{DataManager.ImagesPath}/{name}";
            var image = DataManager.GetImage(name);

            if (image != null)
            {
                imageControl.Source = image;
                return;
            }

            imageControl.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/NoImage.png"));
        }

        private void ImageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox name = sender as TextBox;
            Grid item = name.Parent as Grid;
            Image image = item.FindName("ItemImage") as Image;
            ChangeImage(name.Text, image);
        }

        private void VariantsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (noDelete)
            {
                ComparionsAnswer comparionAnswer = (sender as ListBox).DataContext as ComparionsAnswer;

                foreach (int item in e.RemovedItems)
                {
                    foreach (var comparion in ComparionsAnswers)
                    {
                        if (comparion != comparionAnswer)
                        {
                            comparion.Variants.Add(item);
                        }
                    }
                    comparionAnswer.SelectedVariants.Remove(item);
                }
                foreach (int item in e.AddedItems)
                {
                    foreach (var comparion in ComparionsAnswers)
                    {
                        if (comparion != comparionAnswer)
                        {
                            comparion.Variants.Remove(item);
                        }
                    }
                    comparionAnswer.SelectedVariants.Add(item);
                }
            }
        }
    }
}
