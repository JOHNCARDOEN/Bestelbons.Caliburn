using Caliburn.Micro;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Data;
using WPF_Bestelbons.Events;

namespace WPF_Bestelbons.ViewModels
{
    public class ConfigurationDirectoriesViewModel : Screen
    {
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;

        public ObservableCollection<DirectoryConfiguration> Root { get; set; }


        #region CANEXECUTE


        public bool CanSet { get { return !String.IsNullOrEmpty(SelectedDirectoryItem); } }
        #endregion

        #region BINDABLE FIELDS

        private string _bestelbonsRootDir;

        public string BestelbonsRootDir
        {
            get { return Properties.Settings.Default.BestelbonsRootDirectory; }
            set
            {
                _bestelbonsRootDir = value;
                NotifyOfPropertyChange(() => BestelbonsRootDir);
            }
        }


        private string _signatures;

        public string Signatures
        {
            get { return Properties.Settings.Default.SignaturesPath; }
            set
            {
                _signatures = value;
                NotifyOfPropertyChange(() => Signatures);
            }
        }


        private string _initialRoot;

        public string InitialRoot
        {
            get { return _initialRoot; }
            set
            {
                _initialRoot = value;
                NotifyOfPropertyChange(() => InitialRoot);
            }
        }


        private string _leveranciersListFile;

        public string LeveranciersListFile
        {
            get { return Properties.Settings.Default.LeveranciersListPath; }
            set
            {
                _leveranciersListFile = value;
                NotifyOfPropertyChange(() => LeveranciersListFile);
            }
        }

        private string _bestelbonsDirectory;

        public string BestelbonsDirectory
        {
            get { return Properties.Settings.Default.BestelbonsPath; }
            set
            {
                _bestelbonsDirectory = value;
                NotifyOfPropertyChange(() => BestelbonsDirectory);
            }
        }

        private string _prijsvragenDirectory;

        public string PrijsvragenDirectory
        {
            get { return Properties.Settings.Default.PrijsvragenPath; }
            set
            {
                _prijsvragenDirectory = value;
                NotifyOfPropertyChange(() => PrijsvragenDirectory);
            }
        }


        private string _userListFile;

        public string UserListFile
        {
            get { return Properties.Settings.Default.UserListPath; }
            set
            {
                _userListFile = value;
                NotifyOfPropertyChange(() => UserListFile);
            }
        }

        private string _leveringsvoorwaardenFile;

        public string LeveringsvoorwaardenFile
        {
            get { return Properties.Settings.Default.Leveringsvw; }
            set
            {
                _leveringsvoorwaardenFile = value;
                NotifyOfPropertyChange(() => LeveringsvoorwaardenFile);
            }
        }

        private string _projectDirectoryRoot;

        public string ProjectDirectoryRoot
        {
            get { return Properties.Settings.Default.ProjectDirectory; }
            set
            {
                _projectDirectoryRoot = value;
                NotifyOfPropertyChange(() => ProjectDirectoryRoot);
            }
        }

        private string _dataDirectory;

        public string DataDirectory
        {
            get { return Properties.Settings.Default.DataDirectory; }
            set
            {
                _dataDirectory = value;
                NotifyOfPropertyChange(() => DataDirectory);
            }
        }

        private string _oneDrivePath;

        public string OneDrivePath
        {
            get { return Properties.Settings.Default.OneDrivePath; }
            set
            {
                _oneDrivePath = value;
                NotifyOfPropertyChange(() => OneDrivePath);
            }
        }


        private string _selectedDirectoryItem;

        public string SelectedDirectoryItem
        {
            get { return _selectedDirectoryItem; }
            set
            {
                _selectedDirectoryItem = value;
                NotifyOfPropertyChange(() => SelectedDirectoryItem);
                NotifyOfPropertyChange(() => CanSet);
            }
        }

        private string _selectedFileItem;

        public string SelectedFileItem
        {
            get { return _selectedFileItem; }
            set
            {
                _selectedFileItem = value;
                NotifyOfPropertyChange(() => SelectedFileItem);
            }
        }

        private string _projectIDFile;

        public string ProjectIDFile
        {
            get { return Properties.Settings.Default.ProjectIDPath; }
            set
            {
                _projectIDFile = value;
                NotifyOfPropertyChange(() => ProjectIDFile);
            }
        }

        private string _projectsFile;

        public string ProjectsFile
        {
            get { return Properties.Settings.Default.ProjectsPath; }
            set
            {
                _projectsFile = value;
                NotifyOfPropertyChange(() => ProjectsFile);
            }
        }

