using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheoryOfElectrostatics
{
    public class Question
    {
        public int Type { get; set; }
        public string Quest { get; set; }
        public List<string> TrueAnswers { get; set; }
        public List<string> Answers { get; set; }
        public int Time { get; set; }
        public List<string> SelectedAnswers { get; set; }
    }
}
