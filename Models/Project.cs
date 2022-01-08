using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Bestelbons.Models
{
    public class Project: PropertyChangedBase
    {
        private int? _iD;

        public int? ID
        {
            get { return _iD; }
            set { _iD = value;
                NotifyOfPropertyChange(() => ID);
            }
        }

        private string _description;

        public string Description
        {
            get { return _description; }
            set { _description = value;
                NotifyOfPropertyChange(() => Description);
            }
        }

    }
}
