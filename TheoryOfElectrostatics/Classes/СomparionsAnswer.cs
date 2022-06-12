using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TheoryOfElectrostatics.Classes
{
    public class ComparionsAnswer
    {
        public int Key { get; set; }
        public ObservableCollection<Answer> Variants { get; set; } = new ObservableCollection<Answer>();
        [JsonIgnore]
        public ObservableCollection<Answer> SelectedVariants { get; set; } = new ObservableCollection<Answer>();
    }
}
