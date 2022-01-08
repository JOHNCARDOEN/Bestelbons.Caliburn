using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Bestelbons.Models
{
    public class Bestelbonregel : PropertyChangedBase

    {
        private string _acceptor;

        public string Acceptor
        {
            get { return _acceptor; }
            set
            {
                _acceptor = value;
                NotifyOfPropertyChange(() => Acceptor);
            }
        }

        private string _acceptedDate;

        public string AcceptedDate
        {
            get { return _acceptedDate; }
            set
            {
                _acceptedDate = value;
                NotifyOfPropertyChange(() => AcceptedDate);
            }
        }



        private bool _delivered;

        public bool Delivered
        {
            get { return _delivered; }
            set
            {
                _delivered = value;
                NotifyOfPropertyChange(() => Delivered);
            }
        }

        private bool _notifyItemDelivered;

        public bool NotifyItemDelivered
        {
            get { return _notifyItemDelivered; }
            set
            {
                _notifyItemDelivered = value;
                NotifyOfPropertyChange(() => NotifyItemDelivered);
            }
        }



        private int _aantal;

        public int Aantal
        {
            get { return _aantal; }
            set
            {
                _aantal = value;
                NotifyOfPropertyChange(() => Aantal);
                //Prijsstring = Prijsstring;
                CalculateTotalPrice();
            }
        }

        private string eA;

        public string Eenheid
        {
            get { return eA; }
            set { eA = value; }
        }


        private string _bestelregel;

        public string Bestelregel
        {
            get { return _bestelregel; }
            set
            {
                _bestelregel = value;
                NotifyOfPropertyChange(Bestelregel);
            }

        }

        private string _prijsstring;

        public string Prijsstring
        {
            get { return _prijsstring; }
            set
            {
                _prijsstring = value;
                NotifyOfPropertyChange(() => Prijsstring);


                // XXX.XXX,XX   GER FORMAT

                // XXX,XXX.XX   US FORMAT

                try
                {
                    //string TempPrijsstring = value.TrimStart('.');
                    //Prijs = Decimal.Parse(TempPrijsstring.Replace(".", ","), NumberStyles.AllowDecimalPoint);

                    //Prijs = Decimal.Parse(Prijsstring.Replace(".", ","), NumberStyles.AllowDecimalPoint);


                    Prijs = Decimal.Parse(Prijsstring, new CultureInfo("de-DE"));
                    // Prijs = Decimal.Parse(Prijsstring, CultureInfo.InvariantCulture);


                }
                catch (Exception)
                {

                    Prijs = 0;
                }
            }
        }

        private decimal _prijs;

        public decimal Prijs
        {
            get { return _prijs; }
            set
            {
                _prijs = value;
                NotifyOfPropertyChange(() => Prijs);
                CalculateTotalPrice();
            }
        }

        private decimal _totalePrijs;

        public decimal TotalePrijs
        {
            get { return _totalePrijs; }
            set
            {
                _totalePrijs = value;
                NotifyOfPropertyChange(() => TotalePrijs);
            }
        }




        public void CalculateTotalPrice()
        {
            TotalePrijs = Prijs * Aantal;
        }

    }

    public enum EA
    {
        stuk,
        doos,
    }
}
