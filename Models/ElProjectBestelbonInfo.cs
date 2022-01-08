using Caliburn.Micro;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Xml.Serialization;

namespace WPF_Bestelbons.Models
{
    [DebuggerDisplay("{Name}")]
    public class ElProjectBestelbonInfo : PropertyChangedBase
    {

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

        private ObservableCollection<Bestelbon> _elBetelbons;

        public  ObservableCollection<Bestelbon> ElBestelbons
        {
            get { return _elBetelbons; }
            set
            {
                _elBetelbons = value;
                NotifyOfPropertyChange(() => ElBestelbons);
            }
        }

        [XmlIgnore]
        public IList Children
        {
            get
            {

                return new CompositeCollection()
                    {
                     new CollectionContainer() { Collection = ElBestelbons }
                    };
            }
        }
    }
}
