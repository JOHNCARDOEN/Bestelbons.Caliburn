using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons.Events
{
   public class SelectedPrijsvraagChangedEvent
    {
        public Prijsvraag Prijsvraag { get; set; }

        public SelectedPrijsvraagChangedEvent(Prijsvraag prijsvraag)
        {
            Prijsvraag = prijsvraag;
        }
    }
}
