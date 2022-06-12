using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheoryOfElectrostatics.Classes
{
    public class MultiAnswer
    {
        public ObservableCollection<Answer> FirstAnswers { get; set; } = new ObservableCollection<Answer>();
        public ObservableCollection<Answer> SecondAnswers { get; set; } = new ObservableCollection<Answer>();
        public ObservableCollection<ComparionsAnswer> Comparions { get; set; } = new ObservableCollection<ComparionsAnswer>();
    }
}
