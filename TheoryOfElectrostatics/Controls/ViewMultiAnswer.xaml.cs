using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TheoryOfElectrostatics.Classes;

namespace TheoryOfElectrostatics.Controls
{
    /// <summary>
    /// Логика взаимодействия для ViewMultiAnswers.xaml
    /// </summary>
    public partial class ViewMultiAnswer : UserControl
    {
        public ObservableCollection<Answer> FirstAnswers { get; set; }
        public ObservableCollection<Answer> SecondAnswers { get; set; }
        public ObservableCollection<ComparionsAnswer> ComparionsAnswers { get; set; }

        public ViewMultiAnswer()
        {
            InitializeComponent();
            FirstAnswers = new ObservableCollection<Answer>();
            SecondAnswers = new ObservableCollection<Answer>();
            ComparionsAnswers = new ObservableCollection<ComparionsAnswer>();
            FirstListView.ItemsSource = FirstAnswers;
            SecondListView.ItemsSource = SecondAnswers;
            ComparisonsListView.ItemsSource = ComparionsAnswers;
        }

        private void VariantsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComparionsAnswer comparionAnswer = (sender as ListBox).DataContext as ComparionsAnswer;

            if (comparionAnswer != null)
            {
                foreach (Answer item in e.RemovedItems)
                {
                    foreach (var comparion in ComparionsAnswers)
                    {
                        if (comparion != comparionAnswer)
                        {
                            comparion.SelectedVariants.Add(new Answer() { Id = item.Id, Check = false });
                        }
                    }
                }
                foreach (Answer item in e.AddedItems)
                {
                    foreach (var comparion in ComparionsAnswers)
                    {
                        if (comparion != comparionAnswer)
                        {
                            var element = comparion.SelectedVariants.Where(compar => compar.Id == item.Id).FirstOrDefault();
                            comparion.SelectedVariants.Remove(element);
                        }
                    }
                }
            }
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            if (textBlock.Name == "MainTextBlock")
            {
                if (textBlock.Text == "")
                {
                    textBlock.Visibility = Visibility.Collapsed;
                }
                else
                {
                    textBlock.Visibility = Visibility.Visible;
                }
            }
            else
            {
                Grid grid = textBlock.Parent as Grid;
                if (textBlock.Text == "")
                {
                    grid.Visibility = Visibility.Collapsed;
                }
                else
                {
                    grid.Visibility = Visibility.Visible;
                    Answer answer = textBlock.DataContext as Answer;
                    Image imageControl = grid.FindName("ItemImage") as Image;

                    string name = $"{DataManager.ImagesPath}/{answer.Image}";
                    var image = DataManager.GetImage(name);

                    if (image.Width > 400 || image.Height > 200)
                    {
                        imageControl.Stretch = Stretch.Uniform;
                    }
                    else
                    {
                        imageControl.Stretch = Stretch.None;
                    }

                    if (image != null)
                    {
                        imageControl.Source = image;
                        return;
                    }

                    grid.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
