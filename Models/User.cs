using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Bestelbons.Models
{
    public class User : PropertyChangedBase
    {


        private string _firsteName;

        public string FirstName
        {
            get { return _firsteName; }
            set { _firsteName = value;
                NotifyOfPropertyChange(() => FirstName);
            }
        }

        private string _lastName;

        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value;
                NotifyOfPropertyChange(() => LastName);
            }
        }

        private string _email;

        public string Email
        {
            get { return _email; }
            set { _email = value;
                NotifyOfPropertyChange(() => Email);
            }
        }

        private string _emailAuthentication;

        public string EmailAuthentication
        {
            get { return _emailAuthentication; }
            set
            {
                _emailAuthentication = value;
                NotifyOfPropertyChange(() => EmailAuthentication);
            }
        }

        private string _tel;

        public string Tel
        {
            get { return _tel; }
            set { _tel = value;
                NotifyOfPropertyChange(() => Tel);
            }
        }

        private string _handtekening;

        public string Handtekening
        {
            get { return _handtekening; }
            set { _handtekening = value;
                NotifyOfPropertyChange(() => Handtekening);
            }
        }

        private bool  _canApprove;

        public bool  CanApprove
        {
            get { return _canApprove; }
            set
            {
                _canApprove = value;
                NotifyOfPropertyChange(() => CanApprove);
            }
        }

        private string _macAdress;

        public string MACAdress
        {
            get { return _macAdress; }
            set
            {
                _macAdress = value;
                NotifyOfPropertyChange(() => MACAdress);
            }
        }

        private bool _macAdressOK;

        public bool MACAdressOK
        {
            get { return _macAdressOK; }
            set
            {
                _macAdressOK = value;
                NotifyOfPropertyChange(() => MACAdressOK);
            }
        }

    }
}
