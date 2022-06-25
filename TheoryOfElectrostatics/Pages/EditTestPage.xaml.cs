using Ionic.Zip;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
using System.Windows.Navigation;
using TheoryOfElectrostatics.Classes;
using TheoryOfElectrostatics.Controls;

namespace TheoryOfElectrostatics.Pages
{
    /// <summary>
    /// Логика взаимодействия для EditTestPage.xaml
    /// </summary>
    public partial class EditTestPage : Page
    {
        private double startPosition = 0;
        private double startOffset = 0;
        private List<Question> questions = new List<Question>();
        private string questImage = "";
        private int idQuestion = -1;
        private bool save = false;
        private string theme = "";

        public EditTestPage()
        {
            InitializeComponent();
            theme = DataManager.CurrentTheme;

            Dictionary<int, string> types = new Dictionary<int, string>();
            types[0] = "Выбор одного ответа";
            types[1] = "Выбор нескольких ответов";
            types[2] = "Открытый ответ";
            types[3] = "Установить соответствие";
            TypesComboBox.DisplayMemberPath = "Value";
            TypesComboBox.SelectedValuePath = "Key";
            TypesComboBox.ItemsSource = types;

            using (ZipFile zip = DataManager.OpenZip(DataManager.DataPath))
            {
                using (var stream = new MemoryStream())
                {
                    ZipEntry entry = zip[$"{DataManager.CurrentTheme}/Test.json"];
                    if (entry != null)
                    {
                        entry.Extract(stream);
                        stream.Position = 0;

                        using (var reader = new StreamReader(stream))
                        {
                            string json = reader.ReadToEnd();
                            if (json != null && json != "")
                            {
                                questions = JsonSerializer.Deserialize<List<Question>>(json);
                            }
                        }
                    }
                }
            }

            //QuestionsListView.ItemsSource = questions;

            foreach (var question in questions)
            {
                QuestionsListView.Items.Add(0);
            }

            if (questions.Count > 0)
            {
                save = false;
                QuestionsListView.SelectedIndex = 0;
                save = true;
            }
            else
            {
                QuestionScrollViewer.Visibility = Visibility.Hidden;
                ButtonsGrid.Visibility = Visibility.Hidden;
            }
        }

        private void QuestionsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveChanges(idQuestion);
            if (QuestionsListView.SelectedIndex > -1)
            {
                save = false;
                TypesComboBox.SelectedIndex = questions[QuestionsListView.SelectedIndex].Type;
                SelectQuestion();
                save = true;
            }
            idQuestion = QuestionsListView.SelectedIndex;
            QuestionScrollViewer.ScrollToTop();
        }

