using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons.Events
{
    public class LoadedProjectListEvent
    {
        public BindableCollection<Project> ProjectList { get; set; }
        public LoadedProjectListEvent(BindableCollection<Project> projectlist)
        {
            ProjectList = projectlist;
        }
    }
}