        private string _elbestelijstenListDir;

        public string ElbestelijstenListDir
        {
            get { return Properties.Settings.Default.ElbestelijstenPath; }
            set
            {
                _elbestelijstenListDir = value;
                NotifyOfPropertyChange(() => ElbestelijstenListDir);
            }
        }




        #endregion
        public ConfigurationDirectoriesViewModel(IEventAggregator eventAggregator, IWindowManager windowmanager)
        {
            _windowManager = windowmanager;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            InitialRoot = Properties.Settings.Default.InitialRoot;

            this.Root = new ObservableCollection<DirectoryConfiguration>();
            InitialRoot = Properties.Settings.Default.InitialRoot;
            //SetInitialRootDirectory();
            GetAllDrives();
            NotifyOfPropertyChange(() => CanSet);
        }

        public void SetInitialRootDirectory()  // OBSOLETE vervangen door GetAllDrives
        {
            if (!string.IsNullOrEmpty(InitialRoot))
            {
                var InitRootLetter = Properties.Settings.Default.InitialRoot;

                if (Directory.Exists($"{InitRootLetter}:\\"))
                {
                    try
                    {
                        Root.Clear();
                        DirectoryConfiguration first = new DirectoryConfiguration($"{InitRootLetter}:\\");
                        foreach (var file in first.Files)
                        {
                            file.FullName = $"{InitRootLetter}:\\" + file.FileName;
                        }
                        first.IteminDirectoriesChangedEvent += First_IteminDirectoriesChangedEvent;
                        first.IteminFilesChangedEvent += First_IteminFilesChangedEvent;
                        Root.Add(first);
                    }
                    catch (Exception)
                    {

                    }
                }
            }

        }

        public void GetAllDrives()
        {
            Root.Clear();
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {

                try
                {

                    DirectoryConfiguration first = new DirectoryConfiguration(d.Name);
                    foreach (var file in first.Files)
                    {

                        file.FullName = d.Name + file.FileName;
                    }
                    first.IteminDirectoriesChangedEvent += First_IteminDirectoriesChangedEvent;
                    first.IteminFilesChangedEvent += First_IteminFilesChangedEvent;
                    Root.Add(first);
                }
                catch (Exception)
                {

                }
            }
        }


        public void SetAllDirsAndFiles()
        {
            string BestelbonsDir = $"{Properties.Settings.Default.BestelbonsRootDirectory}\\BESTELBONS";

            Properties.Settings.Default.BestelbonsPath = BestelbonsDir;
            // User scope settings gebruikt omdat we anders niet kunnen saven ! Application scope kun NIET in runtime veranderen !!!
            Properties.Settings.Default.Save();
            NotifyOfPropertyChange(() => BestelbonsDirectory);

            string PrijsvragenDir = $"{Properties.Settings.Default.BestelbonsRootDirectory}\\PRIJSVRAGEN";

            Properties.Settings.Default.PrijsvragenPath = PrijsvragenDir;
            // User scope settings gebruikt omdat we anders niet kunnen saven ! Application scope kun NIET in runtime veranderen !!!
            Properties.Settings.Default.Save();
            NotifyOfPropertyChange(() => PrijsvragenDirectory);

            string Signatures = $"{Properties.Settings.Default.BestelbonsRootDirectory}\\DATA\\SIGNATURES";

            Properties.Settings.Default.SignaturesPath = Signatures;
            // User scope settings gebruikt omdat we anders niet kunnen saven ! Application scope kun NIET in runtime veranderen !!!
            Properties.Settings.Default.Save();
            NotifyOfPropertyChange(() => Signatures);

            string LeveranciersList = $"{Properties.Settings.Default.BestelbonsRootDirectory}\\DATA\\Leveranciers2GEN" +
                $".lev";

            Properties.Settings.Default.LeveranciersListPath = LeveranciersList;
            // User scope settings gebruikt omdat we anders niet kunnen saven ! Application scope kun NIET in runtime veranderen !!!
            Properties.Settings.Default.Save();
            NotifyOfPropertyChange(() => LeveranciersListFile);

            string UserList = $"{Properties.Settings.Default.BestelbonsRootDirectory}\\DATA\\AstratecUsers.users";

            Properties.Settings.Default.UserListPath = UserList;
            // User scope settings gebruikt omdat we anders niet kunnen saven ! Application scope kun NIET in runtime veranderen !!!
            Properties.Settings.Default.Save();
            NotifyOfPropertyChange(() => UserListFile);

            string Leveringsvoorwaarden = $"{Properties.Settings.Default.BestelbonsRootDirectory}\\DATA\\Leveringsvoorwaarden.levvw";

            Properties.Settings.Default.Leveringsvw = Leveringsvoorwaarden;
            // User scope settings gebruikt omdat we anders niet kunnen saven ! Application scope kun NIET in runtime veranderen !!!
            Properties.Settings.Default.Save();
            NotifyOfPropertyChange(() => LeveringsvoorwaardenFile);

            string DataDir = $"{Properties.Settings.Default.BestelbonsRootDirectory}\\DATA";

            Properties.Settings.Default.DataDirectory = DataDir;
            // User scope settings gebruikt omdat we anders niet kunnen saven ! Application scope kun NIET in runtime veranderen !!!
            Properties.Settings.Default.Save();
            NotifyOfPropertyChange(() => DataDirectory);

            string ProjectID = $"{Properties.Settings.Default.BestelbonsRootDirectory}\\DATA\\ProjectID.prj";

            Properties.Settings.Default.ProjectIDPath = ProjectID;
            // User scope settings gebruikt omdat we anders niet kunnen saven ! Application scope kun NIET in runtime veranderen !!!
            Properties.Settings.Default.Save();
            NotifyOfPropertyChange(() => ProjectIDFile);

            string ProjectsList = $"{Properties.Settings.Default.BestelbonsRootDirectory}\\DATA\\Projects.pr";

            Properties.Settings.Default.ProjectsPath = ProjectsList;
            // User scope settings gebruikt omdat we anders niet kunnen saven ! Application scope kun NIET in runtime veranderen !!!
            Properties.Settings.Default.Save();
            NotifyOfPropertyChange(() => ProjectsFile);

            string ElbestelijstenDirectory = $"{Properties.Settings.Default.BestelbonsRootDirectory}\\ELBESTELLIJSTEN";

            Properties.Settings.Default.ElbestelijstenPath = ElbestelijstenDirectory;
            // User scope settings gebruikt omdat we anders niet kunnen saven ! Application scope kun NIET in runtime veranderen !!!
            Properties.Settings.Default.Save();
            NotifyOfPropertyChange(() => ElbestelijstenListDir);


            _eventAggregator.PublishOnUIThreadAsync(new ReloadAllFilesEvent());

        }

