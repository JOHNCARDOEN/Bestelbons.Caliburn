using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Bestelbons.Models
{
   public  class Prijsvraagregel : PropertyChangedBase
    {
        private int _aantal;

        public int Aantal
        {
            get { return _aantal; }
            set
            {
                _aantal = value;
                NotifyOfPropertyChange(() => Aantal);
            }
        }

        private string eA;

        public string Eenheid
        {
            get { return eA; }
            set { eA = value;
                NotifyOfPropertyChange(() => Eenheid);
            }
        }


        private string _prijsregel;

        public string Prijsregel
        {
            get { return _prijsregel; }
            set { _prijsregel = value;
                NotifyOfPropertyChange(() => Prijsregel);
            }

        }
    }
}
