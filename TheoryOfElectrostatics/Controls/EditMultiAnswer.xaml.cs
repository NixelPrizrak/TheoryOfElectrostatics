using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TheoryOfElectrostatics.Classes;

namespace TheoryOfElectrostatics.Controls
{
    /// <summary>
    /// Логика взаимодействия для EditMultiAnswers.xaml
    /// </summary>
    public partial class EditMultiAnswer : UserControl
    {
        public ObservableCollection<Answer> FirstAnswers { get; set; }
        public ObservableCollection<Answer> SecondAnswers { get; set; }
        public ObservableCollection<ComparionsAnswer> ComparionsAnswers { get; set; }
        private bool noDelete = true;

        public EditMultiAnswer()
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
                        foreach (var variant in ComparionsAnswers[id].Variants)
                        {
                            if (variant.Check)
                            {
                                comparionAnswer.Variants.Add(new Answer() { Id = variant.Id, Check = false });
                            }
                        }
                    }
                }

                ComparionsAnswers.RemoveAt(id);
            }
            else
            {
                foreach (var comparisonAnswer in ComparionsAnswers)
                {
                    var element = comparisonAnswer.Variants.Where(compar => compar.Id == SecondAnswers.Count).FirstOrDefault();
                    comparisonAnswer.Variants.Remove(element);
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

                    if (ComparionsAnswers.Count > 0)
                    {
                        foreach (var variant in ComparionsAnswers[0].Variants)
                        {
                            if (!variant.Check)
                            {
                                answer.Variants.Add(new Answer() { Id = variant.Id, Check = false });
                            }
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= SecondAnswers.Count; i++)
                        {
                            answer.Variants.Add(new Answer() { Id = i, Check = false });
                        }
                    }

                    ComparionsAnswers.Add(answer);
                }
            }
            else
            {
                if (SecondAnswers.Count < 6)
                {
                    SecondAnswers.Add(new Answer());
                    foreach (var answer in ComparionsAnswers)
                    {
                        answer.Variants.Add(new Answer() { Id = SecondAnswers.Count, Check = false });
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
                StackPanel item = ((sender as Button).Parent as Grid).Parent as StackPanel;
                TextBox name = item.FindName("ImageTextBox") as TextBox;

                DataManager.RemoveImage(name.Text);
                name.Text = DataManager.LoadImage(open.FileName, DataManager.ImagesPath);
            }
        }

        private void DeleteImageButton_Click(object sender, RoutedEventArgs e)
        {
            StackPanel item = ((sender as Button).Parent as Grid).Parent as StackPanel;
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
                if (image.Width > 400 || image.Height > 200)
                {
                    imageControl.Stretch = Stretch.Uniform;
                }
                else
                {
                    imageControl.Stretch = Stretch.None;
                }

                imageControl.Source = image;
                return;
            }

            imageControl.Stretch = Stretch.Uniform;
            imageControl.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/NoImage.png"));
        }

        private void ImageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox name = sender as TextBox;
            StackPanel item = name.Parent as StackPanel;
            Image image = item.FindName("ItemImage") as Image;
            ChangeImage(name.Text, image);
        }

        private void VariantsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (noDelete)
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
                                comparion.Variants.Add(new Answer() { Id = item.Id, Check = false });
                            }
                        }
                    }
                    foreach (Answer item in e.AddedItems)
                    {
                        foreach (var comparion in ComparionsAnswers)
                        {
                            if (comparion != comparionAnswer)
                            {
                                var element = comparion.Variants.Where(compar => compar.Id == item.Id).FirstOrDefault();
                                comparion.Variants.Remove(element);
                            }
                        }
                    }
                }
            }
        }

        public void RefreshComparions()
        {
            //for (int i = 0; i < ComparisonsListView.Items.Count; i++)
            //{
            //    var listBox2 = ComparisonsListView.Items;
            //    ListBox listBox = ((ComparisonsListView.Items.GetItemAt(i) as ListViewItem).FindName("VariantsListBox") as ListBox);
            //    for (int j = 0; j < listBox.Items.Count; j++)
            //    {
            //        if (ComparionsAnswers[i].SelectedVariants.Contains(j + 1))
            //        {
            //            (listBox.Items[j] as ListBoxItem).IsSelected = true;
            //        }
            //    }
            //}
        }

        private void ImageTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox name = sender as TextBox;
            StackPanel item = name.Parent as StackPanel;
            Image image = item.FindName("ItemImage") as Image;
            ChangeImage(name.Text, image);
        }
    }
}
