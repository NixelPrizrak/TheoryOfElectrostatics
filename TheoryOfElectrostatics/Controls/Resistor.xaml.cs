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
    /// Логика взаимодействия для Resistor.xaml
    /// </summary>
    public partial class Resistor : UserControl
    {
        public UnionLine LeftLine { get; set; }
        public string LeftLineEnd { get; set; }
        public UnionLine RightLine { get; set; }
        public string RightLineEnd { get; set; }
        public Node LeftNode { get; set; }
        public Node RightNode { get; set; }
        public Resistor LeftResistor { get; set; }
        public Resistor RightResistor { get; set; }
        public string Title { get => TitleLabel.Content.ToString(); set => TitleLabel.Content = value; }

        public Resistor()
        {
            InitializeComponent();
        }
    }
}