        #region COMMANDS
        public void SetBestelBonRootDirectory()
        {
            Properties.Settings.Default.BestelbonsRootDirectory = SelectedDirectoryItem;
            // User scope settings gebruikt omdat we anders niet kunnen saven ! Application scope kun NIET in runtime veranderen !!!
            Properties.Settings.Default.Save();
            NotifyOfPropertyChange(() => BestelbonsRootDir);
            SetAllDirsAndFiles();
        }
        public void SetSignatures()
        {
            Properties.Settings.Default.SignaturesPath = SelectedDirectoryItem;
            // User scope settings gebruikt omdat we anders niet kunnen saven ! Application scope kun NIET in runtime veranderen !!!
            Properties.Settings.Default.Save();
            NotifyOfPropertyChange(() => Signatures);
            _eventAggregator.PublishOnUIThreadAsync(new ReloadAllFilesEvent());
        }

        public void SetLeveranciersList()
        {
            Properties.Settings.Default.LeveranciersListPath = SelectedFileItem;
            // User scope settings gebruikt omdat we anders niet kunnen saven ! Application scope kun NIET in runtime veranderen !!!
            Properties.Settings.Default.Save();
            NotifyOfPropertyChange(() => LeveranciersListFile);
            _eventAggregator.PublishOnUIThreadAsync(new ReloadAllFilesEvent());
        }

        public void SetBestelbonsDir()
        {
            Properties.Settings.Default.BestelbonsPath = SelectedDirectoryItem;
            // User scope settings gebruikt omdat we anders niet kunnen saven ! Application scope kun NIET in runtime veranderen !!!
            Properties.Settings.Default.Save();
            NotifyOfPropertyChange(() => BestelbonsDirectory);
            _eventAggregator.PublishOnUIThreadAsync(new ReloadAllFilesEvent());
        }

        public void SetUserList()
        {
            Properties.Settings.Default.UserListPath = SelectedFileItem;
            // User scope settings gebruikt omdat we anders niet kunnen saven ! Application scope kun NIET in runtime veranderen !!!
            Properties.Settings.Default.Save();
            NotifyOfPropertyChange(() => UserListFile);
            _eventAggregator.PublishOnUIThreadAsync(new ReloadAllFilesEvent());
        }
        public void SetLeveringsVoorwaarden()
        {
            Properties.Settings.Default.Leveringsvw = SelectedFileItem;
            // User scope settings gebruikt omdat we anders niet kunnen saven ! Application scope kun NIET in runtime veranderen !!!
            Properties.Settings.Default.Save();
            NotifyOfPropertyChange(() => LeveringsvoorwaardenFile);
            _eventAggregator.PublishOnUIThreadAsync(new ReloadAllFilesEvent());
        }

