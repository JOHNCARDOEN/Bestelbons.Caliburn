using Caliburn.Micro;
using System;
using System.Text.RegularExpressions;
using WPF_Bestelbons.Events;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons.ViewModels
{
    public class AddLeverancierViewModel : Screen

    {
        private readonly IEventAggregator _eventAggregator;

        public CCEmailLeverancier CCEmailLev { get; set; }

        #region CANEXECUTE
        public bool CanAddLeverancier
        {
            get { return (!String.IsNullOrEmpty(AddedLeverancier.Name));}
        }

        public bool CanAddCCEmail
        {
            get { return CCEmailValid && !string.IsNullOrEmpty(Alias); }
        }
        #endregion

        #region BINDABLE FIELDS

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


        private bool _emailValid;

        public bool EmailValid
        {
            get { return _emailValid; }
            set
            {
                _emailValid = value;
                NotifyOfPropertyChange(() => EmailValid);
            }
        }

        private bool _ccemailValid;

        public bool CCEmailValid
        {
            get { return _ccemailValid; }
            set
            {
                _ccemailValid = value;
                NotifyOfPropertyChange(() => CCEmailValid);
            }
        }

        private bool _addedSaved;

        public bool AddedSaved
        {
            get { return _addedSaved; }
            set
            {
                _addedSaved = value;
                NotifyOfPropertyChange(() => AddedSaved);
            }
        }

        private string _capiton;

        public string Capiton
        {
            get { return _capiton; }
            set
            {
                _capiton = value;
                NotifyOfPropertyChange(() => Capiton);
            }
        }

        private Leverancier _addedLeverancier;



        public Leverancier AddedLeverancier
        {
            get { return _addedLeverancier; }
            set
            {
                _addedLeverancier = value;
                NotifyOfPropertyChange(() => AddedLeverancier);
            }
        }

        private string _email;

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                NotifyOfPropertyChange(() => Email);
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


        #endregion


        public AddLeverancierViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            this.AddedLeverancier = new Leverancier();
            AddedSaved = false;
        }

        public void AddLeverancier()
        {
            AddedLeverancier.Name = AddedLeverancier.Name.Replace('-', '_');
            _eventAggregator.PublishOnUIThreadAsync(new LeveranciersListAlteredEvent(AddedLeverancier, false, string.Empty)); // IMPLICIT SAVED !!
            AddedSaved = true;
            AddedLeverancier = new Leverancier();
            EmailValid = false;
        }

        public void Clear()
        {
            AddedLeverancier.Name = "";
            AddedLeverancier.City = "";
            AddedLeverancier.Email = "";
            EmailValid = false;
            CCEmailValid = false;
            Alias = "";
            CCEmail = "";
            AddedLeverancier.CCEmails.Clear();
            AddedLeverancier.Number = "";
            AddedLeverancier.Postcode = "";
            AddedLeverancier.Street = "";
            AddedLeverancier.Tel = "";
            NotifyOfPropertyChange(() => AddedLeverancier);
        }

        public void OK()
        {
            TryCloseAsync();
            AddedSaved = false;
        }

        public void CloseButton()
        {
            TryCloseAsync(false);
            AddedSaved = false;
            AddedLeverancier.Email = "";
            CCEmail = "";
        }

        public void KeyUp()
        {
            NotifyOfPropertyChange(() => AddedLeverancier);
            NotifyOfPropertyChange(() => CanAddLeverancier);
            AddedSaved = false;


        }

        public void KeyUpEmail()
        {
            if (!string.IsNullOrEmpty(AddedLeverancier.Email))
            {
                EmailValid = EmailValidate(AddedLeverancier.Email);
            }
        }

        public void KeyUpCCEmail()
        {
            if (!string.IsNullOrEmpty(CCEmail))
            {
                CCEmailValid = EmailValidate(CCEmail);
                NotifyOfPropertyChange(() => CanAddCCEmail);
            }
        }

        public void AddCCEmail()
        {

            if (CCEmailValid && !string.IsNullOrEmpty(Alias))
            {
                CCEmailLeverancier NewCCEmailLev = new CCEmailLeverancier();
                NewCCEmailLev.Alias = Alias;
                NewCCEmailLev.CCEmail = CCEmail;
                AddedLeverancier.CCEmails.Add(NewCCEmailLev);
            };
            CCEmail = string.Empty;
            Alias = string.Empty;
            CCEmailValid = false;
        }

        public bool EmailValidate(string email)
        {
            Regex regex = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            Match match = regex.Match(email);
            return match.Success;
        }

    }
}
