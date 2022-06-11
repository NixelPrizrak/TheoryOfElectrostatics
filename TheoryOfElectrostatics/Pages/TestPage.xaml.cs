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

            try
            {
                List<int> arr = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
                DataListView.ItemsSource = arr;
                string json = File.ReadAllText($"Test{DataManager.IdTest}.json");
                questions = DataManager.ShuffleList(JsonSerializer.Deserialize<List<Question>>(json)).Take(12).ToList();

                foreach (Question question in questions)
                {
                    question.Time = question.Type == 0 ? 30 : (question.Type == 1 ? 60 : 90);
                    if (question.Answers != null)
                    {
                        question.Answers = DataManager.ShuffleList(question.Answers);
                    }
                    question.SelectedAnswers = new List<int>();
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            List<int> selectedAnswers = new List<int>();
            switch (questions[selectedQuestion].Type)
            {
                case 0:
                    List<RadioAnswer> radioAnswers = AnswersWrapPanel.Children.OfType<RadioAnswer>().ToList();
                    for (int i = 0; i < radioAnswers.Count; i++)
                    {
                        if (radioAnswers[i].Check)
                        {
                            selectedAnswers.Add(questions[selectedQuestion].Answers[i].Id);
                        }
                    }
                    break;
                case 1:
                    List<CheckAnswer> checkAnswers = AnswersWrapPanel.Children.OfType<CheckAnswer>().ToList();
                    for (int i = 0; i < checkAnswers.Count; i++)
                    {
                        if (checkAnswers[i].Check)
                        {
                            selectedAnswers.Add(questions[selectedQuestion].Answers[i].Id);
                        }
                    }
                    break;
                case 2:
                    questions[selectedQuestion].InputAnswer = OpenAnswerTextBox.Text;
                    break;
                default:
                    break;
            }
            questions[selectedQuestion].SelectedAnswers = selectedAnswers;
        }

        public void SelectQuest(int idQuestion)
        {
            AnswersWrapPanel.Children.Clear();
            TitleTextBlock.Text = $"Вопрос {idQuestion + 1}";
            QuestionTextBlock.Text = questions[idQuestion].Quest.Text;

            switch (questions[idQuestion].Type)
            {
                case 0:
                    AnswersWrapPanel.Visibility = Visibility.Visible;
                    OpenAnswerTextBox.Visibility = Visibility.Collapsed;
                    foreach (var answer in questions[idQuestion].Answers)
                    {
                        RadioAnswer radioAnswer = new RadioAnswer();
                        radioAnswer.Text = answer.Text;
                        if (questions[idQuestion].SelectedAnswers.Contains(answer.Id))
                        {
                            radioAnswer.Check = true;
                        }
                        AnswersWrapPanel.Children.Add(radioAnswer);
                    }
                    break;
                case 1:
                    AnswersWrapPanel.Visibility = Visibility.Visible;
                    OpenAnswerTextBox.Visibility = Visibility.Collapsed;
                    foreach (var answer in questions[idQuestion].Answers)
                    {
                        CheckAnswer checkAnswer = new CheckAnswer();
                        checkAnswer.Text = answer.Text;
                        if (questions[idQuestion].SelectedAnswers.Contains(answer.Id))
                        {
                            checkAnswer.Check = true;
                        }
                        AnswersWrapPanel.Children.Add(checkAnswer);
                    }
                    break;
                case 2:
                    AnswersWrapPanel.Visibility = Visibility.Collapsed;
                    OpenAnswerTextBox.Visibility = Visibility.Visible;
                    OpenAnswerTextBox.Text = questions[idQuestion].InputAnswer;
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
