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
using System.Windows.Shapes;
using System.Text.Json;
using System.Windows.Threading;

namespace TheoryOfElectrostatics.Pages
{
    /// <summary>
    /// Логика взаимодействия для Test.xaml
    /// </summary>
    public partial class TestPage : Page
    {
        int selectedQuestion { get; set; }
        List<Question> questions { get; set; }
        DispatcherTimer timer { get; set; }

        public TestPage()
        {
            InitializeComponent();
            List<int> arr = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            DataListView.ItemsSource = arr;
            string json = File.ReadAllText($"Test{DataManager.IdTest}.json");
            questions = ShuffleList(JsonSerializer.Deserialize<List<Question>>(json)).Take(12).ToList();

            foreach (Question question in questions)
            {
                question.Time = question.Type == 0 ? 30 : (question.Type == 1 ? 60 : 90);
                if (question.Answers != null)
                {
                    question.Answers = ShuffleList(question.Answers);
                }
                question.SelectedAnswers = new List<string>();
            }
            SelectQuest(0);
            selectedQuestion = 0;

            int minute = (int)questions[selectedQuestion].Time / 60;
            int second = (int)questions[selectedQuestion].Time % 60;
            TimeLabel.Content = $"Время: {(minute < 10 ? "0" + minute : minute.ToString())}:{(second < 10 ? "0" + second : second.ToString())}";

            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(QuestTimerTick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        private void QuestTimerTick(object sender, EventArgs e)
        {
            TimerQuest();
        }

        private void TimerQuest()
        {
            if (questions[selectedQuestion].Time > 0)
            {
                int minute = (int)questions[selectedQuestion].Time / 60;
                int second = (int)questions[selectedQuestion].Time % 60;
                TimeLabel.Content = $"Время: {(minute < 10 ? "0" + minute : minute.ToString())}:{(second < 10 ? "0" + second : second.ToString())}";
                questions[selectedQuestion].Time--;
            }
            else
            {
                for (int i = 0; i < questions.Count; i++)
                {
                    if (questions[i].Time > 0)
                    {
                        ChangeQuest(i);
                        return;
                    }
                }
                FinishTest();
            }
        }

        private void SelectionQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            int idQuestion = Convert.ToInt32((sender as Button).Content) - 1;
            ChangeQuest(idQuestion);
        }

        private void ChangeQuest(int idQuestion)
        {
            timer.Stop();
            if (questions[idQuestion].Time > 0)
            {
                SaveAnswer();

                SelectQuest(idQuestion);
                selectedQuestion = idQuestion;
            }
            TimerQuest();
            timer.Start();
        }

        private void SaveAnswer()
        {
            List<string> selectedAnswers = new List<string>();
            switch (questions[selectedQuestion].Type)
            {
                case 0:
                    if (FirstRadioButton.IsChecked.Value)
                    {
                        selectedAnswers.Add(questions[selectedQuestion].Answers[0]);
                    }
                    else if (SecondRadioButton.IsChecked.Value)
                    {
                        selectedAnswers.Add(questions[selectedQuestion].Answers[1]);
                    }
                    else if (ThirdRadioButton.IsChecked.Value)
                    {
                        selectedAnswers.Add(questions[selectedQuestion].Answers[2]);
                    }
                    else if (FourthRadioButton.IsChecked.Value)
                    {
                        selectedAnswers.Add(questions[selectedQuestion].Answers[3]);
                    }
                    break;
                case 1:
                    if (FirstCheckBox.IsChecked.Value)
                    {
                        selectedAnswers.Add(questions[selectedQuestion].Answers[0]);
                    }
                    if (SecondCheckBox.IsChecked.Value)
                    {
                        selectedAnswers.Add(questions[selectedQuestion].Answers[1]);
                    }
                    if (ThirdCheckBox.IsChecked.Value)
                    {
                        selectedAnswers.Add(questions[selectedQuestion].Answers[2]);
                    }
                    if (FourthCheckBox.IsChecked.Value)
                    {
                        selectedAnswers.Add(questions[selectedQuestion].Answers[3]);
                    }
                    break;
                case 2:
                    selectedAnswers.Add(OpenAnswerTextBox.Text);
                    break;
                default:
                    break;
            }
            questions[selectedQuestion].SelectedAnswers = selectedAnswers;
        }

        public List<T> ShuffleList<T>(List<T> list)
        {
            Random rand = new Random();

            for (int i = list.Count - 1; i >= 1; i--)
            {
                int j = rand.Next(i + 1);

                T tmp = list[j];
                list[j] = list[i];
                list[i] = tmp;
            }

            return list;
        }

        public void SelectQuest(int idQuestion)
        {
            TitleTextBlock.Text = $"Вопрос {idQuestion + 1}";
            switch (questions[idQuestion].Type)
            {
                case 0:
                    OneAnswerWrapPanel.Visibility = Visibility.Visible;
                    SomeAnswerWrapPanel.Visibility = Visibility.Hidden;
                    OpenAnswerWrapPanel.Visibility = Visibility.Hidden;
                    OneAnswerTextBlock.Text = questions[idQuestion].Quest;
                    FirstRadioButtonTextBlock.Text = questions[idQuestion].Answers[0];
                    SecondRadioButtonTextBlock.Text = questions[idQuestion].Answers[1];
                    ThirdRadioButtonTextBlock.Text = questions[idQuestion].Answers[2];
                    FourthRadioButtonTextBlock.Text = questions[idQuestion].Answers[3];
                    if (questions[idQuestion].SelectedAnswers.Contains(FirstRadioButton.Content))
                    {
                        FirstRadioButton.IsChecked = true;
                    }
                    else if (questions[idQuestion].SelectedAnswers.Contains(SecondRadioButton.Content))
                    {
                        SecondRadioButton.IsChecked = true;
                    }
                    else if (questions[idQuestion].SelectedAnswers.Contains(ThirdRadioButton.Content))
                    {
                        ThirdRadioButton.IsChecked = true;
                    }
                    else if (questions[idQuestion].SelectedAnswers.Contains(FourthRadioButton.Content))
                    {
                        FourthRadioButton.IsChecked = true;
                    }
                    else
                    {
                        FirstRadioButton.IsChecked = false;
                        SecondRadioButton.IsChecked = false;
                        ThirdRadioButton.IsChecked = false;
                        FourthRadioButton.IsChecked = false;
                    }
                    break;
                case 1:
                    OneAnswerWrapPanel.Visibility = Visibility.Hidden;
                    SomeAnswerWrapPanel.Visibility = Visibility.Visible;
                    OpenAnswerWrapPanel.Visibility = Visibility.Hidden;
                    SomeAnswerTextBlock.Text = questions[idQuestion].Quest;
                    FirstCheckBoxTextBlock.Text = questions[idQuestion].Answers[0];
                    SecondCheckBoxTextBlock.Text = questions[idQuestion].Answers[1];
                    ThirdCheckBoxTextBlock.Text = questions[idQuestion].Answers[2];
                    FourthCheckBoxTextBlock.Text = questions[idQuestion].Answers[3];

                    FirstCheckBox.IsChecked = questions[idQuestion].SelectedAnswers.Contains(FirstCheckBox.Content) ? true : false;
                    SecondCheckBox.IsChecked = questions[idQuestion].SelectedAnswers.Contains(SecondCheckBox.Content) ? true : false;
                    ThirdCheckBox.IsChecked = questions[idQuestion].SelectedAnswers.Contains(ThirdCheckBox.Content) ? true : false;
                    FourthCheckBox.IsChecked = questions[idQuestion].SelectedAnswers.Contains(FourthCheckBox.Content) ? true : false;

                    break;
                case 2:
                    OneAnswerWrapPanel.Visibility = Visibility.Hidden;
                    SomeAnswerWrapPanel.Visibility = Visibility.Hidden;
                    OpenAnswerWrapPanel.Visibility = Visibility.Visible;
                    OpenAnswerTextBlock.Text = questions[idQuestion].Quest;
                    if (questions[idQuestion].SelectedAnswers.Count > 0)
                    {
                        OpenAnswerTextBox.Text = questions[idQuestion].SelectedAnswers[0];
                    }
                    else
                    {
                        OpenAnswerTextBox.Text = "";
                    }
                    break;
                default:
                    break;
            }
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            FinishTest();
        }

        private void FinishTest()
        {
            SaveAnswer();
            double score = 0;
            foreach (Question question in questions)
            {
                if (question.SelectedAnswers.Count > 0)
                {
                    switch (question.Type)
                    {
                        case 0:
                            if (question.SelectedAnswers[0] == question.TrueAnswers[0])
                            {
                                score++;
                            }
                            break;
                        case 1:
                            double some = 0;

                            foreach (var selectedAnswer in question.SelectedAnswers)
                            {
                                if (question.TrueAnswers.Contains(selectedAnswer))
                                {
                                    some++;
                                }
                                else
                                {
                                    some--;
                                }
                            }

                            if (some > 0)
                            {
                                score += some / question.TrueAnswers.Count;
                            }
                            break;
                        case 2:
                            if (question.SelectedAnswers[0] == question.TrueAnswers[0])
                            {
                                score++;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            int procent = (int)Math.Ceiling((score / 12) * 100);
            int value = procent > 84 ? 5 : (procent > 74 ? 4 : (procent > 59 ? 3 : 2));
            MessageBox.Show($"Правильных ответов {procent}%.\nТест пройден на оценку {value}");
            DataManager.MainFrame.Navigate(new Pages.MainPage());
        }
    }
}
