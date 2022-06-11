using Ionic.Zip;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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

        public EditTestPage()
        {
            InitializeComponent();

            Dictionary<int, string> types = new Dictionary<int, string>();
            types[0] = "Выбор одного ответа";
            types[1] = "Выбор нескольких ответов";
            types[2] = "Открытый ответ";
            TypesComboBox.DisplayMemberPath = "Value";
            TypesComboBox.SelectedValuePath = "Key";
            TypesComboBox.ItemsSource = types;

            List<string> themes = new List<string>();
            using (ZipFile zip = DataManager.OpenZip(DataManager.LectionsPath))
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

            using (ZipFile zip = DataManager.OpenZip(DataManager.LectionsPath))
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
        }

        private void QuestionsListView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPosition = e.GetPosition(this).X;
            ScrollViewer sv = sender as ScrollViewer;
            startOffset = sv.HorizontalOffset;
            sv.CaptureMouse();
        }

        private void QuestionsListView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (sender as ScrollViewer).ReleaseMouseCapture();
        }

        private void QuestionsListView_MouseMove(object sender, MouseEventArgs e)
        {
            ScrollViewer sv = sender as ScrollViewer;
            if (!sv.IsMouseCaptured)
            {
                return;
            }
            double mousePosition = e.GetPosition(this).X;
            sv.ScrollToHorizontalOffset(startOffset - mousePosition + startPosition);

        }

        private void QuestionsListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
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
                QuestionImage.Source = image;
                return;
            }

            QuestionImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/NoImage.png"));
        }

        private void AddQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            Question question = new Question();
            question.Type = 0;
            question.Answers = new List<Answer>();
            question.TrueAnswers = new List<int>();
            question.Quest = new Answer();
            questions.Add(question);
            QuestionsListView.Items.Add(0);
            QuestionScrollViewer.Visibility = Visibility.Visible;

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
                Question question = questions[id];

                switch (question.Type)
                {
                    case 0:
                        AnswersGrid.Visibility = Visibility.Visible;
                        OpenAnswerGrid.Visibility = Visibility.Collapsed;

                        foreach (var answer in questions[id].Answers)
                        {
                            EditVariantAnswer radioAnswer = new EditVariantAnswer();
                            radioAnswer.Text = answer.Text;
                            radioAnswer.Image = answer.Image;
                            radioAnswer.VisibleRadioButton = true;
                            if (questions[id].TrueAnswers.Contains(answer.Id))
                            {
                                radioAnswer.Check = true;
                            }
                            AnswersWrapPanel.Children.Add(radioAnswer);
                        }
                        break;
                    case 1:
                        AnswersGrid.Visibility = Visibility.Visible;
                        OpenAnswerGrid.Visibility = Visibility.Collapsed;
                        foreach (var answer in questions[id].Answers)
                        {
                            EditVariantAnswer checkAnswer = new EditVariantAnswer();
                            checkAnswer.Text = answer.Text;
                            checkAnswer.Image = answer.Image;
                            checkAnswer.VisibleCheckBox = true;
                            if (questions[id].TrueAnswers.Contains(answer.Id))
                            {
                                checkAnswer.Check = true;
                            }
                            AnswersWrapPanel.Children.Add(checkAnswer);
                        }
                        break;
                    case 2:
                        AnswersGrid.Visibility = Visibility.Collapsed;
                        OpenAnswerGrid.Visibility = Visibility.Visible;
                        if (questions[id].Answers.Count != 0)
                        {
                            OpenAnswerTextBox.Text = questions[id].Answers[0].Text;
                        }
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
            foreach (var answer in questions[id].Answers)
            {
                DataManager.RemoveImage(answer.Image);
            }

            questions.RemoveAt(QuestionsListView.SelectedIndex);
            SaveChanges(-1);
            save = false;
            QuestionsListView.Items.RemoveAt(QuestionsListView.SelectedIndex);
            //QuestionsListView.Items.Refresh();
            if (questions.Count == 0)
            {
                DataManager.AdministrationFrame.Navigate(null);
            }
            else
            {
                QuestionsListView.SelectedIndex = id == 0 ? 0 : id - 1;
            }
            save = true;
        }

        private void AddAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            SaveChanges(QuestionsListView.SelectedIndex);
            questions[QuestionsListView.SelectedIndex].Answers.Add(new Answer());
            SelectQuestion();
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

                    int i;
                    switch (questions[id].Type)
                    {
                        case 0: case 1:
                            questions[id].Answers = new List<Answer>();
                            questions[id].TrueAnswers = new List<int>();

                            i = 0;
                            foreach (var answer in AnswersWrapPanel.Children.OfType<EditVariantAnswer>())
                            {
                                questions[id].Answers.Add(new Answer()
                                {
                                    Id = i,
                                    Text = answer.Text.Trim(),
                                    Image = answer.Image
                                });
                                if (answer.Check)
                                {
                                    questions[id].TrueAnswers.Add(i);
                                }
                                i++;
                            }
                            break;
                        case 2:
                            foreach (var answer in questions[id].Answers)
                            {
                                DataManager.RemoveImage(answer.Image);
                            }

                            questions[id].Answers = new List<Answer>();
                            questions[id].TrueAnswers = new List<int>();
                            questions[id].Answers.Add(new Answer()
                            {
                                Text = OpenAnswerTextBox.Text
                            });
                            break;
                        default:
                            break;
                    }
                }

                using (ZipFile zip = DataManager.OpenZip(DataManager.LectionsPath))
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

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            SaveChanges(QuestionsListView.SelectedIndex);
        }
    }
}
