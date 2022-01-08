using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons.Events
{
    public class ConvertedPrijsvraagEvent
    {
        public Prijsvraag Prijsvraag { get; set; }
        public string PrijsvraagNaam { get; set; }

        public ConvertedPrijsvraagEvent(Prijsvraag prijsvraag,string prijsraagnaam)
        {
            Prijsvraag = prijsvraag;
            PrijsvraagNaam = prijsraagnaam;
        }
    }
}
