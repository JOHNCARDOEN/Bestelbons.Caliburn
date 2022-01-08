using Caliburn.Micro;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WPF_Bestelbons.Events;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons.ViewModels
{
    public class ConfigurationProjectsViewModel : Screen, IHandle<LoadedProjectListEvent>
    {

        #region CANEXECUTE

        public bool CanAddProject
        {
            get { return (!(AddedProjectID == null) && (AddedProjectID > 0) && !string.IsNullOrEmpty(AddedProjectDescription)); }
        }
        public bool CanDelete
        {
            get { return (SelectedProject != null); }
        }
        #endregion

        #region BINDABLE FIELDS

        private bool _makeFolders;

        public bool MakeFolders
        {
            get { return _makeFolders; }
            set
            {
                _makeFolders = value;
                NotifyOfPropertyChange(() => MakeFolders);
            }
        }


        private Project _selectedProject;

        public Project SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                _selectedProject = value;
                NotifyOfPropertyChange(() => SelectedProject);
                NotifyOfPropertyChange(() => CanDelete);
            }
        }


        private BindableCollection<Project> _projectList;

        public BindableCollection<Project> ProjectList
        {
            get { return _projectList; }
            set
            {
                _projectList = value;
                NotifyOfPropertyChange(() => ProjectList);
            }
        }

        private int? _addedProjectID;

        public int? AddedProjectID
        {
            get { return _addedProjectID; }
            set
            {
                _addedProjectID = value;
                NotifyOfPropertyChange(() => AddedProjectID);
                NotifyOfPropertyChange(() => CanAddProject);
            }
        }

        private string _addedProjectDescription;

        public string AddedProjectDescription
        {
            get { return _addedProjectDescription; }
            set
            {
                _addedProjectDescription = value;
                NotifyOfPropertyChange(() => AddedProjectDescription);
                NotifyOfPropertyChange(() => CanAddProject);
            }
        }

        #endregion


        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;

        Project newproject;

        public ConfigurationProjectsViewModel(IEventAggregator eventAggregator, IWindowManager windowmanager)
        {
            _windowManager = windowmanager;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            this.ProjectList = new BindableCollection<Project>();
        }

        public void AddProject()
        {
            bool NotInList = true;
            foreach (var project in ProjectList)
            {
                if (AddedProjectID == project.ID) NotInList = false;
            }
            if (NotInList && !string.IsNullOrEmpty(AddedProjectDescription))
            {
                newproject = new Project();
                newproject.ID = AddedProjectID;
                newproject.Description = AddedProjectDescription;
                ProjectList.Add(newproject);
                newproject.PropertyChanged += ProjectListPropertyChanged;
            }

            // OrderBy does NOT change the original Collection !! Original Collection must be recreated !!!
            ProjectList = new BindableCollection<Project>(ProjectList.OrderBy(i => i.ID));

            if (MakeFolders)
            {
                string NewFolderName = $"{ AddedProjectID} {AddedProjectDescription}";

                if (Directory.Exists($"{Properties.Settings.Default.OneDrivePath}\\bvba\\PROJECTEN\\XXXX-XXXX PROJECTSJABLOON"))
                {
                    var from = $"{Properties.Settings.Default.OneDrivePath}\\bvba\\PROJECTEN\\XXXX-XXXX PROJECTSJABLOON";
                    var to = $"{Properties.Settings.Default.OneDrivePath}\\\\bvba\\PROJECTEN\\{NewFolderName}";
                    GenerateFolders(from, to, true);
                }
                MakeFolders = false;
            }

            Save();

            AddedProjectID = null;
            AddedProjectDescription = string.Empty;

        }

        public void Save()
        {
            var FilePath = Properties.Settings.Default.ProjectsPath;
            var serializer = new XmlSerializer(typeof(BindableCollection<Project>));
            using (var writer = new StreamWriter(FilePath))
            {
                serializer.Serialize(writer, ProjectList);
                writer.Flush();
                _eventAggregator.PublishOnUIThreadAsync(new ProjectListChangedEvent(ProjectList));
            }

        }

        public void ProjectListPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Save();
        }

        public void Delete()
        {
            ProjectList.Remove(SelectedProject);
            Save();
        }

        public void GenerateFolders(string sourceDirName, string destDirName, bool copySubDirs)
        {
            {
                DirectoryInfo dir = new DirectoryInfo(sourceDirName);
                DirectoryInfo[] dirs = dir.GetDirectories();

                // If the source directory does not exist, throw an exception.
                if (!dir.Exists)
                {
                    throw new DirectoryNotFoundException(
                        "Source directory does not exist or could not be found: "
                        + sourceDirName);
                }

                // If the destination directory does not exist, create it.
                if (!Directory.Exists(destDirName))
                {
                    Directory.CreateDirectory(destDirName);
                }


                // Get the file contents of the directory to copy.
                FileInfo[] files = dir.GetFiles();

                foreach (FileInfo file in files)
                {
                    // Create the path to the new copy of the file.
                    string temppath = Path.Combine(destDirName, file.Name);

                    // Copy the file.
                    file.CopyTo(temppath, false);
                }

                // If copySubDirs is true, copy the subdirectories.
                if (copySubDirs)
                {

                    foreach (DirectoryInfo subdir in dirs)
                    {
                        // Create the subdirectory.
                        string temppath = Path.Combine(destDirName, subdir.Name);

                        // Copy the subdirectories.
                        GenerateFolders(subdir.FullName, temppath, copySubDirs);
                    }
                }
            }
        }


        #region EVENTHANDLERS
        public Task HandleAsync(LoadedProjectListEvent message, CancellationToken cancellationToken)
        {
            ProjectList = message.ProjectList;
            foreach (var project in ProjectList)
            {
                project.PropertyChanged += ProjectListPropertyChanged;
            }
            return Task.CompletedTask;
        }
        #endregion
    }
}
