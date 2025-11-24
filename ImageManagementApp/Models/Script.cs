using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManagementApp.Models
{
    public class Script
    {
        public ObservableCollection<Step> Steps { get; set; }

        public Script()
        {
            Steps = new ObservableCollection<Step>();
        }

        public Script DeepCopy()
        {
            var script = new Script();
            foreach (var step in Steps)
            {
                script.Steps.Add(step.Copy());
            }
            return script;
        }
    }
}
