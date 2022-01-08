using Caliburn.Micro;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;
using WPF_Bestelbons.Events;

namespace WPF_Bestelbons.ViewModels
{


    public class ProjectDirectoryViewModel : Screen
    {
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;
        ProjectDirectory first;

        #region BINDABLE FIELDS
        private ObservableCollection<ProjectDirectory> _root;

        public ObservableCollection<ProjectDirectory> Root
        {
            get { return _root; }
            set
            {
                _root = value;
                NotifyOfPropertyChange(() => Root);
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
            }
        }
        #endregion


        public ProjectDirectoryViewModel(IEventAggregator eventAggregator, IWindowManager windowsmanager)
        {
            _eventAggregator = eventAggregator;
            _windowManager = windowsmanager;

            this.Root = new ObservableCollection<ProjectDirectory>();
            if (!string.IsNullOrEmpty(Properties.Settings.Default.ProjectDirectory) && Directory.Exists(Properties.Settings.Default.ProjectDirectory))
            {
                ProjectDirectory first = new ProjectDirectory(Properties.Settings.Default.ProjectDirectory);
                first.IteminDirectoriesChangedEvent += First_IteminDirectoriesChangedEvent;
                first.IsExpanded = true;
                Root.Add(first);
            }

        }




        private void First_IteminDirectoriesChangedEvent(object sender, ProjectDirectory dir)
        {
            SelectedDirectoryItem = dir.DirectoryFullName;
        }

        #region COMMANDS
        public void SetProjectDirectory()
        {
            _eventAggregator.PublishOnUIThreadAsync(message: new ProjectDirectoryChangedEvent(SelectedDirectoryItem));
        }


        #endregion
    }
    public class ProjectDirectory : PropertyChangedBase
    {

        public event EventHandler<ProjectDirectory> IteminDirectoriesChangedEvent;
        public DirectoryInfo Info { get; set; }
        public ProjectDirectory DummyChild { get; set; }

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

        public ProjectDirectory()
        {

            this.Directories = new ObservableCollection<ProjectDirectory>();
            this.Files = new ObservableCollection<FileInDir>();
        }

        public ProjectDirectory(string path)
        {
            this.Directories = new ObservableCollection<ProjectDirectory>();
            this.Files = new ObservableCollection<FileInDir>();
            Info = new DirectoryInfo(path);
            DirectoryName = Info.Name;
            DirectoryFullName = Info.FullName;

            DirectoryInfo[] diArr = Info.GetDirectories();
            foreach (var dir in diArr)
            {
                Regex regex = new Regex(@"[0-9]{6}");
                Match match = regex.Match(dir.Name);
                if (match.Success)
                {
                    ProjectDirectory directory = new ProjectDirectory();
                    directory.DirectoryName = dir.Name;
                    directory.DirectoryFullName = dir.FullName;
                    directory.IteminDirectoriesChangedEvent += Directory_DirectoryItemChangedEvent;
                    if ((dir.GetDirectories()).Length > 0)
                    {
                        directory.Directories.Add(DummyChild);
                        directory.HasDummyChild = true;
                    }

                    Directories.Add(directory);

                };


            }
            Directories = new ObservableCollection<ProjectDirectory>(Directories.OrderBy(x => x.DirectoryName));
            // ENKEL DE DIRECTORIES NODIG !!!

            //foreach (var fil in Info.GetFiles())
            //{
            //    FileInProjDir file = new FileInDir();
            //    file.FileName = fil.Name;
            //    Files.Add(file);
            //}


        }

        public ObservableCollection<ProjectDirectory> Directories { get; set; }
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

            DirectoryInfo[] diArr = Info.GetDirectories();
            foreach (var dir in diArr)
            {
                ProjectDirectory directory = new ProjectDirectory();
                directory.DirectoryName = dir.Name;
                directory.DirectoryFullName = dir.FullName;
                if ((dir.GetDirectories()).Length > 0)
                {
                    directory.Directories.Add(DummyChild);
                    directory.HasDummyChild = true;
                }
                directory.IteminDirectoriesChangedEvent += Directory_DirectoryItemChangedEvent;
                Directories.Add(directory);

            }

            // ENKEL DE DIRECTORIES NODIG !!!

            //foreach (var fil in Info.GetFiles())
            //{
            //    FileInProjDir file = new FileInDir();
            //    file.FileName = fil.Name;
            //    Files.Add(file);
            //}
        }

        private void Directory_DirectoryItemChangedEvent(object sender, ProjectDirectory e)
        { 
           IteminDirectoriesChangedEvent?.Invoke(this, e);
        }



    }

    public class FileInProjDir : PropertyChangedBase
    {
        public string FileName { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
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
