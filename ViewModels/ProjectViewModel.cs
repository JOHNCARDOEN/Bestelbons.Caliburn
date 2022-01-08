using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPF_Bestelbons.Events;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons.ViewModels
{
   public class ProjectViewModel :Screen, IHandle<LoadedProjectListEvent>, IHandle<ProjectListChangedEvent>
    {
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;

        private BindableCollection<Project> SelectedProjectList;

        #region BINDABLE FIELDS

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

        private BindableCollection<Project> _projectListUI;

        public BindableCollection<Project> ProjectListUI
        {
            get { return _projectListUI; }
            set
            {
                _projectListUI = value;
                NotifyOfPropertyChange(() => ProjectListUI);
            }
        }

        private Project _projectsSelectedItem;

        public Project ProjectsSelectedItem
        {
            get { return _projectsSelectedItem; }
            set { _projectsSelectedItem = value;
                NotifyOfPropertyChange(() => ProjectsSelectedItem);
                if (ProjectsSelectedItem != null)
                {
                    _eventAggregator.PublishOnUIThreadAsync(message: new SelectedProjectChangedEvent(ProjectsSelectedItem));
                }
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

        public ProjectViewModel(IEventAggregator eventAggregator, IWindowManager windowmanager)
        {
            _windowManager = windowmanager;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);
            
            SelectedProjectList = new BindableCollection<Project>();
            ProjectListUI = new BindableCollection<Project>();
        }

        public void Select()
        {
            SelectedProjectList.Clear();
            string SelectionLower = Search.ToLower();
            string[] substrings = SelectionLower.Split('+');

            foreach (var project in ProjectList)
            {
                bool ProjectSelected = substrings.All(subs => project.Description.Contains(subs, StringComparison.OrdinalIgnoreCase));

                if (ProjectSelected )
                {
                    SelectedProjectList.Add(project);
                }
            }
            ProjectListUI = SelectedProjectList;
            if (string.IsNullOrEmpty(Search)) ProjectListUI = ProjectList;

        }

        public void ClearFilter()
        {
            Search = "";
            Select();
        }

        #region EVENTHANDLERS
        public Task HandleAsync(LoadedProjectListEvent message, CancellationToken cancellationToken)
        {
            ProjectList = message.ProjectList;
            Search = "";
            Select();
            return Task.CompletedTask;
        }

        public Task HandleAsync(ProjectListChangedEvent message, CancellationToken cancellationToken)
        {
            ProjectList = message.ProjectList;
            Select();
            return Task.CompletedTask;
        }
        #endregion
    }
}
