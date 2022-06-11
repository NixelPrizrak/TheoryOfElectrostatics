using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TheoryOfElectrostatics
{
    public class Question
    {
        public int Type { get; set; }
        public Answer Quest { get; set; }
        public List<int> TrueAnswers { get; set; }
        public List<Answer> Answers { get; set; }
        [JsonIgnore]
        public int Time { get; set; }
        [JsonIgnore]
        public List<int> SelectedAnswers { get; set; }
        [JsonIgnore]
        public string InputAnswer { get; set; }
        public MultiAnswers MultiAnswer { get; set; }
    }
}
