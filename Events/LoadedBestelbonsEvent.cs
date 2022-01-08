using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons.Events
{
    public class LoadedBestelbonsEvent
    {
        public BindableCollection<Bestelbon> Bestelbonlist { get; set; }
        public LoadedBestelbonsEvent(BindableCollection<Bestelbon> loadedbestelbonlist)
        {
            Bestelbonlist = loadedbestelbonlist;
        }
    }
}
