using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using WPF_Bestelbons.Editors;
using WPF_Bestelbons.Events;
using WPF_Bestelbons.Models;


namespace WPF_Bestelbons.ViewModels
{
    public class ShellViewModel : Conductor<Screen>, IHandle<LeverancierChangedEvent>, IHandle<UserChangedEvent>, IHandle<CloseActiveItemEvent>, IHandle<RaiseErrorEvent>,
                                                    IHandle<ProjectDirectoryChangedEvent>, IHandle<ReloadAllFilesEvent>, IHandle<ConvertedPrijsvraagEvent>
    {
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;
        double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
        double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
        private Bestelbon bestelbons;
        public string MACAdressThisPC;






        BindableCollection<Leverancier> Leverancierlist { get; set; }
        BindableCollection<Bestelbon> Bestelbonlist { get; set; }
        BindableCollection<Prijsvraag> Prijsvragenlist { get; set; }
        BindableCollection<string> ProjectIDlist { get; set; }



        #region CANEXECUTE

        public bool CanLeveranciersList
        {
            get { return (Leverancierlist.Count > 0); }
        }

        public bool CanProjectList
        {
            get { return (ProjectIDlist.Count > 0 && ProjectsList.Count > 0); }
        }
        public bool CanBestelbonsList
        {
            get { return (Bestelbonlist.Count > 0); }
        }

        public bool CanPrijsvraagList
        {
            get { return (Prijsvragenlist.Count > 0); }
        }

        #endregion

        #region BINDABLE FIELDS

        private int _progressReadingBons;

        public int ProgressReadingBons
        {
            get { return _progressReadingBons; }
            set
            {
                _progressReadingBons = value;
                NotifyOfPropertyChange(() => ProgressReadingBons);
            }
        }


        private bool _loadVisible;

        public bool LoadVisible
        {
            get { return _loadVisible; }
            set
            {
                _loadVisible = value;
                NotifyOfPropertyChange(() => LoadVisible);
            }
        }


        private bool _modeBestelbon;

        public bool ModeBestelbon
        {
            get { return _modeBestelbon; }
            set
            {
                _modeBestelbon = value;
                _eventAggregator.PublishOnUIThreadAsync(new ModeChangedEvent(value));
                NotifyOfPropertyChange(() => ModeBestelbon);
            }
        }



        private bool _activeItemVisible;

        public bool ActiveItemVisible
        {
            get { return _activeItemVisible; }
            set
            {
                _activeItemVisible = value;
                NotifyOfPropertyChange(() => ActiveItemVisible);
            }
        }


        private string _version;
        public string Version
        {
            get { return _version; }
            set
            {
                _version = value;
                NotifyOfPropertyChange(() => Version);
            }
        }


        private string _projectDir;

        public string ProjectDir
        {
            get { return _projectDir; }
            set
            {
                _projectDir = value;
                NotifyOfPropertyChange(() => ProjectDir);
            }
        }


        private BindableCollection<Error> _errorList;

        public BindableCollection<Error> ErrorList
        {
            get { return _errorList; }
            set
            {
                _errorList = value;
                NotifyOfPropertyChange(() => ErrorList);
            }
        }

        private User _currentuser;

        public User CurrentUser
        {
            get { return _currentuser; }
            set
            {
                _currentuser = value;
                NotifyOfPropertyChange(() => CurrentUser);
            }
        }

        private double _maxHeight;

        public double MaxHeight
        {
            get { return _maxHeight; }
            set
            {
                _maxHeight = value;
                NotifyOfPropertyChange(() => MaxHeight);
            }
        }

        private WindowState _windowState;

        public System.Windows.WindowState WindowState
        {
            get { return _windowState; }
            set
            {
                _windowState = value;
                NotifyOfPropertyChange(() => WindowState);
            }
        }

        private string _tooltipText;

        public string TooltipText
        {
            get { return _tooltipText; }
            set
            {
                _tooltipText = value;
                NotifyOfPropertyChange(() => TooltipText);
            }
        }


        private string _buttonImage;

        public string ButtonImage
        {
            get { return _buttonImage; }
            set
            {
                _buttonImage = value;
                NotifyOfPropertyChange(() => ButtonImage);
            }
        }

        private bool _allOK;

        public bool AllOk
        {
            get { return _allOK; }
            set
            {
                _allOK = value;
                NotifyOfPropertyChange(() => AllOk);
            }
        }

        private string _projectDirectory;

        public string ProjectDirectory
        {
            get { return _projectDirectory; }
            set
            {
                _projectDirectory = value;
                NotifyOfPropertyChange(() => ProjectDirectory);
            }
        }


        #endregion

        public LeveranciersViewModel LeveranciersVM { get; set; }
        public SelectUserViewModel SelectUserVM { get; set; }
        public BestelbonsViewModel BestelbonsVM { get; set; }
        public PrijsvragenViewModel PrijsvragenVM { get; set; }
        public InnerShellViewModel InnerShellVM { get; set; }
        public ProjectShellViewModel ProjectShellVM { get; set; }
        public UsersViewModel UsersVM { get; set; }
        public LeveringsvoorwaardenViewModel LeveringsvoorwaardenVM { get; set; }
        public BindableCollection<User> UserList { get; set; }
        public BindableCollection<Project> ProjectsList { get; set; }
        public ConfigurationViewModel ConfigurationVM { get; set; }
        public ProjectViewModel ProjectsVM { get; set; }
        public LoadViewModel LoadVM { get; set; }
        public ImportElDocBestelbonsViewModel ImportElDocBestelbonsVM { get; set; }
        public LeverancierSuggestionProvider LVSuggestionProvider { get; set; }

        public ShellViewModel(IEventAggregator eventAggregator, IWindowManager windowmanager)
        {
            _windowManager = windowmanager;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            ButtonImage = "pack://application:,,,/Resources/MAXIMIZE.png";
            TooltipText = "Maximize";

            ////Properties.Settings.Default.Reset();

            //foreach (DriveInfo drive in DriveInfo.GetDrives())
            //{
            //    if (drive.DriveType == DriveType.Removable)
            //    {
            //        Console.WriteLine(string.Format("({0}) {1}", drive.Name.Replace("\\", ""), drive.VolumeLabel));
            //    }
            //}

            MaxHeight = screenHeight - 32;

            this.UserList = new BindableCollection<User>();
            this.ErrorList = new BindableCollection<Error>();
            this.Leverancierlist = new BindableCollection<Leverancier>();
            this.Bestelbonlist = new BindableCollection<Bestelbon>();
            this.Prijsvragenlist = new BindableCollection<Prijsvraag>();
            this.ProjectsList = new BindableCollection<Project>();
            this.ProjectIDlist = new BindableCollection<string>();
            this.LVSuggestionProvider = new LeverancierSuggestionProvider();


            // Code to keep the user settings when installing an new version !
            // When installing the initial UpdateSettings is true !!

            if (Properties.Settings.Default.UpdateSettings)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpdateSettings = false;
                Properties.Settings.Default.Save();
            }


            LoadVM = IoC.Get<LoadViewModel>();
            InnerShellVM = IoC.Get<InnerShellViewModel>();
            ConfigurationVM = IoC.Get<ConfigurationViewModel>();
            BestelbonsVM = IoC.Get<BestelbonsViewModel>();
            PrijsvragenVM = IoC.Get<PrijsvragenViewModel>();
            LeveranciersVM = IoC.Get<LeveranciersViewModel>();
            SelectUserVM = IoC.Get<SelectUserViewModel>();
            ProjectShellVM = IoC.Get<ProjectShellViewModel>();
            UsersVM = IoC.Get<UsersViewModel>();
            LeveringsvoorwaardenVM = IoC.Get<LeveringsvoorwaardenViewModel>();
            ProjectsVM = IoC.Get<ProjectViewModel>();
            ImportElDocBestelbonsVM = IoC.Get<ImportElDocBestelbonsViewModel>();

            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //Version = Version.Substring(0, Version.Length - 2);
            ModeBestelbon = true;

            //string addr = "";
            //foreach (NetworkInterface n in NetworkInterface.GetAllNetworkInterfaces())
            //{
            //    if (n.OperationalStatus == OperationalStatus.Up)
            //    {
            //        addr += n.GetPhysicalAddress().ToString();
            //        break;
            //    }
            //}

            ManagementClass managClass = new ManagementClass("win32_processor");
            ManagementObjectCollection managCollec = managClass.GetInstances();

            foreach (ManagementObject managObj in managCollec)
            {
                MACAdressThisPC = managObj.Properties["processorID"].Value.ToString();
                break;
            }
            eventAggregator.PublishOnUIThreadAsync(new MACAdressThisComputerEvent(MACAdressThisPC));

        }

