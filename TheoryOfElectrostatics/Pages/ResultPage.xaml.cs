using System;
using System.Collections.Generic;
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
using TheoryOfElectrostatics.Classes;

namespace TheoryOfElectrostatics.Pages
{
    /// <summary>
    /// Логика взаимодействия для ResultPage.xaml
    /// </summary>
    public partial class ResultPage : Page
    {
        public ResultPage(double score, double maxScore)
        {
            InitializeComponent();
            int procent = (int)Math.Ceiling((score / maxScore) * 100);
            int value = procent > 84 ? 5 : (procent > 74 ? 4 : (procent > 59 ? 3 : 2));
            TitleTextBlock.Text = $"Результат теста по теме «{DataManager.CurrentTheme}»";
            ScoreTextBlock.Text = $"● Количество баллов  {Math.Round(score, 1)}/{maxScore}";
            ProcentTextBlock.Text = $"● Правильных ответов {procent}%";
            ValueTextBlock.Text = $"● Тест пройден на оценку {value}";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            DataManager.MainFrame.Navigate(new Pages.MainPage());
        }
    }
}
