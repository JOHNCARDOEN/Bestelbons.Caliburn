using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using WPF_Bestelbons.Events;
using WPF_Bestelbons.Models;
using Exception = System.Exception;
using System.Windows.Controls;
using System.Threading;
using MimeKit;
using MimeKit.Utils;
using System.Reflection;
using MailKit.Net.Smtp;
using System.Net.Mail;
using System.Windows;
using System.Windows.Forms;
using OfficeOpenXml;
using Spire.Xls;

namespace WPF_Bestelbons.ViewModels
{
    public class BestelbonOpmaakViewModel : Caliburn.Micro.Screen, IHandle<LeverancierChangedEvent>,
        IHandle<SelectedBestelbonChangedEvent>, IHandle<UserChangedEvent>, IHandle<InitialUserLoadedEvent>,
        IHandle<ProjectDirectoryChangedEvent>, IHandle<LeveranciersListUpdatedEvent>, IHandle<LoadedLeveraciersEvent>,
        IHandle<LoadedProjectIDFileEvent>, IHandle<ProjectIDChangedEvent>, IHandle<SelectedProjectChangedEvent>, IHandle<ConvertedPrijsvraagEvent>,
        IHandle<ModeChangedEvent>, IHandle<UserListChangedEvent>, IHandle<SuggestedNewVolgnummerBestelbonEvent>, IHandle<SearchBestelbonsChangedEvent>,
        IHandle<ProjectListChangedEvent>, IHandle<LoadedProjectListEvent>, IHandle<SelectedElDocuBestelbonChangedEvent>, IHandle<ConvertElBestelbonDocuRequestEvent>

    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;
        private PDFCreatorBestelbon _pdfCreator;
        private DSNSmtpClient _mailclient;
        //private Bestelbon _bestelbon;


        string EmailBody { get; set; }

        public SendMailViewModel SendMailVM { get; set; }
        public BindableCollection<Leverancier> LeveranciersList { get; set; }
        public ObservableCollection<User> UserList { get; set; }
        public BindableCollection<Project> ProjectList { get; set; }
        BindableCollection<Bestelbon> Bestelbonlist { get; set; }
        public User CurrentUser { get; set; }
        public string ProjectDirectory { get; set; }
        public string BestelbonNameUsedForOpmerking { get; set; }
        public int DaysToExpireDate { get; set; }
        public Prijsvraag ConvertedPrijsvraag { get; set; }
        public string ConvertedPrijsvraagNaam { get; set; }
        public HashSet<int> ProjectNumbers { get; set; }
        public HashSet<string> ProjectIDs { get; set; }
        public HashSet<string> AllLeveranciers { get; set; }

        public bool ModeBestelbon { get; set; }
        public Bestelbon DeserializedBestelbon { get; set; }
        public string CopiedBestelbonNaam { get; set; }
        public bool AttachedPDFVisible { get; set; }

        public int MaxVolgnummer = 0;

        bool IsSuccesfullySaved = false;
        bool PDFSuccesfullyCreated = false;

        int PDFPage = 0;




        public string FilePath;

        #region CANEXECUTE

        public bool CanDeleteBestelregel
        {
            get { return BestelbonregelsSelectedItem != null; }
        }

        public bool CanAdd
        {
            get { return !String.IsNullOrEmpty(NewBestelregel); }
        }

        public bool CanSave
        {
            get
            {
                return (!String.IsNullOrEmpty(LeveranciersNaamUI) && (ProjectNumberOK || !String.IsNullOrEmpty(ComboboxSelectedItem)) &&
                        !String.IsNullOrEmpty(VolgNummer) && !String.IsNullOrEmpty(BestelbonNaam));
            }
        }

        public bool CanSaveMail
        {
            get
            {
                if (Leverancier.Email != null) return (!String.IsNullOrEmpty(Leverancier.Email) && CanSave);
                else return false;
            }

        }

        public bool CanSaveAttach
        {
            get
            {
                return CanSave;

            }
        }

        public bool CanCopy
        {
            get { return !String.IsNullOrEmpty(BestelbonNaam); }
        }

        #endregion

        #region BINDABLE FIELDS


        private bool _dropAreasVisible;

        public bool DropAreasVisible
        {
            get { return _dropAreasVisible; }
            set
            {
                _dropAreasVisible = value;
                NotifyOfPropertyChange(nameof(DropAreasVisible));
            }
        }

        private string _fullPathAttachementSIEMENS;

        public string FullPathAttachementSIEMENS
        {
            get { return _fullPathAttachementSIEMENS; }
            set
            {
                _fullPathAttachementSIEMENS = value;
                NotifyOfPropertyChange(() => FullPathAttachementSIEMENS);
            }
        }

        private string _fullPathAttachementFESTO;

        public string FullPathAttachementFESTO
        {
            get { return _fullPathAttachementFESTO; }
            set
            {
                _fullPathAttachementFESTO = value;
                NotifyOfPropertyChange(() => FullPathAttachementFESTO);
            }
        }

        private string _fullPathAttachementMISUMI;

        public string FullPathAttachementMISUMI
        {
            get { return _fullPathAttachementMISUMI; }
            set
            {
                _fullPathAttachementMISUMI = value;
                NotifyOfPropertyChange(() => FullPathAttachementMISUMI);
            }
        }

        private bool _ownBestelbon;

        public bool OwnBestelbon
        {
            get { return _ownBestelbon; }
            set
            {
                _ownBestelbon = value;
                NotifyOfPropertyChange(() => OwnBestelbon);
            }
        }


        private LeverancierSuggestionProvider _levProvider;

        public LeverancierSuggestionProvider LevSuggestionProvider
        {
            get { return _levProvider; }
            set
            {
                _levProvider = value;
                NotifyOfPropertyChange(() => LevSuggestionProvider);
            }
        }


        private Leverancier _leverancierUI;

        public Leverancier LeverancierUI
        {
            get { return _leverancierUI; }
            set
            {
                _leverancierUI = value;
                if (value != null & LeveranciersList != null)
                {
                    LeveranciersNaamUI = value.Name;
                    foreach (var lev in LeveranciersList)
                    {
                        if (lev.Name == LeveranciersNaamUI)
                        {
                            Leverancier = lev;

                            break;
                        }
                    }
                }

                NotifyOfPropertyChange(() => LeverancierUI);
            }
        }

        private bool _leverancierOK;

        public bool LeverancierOK
        {
            get { return _leverancierOK; }
            set
            {
                _leverancierOK = value;
                NotifyOfPropertyChange(() => LeverancierOK);
            }
        }


        private bool _projectNumberOK;

