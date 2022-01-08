
using Caliburn.Micro;
using WPF_Bestelbons.Editors;
using WPF_Bestelbons.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Bestelbons.Events;
using System.IO;
using System.Xml.Serialization;

namespace WPF_Bestelbons
{

    public class LeverancierSuggestionProvider :PropertyChangedBase , ISuggestionProvider
    {

        public IEnumerable<Leverancier> ListOfLeveranciers { get; set; }

        public BindableCollection<Leverancier> BindableCollectionAllLeveranciers { get; set; }

        public Leverancier GetExactSuggestion(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) return null;
            return
                ListOfLeveranciers
                    .FirstOrDefault(lev => string.Equals(lev.Name, filter, StringComparison.CurrentCultureIgnoreCase));
        }

        public IEnumerable<Leverancier> GetSuggestions(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter)) return null;
            //  System.Threading.Thread.Sleep(100);
            return
                ListOfLeveranciers
                    .Where(lev => lev.Name.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) > -1)
                    .ToList();
        }

        IEnumerable ISuggestionProvider.GetSuggestions(string filter)
        {
            return GetSuggestions(filter);
        }

    }
}
