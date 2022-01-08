using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons
{
   public class LeveranciersListUpdatedEvent
    {
        public BindableCollection<Leverancier> Updatedleverancierslist { get; set; }

        public LeveranciersListUpdatedEvent(BindableCollection<Leverancier> LeveranciersList)
        {
            Updatedleverancierslist = LeveranciersList;
        }
    }
}
