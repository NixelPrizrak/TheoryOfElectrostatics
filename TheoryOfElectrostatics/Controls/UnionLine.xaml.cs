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
    /// Логика взаимодействия для UnionLine.xaml
    /// </summary>
    public partial class UnionLine : UserControl
    {
        public double X1 { get => FirstLine.X1; set => FirstLine.X1 = value; }
        public double Y1 { get => FirstLine.Y1; set => FirstLine.Y1 = FirstLine.Y2 = SecondLine.Y1 = value; }
        public double X2
        {
            get => FirstLine.X2; set
            {
                if (FirstLine.X2 < FirstLine.X1)
                {
                    FirstLine.X2 = value - 1;
                }
                else
                {
                    FirstLine.X2 = value + 1;
                }
                SecondLine.X1 = SecondLine.X2 = value;
            }
        }
        public double Y2 { get => SecondLine.Y2; set => SecondLine.Y2 = value; }

        public UnionLine()
        {
            InitializeComponent();
        }
    }
}
