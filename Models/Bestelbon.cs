using Caliburn.Micro;
using System;
using System.ComponentModel;
using System.Diagnostics;


namespace WPF_Bestelbons.Models
{

    [DebuggerDisplay("{Name}")]
    public class Bestelbon : PropertyChangedBase
    {
        private EventHandler _totalPriceChanged;
        public event EventHandler TotalPriceChanged;
        public event EventHandler FullDelivery;

        private bool _bestelbonregelsChanged;

        public bool BestelbonregelsChanged
        {
            get { return _bestelbonregelsChanged; }
            set
            {
                _bestelbonregelsChanged = value;
                CalculateTotalPrice();
                NotifyOfPropertyChange(() => BestelbonregelsChanged);
            }
        }


        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _opmerking;

        public string Opmerking
        {
            get { return _opmerking; }
            set
            {
                _opmerking = value;
                NotifyOfPropertyChange(() => Opmerking);
            }
        }

        private string _autoAddedOpmerking;

        public string AutoAddedOpmerking
        {
            get { return _autoAddedOpmerking; }
            set { _autoAddedOpmerking = value; }
        }

        private string _leveringsvoorwaarden;
        public string Leveringsvoorwaarden
        {
            get { return _leveringsvoorwaarden; }
            set { _leveringsvoorwaarden = value; }
        }

        private Leverancier leverancier;

        public Leverancier Leverancier
        {
            get { return leverancier; }
            set { leverancier = value; }
        }

        private string _creator;

        public string Creator
        {
            get { return _creator; }
            set { _creator = value; }
        }


        private string _projectDirectory;

        public string ProjectDirectory
        {
            get { return _projectDirectory; }
            set
            {
                _projectDirectory = value;
            }
        }

        private int _deltaDeliveryTime;

        public int DeltaDeliveryTime
        {
            get { return _deltaDeliveryTime; }
            set
            {
                _deltaDeliveryTime = value;
                NotifyOfPropertyChange(() => DeltaDeliveryTime);
            }
        }

        private DateTime _deliveryTime;

        public DateTime DeliveryTime
        {
            get { return _deliveryTime; }
            set
            {
                _deliveryTime = value;
                NotifyOfPropertyChange(() => DeliveryTime);
            }
        }

        private bool _deliveryTimeExpired;

        public bool DeliveryTimeExpired
        {
            get { return _deliveryTimeExpired; }
            set
            {
                _deliveryTimeExpired = value;
                NotifyOfPropertyChange(() => DeliveryTimeExpired);
            }
        }


        private int _PercentageDelivered;

        public int PercentageDelivered
        {
            get { return _PercentageDelivered; }
            set
            {
                _PercentageDelivered = value;
                CalculateDeliveryTimeExpired();
                NotifyOfPropertyChange(() => PercentageDelivered);
            }
        }

        private bool _attachedFile;

        public bool AttachedFile
        {
            get { return _attachedFile; }
            set
            {
                _attachedFile = value;
                NotifyOfPropertyChange(() => AttachedFile);
            }
        }

        private bool _notifyFullDelivery;

        public bool NotifyFullDelivery
        {
            get { return _notifyFullDelivery; }
            set
            {
                _notifyFullDelivery = value;
                NotifyOfPropertyChange(() => NotifyFullDelivery);
            }
        }

        private bool _bestelbonSuccesfullySaved;

        public bool BestelbonSuccesfullySaved
        {
            get { return _bestelbonSuccesfullySaved; }
            set
            {
                _bestelbonSuccesfullySaved = value;
                NotifyOfPropertyChange(() => BestelbonSuccesfullySaved);
            }
        }


        private bool _bestelbonSend;

        public bool BestelbonSend
        {
            get { return _bestelbonSend; }
            set
            {
                _bestelbonSend = value;
                NotifyOfPropertyChange(() => BestelbonSend);
            }
        }

        private bool _approved;

        public bool Approved
        {
            get { return _approved; }
            set
            {
                _approved = value;
                NotifyOfPropertyChange(() => Approved);
            }
        }

        private bool _askForApproval;

        public bool AskForApproval
        {
            get { return _askForApproval; }
            set
            {
                _askForApproval = value;
                NotifyOfPropertyChange(() => AskForApproval);
            }
        }


        private decimal _totalPrice;

        public decimal TotalPrice
        {
            get { return _totalPrice; }
            set
            {
                _totalPrice = value;
                NotifyOfPropertyChange(() => TotalPrice);
                //OnTotalPriceChanged?.Invoke();
            }
        }


        private BindableCollection<Bestelbonregel> _bestelbonregels;

        public BindableCollection<Bestelbonregel> Bestelbonregels
        {
            get { return _bestelbonregels; }
            set { _bestelbonregels = value; }
        }

        public Bestelbon()
        {
            Name = "Error in xml file";
            Bestelbonregels = new BindableCollection<Bestelbonregel>();
            Leverancier = new Leverancier();

        }

        public void CalculateTotalPrice()
        {
            TotalPrice = 0.0M;
            for (int i = 0; i < Bestelbonregels.Count; i++)
            {
                TotalPrice += Bestelbonregels[i].TotalePrijs;
            }
            TotalPriceChanged?.Invoke(this, EventArgs.Empty);
        }

        public void CalculatePercentageDelivered()
        {
            int aantalgeleverd = 0;
            string name = this.Name;
            foreach (var bestelregel in Bestelbonregels)
            {
                if (bestelregel.Delivered) aantalgeleverd++;
            }
            var previouspercentagedelivered = PercentageDelivered;
            PercentageDelivered = (int)Math.Truncate(((double)aantalgeleverd / (double)Bestelbonregels.Count) * 100);

            if (PercentageDelivered == 100 && previouspercentagedelivered != 100  && NotifyFullDelivery) // SEND EMAIL CREATOR
            {
                FullDelivery?.Invoke(this, EventArgs.Empty);
            }

        }

        public void CalculateDeliveryTimeExpired()
        {
            DeliveryTimeExpired = (DateTime.Now >= DeliveryTime && !(PercentageDelivered == 100));
        }

        public void BestelbonregelChanged(object sender, PropertyChangedEventArgs e)
        {

            switch (e.PropertyName)
            {
                case "Delivered":
                case "Acceptor":
                case "NotifyItemDelivered":
                case "AcceptedDate":
                    break;
                default:
                    if (BestelbonregelsChanged) BestelbonregelsChanged = false;
                    else BestelbonregelsChanged = true;
                    break;
            }

        }
    }

}
