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
        public Line LeftLine { get; set; }
        public Line RightLine { get; set; }
        public Node LeftNode { get; set; }
        public Node RightNode { get; set; }
        public Resistor LeftResistor { get; set; }
        public Resistor RightResistor { get; set; }

        public Resistor()
        {
            InitializeComponent();
        }
    }
}
