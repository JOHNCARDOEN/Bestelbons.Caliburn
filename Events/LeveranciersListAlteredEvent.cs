using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons.Events
{
    public class LeveranciersListAlteredEvent
    {
        public Leverancier Leverancier { get; set; }
        public bool Edit { get; set; }
        public string OldLeveraciersName { get; set; }

        public LeveranciersListAlteredEvent(Leverancier leverancier, bool edit,string oldleveraciersName)
        {
            Leverancier = leverancier;
            Edit = edit;
            OldLeveraciersName = oldleveraciersName;
        }
    }
}
