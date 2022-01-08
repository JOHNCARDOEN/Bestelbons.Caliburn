using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Bestelbons.Events
{
   public  class ProjectIDChangedEvent
    {
        public BindableCollection<string> ProjectIDlist { get; set; }
        public ProjectIDChangedEvent(BindableCollection<string> loadedProjectIDlist)
        {
            ProjectIDlist = loadedProjectIDlist;
        }
    }
}
