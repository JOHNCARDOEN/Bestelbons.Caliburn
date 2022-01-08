using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using WPF_Bestelbons.Events;
using WPF_Bestelbons.Models;
using WPF_Bestelbons.Properties;


namespace WPF_Bestelbons.ViewModels
{

    public class LeveranciersViewModel : Screen, IHandle<LeveranciersListAlteredEvent>, IHandle<LoadedLeveraciersEvent>, IHandle<ClearLeverancierSelectedItemRequestEvent>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;
        private readonly IDataServiceLeveranciers _dataserviceLeveranciers;

        BindableCollection<Bestelbon> Bestelbonlist { get; set; }
        public EditLeverancierViewModel EditLeveranciersVM { get; set; }
        public DialogViewModel DialogVM { get; set; }

        BindableCollection<Leverancier> ListWithSelection = new BindableCollection<Leverancier>();

        #region CANEXECUTE

        public bool CanDelete
        {
            get { return LeveranciersSelectedItem != null; }
        }

        public bool CanAdd
        {
            get { return true; } //!AddLeverancierVM.IsActive; }
        }

        public bool CanEdit
        {
            get { return LeveranciersSelectedItem != null; }
        }

        #endregion

        #region BINDABLE FIELDS

        private Leverancier _leveranciersSeletedItem;
        public Leverancier LeveranciersSelectedItem
        {
            get { return _leveranciersSeletedItem; }
            set
            {
                _leveranciersSeletedItem = value;
                NotifyOfPropertyChange(() => LeveranciersSelectedItem);
                NotifyOfPropertyChange(() => CanDelete);
                NotifyOfPropertyChange(() => CanEdit);
                if (LeveranciersSelectedItem != null)
                {
                    if (LeveranciersSelectedItem != null) _eventAggregator.PublishOnUIThreadAsync(message: new LeverancierChangedEvent(LeveranciersSelectedItem));
                }

            }
        }



        private BindableCollection<Leverancier> _leveranciersLijst;

        public BindableCollection<Leverancier> LeveranciersLijst
        {
            get
            {
                return _leveranciersLijst;
            }
            set
            {
                _leveranciersLijst = value;
                NotifyOfPropertyChange(() => LeveranciersLijst);
            }

        }

        private BindableCollection<Leverancier> _leveranciersLijstUI;

        public BindableCollection<Leverancier> LeveranciersLijstUI
        {
            get { return _leveranciersLijstUI; }
            set
            {
                _leveranciersLijstUI = value;
                NotifyOfPropertyChange(() => LeveranciersLijstUI);
            }
        }

        private string _search;

        public string Search
        {
            get { return _search; }
            set
            {
                _search = value;
                NotifyOfPropertyChange(() => Search);
            }
        }

        #endregion

        public LeveranciersViewModel(IEventAggregator eventAggregator, IWindowManager windowsmanager, IDataServiceLeveranciers dataserviceLeveranciers)
        {
            _eventAggregator = eventAggregator;
            _windowManager = windowsmanager;
            _dataserviceLeveranciers = dataserviceLeveranciers;
            _eventAggregator.Subscribe(this);
            EditLeveranciersVM = IoC.Get<EditLeverancierViewModel>();
            DialogVM = IoC.Get<DialogViewModel>();
            this.LeveranciersLijst = new BindableCollection<Leverancier>();
            this.LeveranciersLijstUI = new BindableCollection<Leverancier>();
            this.Bestelbonlist = new BindableCollection<Bestelbon>();

        }

        public void Add()
        {
            var addLeverancierViewModel = IoC.Get<AddLeverancierViewModel>();
            addLeverancierViewModel.Capiton = "Add Leverancier";
            var result = _windowManager.ShowDialogAsync(addLeverancierViewModel);

        }

        public void Save()
        {
            var FilePath = Properties.Settings.Default.LeveranciersListPath;
            var serializer = new XmlSerializer(typeof(BindableCollection<Leverancier>));
            using (var writer = new System.IO.StreamWriter(FilePath))
            {
                serializer.Serialize(writer, LeveranciersLijst);
                writer.Flush();
                _eventAggregator.PublishOnUIThreadAsync(new LeveranciersListUpdatedEvent(LeveranciersLijst));
            }

        }

        public void Select()
        {
            ListWithSelection.Clear();
            foreach (var leverancier in LeveranciersLijst)
            {
                if (leverancier.Name.ToLower().Contains(Search.ToLower()))
                {
                    ListWithSelection.Add(leverancier);
                }


            }
            LeveranciersLijstUI = ListWithSelection;
            if (string.IsNullOrEmpty(Search)) LeveranciersLijstUI = LeveranciersLijst;

        }

