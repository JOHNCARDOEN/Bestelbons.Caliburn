using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons.Events
{
    public class LeverancierChangedEvent
    {
        public Leverancier Leverancier { get; set; }

        public LeverancierChangedEvent(Leverancier leverancier)
        {
            Leverancier = leverancier;
        }
    }
}