        public async void OnWindowLoaded()
        {
            await InitAsync();
        }

        public async Task InitAsync()  // Used by ReloadAllFilesEvent
        {
            AllOk = false;
            LoadVisible = true;
            await Task.Run(async () =>
            {
                await LoadAllFiles();
                SetCurrentUser();
                LoadVisible = false;
                //CheckOK();
            }
           );
            _eventAggregator.PublishOnUIThreadAsync(new StopTimerEvent());
        }

        public async Task LoadAllFiles()
        {

            //Task LoadLeveranciersTask = LoadLeveranciers();
            //Task LoadProjectIDFileTask = LoadProjectIDFile();
            //Task LoadProjectsFileTask = LoadProjectsFile();
            Task LoadBestelbonsTask = LoadBestelbons();
            //Task LoadPrijsvragenTask = LoadPrijsvragen();

            //Task TasksAllDone = Task.WhenAll(LoadLeveranciersTask, LoadProjectIDFileTask, LoadProjectsFileTask, LoadBestelbonsTask, LoadPrijsvragenTask);
            Task TasksAllDone = Task.WhenAll(LoadBestelbonsTask);
            //Parallel.Invoke(

            //    () => LoadLeveranciers(),
            //    () => LoadProjectIDFile(),
            //    () => LoadProjectsFile(),
            //    () => LoadBestelbons(),
            //    () => LoadPrijsvragen()

            //    );

            try
            {
                await TasksAllDone;
            }
            catch (Exception e)
            {
                string Errorstring = String.Empty;
                AggregateException allExceptions = TasksAllDone.Exception;
                DialogViewModel ErrorVM = IoC.Get<DialogViewModel>();
                ErrorVM.Capiton = "LOADING Errors";
                ErrorVM.DialogStyle = DialogStyle.Ok;
                ErrorVM.FontsizeMessage = 16;

                foreach (var item in allExceptions.InnerExceptions)
                {
                    Errorstring = Errorstring + item.Message + "\n";
                }
                ErrorVM.Message = Errorstring + "\n\nEen of meerdere modules gaan niet werken !";

                Application.Current.Dispatcher.Invoke(new System.Action(() =>
                {
                    _windowManager.ShowDialogAsync(ErrorVM);
                }));

            }
        }



