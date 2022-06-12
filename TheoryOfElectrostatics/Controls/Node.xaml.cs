using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace TheoryOfElectrostatics.Controls
{
    /// <summary>
    /// Логика взаимодействия для Node.xaml
    /// </summary>
    public partial class Node : UserControl
    {
        public List<UnionLine> UnionLines { get; set; }

        public Node()
        {
            InitializeComponent();
        }
    }
}
