using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TheoryOfElectrostatics
{
    public class ComparionsAnswer
    {
        public int Key { get; set; }
        [JsonIgnore]
        public ObservableCollection<int> Variants { get; set; }
        public ObservableCollection<int> SelectedVariants { get; set; }
    }
}