        public bool ProjectNumberOK
        {
            get { return _projectNumberOK; }
            set
            {
                _projectNumberOK = value;
                NotifyOfPropertyChange(() => ProjectNumberOK);
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

        private bool _newBestelbonBusy;

        public bool NewBestelbonBusy
        {
            get { return _newBestelbonBusy; }
            set
            {
                _newBestelbonBusy = value;
                NotifyOfPropertyChange(() => NewBestelbonBusy);
            }
        }

        private bool _newTextVisible;

        public bool NewTextVisible
        {
            get { return _newTextVisible; }
            set
            {
                _newTextVisible = value;
                NotifyOfPropertyChange(() => NewTextVisible);
            }
        }

        private bool _copyTextVisible;

        public bool CopyTextVisible
        {
            get { return _copyTextVisible; }
            set
            {
                _copyTextVisible = value;
                NotifyOfPropertyChange(() => CopyTextVisible);
            }
        }


        private BindableCollection<Bestelbonregel> _bestelbonregels;

        public BindableCollection<Bestelbonregel> Bestelbonregels
        {
            get { return _bestelbonregels; }
            set
            {
                _bestelbonregels = value;
                NotifyOfPropertyChange(() => Bestelbonregels);
            }
        }

        private Bestelbonregel _bestelbonregelSelectedItem;

        public Bestelbonregel BestelbonregelsSelectedItem
        {
            get { return _bestelbonregelSelectedItem; }
            set
            {
                _bestelbonregelSelectedItem = value;
                NotifyOfPropertyChange(() => CanDeleteBestelregel);
                NotifyOfPropertyChange(() => BestelbonregelsSelectedItem);
            }
        }

        private bool _projDirOK;

        public bool ProjDirOK
        {
            get { return _projDirOK; }
            set
            {
                _projDirOK = value;
                NotifyOfPropertyChange(() => ProjDirOK);
            }
        }

        private string _leveranciersNaamUI;

        public string LeveranciersNaamUI
        {
            get { return _leveranciersNaamUI; }
            set
            {
                LeverancierOK = false;
                _leveranciersNaamUI = value;
                if (AllLeveranciers != null)
                {
                    if (AllLeveranciers.Contains(LeveranciersNaamUI)) LeverancierOK = true;
                }

                NotifyOfPropertyChange(() => LeveranciersNaamUI);
                SetBestelbonNaam();
            }
        }

        private string _projectNumber;

        public string ProjectNumber
        {
            get { return _projectNumber; }
            set
            {
                _projectNumber = value;
                int prjnr;
                if (Int32.TryParse(_projectNumber, out prjnr))
                {
                    if (ProjectNumbers.Contains(prjnr))
                    {
                        ProjectNumberOK = true;
                        SetBestelbonNaam();
                    }

                }
                else if (!string.IsNullOrEmpty(_projectNumber))
                {
                    if (ProjectIDList.Contains(_projectNumber))
                    {
                        ProjectNumberOK = true;
                        SetBestelbonNaam();
                    }
                }
                else ProjectNumberOK = false;
                NotifyOfPropertyChange(() => ProjectNumber);
            }
        }

        private string _volgNummer;

        public string VolgNummer
        {
            get { return _volgNummer; }
            set
            {
                _volgNummer = value;
                NotifyOfPropertyChange(() => VolgNummer);
                SetBestelbonNaam();
            }
        }

        private string _autoAddedOpmerking;

        public string AutoAddedOpmerking
        {
            get { return _autoAddedOpmerking; }
            set
            {
                _autoAddedOpmerking = value;
                NotifyOfPropertyChange(() => AutoAddedOpmerking);
            }
        }


        private string _opmerking;

        public string Opmerking
        {
            get { return _opmerking; }
            set
            {
                _opmerking = value;
                Bestelbon.Opmerking = Opmerking;
                NotifyOfPropertyChange(() => Opmerking);
            }
        }

        private bool _pdfViewerVisible;

        public bool PDFViewerVisible
        {
            get { return _pdfViewerVisible; }
            set
            {
                _pdfViewerVisible = value;
                NotifyOfPropertyChange(() => PDFViewerVisible);
            }
        }

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

        private bool _prevNextPDFVisible;

        public bool PrevNextPDFVisible
        {
            get { return _prevNextPDFVisible; }
            set
            {
                _prevNextPDFVisible = value;
                NotifyOfPropertyChange(() => PrevNextPDFVisible);
            }
        }

        private BindableCollection<string> _projectIDList;

        public BindableCollection<string> ProjectIDList
        {
            get { return _projectIDList; }
            set
            {
                _projectIDList = value;
                NotifyOfPropertyChange(() => ProjectIDList);
            }
        }

        //  COMBOBOX ProjectID  vb KJELLBERG,STOCK,ATELIER.....

        private string _comboBoxSelectedItem;

        public string ComboboxSelectedItem
        {
            get { return _comboBoxSelectedItem; }
            set
            {
                _comboBoxSelectedItem = value;
                if (!String.IsNullOrEmpty(value)) ProjectNumber = "";
                NotifyOfPropertyChange(() => ComboboxSelectedItem);
                SetBestelbonNaam();
            }
        }

        private Bestelbon _bestelbon;
        public Bestelbon Bestelbon
        {
            get { return _bestelbon; }
            set
            {
                _bestelbon = value;
                NotifyOfPropertyChange(() => Bestelbon);
            }
        }


        private string _pDFFile;

        public string PDFFile
        {
            get { return _pDFFile; }
            set
            {
                _pDFFile = value;
                NotifyOfPropertyChange(() => PDFFile);
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


        private DateTime _chosenDeliveryTime;

        public DateTime ChosenDeliveryTime
        {
            get { return _chosenDeliveryTime; }
            set
            {
                _chosenDeliveryTime = value;
                NotifyOfPropertyChange(() => ChosenDeliveryTime);
            }
        }


        #endregion

        #region FULLPROPERTIES NEW BESTELREGEL

        private int _newAantal;

        public int NewAantal
        {
            get { return _newAantal; }
            set
            {
                _newAantal = value;
                BerekenTotalePrijsNewBestelregel();
                NotifyOfPropertyChange(() => NewAantal);
            }
        }

        private string _newEenheid;

        public string NewEenheid
        {
            get { return _newEenheid; }
            set
            {
                _newEenheid = value;
                NotifyOfPropertyChange(() => NewEenheid);
            }
        }

        private string _newBestelregel;

        public string NewBestelregel
        {
            get { return _newBestelregel; }
            set
            {
                _newBestelregel = value;
                NotifyOfPropertyChange(() => CanAdd);
                NotifyOfPropertyChange(() => NewBestelregel);
            }
        }


        private string _newPrijsstring;

        public string NewPrijsstring
        {
            get { return _newPrijsstring; }
            set
            {
                _newPrijsstring = value;
                NotifyOfPropertyChange(() => NewPrijsstring);

                try
                {
                    NewPrijs = Decimal.Parse(NewPrijsstring.Replace(".", ","), NumberStyles.AllowDecimalPoint);
                }
                catch (Exception)
                {

                    NewPrijs = 0;
                }
            }
        }

        private decimal _newPrijs;

        public decimal NewPrijs
        {
            get { return _newPrijs; }
            set
            {
                _newPrijs = value;
                BerekenTotalePrijsNewBestelregel();
                NotifyOfPropertyChange(() => NewPrijs);
            }
        }

        private decimal _newTotalePrijs;

        public decimal NewTotalePrijs
        {
            get { return _newTotalePrijs; }
            set
            {
                _newTotalePrijs = value;
                NotifyOfPropertyChange(() => NewTotalePrijs);
            }
        }

        #endregion

        private decimal _totaal;

        public decimal Totaal
        {
            get { return _totaal; }
            set
            {
                _totaal = value;
                NotifyOfPropertyChange(() => Totaal);
            }
        }

        private string _bestelbonNaam;

        public string BestelbonNaam
        {
            get { return _bestelbonNaam; }
            set
            {
                _bestelbonNaam = value;
                NotifyOfPropertyChange(() => BestelbonNaam);
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => CanSaveMail);
                NotifyOfPropertyChange(() => CanSaveAttach);
                NotifyOfPropertyChange(() => CanCopy);
                Bestelbon.Name = BestelbonNaam;

                if (BestelbonNaam != BestelbonNameUsedForOpmerking && !string.IsNullOrEmpty(BestelbonNameUsedForOpmerking))
                {
                    BestelbonNameUsedForOpmerking = BestelbonNaam;
                    AutoAddedOpmerking = $" Te vermelden bij communicatie : {BestelbonNaam}\n Algemene Info & Facturatie : boekhouding @astratec.be";

                }

                if ((!String.IsNullOrEmpty(LeveranciersNaamUI) && (!String.IsNullOrEmpty(ProjectNumber) || !String.IsNullOrEmpty(ComboboxSelectedItem)) && !String.IsNullOrEmpty(VolgNummer)) || !ModeBestelbon)
                {
                    _eventAggregator.PublishOnUIThreadAsync(new RaiseErrorEvent(new Error
                    { Level = ErrorLevel.Error, ErrorMessage = " Bestelbon Naam not set", Active = false }));

                    BestelbonNameUsedForOpmerking = BestelbonNaam;
                    AutoAddedOpmerking = $" Te vermelden bij communicatie : {BestelbonNaam}\n Algemene Info & Facturatie : boekhouding@astratec.be";

                }
                else
                {
                    _eventAggregator.PublishOnUIThreadAsync(new RaiseErrorEvent(new Error
                    { Level = ErrorLevel.Error, ErrorMessage = " Bestelbon Naam not set", Active = true }));
                    AutoAddedOpmerking = "";
                }
            }
        }

        public PDFCreatorBestelbon PdfCreator
        {
            get { return _pdfCreator; }
            set { _pdfCreator = value; }
        }

        private Leverancier _leverancier;
        public Leverancier Leverancier
        {
            get { return _leverancier; }
            set
            {
                _leverancier = value;
                NotifyOfPropertyChange(() => Leverancier);
                NotifyOfPropertyChange(() => CanSaveMail);
                if (Leverancier.Name == null)
                    _eventAggregator.PublishOnUIThreadAsync(new RaiseErrorEvent(new Error
                    { Level = ErrorLevel.Error, ErrorMessage = " Leverancier not set", Active = true }));
                else
                    _eventAggregator.PublishOnUIThreadAsync(new RaiseErrorEvent(new Error
                    { Level = ErrorLevel.Error, ErrorMessage = " Leverancier not set", Active = false }));
                if (Leverancier.Email != null)
                {
                    if (String.IsNullOrEmpty(Leverancier.Email))
                        _eventAggregator.PublishOnUIThreadAsync(new RaiseErrorEvent(new Error
                        { Level = ErrorLevel.Warning, ErrorMessage = " Email Leverancier not set", Active = true }));
                    else
                        _eventAggregator.PublishOnUIThreadAsync(new RaiseErrorEvent(new Error
                        { Level = ErrorLevel.Warning, ErrorMessage = " Email Leverancier not set", Active = false }));
                }
            }
        }


        public BestelbonOpmaakViewModel(IEventAggregator eventAggregator, IWindowManager windowsmanager, PDFCreatorBestelbon pDFCreator, DSNSmtpClient mailclient)
        {
            _eventAggregator = eventAggregator;
            _windowManager = windowsmanager;
            _eventAggregator.Subscribe(this);

            _mailclient = mailclient;
            SendMailVM = IoC.Get<SendMailViewModel>();
            _pdfCreator = pDFCreator;
            this.Bestelbon = new Bestelbon();
            Bestelbon.TotalPriceChanged += OnTotalPriceChanged;
            this.DeserializedBestelbon = new Bestelbon();
            Bestelbon.DeliveryTime = DateTime.Now;
            this.Leverancier = new Leverancier();
            LeveranciersNaamUI = "";
            Opmerking = "";
            NewEenheid = "stuk";
            Bestelbon.Leverancier = new Leverancier();
            Bestelbonregels = Bestelbon.Bestelbonregels;
            Totaal = Bestelbon.TotalPrice;
            NewAantal = 1;
            NewPrijsstring = "0.0";
            BestelbonNaam = "";
            this.CurrentUser = new User();
            this.ProjectIDList = new BindableCollection<string>();
            this.UserList = new ObservableCollection<User>();
            this.ProjectNumbers = new HashSet<int>();
            this.ProjectIDs = new HashSet<string>();
            this.AllLeveranciers = new HashSet<string>();
            this.LeverancierUI = new Leverancier();
            this.LevSuggestionProvider = new LeverancierSuggestionProvider();
            ChosenDeliveryTime = DateTime.Now.AddDays(14);


        }



        #region COMMANDS

        public void Add()
        {
            //EA Eenheid = (EA)Enum.Parse(typeof(EA), NewEenheid);
            Bestelbonregel AddedBestelbonregel = new Bestelbonregel()
            {
                Aantal = NewAantal,
                Eenheid = NewEenheid,
                Bestelregel = NewBestelregel,
                Prijsstring = NewPrijsstring,
                Prijs = NewPrijs,
                TotalePrijs = NewTotalePrijs
            };
            AddedBestelbonregel.PropertyChanged += Bestelbon.BestelbonregelChanged;
            Bestelbon.Bestelbonregels.Add(AddedBestelbonregel);
            Bestelbon.CalculateTotalPrice();
            Totaal = Bestelbon.TotalPrice;
            Bestelbonregels = Bestelbon.Bestelbonregels;
            NewAantal = 1;                             // SPECIAAL OP VERZOEK VAN FREDERIK !!!
        }

        public void Save()
        {

            IsSuccesfullySaved = true;
            bool NewBestelbon = true;
            Bestelbon.Name = BestelbonNaam;
            Bestelbon.Bestelbonregels = Bestelbonregels;

            foreach (var lev in LeveranciersList)
            {
                if (lev.Name == LeveranciersNaamUI)
                { Bestelbon.Leverancier = lev; }
            }
            if (!Bestelbon.Approved && !Bestelbon.AskForApproval && !Bestelbon.BestelbonSend) Bestelbon.AskForApproval = true;
            // Verhinderen dat CanApprovers hun eigen bons moet approven
            if (CurrentUser.CanApprove)
            {
                Bestelbon.AskForApproval = false;
                Bestelbon.Approved = true;
            }


            //Bestelbon.Leverancier.Name = Leverancier.Name;
            //Bestelbon.Leverancier.CCEmails = Leverancier.CCEmails;
            Bestelbon.Opmerking = Opmerking;
            Bestelbon.AutoAddedOpmerking = AutoAddedOpmerking;
            Bestelbon.ProjectDirectory = ProjectDirectory;

            if (Bestelbon.DeliveryTime.Date == DateTime.Today) Bestelbon.DeliveryTime = DateTime.Now + new TimeSpan(14, 0, 0, 0);
            Bestelbon.CalculateDeliveryTimeExpired();
            Bestelbon.CalculateTotalPrice();
            Bestelbon.CalculatePercentageDelivered();
            SaveReminder = false;

            if (Bestelbon.Bestelbonregels.Count > 0 && (!CopyTextVisible || (CopyTextVisible && (CopiedBestelbonNaam != BestelbonNaam))))
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.InitialDirectory = Properties.Settings.Default.BestelbonsPath;
                saveFileDialog1.Filter = "Astratec Bestelbons(*.abb)|*.abb|All files (*.*)|*.*";
                saveFileDialog1.Title = "Save Bestelbon";
                saveFileDialog1.FileName = Bestelbon.Name;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    CopyTextVisible = false;
                    NewTextVisible = false;
                    FilePath = saveFileDialog1.FileName;
                    string FilePathWithoutExtension = FilePath.Substring(0, FilePath.Length - 4);

                    string FilePathLevvw = Properties.Settings.Default.Leveringsvw;
                    var serializerlevvw = new XmlSerializer(typeof(String));
                    using (var stream = File.OpenRead(FilePathLevvw))
                    {
                        var other = (String)(serializerlevvw.Deserialize(stream));
                        Bestelbon.Leveringsvoorwaarden = "";
                        Bestelbon.Leveringsvoorwaarden = other;
                    }

                    if (File.Exists(FilePath)) NewBestelbon = false;


                    using (var writer = new System.IO.StreamWriter(FilePath))
                    {
                        Bestelbon.BestelbonSuccesfullySaved = true;
                        var serializer = new XmlSerializer(typeof(Bestelbon));
                        serializer.Serialize(writer, Bestelbon);
                        writer.Flush();

                    }

                    try
                    {
                        User CreatorBon = CurrentUser;
                        foreach (var user in UserList)
                        {
                            if (Bestelbon.Creator == user.LastName) CreatorBon = user;
                        }

                        if (CreatorBon != null)
                        {
                            PdfCreator.Create(Bestelbon, CreatorBon, FilePathWithoutExtension + ".pdf");
                            PDFSuccesfullyCreated = true;
                        }
                        else
                        {
                            var dialog = IoC.Get<DialogViewModel>();
                            dialog.Capiton = " Creating file and PDF";
                            dialog.Message = "Maker van de bestelbon is niet gekend !";
                            _windowManager.ShowDialogAsync(dialog);
                        }

                    }
                    catch (Exception ex)
                    {
                        var dialog = IoC.Get<DialogViewModel>();
                        dialog.Capiton = " Creating file and PDF";
                        dialog.Message = ex.Message.ToString();
                        _windowManager.ShowDialogAsync(dialog);
                        PDFSuccesfullyCreated = false;
                    }

                    if (Directory.Exists(ProjectDirectory))
                    {
                        try
                        {
                            using (var writer = new System.IO.StreamWriter(ProjectDirectory + "\\" + BestelbonNaam))
                            {
                                Bestelbon.BestelbonSuccesfullySaved = true;
                                var serializer = new XmlSerializer(typeof(Bestelbon));
                                serializer.Serialize(writer, Bestelbon);
                                writer.Flush();
                            }


                        }
                        catch (Exception e)
                        {
                            var dialogViewModel = IoC.Get<DialogViewModel>();
                            dialogViewModel.Capiton = "File Open";
                            dialogViewModel.Message = e.ToString();
                            var result = _windowManager.ShowDialogAsync(dialogViewModel);
                            IsSuccesfullySaved = false;
                        }

                        try
                        {
                            User CreatorBon = CurrentUser;
                            foreach (var user in UserList)
                            {
                                if (Bestelbon.Creator == user.LastName) CreatorBon = user;
                            }

                            string FilePathProjects = ProjectDirectory + "\\" + BestelbonNaam + ".pdf";
                            PdfCreator.Create(Bestelbon, CreatorBon, FilePathProjects);
                            PDFSuccesfullyCreated = true;
                        }
                        catch (Exception ex)
                        {
                            var dialog = IoC.Get<DialogViewModel>();
                            dialog.Capiton = " Creating file and PDF";
                            dialog.Message = ex.ToString();
                            _windowManager.ShowDialogAsync(dialog);
                            PDFSuccesfullyCreated = false;
                        }

                    }
                    if (PDFSuccesfullyCreated) ViewPDF();
                    if (NewBestelbon && IsSuccesfullySaved)
                    {
                        _eventAggregator.PublishOnUIThreadAsync(message: new BestelbonAddedEvent(Bestelbon));
                        NewBestelbonBusy = false;
                    }

                }
                if (CurrentUser.CanApprove)
                {
                    Approved = true;
                }
                else
                {
                    string subject = $"Bestelbon  {Bestelbon.Name} goed te keuren";
                    string emailbody = $"Beste, " +
                                       $" Gelieve mijn bestelbon  {Bestelbon.Name} goed te keuren" +
                                       $"\r\n\r\n\r\n\r Met vriendelijke groeten" +
                                       $"\r\n {CurrentUser.LastName}  { CurrentUser.FirstName}";


                    AskApprovalMail(subject, emailbody);
                }

            }
            else
            {
                var dialog = IoC.Get<DialogViewModel>();
                dialog.Capiton = "Save";
                if (Bestelbon.Bestelbonregels.Count == 0) dialog.Message = "Nog GEEN Bestelbonregels in Bestelbon !!" + Environment.NewLine + Environment.NewLine + "Voeg een of meer bestelregels toe  ";
                else if (CopyTextVisible && CopiedBestelbonNaam == BestelbonNaam) dialog.Message = $"Bestelbon Name NIET gewijzigd !!" + Environment.NewLine + Environment.NewLine + "Verander Projectnr of Volgnr ";
                var result = _windowManager.ShowDialogAsync(dialog);  //result = true of false from TryClose(true) or TryClose(false);
                IsSuccesfullySaved = false;
            }
            _eventAggregator.PublishOnUIThreadAsync(new ClearSearchEvent());

        }

        public void Copy()
        {
            NewBestelbonBusy = true;       // NOW POSSIBLE TO CHANGE LEVERANCIER WHEN COPYING A BESTELBON
            CopyTextVisible = true;
            CopiedBestelbonNaam = BestelbonNaam;

            Bestelbon.DeliveryTime = DateTime.Now + new TimeSpan(14, 0, 0, 0);
            Bestelbon.Creator = CurrentUser.LastName;
            Bestelbon.PercentageDelivered = 0;

            foreach (var bestelregel in Bestelbonregels)
            {
                bestelregel.Acceptor = null;
                bestelregel.AcceptedDate = null;
                bestelregel.Delivered = false;
            }

            Opmerking = string.Empty;
            Bestelbon.Opmerking = string.Empty;

            Bestelbon.Approved = false;
            Bestelbon.AskForApproval = true;
            Bestelbon.BestelbonSend = false;

        }

        public void SaveMail()
        {
            if (Bestelbon.BestelbonSuccesfullySaved)
            {
                _eventAggregator.PublishOnUIThreadAsync(new BestelbonToMailEvent(Bestelbon, Leverancier, CurrentUser, ProjectNumber));
                _windowManager.ShowWindowAsync(SendMailVM);
            }
        }

        public void SaveAttach()
        {
            string AttachedPDF;
            Save();
            if (IsSuccesfullySaved)
            {
                OpenFileDialog openfiledialog = new OpenFileDialog();
                openfiledialog.Title = "Attach PFD file";
                openfiledialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                if (openfiledialog.ShowDialog() == DialogResult.OK)

                {
                    string CopiedPDFfilename = $"{Properties.Settings.Default.BestelbonsPath}\\{BestelbonNaam}-attached.pdf";
                    if (File.Exists($"{Properties.Settings.Default.BestelbonsPath}\\{BestelbonNaam}-attached.pdf"))
                    {
                        File.Delete($"{Properties.Settings.Default.BestelbonsPath}\\{BestelbonNaam}-attached.pdf");
                    }
                    AttachedPDF = openfiledialog.FileName;
                    File.Copy(AttachedPDF, CopiedPDFfilename);
                    Bestelbon.AttachedFile = true;

                    // Saving of Attached file field !!
                    using (var writer = new System.IO.StreamWriter(FilePath))
                    {
                        var serializer = new XmlSerializer(typeof(Bestelbon));
                        serializer.Serialize(writer, Bestelbon);
                        writer.Flush();
                    }
                }
            }
        }

        public void DropAreas()
        {
            if (DropAreasVisible) DropAreasVisible = false;
            else DropAreasVisible = true;
        }

        public void NewBestelbon()
        {
            NotifyOfPropertyChange(() => LevSuggestionProvider);
            NewTextVisible = true;
            NewBestelbonBusy = true;

            FullPathAttachementSIEMENS = string.Empty;
            FullPathAttachementFESTO = string.Empty;

            Bestelbon.PropertyChanged -= BestelbonChanged;
            Bestelbon.FullDelivery -= OnNotifyFullDelivery;
            Bestelbon.Bestelbonregels.CollectionChanged -= BestelbonregelsChanged;

            Bestelbon.DeliveryTime = DateTime.Now + new TimeSpan(14, 0, 0, 0);
            _eventAggregator.PublishOnUIThreadAsync(new ClearLeverancierSelectedItemRequestEvent());
            ProjectNumber = "";
            LeveranciersNaamUI = "";

            VolgNummer = "";
            Bestelbon.Name = string.Empty;

            Opmerking = "";
            Bestelbonregels.Clear();
            Bestelbon.CalculateTotalPrice();
            Bestelbon.AttachedFile = false;
            Bestelbon.Creator = CurrentUser.LastName;
            Bestelbon.Approved = false;
            Bestelbon.BestelbonSend = false;
            Approved = false;
            NewBestelbonregel();
            OwnBestelbon = true;
            CLosePDF();

            _eventAggregator.PublishOnUIThreadAsync(message: new CloseBestelbonViewEvent());
        }

        public void NewBestelbonregel()
        {
            NewEenheid = "stuk";
            NewAantal = 1;
            NewBestelregel = "";
            NewPrijsstring = "0.0";
            NewTotalePrijs = 0.0M;
        }

        public void DeleteBestelregel()
        {
            Bestelbon.Bestelbonregels.Remove(BestelbonregelsSelectedItem);
            Bestelbon.CalculateTotalPrice();

        }

        public void SetDeliverydate(SelectionChangedEventArgs e)
        {
            ChosenDeliveryTime = (DateTime)e.AddedItems[0];
            Bestelbon.DeliveryTime = ChosenDeliveryTime;
            Bestelbon.CalculateDeliveryTimeExpired();
            string fullpath = Path.GetFullPath($"{Properties.Settings.Default.BestelbonsPath}\\{Bestelbon.Name}.abb");
            using (var writer = new System.IO.StreamWriter(fullpath))
            {
                var serializer = new XmlSerializer(typeof(Bestelbon));
                serializer.Serialize(writer, Bestelbon);
                writer.Flush();
            }
        }

        public void NotifyItemDelivered(Bestelbonregel bestelbonregel)
        {
            if (!bestelbonregel.NotifyItemDelivered) bestelbonregel.NotifyItemDelivered = true;
            else bestelbonregel.NotifyItemDelivered = false;

            string fullpath = Path.GetFullPath($"{Properties.Settings.Default.BestelbonsPath}\\{Bestelbon.Name}.abb");
            using (var writer = new System.IO.StreamWriter(fullpath))
            {
                var serializer = new XmlSerializer(typeof(Bestelbon));
                serializer.Serialize(writer, Bestelbon);
                writer.Flush();
            }

            if (bestelbonregel.NotifyItemDelivered)
            {
                string subject = $"Bestelbon  {Bestelbon.Name}";
                string emailbody = $"Beste, " +
                                   $" Uw bestelbonregel {bestelbonregel.Bestelregel} geleverd !!" +
                                   $"\r\n\r\n\r\n\r Met vriendelijke groeten" +
                                   $"\r\n {CurrentUser.LastName}  { CurrentUser.FirstName}";

                SendNotificationMail(subject, emailbody);
            }

        }

        public void Delivered(Bestelbonregel bestelbonregel)
        {
            if (!bestelbonregel.Delivered)
            {
                bestelbonregel.Delivered = true;
                bestelbonregel.Acceptor = CurrentUser.LastName;
                bestelbonregel.AcceptedDate = DateTime.Now.ToString();
                Bestelbon.AskForApproval = false;
                Bestelbon.Approved = false;
                Bestelbon.BestelbonSend = false;
            }
            else
            {
                bestelbonregel.Delivered = false;
                bestelbonregel.Acceptor = string.Empty;
                bestelbonregel.AcceptedDate = string.Empty;
            }
            Bestelbon.CalculatePercentageDelivered();

            string fullpath = Path.GetFullPath($"{Properties.Settings.Default.BestelbonsPath}\\{Bestelbon.Name}.abb");
            using (var writer = new System.IO.StreamWriter(fullpath))
            {
                var serializer = new XmlSerializer(typeof(Bestelbon));
                serializer.Serialize(writer, Bestelbon);
                writer.Flush();
            }

            _eventAggregator.PublishOnUIThreadAsync(new BestelbonDeliveredChangedEvent(Bestelbon));

        }

        public void ViewPDF()
        {
            try
            {
                if (File.Exists($"{Properties.Settings.Default.BestelbonsPath}\\{Bestelbon.Name}.pdf"))
                {
                    PDFFile = $"{Properties.Settings.Default.BestelbonsPath}\\{Bestelbon.Name}.pdf";
                    PDFViewerVisible = true;
                }
            }
            catch (Exception ex)
            {
                var dialog = IoC.Get<DialogViewModel>();
                dialog.Capiton = " Opening PDF";
                dialog.Message = ex.ToString();
                _windowManager.ShowDialogAsync(dialog);
            }

        }

        public void ViewAttachedPDF()
        {
            try
            {
                if (File.Exists($"{Properties.Settings.Default.BestelbonsPath}\\{Bestelbon.Name}-attached.pdf"))
                {
                    PDFFile = $"{Properties.Settings.Default.BestelbonsPath}\\{Bestelbon.Name}-attached.pdf";
                    PDFViewerVisible = true;
                }
            }
            catch (Exception ex)
            {
                var dialog = IoC.Get<DialogViewModel>();
                dialog.Capiton = " Opening attached PDF";
                dialog.Message = ex.ToString();
                _windowManager.ShowDialogAsync(dialog);
            }
        }

        public void CLosePDF()
        {
            PDFFile = "";
            PDFViewerVisible = false;
            PDFFile = "";
        }

        public void PrintPDF()
        {
            ProcessStartInfo info;
            if (AttachedPDFVisible) info = new ProcessStartInfo($"{Properties.Settings.Default.BestelbonsPath}\\{Bestelbon.Name}-attached.pdf");
            else info = new ProcessStartInfo($"{Properties.Settings.Default.BestelbonsPath}\\{Bestelbon.Name}.pdf");
            info.Verb = "printto";
            info.CreateNoWindow = false;
            info.UseShellExecute = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            try
            {
                Process.Start(info);
                _eventAggregator.PublishOnUIThreadAsync(new RaiseErrorEvent(new Error { Level = ErrorLevel.Error, ErrorMessage = " File to be printed not found !", Active = false }));
            }
            catch (Exception)
            {
                _eventAggregator.PublishOnUIThreadAsync(new RaiseErrorEvent(new Error { Level = ErrorLevel.Error, ErrorMessage = " File to be printed not found !", Active = true }));
            }
        }

        public void OnNotifyFullDelivery(object sender, EventArgs e)
        {
            // mail creator bestelbon fully delivered.

            string subject = $"Bestelbon  {Bestelbon.Name}";
            string emailbody = $"Beste, " +
                               $" Uw bestelbon  {Bestelbon.Name} is volledig geleverd !!" +
                               $"\r\n\r\n\r\n\r Met vriendelijke groeten" +
                               $"\r\n {CurrentUser.LastName}  { CurrentUser.FirstName}";


            SendNotificationMail(subject, emailbody);

        }

        public async void AskApprovalMail(string subject, string body)
        {
            try
            {
                var mailadress = string.Empty;
                foreach (var user in UserList)
                {
                    if ("Joost" == user.FirstName) mailadress = user.Email;
                }
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(CurrentUser.FirstName, CurrentUser.Email));
                message.To.Add(new MailboxAddress("Joost", mailadress));
                message.Subject = subject;

                var builder = new BodyBuilder();
                MimeEntity image;

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WPF_Bestelbons.Resources.LOGO_275.png"))
                {
                    image = builder.LinkedResources.Add("LOGO_275.png", stream);
                    image.ContentId = MimeUtils.GenerateMessageId();
                }


                string footer = string.Format(@"

                   <p><br></p>
                   <left><img src=""cid:{0}""></left>
                   <p>Industrielaan 19,zone C2 - B-8810 Lichtervelde<br>
                   Tel. +32 (0)51/72.24.46   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp      <a href=""http://www.astratec.be""> www.astratec.be</a> "
                     , image.ContentId);


                builder.HtmlBody = body + footer;
                message.Body = builder.ToMessageBody();

                using (_mailclient)
                {
                    try
                    {

                        await _mailclient.ConnectAsync("smtp.office365.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                        await _mailclient.AuthenticateAsync(CurrentUser.Email, CurrentUser.EmailAuthentication);

                        // Check DNS capabilities
                        var supportsDsn = _mailclient.Capabilities.HasFlag(SmtpCapabilities.Dsn);

                        await _mailclient.SendAsync(message);
                        await _mailclient.DisconnectAsync(true);

                    }
                    catch (Exception ex)
                    {
                        var dialogViewModel = IoC.Get<DialogViewModel>();
                        dialogViewModel.Capiton = "Attachment Error";
                        dialogViewModel.Message = ex.Message.ToString();
                        var result = _windowManager.ShowDialogAsync(dialogViewModel);
                    }
                }


            }
            catch (Exception e)
            {
                var dialogViewModel = IoC.Get<DialogViewModel>();
                dialogViewModel.Capiton = "Connect Error";
                dialogViewModel.Message = $"{e.Message}";
                var result = _windowManager.ShowDialogAsync(dialogViewModel);

            }
        }

        public async void SendNotificationMail(string subject, string body)
        {
            try
            {
                var mailadress = string.Empty;
                foreach (var user in UserList)
                {
                    if (Bestelbon.Creator == user.LastName) mailadress = user.Email;
                }
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(CurrentUser.FirstName, CurrentUser.Email));
                message.To.Add(new MailboxAddress(Bestelbon.Creator, mailadress));
                message.Subject = subject;

                var builder = new BodyBuilder();
                MimeEntity image;

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WPF_Bestelbons.Resources.LOGO_275.png"))
                {
                    image = builder.LinkedResources.Add("LOGO_275.png", stream);
                    image.ContentId = MimeUtils.GenerateMessageId();
                }


                string footer = string.Format(@"

                   <p><br></p>
                   <left><img src=""cid:{0}""></left>
                   <p>Industrielaan 19,zone C2 - B-8810 Lichtervelde<br>
                   Tel. +32 (0)51/72.24.46   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp      <a href=""http://www.astratec.be""> www.astratec.be</a> "
                     , image.ContentId);


                builder.HtmlBody = body + footer;
                message.Body = builder.ToMessageBody();

                try
                {

                    await _mailclient.ConnectAsync("smtp.office365.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await _mailclient.AuthenticateAsync(CurrentUser.Email, CurrentUser.EmailAuthentication);

                    // Check DNS capabilities
                    var supportsDsn = _mailclient.Capabilities.HasFlag(SmtpCapabilities.Dsn);

                    await _mailclient.SendAsync(message);
                    await _mailclient.DisconnectAsync(true);

                }
                catch (Exception ex)
                {
                    var dialogViewModel = IoC.Get<DialogViewModel>();
                    dialogViewModel.Capiton = "Attachment Error";
                    dialogViewModel.Message = ex.Message.ToString();
                    var result = _windowManager.ShowDialogAsync(dialogViewModel);
                }


            }
            catch (Exception e)
            {
                var dialogViewModel = IoC.Get<DialogViewModel>();
                dialogViewModel.Capiton = "Connect Error";
                dialogViewModel.Message = $"{e.Message}";
                var result = _windowManager.ShowDialogAsync(dialogViewModel);

            }


        }

        #endregion

        #region DRAG & DROP

        public void FilePreviewDragEnterSIEMENS(System.Windows.DragEventArgs e)
        {
            // TODO check if dragged item is a drive !!!
            e.Handled = true;
        }
        public void FileDroppedSIEMENS(System.Windows.DragEventArgs e) // OUDE XLS FILES !!
        {
            Bestelbonregels.Clear();
            if (true)
            {
                string[] data = ((System.Windows.IDataObject)e.Data).GetData("FileName") as string[];
                FullPathAttachementSIEMENS = data[0];

                string DirectroyName = Path.GetDirectoryName(FullPathAttachementSIEMENS);
                string FileName = Path.GetFileNameWithoutExtension(FullPathAttachementSIEMENS);


                Workbook workbook = new Workbook();
                workbook.LoadFromFile(FullPathAttachementSIEMENS);
                string OutputFilename = DirectroyName + FileName + "xslx";
                workbook.SaveToFile(OutputFilename, ExcelVersion.Version2013);

                FileInfo OnderdelenLijstfile = new FileInfo(OutputFilename);
                if (File.Exists(OutputFilename))
                {
                    using (var p = new ExcelPackage(OnderdelenLijstfile))
                    {
                        var worksheet = p.Workbook.Worksheets["Items"];
                        int i = 2;
                        while (!String.IsNullOrEmpty(worksheet.Cells[i, 2].GetValue<string>()))
                        {

                            Bestelbonregel bestelbonregel = new Bestelbonregel();
                            bestelbonregel.Aantal = worksheet.Cells[i, 8].GetValue<int>();
                            bestelbonregel.Eenheid = "stuk";

                            int teller = 0;
                            string FormattedString = string.Empty;

                            string[] subs = (worksheet.Cells[i, 6].GetValue<string>()).Split(',');

                            foreach (var substring in subs)
                            {
                                teller += substring.Length;
                                if (teller < 150) FormattedString += substring + ", ";
                                else
                                {
                                    FormattedString += "\n";
                                    teller = 0;
                                }
                            }

                            bestelbonregel.Bestelregel = worksheet.Cells[i, 3].GetValue<string>() + "\n" + FormattedString;
                            bestelbonregel.Prijsstring = (worksheet.Cells[i, 16].GetValue<string>()).Split()[0];

                            Bestelbonregels.Add(bestelbonregel);

                            i++;
                        }
                        Bestelbon.CalculateTotalPrice();
                    }
                }

            }
        }
        public void FilePreviewDragEnterFESTO(System.Windows.DragEventArgs e)
        {
            // TODO check if dragged item is a drive !!!
            e.Handled = true;
        }
        public void FileDroppedFESTO(System.Windows.DragEventArgs e) // CSV FILES !!
        {
            if (true)
            {
                Bestelbonregels.Clear();
                string[] data = ((System.Windows.IDataObject)e.Data).GetData("FileName") as string[];
                FullPathAttachementFESTO = data[0];

                string DirectroyName = Path.GetDirectoryName(FullPathAttachementFESTO);
                string FileName = Path.GetFileNameWithoutExtension(FullPathAttachementFESTO);
                List<string> Description1 = new List<string>();
                List<string> Description2 = new List<string>();
                List<string> Aantal = new List<string>();
                List<string> Prijsstring = new List<string>();

                using (var reader = new StreamReader(FullPathAttachementFESTO))
                {

                    int teller = 0;
                    bool einde = false;
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(';');
                        if (teller > 7 && !einde)
                        {
                            if (!values[5].Contains("Net"))
                            {
                                Description1.Add(values[2].TrimStart(new char[] { '0' }).PadRight(15));
                                Description2.Add(values[1]);
                                Aantal.Add(values[5]);
                                Prijsstring.Add(values[4]);
                            }
                            else einde = true;

                        }
                        teller++;
                    }
                }

                for (int i = 0; i < Description1.Count; i++)
                {
                    Bestelbonregel bestelbonregel = new Bestelbonregel();
                    bestelbonregel.Aantal = Int32.Parse(Aantal[i]);
                    bestelbonregel.Eenheid = "stuk";

                    int teller = 0;
                    string FormattedString = string.Empty;

                    string[] subs = (Description2[i]).Split(',');

                    foreach (var substring in subs)
                    {
                        teller += substring.Length;
                        if (teller < 150) FormattedString += substring + ", ";
                        else
                        {
                            FormattedString += "\n";
                            teller = 0;
                        }
                    }

                    bestelbonregel.Bestelregel = Description1[i] + "  " + FormattedString;
                    bestelbonregel.Prijsstring = Prijsstring[i];

                    Bestelbonregels.Add(bestelbonregel);
                }

                Bestelbon.CalculateTotalPrice();
            }
        }
        public void FilePreviewDragEnterMISUMI(System.Windows.DragEventArgs e)
        {
            // TODO check if dragged item is a drive !!!
            e.Handled = true;
        }
        public void FileDroppedMISUMI(System.Windows.DragEventArgs e) // CSV FILES !!
        {
            char[] UnwantedChars = { '\\', '"' };
            if (true)
            {
                Bestelbonregels.Clear();
                string[] data = ((System.Windows.IDataObject)e.Data).GetData("FileName") as string[];
                FullPathAttachementMISUMI = data[0];

                string DirectroyName = Path.GetDirectoryName(FullPathAttachementMISUMI);
                string FileName = Path.GetFileNameWithoutExtension(FullPathAttachementMISUMI);

                List<string> Description1 = new List<string>();
                List<string> Description2 = new List<string>();
                List<string> Aantal = new List<string>();
                List<string> Prijsstring = new List<string>();

                using (var reader = new StreamReader(FullPathAttachementMISUMI))
                {

                    int teller = 0;
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        if (teller > 0)
                        {
                            Description1.Add(values[22].TrimStart(UnwantedChars).TrimEnd(UnwantedChars));
                            Description2.Add(values[21].TrimStart(UnwantedChars).TrimEnd(UnwantedChars));
                            Aantal.Add(values[23].TrimStart(UnwantedChars).TrimEnd(UnwantedChars));
                            Prijsstring.Add(values[24].TrimStart(UnwantedChars).TrimEnd(UnwantedChars).Replace(',', '.').Replace('.', ','));

                        }
                        teller++;
                    }
                }

                for (int i = 0; i < Description1.Count; i++)
                {
                    Bestelbonregel bestelbonregel = new Bestelbonregel();
                    bestelbonregel.Aantal = Int32.Parse(Aantal[i]);
                    bestelbonregel.Eenheid = "stuk";

                    int teller = 0;
                    string FormattedString = string.Empty;

                    string[] subs = (Description2[i]).Split(',');

                    foreach (var substring in subs)
                    {
                        teller += substring.Length;
                        if (teller < 150) FormattedString += substring + ", ";
                        else
                        {
                            FormattedString += "\n";
                            teller = 0;
                        }
                    }

                    bestelbonregel.Bestelregel = Description1[i] + "  " + FormattedString;
                    bestelbonregel.Prijsstring = Prijsstring[i];

                    Bestelbonregels.Add(bestelbonregel);
                }

                Bestelbon.CalculateTotalPrice();
            }
        }

        #endregion

        public void OnTotalPriceChanged(object sender, EventArgs e)
        {
            Totaal = Bestelbon.TotalPrice;
        }

        public void BerekenTotalePrijsNewBestelregel()
        {
            NewTotalePrijs = NewAantal * NewPrijs;

        }

        public void BestelregelChange()
        {
            Bestelbon.CalculateTotalPrice();
            Bestelbon.CalculatePercentageDelivered();
            Bestelbon.CalculateDeliveryTimeExpired();
            _eventAggregator.PublishOnUIThreadAsync(new BestelbonAlteredEvent(Bestelbon));
        }

        public void SetBestelbonNaam()
        {
            BestelbonNaam = string.Empty;
            if (!String.IsNullOrEmpty(ComboboxSelectedItem)) BestelbonNaam = $"{ComboboxSelectedItem}-{LeveranciersNaamUI}-{VolgNummer}";

            if (ProjectNumberOK && LeverancierOK && !string.IsNullOrEmpty(VolgNummer))
            {
                BestelbonNaam = $"{ProjectNumber}-{LeveranciersNaamUI}-{VolgNummer}";

            }
            string bb = Bestelbon.Name;
            if (ProjectNumberOK && LeverancierOK && VolgNummer == string.Empty && NewBestelbonBusy)
            {
                _eventAggregator.PublishOnUIThreadAsync(new DetermineVolgnumberBestelbonEvent(ProjectNumber + "+" + LeveranciersNaamUI));
            }


            NotifyOfPropertyChange(() => CanSave);
            NotifyOfPropertyChange(() => CanSaveMail);
        }

        public void SetProjDirOK()
        {
            ProjDirOK = !String.IsNullOrEmpty(ProjectDirectory);
        }

        public void BestelbonChanged(object sender, PropertyChangedEventArgs e)
        {
            bool RaiseSaveReminder = true;
            switch (e.PropertyName)
            {
                case "DeliveryTimeExpired":
                case "PercentageDelivered":
                case "AttachedFile":
                case "TotalPrice":
                case "DeliveryTime":
                case "BestelbonSuccesfullySaved":
                case "BestelbonSend":
                case "Approved":
                case "AskForApproval":

                    RaiseSaveReminder = false;
                    break;
                default:
                    break;
            }
            if (RaiseSaveReminder) SaveReminder = true;

        }

        public void BestelbonregelsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SaveReminder = true;
        }

        public void NotifyFullDelivery()
        {
            if (Bestelbon.NotifyFullDelivery) Bestelbon.NotifyFullDelivery = false;
            else Bestelbon.NotifyFullDelivery = true;
        }

        #region EVENTHANDLERS

        public Task HandleAsync(SelectedBestelbonChangedEvent message, CancellationToken cancellationToken)
        {
            NewTextVisible = false;
            NewBestelbonBusy = false;
            DropAreasVisible = false;

            Approved = message.Bestelbon.Approved;
            ShowSelectedBestelbon(message.Bestelbon);
            return Task.CompletedTask;
        }

        public Task HandleAsync(SelectedElDocuBestelbonChangedEvent message, CancellationToken cancellationToken)
        {
            Bestelbonlist.Clear();
            LoadAllBestelbons();

            Bestelbon TempBestelbon = new Bestelbon();
            TempBestelbon = message.Bestelbon;

            string[] Parts = TempBestelbon.Name.Split('-');

            string Tempbestelbonprojectnaam = Parts[0];
            string TempbestelbonLeveranciersnaam = Parts[1];
            string Leveranciersnaam = string.Empty;
            int? ProjectNumber = 0;
            int Volgnummer = 0;

            foreach (var project in ProjectList)
            {
                if (project.Description.Contains(Tempbestelbonprojectnaam))
                {
                    ProjectNumber = project.ID;
                }
            }

            foreach (var lev in LeveranciersList)
            {
                if (lev.Name.Contains(TempbestelbonLeveranciersnaam))
                {
                    TempBestelbon.Leverancier.Name = lev.Name;
                    Leveranciersnaam = lev.Name;
                    break;
                }
            }

            // Zoeken in directory naar volgend volgnummer voor de bon !!

            string[] filenamesbestelbons = Directory.GetFiles(Properties.Settings.Default.BestelbonsPath);

            foreach (var bestelbon in Bestelbonlist.Where(b => b.Name.Contains(ProjectNumber.ToString()) && b.Name.Contains(Leveranciersnaam))
)
            {
                string[] substrings = bestelbon.Name.Split('-');
                int volgnummer;
                if (Int32.TryParse(substrings[2], out volgnummer))
                {

                    if (volgnummer > MaxVolgnummer) MaxVolgnummer = volgnummer;
                }
            }


            _eventAggregator.PublishOnUIThreadAsync(new DetermineVolgnumberBestelbonEvent($"{ProjectNumber}-{Leveranciersnaam}"));
            ShowSelectedBestelbon(TempBestelbon);
            return Task.CompletedTask;
        }

        public void LoadAllBestelbons()
        {
            bool NoException = true;

            string FilePath = Properties.Settings.Default.BestelbonsPath;
            if (Directory.Exists(FilePath)) ;
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
                                NoException = false;
                                throw new NotImplementedException($"Bestelbon {Path.GetFileName(file.FullName)} not LOADED !");
                            }
                        }
                        bestelbon.DeltaDeliveryTime = (int)(bestelbon.DeliveryTime).Subtract(DateTime.Now).TotalDays;
                        Bestelbonlist.Add(bestelbon);
                    }
                }

                if (NoException)
                {

                }

            }


        }

        public void ShowSelectedBestelbon(Bestelbon bestelbon)
        {
            CopyTextVisible = false;
            Bestelbon.PropertyChanged -= BestelbonChanged;
            Bestelbon.Bestelbonregels.CollectionChanged -= BestelbonregelsChanged;
            Bestelbon.TotalPriceChanged -= OnTotalPriceChanged;
            Bestelbon.FullDelivery -= OnNotifyFullDelivery;

            CLosePDF();
            NewBestelbonregel();

            SaveReminder = false;

            //Bestelbon = message.Bestelbon;

            string FilePath = Properties.Settings.Default.BestelbonsPath;
            string bestelbonnaam = bestelbon.Name;
            //string bestelbonnaam = Bestelbon.Name;
            string filename = $"{FilePath}\\{bestelbonnaam}.abb";

            if (File.Exists(filename))
            {
                using (var stream = System.IO.File.OpenRead(filename))
                {
                    try
                    {
                        var serializer = new XmlSerializer(typeof(Bestelbon));
                        DeserializedBestelbon = serializer.Deserialize(stream) as Bestelbon;
                    }
                    catch (Exception)
                    {

                    }
                }
            }


            Bestelbon.Name = DeserializedBestelbon.Name;
            Bestelbon.Creator = DeserializedBestelbon.Creator;
            Bestelbon.DeliveryTime = DeserializedBestelbon.DeliveryTime;
            Bestelbon.AttachedFile = DeserializedBestelbon.AttachedFile;
            Bestelbon.ProjectDirectory = DeserializedBestelbon.ProjectDirectory;
            Bestelbon.NotifyFullDelivery = DeserializedBestelbon.NotifyFullDelivery;
            Bestelbon.BestelbonSuccesfullySaved = DeserializedBestelbon.BestelbonSuccesfullySaved;

            TimeSpan TimeDifference = Bestelbon.DeliveryTime.Subtract(DateTime.Now);
            DaysToExpireDate = TimeDifference.Days;
            if (DaysToExpireDate < 0) DaysToExpireDate = 0;
            ProjectDirectory = Bestelbon.ProjectDirectory;
            _eventAggregator.PublishOnUIThreadAsync(new ProjectDirectoryChangedEvent(ProjectDirectory));
            SetProjDirOK();
            Bestelbon.TotalPriceChanged += OnTotalPriceChanged;
            Bestelbon.Bestelbonregels.Clear();
            foreach (var bestelbonregel in DeserializedBestelbon.Bestelbonregels)
            {
                bestelbonregel.PropertyChanged += Bestelbon.BestelbonregelChanged;
                Bestelbon.Bestelbonregels.Add(bestelbonregel);
            }

            Bestelbonregels = Bestelbon.Bestelbonregels;
            Bestelbon.CalculateTotalPrice();
            string[] data = Bestelbon.Name.Split('-');

            try
            {
                ProjectNumber = data[0];
                LeveranciersNaamUI = data[1];
                VolgNummer = data[2];
            }
            catch (Exception)
            {

            }

            foreach (var lev in LeveranciersList)
            {
                if (lev.Name == bestelbon.Leverancier.Name)
                {
                    Leverancier = lev;
                    Bestelbon.Leverancier = lev;

                    break;
                }
            }

            Opmerking = DeserializedBestelbon.Opmerking;

            //_eventAggregator.PublishOnUIThreadAsync(new DetermineVolgnumberEvent(ProjectNumber + "+" + LeveranciersNaamUI));

            Totaal = Bestelbon.TotalPrice;
            Bestelbon.PropertyChanged += BestelbonChanged;
            Bestelbon.Bestelbonregels.CollectionChanged += BestelbonregelsChanged;
            Bestelbon.FullDelivery += OnNotifyFullDelivery;
            if (Bestelbon.Creator == CurrentUser.LastName) OwnBestelbon = true;
            else OwnBestelbon = false;

        }

        public Task HandleAsync(UserChangedEvent message, CancellationToken cancellationToken)
        {
            CurrentUser = message.user;
            return Task.CompletedTask;
        }

        public Task HandleAsync(ProjectDirectoryChangedEvent projectDirectory, CancellationToken cancellationToken)
        {
            ProjectDirectory = projectDirectory.ProjectDirectory;
            SetProjDirOK();
            return Task.CompletedTask;
        }

        public Task HandleAsync(LeveranciersListUpdatedEvent leverancierslist, CancellationToken cancellationToken)
        {
            AllLeveranciers.Clear();
            LeveranciersList = leverancierslist.Updatedleverancierslist;
            LevSuggestionProvider.ListOfLeveranciers = leverancierslist.Updatedleverancierslist;
            foreach (var lev in LeveranciersList)
            {
                AllLeveranciers.Add((string)lev.Name);
            }
            return Task.CompletedTask;
        }

        public Task HandleAsync(LeverancierChangedEvent message, CancellationToken cancellationToken)
        {
            //Leverancier = message.Leverancier;
            if (NewBestelbonBusy)
            {
                Leverancier = message.Leverancier.DeepCopy();
                LeverancierUI = message.Leverancier.DeepCopy();
                LeveranciersNaamUI = Leverancier.Name;
                SetBestelbonNaam();
            }

            //if (!string.IsNullOrEmpty(ProjectNumber)) _eventAggregator.PublishOnUIThreadAsync(new DetermineVolgnumberEvent(ProjectNumber + "+" + LeveranciersNaamUI));
            //if (!string.IsNullOrEmpty(LeveranciersNaamUI)) _eventAggregator.PublishOnUIThreadAsync(new DetermineVolgnumberEvent(ComboboxSelectedItem + "+" + LeveranciersNaamUI));
            return Task.CompletedTask;
        }

        public Task HandleAsync(LoadedLeveraciersEvent message, CancellationToken cancellationToken)
        {
            AllLeveranciers.Clear();
            LeveranciersList = message.Leveranciers;
            LevSuggestionProvider.ListOfLeveranciers = message.Leveranciers;
            foreach (var lev in LeveranciersList)
            {
                AllLeveranciers.Add((string)lev.Name);
            }
            return Task.CompletedTask;
        }

        public Task HandleAsync(LoadedProjectIDFileEvent message, CancellationToken cancellationToken)
        {

            ProjectIDs.Clear();
            ProjectIDList = message.ProjectIDlist;
            foreach (var projectID in ProjectIDList)
            {
                ProjectIDs.Add((string)projectID);
            }


            return Task.CompletedTask;
        }

        public Task HandleAsync(ProjectIDChangedEvent message, CancellationToken cancellationToken)
        {
            ProjectIDs.Clear();
            ProjectIDList = message.ProjectIDlist;
            foreach (var projectID in ProjectIDList)
            {
                ProjectIDs.Add((string)projectID);
            }

            return Task.CompletedTask;
        }

        public Task HandleAsync(InitialUserLoadedEvent message, CancellationToken cancellationToken)
        {
            CurrentUser = message.user;
            return Task.CompletedTask;
        }

        public Task HandleAsync(SelectedProjectChangedEvent message, CancellationToken cancellationToken)
        {
            if (NewBestelbonBusy) ProjectNumber = message.Project.ID.ToString();
            return Task.CompletedTask;
        }

        public Task HandleAsync(ConvertedPrijsvraagEvent message, CancellationToken cancellationToken)
        {
            ConvertedPrijsvraag = message.Prijsvraag;
            ConvertedPrijsvraagNaam = message.PrijsvraagNaam;

            Bestelbon = new Bestelbon();
            Bestelbon.Name = ConvertedPrijsvraagNaam;
            Bestelbon.Creator = ConvertedPrijsvraag.Creator;
            Bestelbon.Leverancier = ConvertedPrijsvraag.Leverancier;
            foreach (var prijsvraagregel in ConvertedPrijsvraag.Prijsvraagregels)
            {
                var bestelbonregel = new Bestelbonregel
                {
                    Delivered = false,
                    Aantal = prijsvraagregel.Aantal,
                    Eenheid = prijsvraagregel.Eenheid,
                    Bestelregel = prijsvraagregel.Prijsregel,
                    Prijsstring = "0.0",
                    Prijs = 0.0M,
                    TotalePrijs = 0.0M
                };
                Bestelbon.Bestelbonregels.Add(bestelbonregel);
                Bestelbon.DeliveryTime = DateTime.Now + new TimeSpan(14, 0, 0, 0);
            }
            Bestelbon.ProjectDirectory = ConvertedPrijsvraag.ProjectDirectory;

            // SAVE FIRST WITHOUT SAVE DIALOG
            FilePath = $"{Properties.Settings.Default.BestelbonsPath}\\{ConvertedPrijsvraagNaam}.abb";

            using (var writer = new System.IO.StreamWriter(FilePath))
            {
                var serializer = new XmlSerializer(typeof(Bestelbon));
                serializer.Serialize(writer, Bestelbon);
                writer.Flush();
            }
            _eventAggregator.PublishOnUIThreadAsync(message: new BestelbonAddedEvent(Bestelbon));
            _eventAggregator.PublishOnUIThreadAsync(new SelectedBestelbonChangedEvent(Bestelbon));
            return Task.CompletedTask;
        }

        public Task HandleAsync(ModeChangedEvent message, CancellationToken cancellationToken)
        {
            ModeBestelbon = message.ModeBestelbon;
            BestelbonNaam = BestelbonNaam;
            return Task.CompletedTask;
        }

        public Task HandleAsync(UserListChangedEvent message, CancellationToken cancellationToken)
        {
            UserList = message.UserList;
            return Task.CompletedTask;

        }

        public Task HandleAsync(SuggestedNewVolgnummerBestelbonEvent message, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(VolgNummer))
            {
                VolgNummer = message.SuggestedNewVolgnummer.ToString();
            }
            return Task.CompletedTask;
        }

        public Task HandleAsync(SearchBestelbonsChangedEvent message, CancellationToken cancellationToken)
        {
            Search = message.Search;
            return Task.CompletedTask;

        }

        public Task HandleAsync(ProjectListChangedEvent message, CancellationToken cancellationToken)
        {
            ProjectNumbers.Clear();
            ProjectList = message.ProjectList;
            foreach (var project in ProjectList)
            {
                ProjectNumbers.Add((int)project.ID);
            }
            return Task.CompletedTask;
        }

        public Task HandleAsync(LoadedProjectListEvent message, CancellationToken cancellationToken)
        {
            ProjectNumbers.Clear();
            ProjectList = message.ProjectList;
            foreach (var project in ProjectList)
            {
                ProjectNumbers.Add((int)project.ID);
            }
            return Task.CompletedTask;
        }

        public Task HandleAsync(ConvertElBestelbonDocuRequestEvent message, CancellationToken cancellationToken)
        {
            NewBestelbon();

            string[] chuncks = message.Bestelbon.Name.Split('-');
            ProjectNumber = chuncks[0];

            foreach (var lev in LeveranciersList)
            {
                if (lev.Name.Contains(message.Bestelbon.Leverancier.Name))
                {
                    if (NewBestelbonBusy)
                    {
                        Leverancier = lev.DeepCopy();
                        LeverancierUI = lev.DeepCopy();
                        LeveranciersNaamUI = Leverancier.Name;
                        SetBestelbonNaam();

                    }
                    //if (!string.IsNullOrEmpty(ProjectNumber)) _eventAggregator.PublishOnUIThreadAsync(new DetermineVolgnumberEvent(ProjectNumber + "+" + LeveranciersNaamUI));
                    //if (!string.IsNullOrEmpty(LeveranciersNaamUI)) _eventAggregator.PublishOnUIThreadAsync(new DetermineVolgnumberEvent(ComboboxSelectedItem + "+" + LeveranciersNaamUI));
                }
            }
            foreach (var bestregel in message.Bestelbon.Bestelbonregels)
            {
                Bestelbon.Bestelbonregels.Add(bestregel);
            }
            SetBestelbonNaam();
            return Task.CompletedTask;
        }


        #endregion


    }
}