        #region LOAD FILES

        #region LOAD LEVERANCIERS
        public async Task LoadLeveranciers()
        {
            string FilePath = Properties.Settings.Default.LeveranciersListPath;
            XmlSerializer serializer = new XmlSerializer(typeof(BindableCollection<Leverancier>));
            if (File.Exists(FilePath))
            {
                using (var stream = System.IO.File.OpenRead(FilePath))
                {
                    try
                    {

                        Leverancierlist = serializer.Deserialize(stream) as BindableCollection<Leverancier>;
                        NotifyOfPropertyChange(() => CanLeveranciersList);
                        _eventAggregator.PublishOnUIThreadAsync(new LoadedLeveraciersEvent(Leverancierlist));
                    }
                    catch (Exception ex)
                    {
                        throw new NotImplementedException("Leveranciers not LOADED !");
                    }
                }
            }
        }
        #endregion

        #region LOAD PROJECTID FILE
        public async Task LoadProjectIDFile()
        {
            string FilePath = Properties.Settings.Default.ProjectIDPath;
            if (File.Exists(FilePath))
            {
                using (var stream = System.IO.File.OpenRead(FilePath))
                {
                    try
                    {
                        var serializer = new XmlSerializer(typeof(BindableCollection<string>));
                        ProjectIDlist = serializer.Deserialize(stream) as BindableCollection<string>;
                        _eventAggregator.PublishOnUIThreadAsync(new LoadedProjectIDFileEvent(ProjectIDlist));
                    }
                    catch (Exception)
                    {
                        throw new NotImplementedException("ProjectID's not LOADED !");
                    }
                }
            }
        }

        #endregion

        #region LOAD PROJECTSFILE
        public async Task LoadProjectsFile()
        {
            string FilePath = Properties.Settings.Default.ProjectsPath;
            XmlSerializer serializer = new XmlSerializer(typeof(BindableCollection<Project>));
            if (File.Exists(FilePath))
            {
                using (var stream = System.IO.File.OpenRead(FilePath))
                {
                    try
                    {

                        ProjectsList = serializer.Deserialize(stream) as BindableCollection<Project>;
                        NotifyOfPropertyChange(() => CanProjectList);
                        _eventAggregator.PublishOnUIThreadAsync(new LoadedProjectListEvent(ProjectsList));
                    }
                    catch (Exception)
                    {
                        throw new NotImplementedException("ProjectsFile not LOADED !");
                    }

                }
            }
        }
        #endregion

