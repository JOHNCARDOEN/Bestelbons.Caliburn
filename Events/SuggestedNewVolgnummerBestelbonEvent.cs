using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Bestelbons.Events
{
    public class SuggestedNewVolgnummerBestelbonEvent
    {
        public int SuggestedNewVolgnummer { get; set; }

        public SuggestedNewVolgnummerBestelbonEvent(int message)
        {
            SuggestedNewVolgnummer = message;
        }
    }
}