        public void SetProjectDirectoryRoot()
        {
            Properties.Settings.Default.ProjectDirectory = SelectedDirectoryItem;
            // User scope settings gebruikt omdat we anders niet kunnen saven ! Application scope kun NIET in runtime veranderen !!!
            Properties.Settings.Default.Save();
            NotifyOfPropertyChange(() => ProjectDirectoryRoot);
        }

        public void SetDataDirectory()
        {
            Properties.Settings.Default.DataDirectory = SelectedDirectoryItem;
            // User scope settings gebruikt omdat we anders niet kunnen saven ! Application scope kun NIET in runtime veranderen !!!
            Properties.Settings.Default.Save();
            NotifyOfPropertyChange(() => DataDirectory);
            _eventAggregator.PublishOnUIThreadAsync(new ReloadAllFilesEvent());
        }

        public void SetProjectIDList()
        {
            Properties.Settings.Default.ProjectIDPath = SelectedFileItem;
            // User scope settings gebruikt omdat we anders niet kunnen saven ! Application scope kun NIET in runtime veranderen !!!
            Properties.Settings.Default.Save();
            NotifyOfPropertyChange(() => ProjectIDFile);
            _eventAggregator.PublishOnUIThreadAsync(new ReloadAllFilesEvent());
        }

        public void SetProjectsList()
        {
            Properties.Settings.Default.ProjectsPath = SelectedFileItem;
            // User scope settings gebruikt omdat we anders niet kunnen saven ! Application scope kun NIET in runtime veranderen !!!
            Properties.Settings.Default.Save();
            NotifyOfPropertyChange(() => ProjectsFile);
            _eventAggregator.PublishOnUIThreadAsync(new ReloadAllFilesEvent());
        }

        public void SetRootDirectory()
        {
            Properties.Settings.Default.InitialRoot = InitialRoot;
            // User scope settings gebruikt omdat we anders niet kunnen saven ! Application scope kun NIET in runtime veranderen !!!
            Properties.Settings.Default.Save();
            NotifyOfPropertyChange(() => InitialRoot);
            SetInitialRootDirectory();
        }

        public void SetPrijsvragensDir()
        {
            Properties.Settings.Default.PrijsvragenPath = SelectedDirectoryItem;
            // User scope settings gebruikt omdat we anders niet kunnen saven ! Application scope kun NIET in runtime veranderen !!!
            Properties.Settings.Default.Save();
            NotifyOfPropertyChange(() => ElbestelijstenListDir);
            _eventAggregator.PublishOnUIThreadAsync(new ReloadAllFilesEvent());
        }

        public void SetElDocDir()
        {
            Properties.Settings.Default.ElbestelijstenPath = SelectedDirectoryItem;
            // User scope settings gebruikt omdat we anders niet kunnen saven ! Application scope kun NIET in runtime veranderen !!!
            Properties.Settings.Default.Save();
            NotifyOfPropertyChange(() => PrijsvragenDirectory);
            _eventAggregator.PublishOnUIThreadAsync(new ReloadAllFilesEvent());
        }
        public void SetOneDrivePath()
        {
            Properties.Settings.Default.OneDrivePath = SelectedDirectoryItem;
            // User scope settings gebruikt omdat we anders niet kunnen saven ! Application scope kun NIET in runtime veranderen !!!
            Properties.Settings.Default.Save();
            NotifyOfPropertyChange(() => OneDrivePath);
            _eventAggregator.PublishOnUIThreadAsync(new ReloadAllFilesEvent());
        }

        #endregion

        private void First_IteminDirectoriesChangedEvent(object sender, DirectoryConfiguration dir)
        {
            SelectedDirectoryItem = dir.DirectoryFullName;

        }

        private void First_IteminFilesChangedEvent(object sender, FileInDir file)
        {
            SelectedFileItem = file.FullName;
        }

    }
    public class DirectoryConfiguration : PropertyChangedBase
    {

        public event EventHandler<FileInDir> IteminFilesChangedEvent;
        public event EventHandler<DirectoryConfiguration> IteminDirectoriesChangedEvent;
        public DirectoryInfo Info { get; set; }
        public DirectoryConfiguration DummyChild { get; set; }

        public bool HasDummyChild { get; set; }
        public string DirectoryName { get; set; }

