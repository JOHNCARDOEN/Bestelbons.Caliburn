using Caliburn.Micro;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WPF_Bestelbons.Events;

namespace WPF_Bestelbons.ViewModels
{
    public class ConfigurationProjectIDViewModel : Screen, IHandle<LoadedProjectIDFileEvent>
    {
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;

        #region BINDABLE FIELDS

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

        private string _addedProjectID;

        public string AddedProjectID
        {
            get { return _addedProjectID; }
            set
            {
                _addedProjectID = value;
                NotifyOfPropertyChange(() => AddedProjectID);
            }
        }


        #endregion

        public ConfigurationProjectIDViewModel(IEventAggregator eventAggregator, IWindowManager windowmanager)
        {
            _windowManager = windowmanager;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            this.ProjectIDList = new BindableCollection<string>();

        }

        public void AddProjectID()
        {
            bool NotInList = true;
            foreach (var projectid in ProjectIDList)
            {
                if (AddedProjectID == projectid) NotInList = false;
            }
            if (NotInList && !string.IsNullOrEmpty(AddedProjectID)) ProjectIDList.Add(AddedProjectID);

            var FilePath = Properties.Settings.Default.ProjectIDPath;
            var serializer = new XmlSerializer(typeof(BindableCollection<string>));
            using (var writer = new StreamWriter(FilePath))
            {
                serializer.Serialize(writer, ProjectIDList);
                writer.Flush();
                _eventAggregator.PublishOnUIThreadAsync(new ProjectIDChangedEvent(ProjectIDList));
            }

        }

        #region EVENTHANDLER
        public Task HandleAsync(LoadedProjectIDFileEvent message, CancellationToken cancellationToken)
        {
            ProjectIDList = message.ProjectIDlist;
            return Task.CompletedTask;
        }
        #endregion
    }
}
