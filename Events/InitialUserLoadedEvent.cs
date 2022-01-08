using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons.Events
{
   public  class InitialUserLoadedEvent
    {
        public User user { get; set; }

        public InitialUserLoadedEvent(User message)
        {
            user = message;
        }
    }
}