        public string DirectoryFullName { get; set; }

        private string _selectedItem;

        public string SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);
            }
        }


        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                IteminDirectoriesChangedEvent?.Invoke(this, this);
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        private bool _isExpanded;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    NotifyOfPropertyChange(() => IsExpanded);
                }
                if (this.HasDummyChild)
                {
                    this.Directories.Remove(DummyChild);
                    this.HasDummyChild = false;
                    this.LoadChildren(this.DirectoryFullName);
                }
            }
        }

        public DirectoryConfiguration()
        {

            this.Directories = new ObservableCollection<DirectoryConfiguration>();
            this.Files = new ObservableCollection<FileInDir>();
        }

        public DirectoryConfiguration(string path)
        {
            this.Directories = new ObservableCollection<DirectoryConfiguration>();
            this.Files = new ObservableCollection<FileInDir>();

            Info = new DirectoryInfo(path);
            DirectoryName = Info.Name;
            DirectoryFullName = Info.FullName;

            DirectoryInfo[] diArr = Info.GetDirectories();
            foreach (var dir in diArr)
            {
                DirectoryConfiguration directory = new DirectoryConfiguration();
                directory.DirectoryName = dir.Name;
                directory.DirectoryFullName = dir.FullName;
                directory.IteminFilesChangedEvent += Directory_IteminFilesChangedEvent;
                directory.IteminDirectoriesChangedEvent += Directory_IteminDirectoryChangedEvent;
                directory.IteminFilesChangedEvent += Directory_IteminFilesChangedEvent;

                try
                {
                    if ((dir.Attributes & FileAttributes.Hidden) == 0)
                    {
                        if ((dir.GetDirectories()).Length > 0)
                        {
                            directory.Directories.Add(DummyChild);
                            directory.HasDummyChild = true;
                        }
                    }
                }
                catch (Exception)
                {

                }


                Directories.Add(directory);

            }

            // ENKEL DE DIRECTORIES NODIG !!!

            foreach (var fil in Info.GetFiles())
            {
                FileInDir file = new FileInDir();
                file.FileName = fil.Name;
                file.IteminFilesChangedEvent += Directory_IteminFilesChangedEvent;
                Files.Add(file);
            }


        }

        public ObservableCollection<DirectoryConfiguration> Directories { get; set; }
        public ObservableCollection<FileInDir> Files { get; set; }


        public IList Children
        {
            get
            {

                return new CompositeCollection()
                    {
                     new CollectionContainer() { Collection = Directories },
                     new CollectionContainer() { Collection = Files }

                    };
            }
        }

        public void LoadChildren(string path)
        {
            Info = new DirectoryInfo(path);
            //  DirectoryName = Info.Name;
            var HasAccess = false;
            DirectoryInfo[] diArr = Info.GetDirectories();
            foreach (var dir in diArr)
            {

                try
                {
                    DirectoryConfiguration directory = new DirectoryConfiguration();
                    directory.DirectoryName = dir.Name;
                    directory.DirectoryFullName = dir.FullName;

                    if ((dir.GetDirectories()).Length > 0 || (dir.GetFiles()).Length > 0)
                    {
                        directory.Directories.Add(DummyChild);
                        directory.HasDummyChild = true;
                    }

                    directory.IteminDirectoriesChangedEvent += Directory_IteminDirectoryChangedEvent;
                    directory.IteminFilesChangedEvent += Directory_IteminFilesChangedEvent;
                    Directories.Add(directory);
                }
                catch (Exception)
                {


                }

            }

            // ENKEL DE DIRECTORIES NODIG !!!

            foreach (var fil in Info.GetFiles())
            {
                FileInDir file = new FileInDir();
                file.FileName = fil.Name;
                file.FullName = fil.FullName;
                file.IteminFilesChangedEvent += Directory_IteminFilesChangedEvent;
                Files.Add(file);
            }
        }

        private void Directory_IteminFilesChangedEvent(object sender, FileInDir e)
        {
            IteminFilesChangedEvent?.Invoke(this, e);
        }

        private void Directory_IteminDirectoryChangedEvent(object sender, DirectoryConfiguration e)
        {
            IteminDirectoriesChangedEvent?.Invoke(this, e);
        }

    }



    public class FileInDir : PropertyChangedBase
    {
        public event EventHandler<FileInDir> IteminFilesChangedEvent;
        public string FileName { get; set; }
        public string FullName { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                IteminFilesChangedEvent?.Invoke(this, this);
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                NotifyOfPropertyChange(() => IsExpanded);
            }

        }


    }


}
