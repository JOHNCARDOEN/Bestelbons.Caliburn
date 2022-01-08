using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Bestelbons
{
    public class DaysToExpireDateChangedEvent
    {
        public int DaysToExpireDate { get; set; }

        public DaysToExpireDateChangedEvent(int daystoexpiredate)
        {
            DaysToExpireDate = daystoexpiredate;
        }
    }
}