        #region LOAD BESTELBONS

        public async Task LoadBestelbons()
        {
            //bool NoException = true;

            //XmlSerializer serializer = new XmlSerializer(typeof(Bestelbon));

            //string FilePath = Properties.Settings.Default.BestelbonsPath;
            //if (Directory.Exists(FilePath))
            //{
            //    DirectoryInfo directoryInfo = new DirectoryInfo(FilePath);
            //    Bestelbonlist.Clear();
            //    System.IO.FileInfo[] allfiles = directoryInfo.GetFiles();
            //    Array.Reverse(allfiles);
            //    var nonprojectfiles = new List<System.IO.FileInfo>();
            //    var projectfiles = new List<System.IO.FileInfo>();
            //    var oddprojectfiles = new List<System.IO.FileInfo>();
            //    var files = new List<System.IO.FileInfo>();
            //    foreach (var fileinfo in allfiles)
            //    {
            //        if (!Char.IsLetter(fileinfo.Name[0]))
            //        {
            //            if (fileinfo.Name.Substring(0, 2) == "20") projectfiles.Add(fileinfo);
            //            else oddprojectfiles.Add(fileinfo);

            //        }
            //        else nonprojectfiles.Add(fileinfo);
            //    }

            //    projectfiles = projectfiles.OrderByDescending(x => x.Name).ToList();

            //    foreach (var fileinfo in projectfiles)
            //    {
            //        files.Add(fileinfo);
            //    }

            //    foreach (var fileinfo in oddprojectfiles)
            //    {
            //        files.Add(fileinfo);
            //    }

            //    foreach (var fileinfo in nonprojectfiles)
            //    {
            //        files.Add(fileinfo);
            //    }

            //    foreach (var file in files)
            //    {
            //        if (file.Extension.Contains("abb"))
            //        {
            //            Bestelbon bestelbon = new Bestelbon();
            //            using (var stream = System.IO.File.OpenRead(file.FullName))
            //            {
            //                try
            //                {

            //                    bestelbon = serializer.Deserialize(stream) as Bestelbon;
            //                }
            //                catch (Exception)
            //                {
            //                    NoException = false;
            //                    throw new NotImplementedException($"Bestelbon {Path.GetFileName(file.FullName)} not LOADED !");
            //                }
            //            }
            //            bestelbon.DeltaDeliveryTime = (int)(bestelbon.DeliveryTime).Subtract(DateTime.Now).TotalDays;
            //            Bestelbonlist.Add(bestelbon);

            //        }
            //    }

            //    if (NoException)
            //    {
            //        NotifyOfPropertyChange(() => CanBestelbonsList);
            //        _eventAggregator.PublishOnUIThreadAsync(new LoadedBestelbonsEvent(Bestelbonlist));
            //    }
            //}

            Bestelbon bestelbonnew = new Bestelbon();
            bestelbonnew.Name = "202201-FESTO-1";
            Bestelbonlist.Add(bestelbonnew);
            NotifyOfPropertyChange(() => CanBestelbonsList);
            _eventAggregator.PublishOnUIThreadAsync(new LoadedBestelbonsEvent(Bestelbonlist));
        }
        #endregion

        #region LOAD PRIJSVRAGEN
        public async Task LoadPrijsvragen()
        {
            bool NoException = true;
            XmlSerializer serializer = new XmlSerializer(typeof(Prijsvraag));
            string FilePath = Properties.Settings.Default.PrijsvragenPath;
            if (Directory.Exists(FilePath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(FilePath);
                Prijsvragenlist.Clear();
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
                    if (file.Extension.Contains("prv"))
                    {
                        Prijsvraag prijsvraag = new Prijsvraag();
                        using (var stream = System.IO.File.OpenRead(file.FullName))
                        {
                            try
                            {

                                prijsvraag = serializer.Deserialize(stream) as Prijsvraag;
                            }
                            catch (Exception)
                            {
                                NoException = false;
                                throw new NotImplementedException($"Prijsvraag {Path.GetFileName(file.FullName)} not LOADED !");
                            }
                        }
                        Prijsvragenlist.Add(prijsvraag);
                    }
                }
                if (NoException)
                {
                    NotifyOfPropertyChange(() => CanPrijsvraagList);
                    _eventAggregator.PublishOnUIThreadAsync(new LoadedPrijsvragenListEvent(Prijsvragenlist));
                }
            }
        }
        #endregion

        #endregion

        #region VERTICAL BUTTONS

