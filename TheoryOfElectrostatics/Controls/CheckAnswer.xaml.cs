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

namespace TheoryOfElectrostatics
{
    /// <summary>
    /// Логика взаимодействия для CheckAnswer.xaml
    /// </summary>
    public partial class CheckAnswer : UserControl
    {
        public bool Check { get => MainCheckBox.IsChecked.Value; set => MainCheckBox.IsChecked = value; }
        public string Text { get => MainTextBlock.Text; set => MainTextBlock.Text = value; }

        public CheckAnswer()
        {
            InitializeComponent();
        }
    }
}
