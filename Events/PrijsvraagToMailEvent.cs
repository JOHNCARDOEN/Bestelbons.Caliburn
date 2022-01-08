using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons.Events
{
    public class PrijsvraagToMailEvent
    {
        public Prijsvraag PrijsvraagToMail { get; set; }
        public User CurrentUser { get; set; }
        public Leverancier Leverancier { get; set; }
        public string ProjectNumber { get; set; }

        public PrijsvraagToMailEvent(Prijsvraag prijsvraagToMail, Leverancier leverancier, User currentuser, string projectnumber)
        {
            PrijsvraagToMail = prijsvraagToMail;
            CurrentUser = currentuser;
            ProjectNumber = projectnumber;
            Leverancier = leverancier;
        }
    }
}
