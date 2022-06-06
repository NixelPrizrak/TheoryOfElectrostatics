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
        public TextImage Quest { get; set; }
        public List<string> TrueAnswers { get; set; }
        public List<TextImage> Answers { get; set; }
        [JsonIgnore]
        public int Time { get; set; }
        [JsonIgnore]
        public List<string> SelectedAnswers { get; set; }
    }
}
