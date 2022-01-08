using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons.Events
{
    public class BestelbonToMailEvent
    {
        public Bestelbon BestelbonToMail { get; set; }
        public User CurrentUser { get; set; }
        public Leverancier Leverancier { get; set; }
        public string ProjectNumber { get; set; }

        public BestelbonToMailEvent(Bestelbon bestelbonToMail,Leverancier leverancier, User currentuser, string projectnumber)
        {
            BestelbonToMail = bestelbonToMail;
            CurrentUser = currentuser;
            ProjectNumber = projectnumber;
            Leverancier = leverancier;
        }
    }
}
