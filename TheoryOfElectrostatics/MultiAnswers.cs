using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheoryOfElectrostatics
{
    public class MultiAnswers
    {
        public ObservableCollection<Answer> FirstAnswers { get; set; }
        public ObservableCollection<Answer> SecondAnswers { get; set; }
        public ObservableCollection<ComparionsAnswer> Comparions { get; set; }
    }
}
