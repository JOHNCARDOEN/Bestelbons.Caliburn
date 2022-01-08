using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons.Events
{
    public class SelectedElDocuBestelbonChangedEvent
    {
        public Bestelbon Bestelbon { get; set; }

        public SelectedElDocuBestelbonChangedEvent(Bestelbon bestelbon)
        {
            Bestelbon = bestelbon;
        }
    }
}