        public void SwapBestelbonPrijsaanvraag()
        {
            if (ModeBestelbon)
            {
                ModeBestelbon = false;
                PrijsvraagList();

            }
            else
            {
                ModeBestelbon = true;
                BestelbonsList();
            }
        }

        public void LeveranciersList()
        {
            ActivateItemAsync(LeveranciersVM);
            ActiveItemVisible = true;

        }

        public void ProjectList()
        {
            ActivateItemAsync(ProjectsVM);
            ActiveItemVisible = true;

        }


        public void BestelbonsList()
        {
            ActivateItemAsync(BestelbonsVM);
            ActiveItemVisible = true;
        }

        public void PrijsvraagList()
        {
            ActivateItemAsync(PrijsvragenVM);
            ActiveItemVisible = true;
        }

        public void UsersList()
        {
            ActivateItemAsync(UsersVM);
            ActiveItemVisible = true;
        }

        public void Leveringsvoorwaarden()
        {
            ActivateItemAsync(LeveringsvoorwaardenVM);
            ActiveItemVisible = true;
            _eventAggregator.PublishOnUIThreadAsync(message: new LoadLeveringsvoorwaardenEvent());
        }

        public void ImportElDoc()
        {
            ActivateItemAsync(ImportElDocBestelbonsVM);
            ActiveItemVisible = true;
        }

        #endregion

        #region WINDOW BUTTONS

        public void MinimizeButton()
        {
            WindowState = WindowState.Minimized;
            var i = screenWidth;
            var j = screenHeight;
        }


