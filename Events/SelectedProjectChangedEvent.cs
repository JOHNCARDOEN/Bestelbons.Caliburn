using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons.Events
{
    public class SelectedProjectChangedEvent
    {
        public Project Project { get; set; }

        public SelectedProjectChangedEvent(Project project)
        {
            Project = project; 
        }
    }
}
