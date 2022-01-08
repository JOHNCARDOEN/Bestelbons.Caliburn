using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Bestelbons.Events
{
    public class ProjectDirectoryChangedEvent
    {
        public string ProjectDirectory { get; set; }

        public ProjectDirectoryChangedEvent(string projectDirectory)
        {
            ProjectDirectory = projectDirectory;
        }
    }
}