        public void MaximizeButton()
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    WindowState = WindowState.Maximized;
                    ButtonImage = "pack://application:,,,/Resources/RESTORE.png";
                    TooltipText = "Restore Down";
                    break;
                case WindowState.Minimized:
                    break;
                case WindowState.Maximized:
                    WindowState = WindowState.Normal;
                    ButtonImage = "pack://application:,,,/Resources/MAXIMIZE.png";
                    TooltipText = "Maximize";
                    break;
                default:
                    break;
            }
        }

        public void CloseButton()
        {
            foreach (Window window in Application.Current.Windows)
            {
                window.Close();
            }
            TryCloseAsync();
        }

        #endregion

        #region MENUCOMMANDS

        public void Configuration()
        {
            _windowManager.ShowWindowAsync(ConfigurationVM);

        }

        public void SelectUser()
        {
            SetCurrentUser();
            SelectUserVM.Capiton = "Select User";
            var result = _windowManager.ShowDialogAsync(SelectUserVM);
            CheckOK();
        }


        public void SelectProjectDirectory()
        {
            _windowManager.ShowWindowAsync(ProjectShellVM);
        }

        public void Help()
        {
            //Process.Start("Bestelbons.chm");
        }

        #endregion

        public void SetCurrentUser()
        {
            string FilePath = Properties.Settings.Default.UserListPath;
            if (!String.IsNullOrEmpty(Properties.Settings.Default.UserListPath) && File.Exists(Properties.Settings.Default.UserListPath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(BindableCollection<User>));
                using (var stream = File.OpenRead(FilePath))
                {
                    var other = serializer.Deserialize(stream) as BindableCollection<User>;
                    UserList.Clear();
                    UserList.AddRange(other);
                }

                foreach (var user in UserList)
                {
                    string Handtekening = $"{Properties.Settings.Default.SignaturesPath}\\{user.FirstName}.png";
                    user.Handtekening = Handtekening;
                }

                foreach (var user in UserList)
                {
                    if (Properties.Settings.Default.CurrentUser == user.FirstName)
                    {
                        CurrentUser = user;
                        _eventAggregator.PublishOnUIThreadAsync(message: new InitialUserLoadedEvent(CurrentUser));
                        break;

                    }
                }
                _eventAggregator.PublishOnUIThreadAsync(message: new UserListChangedEvent(UserList));
            }
        }

        public void CheckOK()
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.BestelbonsPath) &&
                !string.IsNullOrEmpty(Properties.Settings.Default.UserListPath) &&
                !string.IsNullOrEmpty(Properties.Settings.Default.Leveringsvw) &&
                !string.IsNullOrEmpty(Properties.Settings.Default.LeveranciersListPath) &&
                File.Exists(Properties.Settings.Default.LeveranciersListPath) &&
                File.Exists(Properties.Settings.Default.Leveringsvw) &&
                Directory.Exists(Properties.Settings.Default.BestelbonsPath) &&
                File.Exists(Properties.Settings.Default.UserListPath) &&
                !string.IsNullOrEmpty(Properties.Settings.Default.CurrentUser)) AllOk = true;

            if (!string.IsNullOrEmpty(Properties.Settings.Default.BestelbonsPath) &&
                Directory.Exists(Properties.Settings.Default.BestelbonsPath)) UpdateErrorList(new Error { Level = ErrorLevel.Error, ErrorMessage = " Bestelbons Path not set", Active = false });
            else UpdateErrorList(new Error { Level = ErrorLevel.Error, ErrorMessage = " Bestelbons Path not set", Active = true });

            if (!String.IsNullOrEmpty(Properties.Settings.Default.UserListPath) && File.Exists(Properties.Settings.Default.UserListPath)) UpdateErrorList(new Error { Level = ErrorLevel.Error, ErrorMessage = " Userlist Path not set", Active = false });
            else UpdateErrorList(new Error { Level = ErrorLevel.Error, ErrorMessage = " Userlist Path not set", Active = true });

            if (!String.IsNullOrEmpty(Properties.Settings.Default.Leveringsvw) && File.Exists(Properties.Settings.Default.Leveringsvw)) UpdateErrorList(new Error { Level = ErrorLevel.Error, ErrorMessage = " Leveringsvoorwaarden Path not set", Active = false });
            else UpdateErrorList(new Error { Level = ErrorLevel.Error, ErrorMessage = " Leveringsvoorwaarden Path not set", Active = true });

            if (!String.IsNullOrEmpty(Properties.Settings.Default.LeveranciersListPath) && File.Exists(Properties.Settings.Default.LeveranciersListPath)) UpdateErrorList(new Error { Level = ErrorLevel.Error, ErrorMessage = " LeveranciersList Path not set", Active = false });
            else UpdateErrorList(new Error { Level = ErrorLevel.Error, ErrorMessage = " LeveranciersList Path not set", Active = true });

            if (!String.IsNullOrEmpty(Properties.Settings.Default.CurrentUser)) UpdateErrorList(new Error { Level = ErrorLevel.Error, ErrorMessage = " User not set", Active = false });
            else UpdateErrorList(new Error { Level = ErrorLevel.Error, ErrorMessage = " User not set", Active = true });
        }


        public void UpdateErrorList(Error error)
        {
            if (!error.Active)
            {
                foreach (var err in ErrorList)
                {
                    if (err.ErrorMessage == error.ErrorMessage)
                    {
                        ErrorList.Remove(err);
                        break;
                    }

                }
            }
            else
            {
                bool AlreadyInList = false;
                foreach (var err in ErrorList)
                {
                    if (err.ErrorMessage == error.ErrorMessage)
                    {
                        AlreadyInList = true;
                    }

                }
                if (!AlreadyInList)
                    ErrorList.Add(error);
            }

        }

        #region EVENT HANDLERS

        public Task HandleAsync(LeverancierChangedEvent message, CancellationToken cancellationToken)
        {
            // ActiveItemVisible = false;  //UNCOMMENT IF AUTO CLOSING OF LEVERANCIERSVIEW IS REQUIRED AFTER SELECTEDITEM IS CHANGED !
            return Task.CompletedTask;
        }

        public Task HandleAsync(UserChangedEvent message, CancellationToken cancellationToken)
        {
            if (message.user != null)
            {
                CurrentUser = message.user;
            }
            CheckOK();
            return Task.CompletedTask;
        }

        public Task HandleAsync(CloseActiveItemEvent message, CancellationToken cancellationToken)
        {
            ActiveItemVisible = false;
            return Task.CompletedTask;
        }

        public Task HandleAsync(RaiseErrorEvent error, CancellationToken cancellationToken)
        {
            UpdateErrorList(error.Error);
            return Task.CompletedTask;
        }


        public Task HandleAsync(ProjectDirectoryChangedEvent message, CancellationToken cancellationToken)
        {
            ProjectDir = message.ProjectDirectory;
            return Task.CompletedTask;
        }

        public Task HandleAsync(ReloadAllFilesEvent message, CancellationToken cancellationToken)
        {
            InitAsync();
            return Task.CompletedTask;
        }

        public Task HandleAsync(ConvertedPrijsvraagEvent message, CancellationToken cancellationToken)
        {
            SwapBestelbonPrijsaanvraag();
            BestelbonsList();
            return Task.CompletedTask;
        }

        #endregion

        #region INPUT GESTURES

        public void SearchItems()
        {

        }

        #endregion

    }
}
