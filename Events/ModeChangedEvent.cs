using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Bestelbons.Events
{
    public class ModeChangedEvent
    {
        public bool ModeBestelbon { get; set; }
        public ModeChangedEvent(bool message)
        {
            ModeBestelbon = message;

        }
    }
}
