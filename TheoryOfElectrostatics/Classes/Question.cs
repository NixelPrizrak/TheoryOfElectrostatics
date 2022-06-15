using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TheoryOfElectrostatics.Classes
{
    public class Question
    {
        public int Type { get; set; }
        public TextImage Quest { get; set; } = new TextImage();
        public List<Answer> Answers { get; set; } = new List<Answer>();
        public MultiAnswer MultiAnswer { get; set; } = new MultiAnswer();
        [JsonIgnore]
        public double Time { get; set; }
        [JsonIgnore]
        public List<Answer> SelectedAnswers { get; set; } = new List<Answer>();
        [JsonIgnore]
        public string InputAnswer { get; set; }
        [JsonIgnore]
        public bool IsTime { get; set; } = true;
    }
}
