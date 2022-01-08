using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Bestelbons.Events
{
   public class SearchPrijsvragenChangedEvent
    {
        public string Search { get; set; }

        public SearchPrijsvragenChangedEvent(string message)
        {
            Search = message;
        }
    }
}
