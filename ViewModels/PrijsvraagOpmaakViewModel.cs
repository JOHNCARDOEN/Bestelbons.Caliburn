using Caliburn.Micro;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using WPF_Bestelbons.Events;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons.ViewModels
{
    public class PrijsvraagOpmaakViewModel : Caliburn.Micro.Screen, IHandle<LeverancierChangedEvent>, IHandle<LoadedLeveraciersEvent>,
               IHandle<SelectedPrijsvraagChangedEvent>, IHandle<UserChangedEvent>, IHandle<InitialUserLoadedEvent>, IHandle<LeveranciersListUpdatedEvent>,
               IHandle<LoadedProjectIDFileEvent>, IHandle<ProjectIDChangedEvent>, IHandle<SelectedProjectChangedEvent>, IHandle<ModeChangedEvent>, IHandle<UserListChangedEvent>,
               IHandle<ProjectListChangedEvent>, IHandle<LoadedProjectListEvent>, IHandle<SearchPrijsvragenChangedEvent>, IHandle<SuggestedNewVolgnummerPrijsvraagEvent>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;
        private PDFCreatorPrijsaanvraag _pdfCreator;

        public SendMailViewModel SendMailVM { get; set; }
        public BindableCollection<Leverancier> LeveranciersList { get; set; }
        public ObservableCollection<User> UserList { get; set; }
        public BindableCollection<Project> ProjectList { get; set; }
        BindableCollection<Bestelbon> Bestelbonlist { get; set; }
        BindableCollection<Prijsvraag> Prijsvragenlist { get; set; }
        public User CurrentUser { get; set; }
        public Prijsvraag DeserializedPrijsvraag { get; set; }
        public string ProjectDirectory { get; set; }

        public string PrijsvraagNameUsedForOpmerking { get; set; }
        public string CopiedPrijsvraagNaam { get; set; }

        public HashSet<string> AllLeveranciers { get; set; }
        public HashSet<int> ProjectNumbers { get; set; }
        public bool ModePrijsvraag { get; set; }
        public bool DetermineNextNumber { get; set; }

        bool IsSuccesfullySaved = false;

        int PDFPage = 0;
        bool SaveAndMail = false;

        public string FilePath;

        #region CANEXECUTE

        public bool CanDeleteBestelregel
        {
            get { return PrijsvraagregelsSelectedItem != null; }
        }

        public bool CanAdd
        {
            get { return !String.IsNullOrEmpty(NewPrijsregel); }
        }

        public bool CanSave
        {
            get
            {
                return (!String.IsNullOrEmpty(LeveranciersNaamUI) && (!String.IsNullOrEmpty(ProjectNumber) || !String.IsNullOrEmpty(ComboboxSelectedItem)) &&
                        !String.IsNullOrEmpty(VolgNummer));
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
            get { return !String.IsNullOrEmpty(PrijsvraagNaam); }
        }

        public bool CanConvert
        {
            get { return !String.IsNullOrEmpty(PrijsvraagNaam); }
        }

        #endregion

        #region BINDABLE FIELDS

        private bool _converting;

        public bool Converting
        {
            get { return _converting; }
            set
            {
                _converting = value;
                NotifyOfPropertyChange(() => Converting);
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

        private string _messageText;

        public string MessageText
        {
            get { return _messageText; }
            set
            {
                _messageText = value;
                NotifyOfPropertyChange(() => MessageText);
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



        private bool _newPrijsvraagBusy;

        public bool NewPrijsvraagBusy
        {
            get { return _newPrijsvraagBusy; }
            set
            {
                _newPrijsvraagBusy = value;
                NotifyOfPropertyChange(() => NewPrijsvraagBusy);
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

        private BindableCollection<Prijsvraagregel> _prijsvraagregels;

        public BindableCollection<Prijsvraagregel> Prijsvraagregels
        {
            get { return _prijsvraagregels; }
            set
            {
                _prijsvraagregels = value;
                NotifyOfPropertyChange(() => Prijsvraagregels);
            }
        }

        private Prijsvraagregel _prijsvraagregelSelectedItem;

        public Prijsvraagregel PrijsvraagregelsSelectedItem
        {
            get { return _prijsvraagregelSelectedItem; }
            set
            {
                _prijsvraagregelSelectedItem = value;
                NotifyOfPropertyChange(() => CanDeleteBestelregel);
                NotifyOfPropertyChange(() => PrijsvraagregelsSelectedItem);
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
                SetPrijsvraagNaam();
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
                        SetPrijsvraagNaam();
                    }

                }
                else ProjectNumberOK = false;
                NotifyOfPropertyChange(() => ProjectNumber);
                SetPrijsvraagNaam();
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
                SetPrijsvraagNaam();
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
                Prijsvraag.Opmerking = Opmerking;
                NotifyOfPropertyChange(() => Opmerking);
            }
        }

        private Prijsvraag _prijsvraag;
        public Prijsvraag Prijsvraag
        {
            get { return _prijsvraag; }
            set
            {
                _prijsvraag = value;
                NotifyOfPropertyChange(() => Prijsvraag);
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

        private string _comboBoxSelectedItem;

        public string ComboboxSelectedItem
        {
            get { return _comboBoxSelectedItem; }
            set
            {
                _comboBoxSelectedItem = value;
                if (!String.IsNullOrEmpty(value)) ProjectNumber = "";
                NotifyOfPropertyChange(() => ComboboxSelectedItem);
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

        #endregion

        #region FULLPROPERTIES NEW PRIJSVRAAGREGEL

        private int _newAantal;

        public int NewAantal
        {
            get { return _newAantal; }
            set
            {
                _newAantal = value;
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

        private string _newPrijsregel;

        public string NewPrijsregel
        {
            get { return _newPrijsregel; }
            set
            {
                _newPrijsregel = value;
                NotifyOfPropertyChange(() => CanAdd);
                NotifyOfPropertyChange(() => NewPrijsregel);
            }
        }

        #endregion


        private string _prijsvraagNaam;

        public string PrijsvraagNaam
        {
            get { return _prijsvraagNaam; }
            set
            {
                _prijsvraagNaam = value;
                NotifyOfPropertyChange(() => PrijsvraagNaam);
                NotifyOfPropertyChange(() => CanSave);
                NotifyOfPropertyChange(() => CanSaveMail);
                NotifyOfPropertyChange(() => CanSaveAttach);
                NotifyOfPropertyChange(() => CanCopy);
                NotifyOfPropertyChange(() => CanConvert);
                Prijsvraag.Name = PrijsvraagNaam;

                if (PrijsvraagNaam != PrijsvraagNameUsedForOpmerking && !string.IsNullOrEmpty(PrijsvraagNameUsedForOpmerking))
                {
                    PrijsvraagNameUsedForOpmerking = PrijsvraagNaam;
                    AutoAddedOpmerking = $" Te vermelden bij communicatie : {PrijsvraagNaam}";

                }


                if ((!String.IsNullOrEmpty(LeveranciersNaamUI) && (!String.IsNullOrEmpty(ProjectNumber) || !String.IsNullOrEmpty(ComboboxSelectedItem)) && !String.IsNullOrEmpty(VolgNummer)) || !ModePrijsvraag)
                {
                    _eventAggregator.PublishOnUIThreadAsync(new RaiseErrorEvent(new Error
                    { Level = ErrorLevel.Error, ErrorMessage = " Prijsvraag Naam not set", Active = false }));

                    PrijsvraagNameUsedForOpmerking = PrijsvraagNaam;
                    AutoAddedOpmerking = $" Te vermelden bij communicatie : {PrijsvraagNaam}";

                }
                else
                {
                    _eventAggregator.PublishOnUIThreadAsync(new RaiseErrorEvent(new Error
                    { Level = ErrorLevel.Error, ErrorMessage = " Prijsvraag Naam not set", Active = true }));
                    AutoAddedOpmerking = "";
                }

            }
        }

        public PDFCreatorPrijsaanvraag PdfCreator
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

        public PrijsvraagOpmaakViewModel(IEventAggregator eventAggregator, IWindowManager windowsmanager, PDFCreatorPrijsaanvraag pDFCreator)
        {
            _eventAggregator = eventAggregator;
            _windowManager = windowsmanager;
            _eventAggregator.Subscribe(this);
            _pdfCreator = pDFCreator;

            SendMailVM = IoC.Get<SendMailViewModel>();

            this.Prijsvraag = new Prijsvraag();
            this.DeserializedPrijsvraag = new Prijsvraag();
            this.Leverancier = new Leverancier();
            LeveranciersNaamUI = "";
            Opmerking = "";
            NewEenheid = "stuk";
            Prijsvraag.Leverancier = new Leverancier();
            Prijsvraagregels = Prijsvraag.Prijsvraagregels;
            NewAantal = 1;
            PrijsvraagNaam = "";
            this.CurrentUser = new User();
            this.ProjectIDList = new BindableCollection<string>();
            this.UserList = new ObservableCollection<User>();
            this.Bestelbonlist = new BindableCollection<Bestelbon>();
            this.Prijsvragenlist = new BindableCollection<Prijsvraag>();
            this.AllLeveranciers = new HashSet<string>();
            this.ProjectNumbers = new HashSet<int>();
            this.LevSuggestionProvider = new LeverancierSuggestionProvider();


        }

        #region COMMANDS

        public void Add()
        {
            //EA Eenheid = (EA)Enum.Parse(typeof(EA), NewEenheid);  // gives errors when altering the eenheid
            Prijsvraag.Prijsvraagregels.Add(new Prijsvraagregel()
            {
                Aantal = NewAantal,
                Eenheid = NewEenheid,
                Prijsregel = NewPrijsregel,

            });

            Prijsvraagregels = Prijsvraag.Prijsvraagregels;
        }

        public void Save()
        {
            NewPrijsvraagBusy = false;
            IsSuccesfullySaved = true;
            bool NewPrijsvraag = true;
            Prijsvraag.Name = PrijsvraagNaam;
            Prijsvraag.Leverancier.Name = Leverancier.Name;
            Prijsvraag.Prijsvraagregels = Prijsvraagregels;
            Prijsvraag.Opmerking = Opmerking;
            Prijsvraag.ProjectDirectory = ProjectDirectory;

            SaveReminder = false;

            if (Prijsvraag.Prijsvraagregels.Count > 0 && (!CopyTextVisible || (CopyTextVisible && (CopiedPrijsvraagNaam != PrijsvraagNaam))))
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.InitialDirectory = Properties.Settings.Default.PrijsvragenPath;
                saveFileDialog1.Filter = "Astratec Prijsvragen(*.prv)|*.prv|All files (*.*)|*.*";
                saveFileDialog1.Title = "Save Prijsvraag";
                saveFileDialog1.FileName = Prijsvraag.Name;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    CopyTextVisible = false;
                    NewTextVisible = false;
                    FilePath = saveFileDialog1.FileName;
                    string FilePathWithoutExtension = FilePath.Substring(0, FilePath.Length - 4);


                    if (File.Exists(FilePath)) NewPrijsvraag = false;

                    using (var writer = new System.IO.StreamWriter(FilePath))
                    {
                        var serializer = new XmlSerializer(typeof(Prijsvraag));
                        serializer.Serialize(writer, Prijsvraag);
                        writer.Flush();
                    }

                    try
                    {
                        User CreatorPrijsvraag = CurrentUser;
                        foreach (var user in UserList)
                        {
                            if (Prijsvraag.Creator == user.LastName) CreatorPrijsvraag = user;
                        }

                        if (CreatorPrijsvraag != null)
                        {
                            PdfCreator.Create(Prijsvraag, CreatorPrijsvraag, FilePathWithoutExtension + ".pdf");
                        }
                        else
                        {
                            var dialog = IoC.Get<DialogViewModel>();
                            dialog.Capiton = " Creating file and PDF";
                            dialog.Message = "Maker van de bestelbon is niet gekend !";
                            _windowManager.ShowDialogAsync(dialog);
                        }

                        //FileStream fs = new FileStream(FilePathWithoutExtension + ".pdf", FileMode.Create);
                        //PdfCreator.Create(Prijsvraag, CreatorPrijsvraag, fs);
                        //fs.Close();
                    }
                    catch (Exception ex)
                    {
                        var dialog = IoC.Get<DialogViewModel>();
                        dialog.Capiton = " Creating file and PDF";
                        dialog.Message = ex.ToString();
                        _windowManager.ShowDialogAsync(dialog);
                        IsSuccesfullySaved = false;
                    }

                    if (Directory.Exists(ProjectDirectory))
                    {
                        try
                        {
                            using (var writer = new System.IO.StreamWriter(ProjectDirectory + "\\" + PrijsvraagNaam))
                            {
                                var serializer = new XmlSerializer(typeof(Prijsvraag));
                                serializer.Serialize(writer, Prijsvraag);
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
                            User CreatorPrijsvraag = CurrentUser;
                            foreach (var user in UserList)
                            {
                                if (Prijsvraag.Creator == user.LastName) CreatorPrijsvraag = user;
                            }

                            string FilePathProjects = ProjectDirectory + "\\" + PrijsvraagNaam + ".pdf";
                            PdfCreator.Create(Prijsvraag, CreatorPrijsvraag, FilePathProjects);

                        }
                        catch (Exception ex)
                        {
                            var dialog = IoC.Get<DialogViewModel>();
                            dialog.Capiton = " Creating file and PDF";
                            dialog.Message = ex.ToString();
                            _windowManager.ShowDialogAsync(dialog);
                            IsSuccesfullySaved = false;
                        }

                    }
                    if (!SaveAndMail && IsSuccesfullySaved) ViewPDF();
                    if (NewPrijsvraag && IsSuccesfullySaved)
                        _eventAggregator.PublishOnUIThreadAsync(message: new PrijsvraagAddedEvent(Prijsvraag));
                }

            }
            else
            {
                var dialog = IoC.Get<DialogViewModel>();
                dialog.Capiton = " Save";

                if (Prijsvraag.Prijsvraagregels.Count == 0) dialog.Message = $"GEEN Prijsvraagregels in Bestelbon !!" + Environment.NewLine + Environment.NewLine + "Voeg een of meer prijsvraagregels toe  ";
                else if (CopyTextVisible && CopiedPrijsvraagNaam == PrijsvraagNaam) dialog.Message = $"Prijsvraag Name NIET gewijzigd !!" + Environment.NewLine + Environment.NewLine + "Verander Leverancier of projectnr of Volgnr ";
                var result = _windowManager.ShowDialogAsync(dialog);  //result = true of false from TryClose(true) or TryClose(false);
                IsSuccesfullySaved = false;
            }

            _eventAggregator.PublishOnUIThreadAsync(new ClearSearchEvent());

        }

        public void Copy()
        {
            NewPrijsvraagBusy = true;  // NOW POSSIBLE TO CHANGE LEVERANCIER WHEN COPYING A PRIJSVRAAG
            CopyTextVisible = true;
            Prijsvraag.Creator = CurrentUser.LastName;
            CopiedPrijsvraagNaam = PrijsvraagNaam;
            DetermineNextNumber = true;
        }

        public void SaveMail()
        {
            SaveAndMail = true;
            Save();
            if (IsSuccesfullySaved)
            {
                _eventAggregator.PublishOnUIThreadAsync(new PrijsvraagToMailEvent(Prijsvraag, Leverancier, CurrentUser, ProjectNumber));
                _windowManager.ShowWindowAsync(SendMailVM);
            }
            SaveAndMail = false;
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
                    string CopiedPDFfilename = $"{Properties.Settings.Default.PrijsvragenPath}\\{PrijsvraagNaam}-attached.pdf";
                    AttachedPDF = openfiledialog.FileName;
                    File.Copy(AttachedPDF, CopiedPDFfilename);
                    Prijsvraag.AttachedFile = true;

                    // Saving of Attached file field !!
                    using (var writer = new System.IO.StreamWriter(FilePath))
                    {
                        var serializer = new XmlSerializer(typeof(Prijsvraag));
                        serializer.Serialize(writer, Prijsvraag);
                        writer.Flush();
                    }

                }
            }


        }

        public bool LoadBestelbons()
        {
            bool OK = true;
            string FilePath = Properties.Settings.Default.BestelbonsPath;
            if (Directory.Exists(FilePath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(FilePath);
                Bestelbonlist.Clear();
                System.IO.FileInfo[] allfiles = directoryInfo.GetFiles();

                for (int i = 0; i < allfiles.Length; i++)
                {
                    if (allfiles[i].Extension.Contains("abb"))
                    {
                        Bestelbon bestelbon = new Bestelbon();
                        using (var stream = System.IO.File.OpenRead(allfiles[i].FullName))
                        {
                            try
                            {
                                var serializer = new XmlSerializer(typeof(Bestelbon));
                                bestelbon = serializer.Deserialize(stream) as Bestelbon;
                            }
                            catch (Exception)
                            {
                                OK = false;
                            }
                        }
                        bestelbon.DeltaDeliveryTime = (int)(bestelbon.DeliveryTime).Subtract(DateTime.Now).TotalDays;
                        Bestelbonlist.Add(bestelbon);
                    }
                }
            }

            if (OK) return true;
            else return false;

        }

        public async Task Convert()
        {
            Converting = true;
            MessageText = "     Loading Bestelbons......";
            // Eerst laden van alle bestelbons

            bool OK = await Task.Run(() => LoadBestelbons());

            // Checken of het volgnummer van de prijsvraag als bestelbon nog niet bestaat !!
            MessageText = "     Check of Prijsvraag als Bestelbon al bestaat......";
            string[] Prijsvraagsubstrings = PrijsvraagNaam.Split('-');
            int CurrentPrijsvraagVolgnummer;
            bool prijsvraagvolgnummerconverted = Int32.TryParse(Prijsvraagsubstrings[2], out CurrentPrijsvraagVolgnummer);

            if (OK && prijsvraagvolgnummerconverted)
            {

                int MaxVolgnummer = 0;
                int volgnummer;
                foreach (var bestelbon in Bestelbonlist)
                {
                    string[] substrings = bestelbon.Name.Split('-');
                    if ((Prijsvraagsubstrings[0] == substrings[0]) && (Prijsvraagsubstrings[1] == substrings[1]))
                    {
                        if (Int32.TryParse(substrings[2], out volgnummer))
                        {
                            if (volgnummer > MaxVolgnummer) MaxVolgnummer = volgnummer;
                        }
                    }

                }
                MessageText = "     Genereer Bestelbon......";
                // Als prijsaavraagvolgnummer > max al bestaand volgnummer in de bestelbons --> kan direct converteren
                if (CurrentPrijsvraagVolgnummer > MaxVolgnummer) _eventAggregator.PublishOnUIThreadAsync(new ConvertedPrijsvraagEvent(Prijsvraag, PrijsvraagNaam));
                // als prijsaanvraagvolgnummer < max al bestaand volgnummer in de bestelbons --> aanpassen van  prijsvraagvolgnummer naar maxvolgnummer+1 en dan converteren !
                else
                {

                    // Eerst file met oude Prijsvraagnaam DELETEN
                    string FilePathToDelete = Properties.Settings.Default.PrijsvragenPath + '\\' + PrijsvraagNaam + ".prv";
                    string PDFFilePathToDelete = Properties.Settings.Default.PrijsvragenPath + '\\' + PrijsvraagNaam + ".pdf";
                    try
                    {
                        File.Delete(FilePathToDelete);
                        File.Delete(PDFFilePathToDelete);
                        _eventAggregator.PublishOnUIThreadAsync(new PrijsvraagRemovedEvent(Prijsvraag));
                    }
                    catch (Exception)
                    {


                    }

                    // verander volgnummer in PrijsaanvraagNaame en ook Prijsaanvraag

                    PrijsvraagNaam = Prijsvraagsubstrings[0] + '-' + Prijsvraagsubstrings[1] + '-' + (MaxVolgnummer + 1).ToString();
                    VolgNummer = (MaxVolgnummer + 1).ToString();
                    Save();
                    _eventAggregator.PublishOnUIThreadAsync(new ConvertedPrijsvraagEvent(Prijsvraag, PrijsvraagNaam));

                }
            }

            Converting = false;

        }

        public void NewPrijsvraag()
        {
            NotifyOfPropertyChange(() => LevSuggestionProvider);
            NewTextVisible = true;

            NewPrijsvraagBusy = true;
            Prijsvraag.PropertyChanged -= PrijsvraagChanged;
            Prijsvraag.Prijsvraagregels.CollectionChanged -= PrijsvraagregelsChanged;

            _eventAggregator.PublishOnUIThreadAsync(new ClearLeverancierSelectedItemRequestEvent());
            ProjectNumber = "";
            VolgNummer = "";
            LeveranciersNaamUI = "";
            Opmerking = "";
            Prijsvraag.Creator = CurrentUser.LastName;
            Prijsvraagregels.Clear();
            NewPrijsvraaagregel();

            //NewOrCopyPrijsvraagBusy = true;
            //Prijsvraag.PropertyChanged -= PrijsvraagChanged;
            //Prijsvraag.Prijsvraagregels.CollectionChanged -= PrijsvraagregelsChanged;
            ////Bestelbon = null;
            //Prijsvraag = new Prijsvraag();
            //_eventAggregator.PublishOnUIThread(new ClearLeverancierSelectedItemRequestEvent());
            //ProjectNumber = "";
            //VolgNummer = "";
            //LeveranciersNaamUI = "";
            //Opmerking = "";
            //Prijsvraagregels = Prijsvraag.Prijsvraagregels;
            //NewPrijsvraaagregel();

        }

        public void NewPrijsvraaagregel()
        {
            NewAantal = 1;
            NewPrijsregel = "";

        }


        public void DeletePrijsregel()
        {
            Prijsvraag.Prijsvraagregels.Remove(PrijsvraagregelsSelectedItem);
        }

        public void ViewPDF()
        {

            try
            {
                if (File.Exists($"{Properties.Settings.Default.PrijsvragenPath}\\{Prijsvraag.Name}.pdf"))
                {
                    PDFFile = $"{Properties.Settings.Default.PrijsvragenPath}\\{Prijsvraag.Name}.pdf";
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
                if (File.Exists($"{Properties.Settings.Default.PrijsvragenPath}\\{Prijsvraag.Name}-attached.pdf"))
                {
                    PDFFile = $"{Properties.Settings.Default.PrijsvragenPath}\\{Prijsvraag.Name}-attached.pdf";
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
        }

        public void PrintPDF()
        {
            ProcessStartInfo info = new ProcessStartInfo($"{Properties.Settings.Default.PrijsvragenPath}\\{Prijsvraag.Name}.pdf");
            info.Verb = "printto";
            info.CreateNoWindow = true;
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

        #endregion

        public void SetPrijsvraagNaam()
        {
            if (!String.IsNullOrEmpty(ComboboxSelectedItem)) PrijsvraagNaam = $"{ComboboxSelectedItem}-{LeveranciersNaamUI}-{VolgNummer}";
            // else PrijsvraagNaam = $"{ProjectNumber}-{LeveranciersNaamUI}-{VolgNummer}";
            else
            {
                if (_projectNumber != null)
                {
                    bool projectnrInProjectIDList = false;
                    foreach (var _projnr in ProjectIDList)
                    {
                        if (_projectNumber == _projnr) projectnrInProjectIDList = true;
                    }
                    if (ProjectNumberOK || projectnrInProjectIDList)   //Regex.Match(_projectNumber, @"^[0-9]{6}$").Success
                    {
                        PrijsvraagNaam = $"{ProjectNumber}-{LeveranciersNaamUI}-{VolgNummer}";
                        NotifyOfPropertyChange(() => CanSave);
                        NotifyOfPropertyChange(() => CanSaveMail);
                    }
                    else
                    {
                        PrijsvraagNaam = string.Empty;
                        NotifyOfPropertyChange(() => CanSave);
                        NotifyOfPropertyChange(() => CanSaveMail);
                    }

                }
                if (ProjectNumberOK && LeverancierOK && DetermineNextNumber)
                {
                    _eventAggregator.PublishOnUIThreadAsync(new DetermineVolgnumberPrijsvraagEvent(ProjectNumber + "+" + LeveranciersNaamUI));
                    DetermineNextNumber = false;
                }

            }

        }

        public void SetProjDirOK()
        {
            ProjDirOK = !String.IsNullOrEmpty(ProjectDirectory);
        }

        public void PrijsvraagChanged(object sender, PropertyChangedEventArgs e)
        {
            bool RaiseSaveReminder = true;
            switch (e.PropertyName)
            {
                case "DeliveryTimeExpired":
                case "PercentageDelivered":
                case "TotalPrice":
                case "DeliveryTime":
                    RaiseSaveReminder = false;
                    break;
                default:
                    break;
            }
            if (RaiseSaveReminder) SaveReminder = true;

        }

        public void PrijsvraagregelsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SaveReminder = true;
        }

        #region EVENTHANDLERS

        public Task HandleAsync(SelectedPrijsvraagChangedEvent message, CancellationToken cancellationToken)
        {
            NewPrijsvraagBusy = false;
            NewTextVisible = false;
            CopyTextVisible = false;
            Prijsvraag.PropertyChanged -= PrijsvraagChanged;
            Prijsvraag.Prijsvraagregels.CollectionChanged -= PrijsvraagregelsChanged;

            CLosePDF();
            NewPrijsvraaagregel();

            SaveReminder = false;

            Prijsvraag = message.Prijsvraag;

            string FilePath = Properties.Settings.Default.PrijsvragenPath;
            string bestelbonnaam = message.Prijsvraag.Name;
            // string bestelbonnaam = Prijsvraag.Name;
            string filename = $"{FilePath}\\{bestelbonnaam}.prv";

            if (File.Exists(filename))
            {
                using (var stream = System.IO.File.OpenRead(filename))
                {
                    try
                    {
                        var serializer = new XmlSerializer(typeof(Prijsvraag));
                        DeserializedPrijsvraag = serializer.Deserialize(stream) as Prijsvraag;
                    }
                    catch (Exception)
                    {

                    }
                }
            }


            Prijsvraag.Name = DeserializedPrijsvraag.Name;
            Prijsvraag.AttachedFile = DeserializedPrijsvraag.AttachedFile;
            Prijsvraag.ProjectDirectory = DeserializedPrijsvraag.ProjectDirectory;

            ProjectDirectory = Prijsvraag.ProjectDirectory;
            _eventAggregator.PublishOnUIThreadAsync(new ProjectDirectoryChangedEvent(ProjectDirectory));
            SetProjDirOK();
            Prijsvraag.Prijsvraagregels.Clear();
            foreach (var prijsvraagregel in DeserializedPrijsvraag.Prijsvraagregels)
            {
                prijsvraagregel.PropertyChanged += Prijsvraag.PrijsvraagregelChanged;
                Prijsvraag.Prijsvraagregels.Add(prijsvraagregel);
            }

            Prijsvraagregels = Prijsvraag.Prijsvraagregels;
            string[] data = Prijsvraag.Name.Split('-');

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
                if (lev.Name == Prijsvraag.Leverancier.Name)
                {
                    Leverancier = lev;
                    LeveranciersNaamUI = Leverancier.Name;
                    break;
                }
            }

            Opmerking = Prijsvraag.Opmerking;

            Prijsvraag.PropertyChanged += PrijsvraagChanged;
            Prijsvraag.Prijsvraagregels.CollectionChanged += PrijsvraagregelsChanged;
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

            // Leverancier = message.Leverancier;
            if (NewPrijsvraagBusy)
            {
                Leverancier = message.Leverancier.DeepCopy();
                LeverancierUI = message.Leverancier.DeepCopy();
                LeveranciersNaamUI = message.Leverancier.Name;
            }
            foreach (var lev in LeveranciersList)
            {
                AllLeveranciers.Add((string)lev.Name);
            }
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
            ProjectIDList = message.ProjectIDlist;
            return Task.CompletedTask;
        }
        public Task HandleAsync(ProjectIDChangedEvent message, CancellationToken cancellationToken)
        {
            ProjectIDList = message.ProjectIDlist;
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
        public Task HandleAsync(UserChangedEvent message, CancellationToken cancellationToken)
        {
            CurrentUser = message.user;
            return Task.CompletedTask;
        }
        public Task HandleAsync(InitialUserLoadedEvent message, CancellationToken cancellationToken)
        {
            CurrentUser = message.user;
            return Task.CompletedTask;
        }
        public Task HandleAsync(SelectedProjectChangedEvent message, CancellationToken cancellationToken)
        {
            ProjectNumber = message.Project.ID.ToString();
            return Task.CompletedTask;
        }
        public Task HandleAsync(ModeChangedEvent message, CancellationToken cancellationToken)
        {
            ModePrijsvraag = !message.ModeBestelbon;
            PrijsvraagNaam = PrijsvraagNaam;
            return Task.CompletedTask;
        }
        public Task HandleAsync(UserListChangedEvent message, CancellationToken cancellationToken)
        {
            UserList = message.UserList;
            return Task.CompletedTask;
        }
        public Task HandleAsync(SearchPrijsvragenChangedEvent message, CancellationToken cancellationToken)
        {
            Search = message.Search;
            return Task.CompletedTask;
        }
        public Task HandleAsync(SuggestedNewVolgnummerPrijsvraagEvent message, CancellationToken cancellationToken)
        {
            VolgNummer = message.SuggestedNewVolgnummer.ToString();
            return Task.CompletedTask;
        }

        #endregion
    }
}
