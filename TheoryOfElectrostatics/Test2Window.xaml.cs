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
using System.Windows.Shapes;

namespace TheoryOfElectrostatics
{
    /// <summary>
    /// Логика взаимодействия для Test2Wndow.xaml
    /// </summary>
    public partial class Test2Window : Window
    {
        public Test2Window()
        {
            InitializeComponent();
            MultiAnswers multiAnswer = new MultiAnswers();
            multiAnswer.FirstAnswers = new ObservableCollection<Answer>();
            multiAnswer.FirstAnswers.Add(new Answer());
            multiAnswer.FirstAnswers.Add(new Answer());
            multiAnswer.FirstAnswers.Add(new Answer());
            multiAnswer.FirstAnswers.Add(new Answer());
            multiAnswer.SecondAnswers = new ObservableCollection<Answer>();
            multiAnswer.SecondAnswers.Add(new Answer());
            multiAnswer.SecondAnswers.Add(new Answer());
            multiAnswer.SecondAnswers.Add(new Answer());
            multiAnswer.SecondAnswers.Add(new Answer());
            multiAnswer.Comparions = new ObservableCollection<ComparionsAnswer>();
            for (int i = 0; i < multiAnswer.FirstAnswers.Count; i++)
            {
                ComparionsAnswer comparisonAnswer = new ComparionsAnswer();
                comparisonAnswer.Variants = new ObservableCollection<int>();
                comparisonAnswer.SelectedVariants = new ObservableCollection<int>();

                for (int j = 1; j <= multiAnswer.SecondAnswers.Count; j++)
                {
                    comparisonAnswer.Variants.Add(j);
                }
                multiAnswer.Comparions.Add(comparisonAnswer);
            }

            foreach (var answer in multiAnswer.FirstAnswers)
            {
                MainEditMultiAnswer.FirstAnswers.Add(answer);
            }
            foreach (var answer in multiAnswer.SecondAnswers)
            {
                MainEditMultiAnswer.SecondAnswers.Add(answer);
            }
            foreach (var answer in multiAnswer.Comparions)
            {
                MainEditMultiAnswer.ComparionsAnswers.Add(answer);
            }
        }
    }
}
