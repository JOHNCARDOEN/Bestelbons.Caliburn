using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons
{
   public class RaiseErrorEvent
    {
        public Error Error { get; set; }

        public RaiseErrorEvent(Error error)
        {
            Error = error;
        }
    }
}