        private void QuestionsScrollViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPosition = e.GetPosition(this).X;
            ScrollViewer sv = sender as ScrollViewer;
            startOffset = sv.HorizontalOffset;
            sv.CaptureMouse();
        }

        private void QuestionsScrollViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as ScrollViewer).ReleaseMouseCapture();
        }

        private void QuestionsScrollViewer_MouseMove(object sender, MouseEventArgs e)
        {
            ScrollViewer sv = sender as ScrollViewer;
            if (!sv.IsMouseCaptured)
            {
                return;
            }
            double mousePosition = e.GetPosition(this).X;
            sv.ScrollToHorizontalOffset(startOffset - mousePosition + startPosition);

        }

        private void QuestionsScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer sv = sender as ScrollViewer;
            sv.ScrollToHorizontalOffset(sv.HorizontalOffset - e.Delta);
        }

        private void ChangeImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Все (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png|BMP(*.bmp)|*.bmp|JPEG(*.jpg)|*.jpg|PNG(*.png)|*.png";
            if (open.ShowDialog().Value)
            {
                DataManager.RemoveImage(questImage);
                questImage = DataManager.LoadImage(open.FileName, DataManager.ImagesPath);
                ChangeImage(questImage);
            }
        }

        private void ChangeImage(string name)
        {
            questImage = name;
            name = $"{DataManager.ImagesPath}/{name}";
            var image = DataManager.GetImage(name);


            if (image != null)
            {
                if (image.Width > 100 || image.Height > 50)
                {
                    QuestionImage.Stretch = Stretch.Uniform;
                }
                else
                {
                    QuestionImage.Stretch = Stretch.None;
                }

                QuestionImage.Source = image;
                return;
            }

            QuestionImage.Stretch = Stretch.Uniform;
            QuestionImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/NoImage.png"));
        }

        private void AddQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            Question question = new Question();
            question.Type = 0;
            questions.Add(question);
            QuestionsListView.Items.Add(0);
            QuestionScrollViewer.Visibility = Visibility.Visible;
            ButtonsGrid.Visibility = Visibility.Visible;

            QuestionsListView.SelectedIndex = questions.Count - 1;
            save = false;
            TypesComboBox.SelectedIndex = 0;
            save = true;

            HeaderScrollViewer.ScrollToRightEnd();
        }

        private void SelectQuestion()
        {
            int id = QuestionsListView.SelectedIndex;

            if (id >= 0)
            {
                QuestionTextBox.Text = questions[id].Quest.Text;
                ChangeImage(questions[id].Quest.Image);

                AnswersWrapPanel.Children.Clear();
                OpenAnswerTextBox.Text = "";
                EditMultiAnswer.FirstAnswers.Clear();
                EditMultiAnswer.SecondAnswers.Clear();
                EditMultiAnswer.ComparionsAnswers.Clear();

                Question question = questions[id];

                switch (question.Type)
                {
                    case 0:
                        AnswersGrid.Visibility = Visibility.Visible;
                        OpenAnswerGrid.Visibility = Visibility.Collapsed;
                        EditMultiAnswer.Visibility = Visibility.Collapsed;

                        foreach (var answer in questions[id].Answers)
                        {
                            EditVariantAnswer radioAnswer = new EditVariantAnswer();
                            radioAnswer.Text = answer.Text;
                            radioAnswer.Image = answer.Image;
                            radioAnswer.VisibleRadioButton = true;
                            radioAnswer.Check = answer.Check;

                            AnswersWrapPanel.Children.Add(radioAnswer);
                        }
                        break;
                    case 1:
                        AnswersGrid.Visibility = Visibility.Visible;
                        OpenAnswerGrid.Visibility = Visibility.Collapsed;
                        EditMultiAnswer.Visibility = Visibility.Collapsed;

                        foreach (var answer in questions[id].Answers)
                        {
                            EditVariantAnswer checkAnswer = new EditVariantAnswer();
                            checkAnswer.Text = answer.Text;
                            checkAnswer.Image = answer.Image;
                            checkAnswer.VisibleCheckBox = true;
                            checkAnswer.Check = answer.Check;

                            AnswersWrapPanel.Children.Add(checkAnswer);
                        }
                        break;
                    case 2:
                        AnswersGrid.Visibility = Visibility.Collapsed;
                        OpenAnswerGrid.Visibility = Visibility.Visible;
                        EditMultiAnswer.Visibility = Visibility.Collapsed;

                        if (questions[id].Answers.Count != 0)
                        {
                            OpenAnswerTextBox.Text = questions[id].Answers[0].Text;
                        }
                        break;
                    case 3:
                        AnswersGrid.Visibility = Visibility.Collapsed;
                        OpenAnswerGrid.Visibility = Visibility.Collapsed;
                        EditMultiAnswer.Visibility = Visibility.Visible;

                        foreach (var answer in question.MultiAnswer.FirstAnswers)
                        {
                            EditMultiAnswer.FirstAnswers.Add(answer);
                        }
                        foreach (var answer in question.MultiAnswer.SecondAnswers)
                        {
                            EditMultiAnswer.SecondAnswers.Add(answer);
                        }
                        foreach (var answer in question.MultiAnswer.Comparions)
                        {
                            EditMultiAnswer.ComparionsAnswers.Add(answer);
                        }

                        EditMultiAnswer.RefreshComparions();
                        break;
                    default:
                        break;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges(QuestionsListView.SelectedIndex);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int id = QuestionsListView.SelectedIndex;
            SaveChanges(id);

            DataManager.RemoveImage(questions[id].Quest.Image);
            RemoveAnswers(id, true, true);

            questions.RemoveAt(id);
            SaveChanges(-1);
            save = false;
            QuestionsListView.Items.RemoveAt(id);
            //QuestionsListView.Items.Refresh();
            if (questions.Count == 0)
            {
                QuestionScrollViewer.Visibility = Visibility.Hidden;
                ButtonsGrid.Visibility = Visibility.Hidden;
            }
            else
            {
                QuestionsListView.SelectedIndex = id == 0 ? 0 : id - 1;
            }
            save = true;
        }

        private void AddAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            if (questions[QuestionsListView.SelectedIndex].Answers.Count < 6)
            {
                SaveChanges(QuestionsListView.SelectedIndex);
                questions[QuestionsListView.SelectedIndex].Answers.Add(new Answer());
                SelectQuestion();
            }
        }

        private void DeleteImageButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeImage(null);
        }

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveChanges(QuestionsListView.SelectedIndex);
            questions[QuestionsListView.SelectedIndex].Type = TypesComboBox.SelectedIndex;
            if (save)
            {
                SelectQuestion();
            }
        }

        private void SaveChanges(int id)
        {
            if (save)
            {
                if (id >= 0 && id < questions.Count)
                {
                    questions[id].Quest.Text = QuestionTextBox.Text.Trim();
                    questions[id].Quest.Image = questImage;

                    switch (questions[id].Type)
                    {
                        case 0:
                        case 1:
                            RemoveAnswers(id, false, true);
                            questions[id].Answers = new List<Answer>();

                            int i = 0;
                            foreach (var answer in AnswersWrapPanel.Children.OfType<EditVariantAnswer>())
                            {
                                questions[id].Answers.Add(new Answer()
                                {
                                    Id = i,
                                    Text = answer.Text.Trim(),
                                    Image = answer.Image,
                                    Check = answer.Check,
                                });
                                i++;
                            }
                            break;
                        case 2:
                            RemoveAnswers(id, true, true);

                            questions[id].Answers.Add(new Answer()
                            {
                                Text = OpenAnswerTextBox.Text
                            });
                            break;
                        case 3:
                            RemoveAnswers(id, true, false);

                            questions[id].MultiAnswer.FirstAnswers = new ObservableCollection<Answer>();
                            questions[id].MultiAnswer.SecondAnswers = new ObservableCollection<Answer>();
                            questions[id].MultiAnswer.Comparions = new ObservableCollection<ComparionsAnswer>();

                            for (int j = 0; j < EditMultiAnswer.FirstAnswers.Count; j++)
                            {
                                EditMultiAnswer.FirstAnswers[j].Id = j + 1;
                                questions[id].MultiAnswer.FirstAnswers.Add(EditMultiAnswer.FirstAnswers[j]);
                            }
                            for (int j = 0; j < EditMultiAnswer.SecondAnswers.Count; j++)
                            {
                                EditMultiAnswer.SecondAnswers[j].Id = j + 1;
                                questions[id].MultiAnswer.SecondAnswers.Add(EditMultiAnswer.SecondAnswers[j]);
                            }
                            for (int j = 0; j < EditMultiAnswer.ComparionsAnswers.Count; j++)
                            {
                                EditMultiAnswer.ComparionsAnswers[j].Key = j + 1;
                                questions[id].MultiAnswer.Comparions.Add(EditMultiAnswer.ComparionsAnswers[j]);
                            }
                            break;
                        default:
                            break;
                    }
                }

                using (ZipFile zip = DataManager.OpenZip(DataManager.DataPath))
                {
                    var entry = zip[$"{DataManager.CurrentTheme}/Test.json"];
                    if (entry != null)
                    {
                        zip.RemoveEntry(entry);
                    }
                    zip.Save();

                    string json = JsonSerializer.Serialize<List<Question>>(questions);
                    zip.AddEntry($"{DataManager.CurrentTheme}/Test.json", json);
                    zip.Save();
                }

            }
        }

        private void RemoveAnswers(int id, bool variantAnswer, bool multiAnswer)
        {
            if (variantAnswer)
            {
                foreach (var answer in questions[id].Answers)
                {
                    DataManager.RemoveImage(answer.Image);
                }
                questions[id].Answers = new List<Answer>();
            }

            if (multiAnswer)
            {
                foreach (var answer in questions[id].MultiAnswer.FirstAnswers)
                {
                    DataManager.RemoveImage(answer.Image);
                }
                foreach (var answer in questions[id].MultiAnswer.SecondAnswers)
                {
                    DataManager.RemoveImage(answer.Image);
                }

                questions[id].MultiAnswer.FirstAnswers.Clear();
                questions[id].MultiAnswer.SecondAnswers.Clear();
                questions[id].MultiAnswer.Comparions.Clear();
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            string newTheme = DataManager.CurrentTheme;
            DataManager.CurrentTheme = theme;
            SaveChanges(QuestionsListView.SelectedIndex);
            DataManager.CurrentTheme = newTheme;
        }
    }
}
