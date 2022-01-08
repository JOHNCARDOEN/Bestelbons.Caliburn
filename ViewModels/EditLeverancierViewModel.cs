using Caliburn.Micro;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WPF_Bestelbons.Events;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons.ViewModels
{
    public class EditLeverancierViewModel : Screen, IHandle<LeverancierChangedEvent>
    {
        private readonly IEventAggregator _eventAggregator;
        public CCEmailLeverancier CCEmailLev { get; set; }
        public string OldLeveranciersName { get; set; }

        #region CANEXECUTE
        public bool CanEditLeverancier
        {
            get { return !string.IsNullOrEmpty(EditedLeverancier.Name); }
        }

        public bool CanAddCCEmail
        {
            get { return CCEmailValid && !string.IsNullOrEmpty(Alias); }
        }

        #endregion

        #region BINDABLE FIELDS

        private bool _saveReminder;

        public bool SaveReminder
        {
            get { return _saveReminder; }
            set
            {
                _saveReminder = value;
                NotifyOfPropertyChange(() => SaveReminder);
            }
        }


        private string _alias;

        public string Alias
        {
            get { return _alias; }
            set
            {
                _alias = value;
                NotifyOfPropertyChange(() => Alias);
                NotifyOfPropertyChange(() => CanAddCCEmail);
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

        private bool _editSaved;

        public bool EditSaved
        {
            get { return _editSaved; }
            set
            {
                _editSaved = value;
                NotifyOfPropertyChange(() => EditSaved);
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

        private Leverancier _editedLeverancier;



        public Leverancier EditedLeverancier
        {
            get { return _editedLeverancier; }
            set
            {
                _editedLeverancier = value;
                NotifyOfPropertyChange(() => EditedLeverancier);
                NotifyOfPropertyChange(() => CanEditLeverancier);
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
        public EditLeverancierViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            EditedLeverancier = new Leverancier();

            EditSaved = false;
        }

        public void SaveEditedLeverancier()
        {
            if (OldLeveranciersName != EditedLeverancier.Name)   _eventAggregator.PublishOnUIThreadAsync(message: new LeveranciersListAlteredEvent(EditedLeverancier, true,OldLeveranciersName));
            else _eventAggregator.PublishOnUIThreadAsync(message: new LeveranciersListAlteredEvent(EditedLeverancier, true, string.Empty));

            EditSaved = true;
            SaveReminder = false;
        }

        public void OK()
        {
            TryCloseAsync();
            EditSaved = false;
            CCEmail = "";
        }

        public void CloseButton()
        {
            TryCloseAsync(false);
            EditSaved = false;
            CCEmail = "";
        }


        public void KeyUp()
        {
            EditSaved = false;
            if (!string.IsNullOrEmpty(EditedLeverancier.Email.ToString()))
            {
                EmailValid = EmailValidate(EditedLeverancier.Email);
            }


            NotifyOfPropertyChange(() => CanEditLeverancier);
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
                EditedLeverancier.CCEmails.Add(NewCCEmailLev);
            };
            CCEmail = string.Empty;
            Alias = string.Empty;
            CCEmailValid = false;
            NotifyOfPropertyChange(() => CanAddCCEmail);
        }
        public bool EmailValidate(string email)
        {
            Regex regex = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            Match match = regex.Match(email);
            return match.Success;
        }

        public void SetSaveReminder(object sender, EventArgs e)
        {
            SaveReminder = true;

        }



        #region EVENTHANDLERS

        public Task HandleAsync(LeverancierChangedEvent message, CancellationToken cancellationToken)
        {

            EditedLeverancier.CCEmails.CollectionChanged -= SetSaveReminder;
            EditedLeverancier.PropertyChanged -= SetSaveReminder;
            EditedLeverancier.ItemsChanged -= SetSaveReminder;
            EmailValid = false;
            CCEmailValid = false;
            EditedLeverancier = message.Leverancier;

            OldLeveranciersName = EditedLeverancier.Name;

            Email = EditedLeverancier.Email;
            EditedLeverancier.CCEmails.CollectionChanged += SetSaveReminder;
            EditedLeverancier.PropertyChanged += SetSaveReminder;
            EditedLeverancier.ItemsChanged += SetSaveReminder;


            if (EditedLeverancier.Email != null) EmailValid = EmailValidate(EditedLeverancier.Email);
            return Task.CompletedTask;
        }

        #endregion
    }
}
