using Caliburn.Micro;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons.Models
{
    [DebuggerDisplay("{Name}")]
    public class Leverancier : PropertyChangedBase
    {
        [field: NonSerialized]
        public event EventHandler ItemsChanged;
  
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyOfPropertyChange(() => Name);
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
        private BindableCollection<CCEmailLeverancier> _ccEmails;

        public BindableCollection<CCEmailLeverancier> CCEmails
        {
            get { return _ccEmails; }
            set
            {
                _ccEmails = value;
                NotifyOfPropertyChange(() => CCEmails);
            }
        }

        private string _street;

        public string Street
        {
            get { return _street; }
            set
            {
                _street = value;
                NotifyOfPropertyChange(() => Street);
            }
        }

        private string _number;

        public string Number
        {
            get { return _number; }
            set
            {
                _number = value;
                NotifyOfPropertyChange(() => Number);
            }
        }
        private string _postcode;

        public string Postcode
        {
            get { return _postcode; }
            set
            {
                _postcode = value;
                NotifyOfPropertyChange(() => Postcode);
            }
        }
        private string _city;

        public string City
        {
            get { return _city; }
            set
            {
                _city = value;
                NotifyOfPropertyChange(() => City);
            }
        }
        private string _tel;

        public string Tel
        {
            get { return _tel; }
            set
            {
                _tel = value;
                NotifyOfPropertyChange(() => Tel);
            }
        }

        public Leverancier()
        {
            CCEmails = new BindableCollection<CCEmailLeverancier>();
            CCEmails.CollectionChanged += CCEmails_CollectionChanged;
        }

        void CCEmails_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (CCEmailLeverancier item in e.NewItems)
                    item.PropertyChanged += MyType_PropertyChanged;

            if (e.OldItems != null)
                foreach (CCEmailLeverancier item in e.OldItems)
                    item.PropertyChanged -= MyType_PropertyChanged;
        }

        void MyType_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ItemsChanged?.Invoke(this,e);
        }

    }
}

