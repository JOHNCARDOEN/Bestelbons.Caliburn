using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons.Events
{
    public class LeverancierAlteredEvent
    {
        public Leverancier Leverancier { get; set; }
        public LeverancierAlteredEvent(Leverancier leverancier)
        {
            Leverancier = leverancier;
        }
    }
}