        public void Delete()
        {
            DialogVM.Capiton = "Delete Leverancier";
            DialogVM.Message = $"Are you sure to delete {LeveranciersSelectedItem.Name} ?";
            DialogVM.FontsizeMessage = 18;
            DialogVM.DialogStyle = DialogStyle.YesNo;
            _windowManager.ShowDialogAsync(DialogVM);
            if (DialogVM.MyDialogResult == System.Windows.Forms.DialogResult.Yes)
            {
                LeveranciersLijst.Remove(LeveranciersSelectedItem);
                Save();
                if (LeveranciersSelectedItem != null) _eventAggregator.PublishOnUIThreadAsync(message: new LeverancierChangedEvent(new Leverancier() { Name = "" }));
            }   
        }


        public void Edit()
        {
            EditLeveranciersVM.Capiton = "Edit Leverancier";
            var result = _windowManager.ShowDialogAsync(EditLeveranciersVM);
        }


        public Task HandleAsync(LeveranciersListAlteredEvent message, CancellationToken cancellationToken)
        {
            var OldLeveranciersName = message.OldLeveraciersName;

            if (!message.Edit)
            {
                Leverancier leverancier = new Leverancier();
                leverancier = message.Leverancier.DeepCopy();
                LeveranciersLijst.Add(leverancier);
            }
            // INDIEN DE NAAM IS VERANDERD, MOETEN ALLE BONS VERANDEREN DIE DEZE LEVERANCIER HEBBEN !!
            //if (message.Edit && !string.IsNullOrEmpty(OldLeveranciersName))
            //{
            //    await LoadBestelbons();
            //    foreach (var bestelbon in Bestelbonlist)
            //    {
            //        if (bestelbon.Leverancier.Name == OldLeveranciersName)
            //        {
            //           // bestelbon.Leverancier.Name = leverancier
            //            string FilePath = $"{Properties.Settings.Default.BestelbonsPath}\\{bestelbon.Name}.abb";
            //            if (File.Exists(FilePath)) File.Delete(FilePath);

            //            using (var writer = new System.IO.StreamWriter(Properties.Settings.Default.BestelbonsPath))
            //            {
            //                var serializer = new XmlSerializer(typeof(Bestelbon));
            //                serializer.Serialize(writer, bestelbon);
            //                writer.Flush();
            //            }
            //        }

            //    }


            //}

            // OrderBy does NOT change the original Collection !! Original Collection must be recreated !!!
            LeveranciersLijst = new BindableCollection<Leverancier>(LeveranciersLijst.OrderBy(i => i.Name));
            Save();
            _eventAggregator.PublishOnUIThreadAsync(message: new LeverancierAlteredEvent(message.Leverancier));
            LeveranciersLijstUI = LeveranciersLijst;
            return Task.CompletedTask;

        }

        public void Close()
        {
            _eventAggregator.PublishOnUIThreadAsync(message: new CloseActiveItemEvent());
        }

        public void ClearFilter()
        {
            Search = "";
            Select();
        }

        public Task HandleAsync(LoadedLeveraciersEvent message, CancellationToken cancellationToken)
        {
            LeveranciersLijst = message.Leveranciers;
            LeveranciersLijst = new BindableCollection<Leverancier>(LeveranciersLijst.OrderBy(i => i.Name));
            LeveranciersLijstUI = LeveranciersLijst;
            return Task.CompletedTask;
        }

        public Task HandleAsync(ClearLeverancierSelectedItemRequestEvent message, CancellationToken cancellationToken)
        {
            LeveranciersSelectedItem = null;
            return Task.CompletedTask;
        }

        public async Task LoadBestelbons()
        {
            string FilePath = Properties.Settings.Default.BestelbonsPath;
            if (Directory.Exists(FilePath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(FilePath);
                Bestelbonlist.Clear();
                System.IO.FileInfo[] allfiles = directoryInfo.GetFiles();
                Array.Reverse(allfiles);
                var nonprojectfiles = new List<System.IO.FileInfo>();
                var files = new List<System.IO.FileInfo>();
                foreach (var fileinfo in allfiles)
                {
                    if (Char.IsLetter(fileinfo.Name[0])) nonprojectfiles.Add(fileinfo);
                    else files.Add(fileinfo);
                }
                foreach (var fileinfo in nonprojectfiles)
                {
                    files.Add(fileinfo);
                }

                foreach (var file in files)
                {
                    if (file.Extension.Contains("abb"))
                    {
                        Bestelbon bestelbon = new Bestelbon();
                        using (var stream = System.IO.File.OpenRead(file.FullName))
                        {
                            try
                            {
                                var serializer = new XmlSerializer(typeof(Bestelbon));
                                bestelbon = serializer.Deserialize(stream) as Bestelbon;
                            }
                            catch (Exception)
                            {
                                throw new NotImplementedException($"Bestelbon {Path.GetFileName(file.FullName)} not LOADED !");
                            }
                        }
                        bestelbon.DeltaDeliveryTime = (int)(bestelbon.DeliveryTime).Subtract(DateTime.Now).TotalDays;
                        Bestelbonlist.Add(bestelbon);
                    }
                }
            }
        }
    }
}
