using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Bestelbons.ViewModels
{
    public class ProjectShellViewModel : Conductor<object>
    {
        public ProjectDirectoryViewModel ProjectDirectoryVM { get; set; }


        public ProjectShellViewModel()
        {
            ProjectDirectoryVM = IoC.Get<ProjectDirectoryViewModel>();
            ActivateItemAsync(ProjectDirectoryVM);

        }

        public void CloseButton()
        {
            TryCloseAsync();
        }

        public void Close()
        {
            TryCloseAsync();
        }
    }
}
