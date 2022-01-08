using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons.Events
{
    public class PrijsvraagRemovedEvent
    {
        public Prijsvraag Prijsaanvraag { get; set; }

        public PrijsvraagRemovedEvent(Prijsvraag prijsaanvraag)
        {
            Prijsaanvraag = prijsaanvraag;

        }
    }
}
