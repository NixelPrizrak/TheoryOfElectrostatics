using System;
using System.Windows;
using System.Windows.Controls;

namespace TheoryOfElectrostatics.Controls
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
        public Point Start
        {
            get => new Point(X1, Y1); set
            {
                X1 = value.X;
                Y1 = value.Y;
            }
        }
        public Point Finish
        {
            get => new Point(X2, Y2); set
            {
                X2 = value.X;
                Y2 = value.Y;
            }
        }
        public object this[string propertyName]
        {
            get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }

        public UnionLine()
        {
            InitializeComponent();

        }
    }
}
