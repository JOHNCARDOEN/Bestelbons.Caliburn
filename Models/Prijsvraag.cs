using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_Bestelbons.Models

{
    [DebuggerDisplay("{Name}")]
    public class Prijsvraag : PropertyChangedBase
    {

        private bool _prijsvraagregelsChanged;

        public bool PrijsvraagregelsChanged
        {
            get { return _prijsvraagregelsChanged; }
            set
            {
                _prijsvraagregelsChanged = value;
                NotifyOfPropertyChange(() => PrijsvraagregelsChanged);
            }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }

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
        private string _projectDirectory;

        public string ProjectDirectory
        {
            get { return _projectDirectory; }
            set
            {
                _projectDirectory = value;
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
        private BindableCollection<Prijsvraagregel> _prijsvraagregels;

        public BindableCollection<Prijsvraagregel> Prijsvraagregels
        {
            get { return _prijsvraagregels; }
            set { _prijsvraagregels = value; }
        }

        public Prijsvraag()
        {
            Name = "Prijsvraag  ";
            Prijsvraagregels = new BindableCollection<Prijsvraagregel>();
            Leverancier = new Leverancier();
        }

        public void PrijsvraagregelChanged(object sender, PropertyChangedEventArgs e)
        {
            string str = e.PropertyName;
            if (e.PropertyName != "Delivered")
            {
                if (PrijsvraagregelsChanged) PrijsvraagregelsChanged = false;
                else PrijsvraagregelsChanged = true;
            }

        }
    }
}
