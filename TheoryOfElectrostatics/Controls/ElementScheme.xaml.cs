using System;
using System.Windows;
using System.Windows.Controls;

namespace TheoryOfElectrostatics.Controls
{
    /// <summary>
    /// Логика взаимодействия для Resistor.xaml
    /// </summary>
    public partial class ElementScheme : UserControl
    {
        private byte type = 0;
        public byte Type
        {
            get => type; set
            {
                switch (value)
                {
                    case 0:
                        Resistor.Visibility = Visibility.Visible;
                        Kondensator.Visibility = Visibility.Hidden;
                        this.Width = 160;
                        TitleLabel.Margin = new Thickness(0, 0, 0, 0);
                        TitleLabel.HorizontalAlignment = HorizontalAlignment.Center;
                        TitleLabel.VerticalAlignment = VerticalAlignment.Center;
                        TitleLabel.SetValue(Grid.ColumnProperty, 2);
                        break;
                    case 1:
                        Resistor.Visibility = Visibility.Hidden;
                        Kondensator.Visibility = Visibility.Visible;
                        this.Width = 88;
                        TitleLabel.Margin = new Thickness(0, -6, 0, 0);
                        TitleLabel.HorizontalAlignment = HorizontalAlignment.Left;
                        TitleLabel.VerticalAlignment = VerticalAlignment.Top;
                        TitleLabel.SetValue(Grid.ColumnProperty, 4);
                        break;
                    default:
                        break;
                }
                type = value;
            }
        }
        public UnionLine LeftLine { get; set; }
        public string LeftLineEnd { get; set; }
        public UnionLine RightLine { get; set; }
        public string RightLineEnd { get; set; }
        public Node LeftNode { get; set; }
        public Node RightNode { get; set; }
        public ElementScheme LeftElement { get; set; }
        public ElementScheme RightElement { get; set; }
        public string Title { get => TitleLabel.Content.ToString(); set => TitleLabel.Content = value; }

        public ElementScheme()
        {
            InitializeComponent();
        }
    }
}
