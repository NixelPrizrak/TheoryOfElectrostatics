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
using System.Text.Json;
using System.Windows.Threading;
using Ionic.Zip;
using TheoryOfElectrostatics.Controls;
using TheoryOfElectrostatics.Classes;

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
                questions = new List<Question>();

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
                                    questions = DataManager.ShuffleList(JsonSerializer.Deserialize<List<Question>>(json)).Take(12).ToList();
                                }
                            }
                        }
                    }
                }

                DataListView.ItemsSource = questions;

                foreach (Question question in questions)
                {
                    question.Time = question.Type == 0 ? 30 : (question.Type == 1 ? 60 : (question.Type == 2 ? 90 : 150));
                    if (question.Type == 0 || question.Type == 1)
                    {
                        question.Answers = DataManager.ShuffleList(question.Answers);
                    }
                    if (question.Type == 3)
                    {
                        List<Answer> answers = DataManager.ShuffleList(question.MultiAnswer.SecondAnswers.ToList());
                        question.MultiAnswer.SecondAnswers.Clear();
                        foreach (var answer in answers)
                        {
                            question.MultiAnswer.SecondAnswers.Add(answer);
                        }

                        answers = DataManager.ShuffleList(question.MultiAnswer.FirstAnswers.ToList());
                        question.MultiAnswer.FirstAnswers.Clear();

                        List<ComparionsAnswer> comparionsAnswers = new List<ComparionsAnswer>();
                        for (int i = 0; i < answers.Count; i++)
                        {
                            question.MultiAnswer.FirstAnswers.Add(answers[i]);
                            comparionsAnswers.Add(question.MultiAnswer.Comparions[answers[i].Id - 1]);
                            for (int j = 1; j <= question.MultiAnswer.SecondAnswers.Count; j++)
                            {
                                comparionsAnswers[i].SelectedVariants.Add(new Answer() { Id = j, Check = false });
                            }
                        }
                    }
                }
                SelectQuest(0);
                selectedQuestion = 0;

                int minute = questions[selectedQuestion].Time / 60;
                int second = questions[selectedQuestion].Time % 60;
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
            switch (questions[selectedQuestion].Type)
            {
                case 0:
                case 1:
                    List<Answer> selectedAnswers = new List<Answer>();
                    List<VariantAnswer> checkAnswers = AnswersStackPanel.Children.OfType<VariantAnswer>().ToList();
                    for (int i = 0; i < checkAnswers.Count; i++)
                    {
                        if (checkAnswers[i].Check)
                        {
                            selectedAnswers.Add(questions[selectedQuestion].Answers[i]);
                        }
                    }
                    questions[selectedQuestion].SelectedAnswers = selectedAnswers;
                    break;
                case 2:
                    questions[selectedQuestion].InputAnswer = OpenAnswerTextBox.Text;
                    break;
                case 3:
                    ViewMultiAnswer.ComparionsAnswers.Clear();
                    foreach (var answer in ViewMultiAnswer.ComparionsAnswers)
                    {
                        questions[selectedQuestion].MultiAnswer.Comparions.Add(answer);
                    }
                    break;
                default:
                    break;
            }
        }

        public void SelectQuest(int idQuestion)
        {
            AnswersStackPanel.Children.Clear();
            TitleTextBlock.Text = $"Вопрос {idQuestion + 1}";
            QuestionTextBlock.Text = questions[idQuestion].Quest.Text;
            ViewMultiAnswer.FirstAnswers.Clear();
            ViewMultiAnswer.SecondAnswers.Clear();
            ViewMultiAnswer.ComparionsAnswers.Clear();

            switch (questions[idQuestion].Type)
            {
                case 0:
                    AnswersStackPanel.Visibility = Visibility.Visible;
                    OpenAnswerTextBox.Visibility = Visibility.Collapsed;
                    ViewMultiAnswer.Visibility = Visibility.Collapsed;

                    foreach (var answer in questions[idQuestion].Answers)
                    {
                        VariantAnswer radioAnswer = new VariantAnswer();
                        radioAnswer.VisibleRadioButton = true;
                        radioAnswer.Text = answer.Text;
                        if (questions[idQuestion].SelectedAnswers.Contains(answer))
                        {
                            radioAnswer.Check = true;
                        }
                        AnswersStackPanel.Children.Add(radioAnswer);
                    }
                    break;
                case 1:
                    AnswersStackPanel.Visibility = Visibility.Visible;
                    OpenAnswerTextBox.Visibility = Visibility.Collapsed;
                    ViewMultiAnswer.Visibility = Visibility.Collapsed;

                    foreach (var answer in questions[idQuestion].Answers)
                    {
                        VariantAnswer checkAnswer = new VariantAnswer();
                        checkAnswer.VisibleCheckBox = true;
                        checkAnswer.Text = answer.Text;
                        if (questions[idQuestion].SelectedAnswers.Contains(answer))
                        {
                            checkAnswer.Check = true;
                        }
                        AnswersStackPanel.Children.Add(checkAnswer);
                    }
                    break;
                case 2:
                    AnswersStackPanel.Visibility = Visibility.Collapsed;
                    OpenAnswerTextBox.Visibility = Visibility.Visible;
                    ViewMultiAnswer.Visibility = Visibility.Collapsed;

                    OpenAnswerTextBox.Text = questions[idQuestion].InputAnswer;
                    break;
                case 3:
                    AnswersStackPanel.Visibility = Visibility.Collapsed;
                    OpenAnswerTextBox.Visibility = Visibility.Collapsed;
                    ViewMultiAnswer.Visibility = Visibility.Visible;

                    foreach (var answer in questions[idQuestion].MultiAnswer.FirstAnswers)
                    {
                        ViewMultiAnswer.FirstAnswers.Add(answer);
                    }
                    foreach (var answer in questions[idQuestion].MultiAnswer.SecondAnswers)
                    {
                        ViewMultiAnswer.SecondAnswers.Add(answer);
                    }
                    foreach (var answer in questions[idQuestion].MultiAnswer.Comparions)
                    {
                        ViewMultiAnswer.ComparionsAnswers.Add(answer);
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
            timer.Stop();
            double score = 0;
            int maxScore = 0;

            foreach (Question question in questions)
            {
                switch (question.Type)
                {
                    case 0:
                    case 1:
                    case 2:
                        maxScore++;
                        break;
                    case 3:
                        maxScore += 3;
                        break;
                    default:
                        break;
                }
                switch (question.Type)
                {
                    case 0:
                        if (question.SelectedAnswers[0].Check)
                        {
                            score++;
                        }
                        break;
                    case 1:
                        double some = 0;
                        int trueCount = 0;

                        foreach (var selectedAnswer in question.SelectedAnswers)
                        {
                            if (selectedAnswer.Check)
                            {
                                some++;
                            }
                            else
                            {
                                some--;
                            }
                        }
                        foreach (var answer in question.Answers)
                        {
                            if (answer.Check)
                            {
                                trueCount++;
                            }
                        }

                        if (some > 0)
                        {
                            score += some / trueCount;
                        }
                        break;
                    case 2:
                        if (question.InputAnswer == question.Answers[0].Text)
                        {
                            score++;
                        }
                        break;
                    case 3:
                        double trueComparions = 0;
                        foreach (var comparion in question.MultiAnswer.Comparions)
                        {
                            int countVariants = comparion.Variants.Count;
                            int trueVariants = 0;
                            foreach (var variant in comparion.Variants)
                            {
                                foreach (var selectedVariant in comparion.SelectedVariants)
                                {
                                    if (selectedVariant.Check)
                                    {
                                        if (variant.Check)
                                        {
                                            trueVariants++;
                                        }
                                    }
                                }
                            }

                            if (countVariants != 0)
                            {
                                trueComparions += trueVariants / countVariants;
                            }
                            else
                            {
                                trueComparions++;
                            }
                        }

                        if (trueComparions > 0)
                        {
                            score += 3 * (trueComparions / question.MultiAnswer.Comparions.Count);
                        }
                        break;
                    default:
                        break;
                }
            }

            int procent = (int)Math.Ceiling((score / maxScore) * 100);
            int value = procent > 84 ? 5 : (procent > 74 ? 4 : (procent > 59 ? 3 : 2));
            MessageBox.Show($"Количество баллов {Math.Round(score, 1)}/{maxScore}. \nПравильных ответов {procent}%.\nТест пройден на оценку {value}");
            DataManager.MainFrame.Navigate(new Pages.MainPage());
        }
    }
}
