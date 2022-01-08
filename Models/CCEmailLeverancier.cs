using Caliburn.Micro;
using System;

namespace WPF_Bestelbons.Models
{
    public class CCEmailLeverancier : PropertyChangedBase
    {


        private string _alias;

        public string Alias
        {
            get { return _alias; }
            set
            {
                _alias = value;
                NotifyOfPropertyChange(() => Alias);
            }
        }

        private string _ccEmail;

        public string CCEmail
        {
            get { return _ccEmail; }
            set
            {
                _ccEmail = value;
                NotifyOfPropertyChange(() => CCEmail);
            }
        }

        public CCEmailLeverancier()
        {
            Alias = string.Empty;
            CCEmail = string.Empty;
        }

    }
}
