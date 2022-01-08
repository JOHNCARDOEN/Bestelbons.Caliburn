using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WPF_Bestelbons.Events;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons.ViewModels
{
    public class ImportElDocBestelbonsViewModel : Screen
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;

        #region BINDABLE FIELDS

        private System.Windows.Forms.TreeNode _selectedNode;

        public System.Windows.Forms.TreeNode SelectedNode
        {
            get { return _selectedNode; }
            set { _selectedNode = value;
                NotifyOfPropertyChange(() => SelectedNode);
            }
        }



        private ObservableCollection<ElProjectBestelbonInfo> elProjectsList;

        public ObservableCollection<ElProjectBestelbonInfo> ElProjectsList
        {
            get { return elProjectsList; }
            set
            {
                elProjectsList = value;
                NotifyOfPropertyChange(() => ElProjectsList);
            }
        }


        private ObservableCollection<Bestelbon> _elDocumentationLijstUI;

        public ObservableCollection<Bestelbon> ElDocumentationLijstUI
        {
            get { return _elDocumentationLijstUI; }
            set
            {
                _elDocumentationLijstUI = value;
                NotifyOfPropertyChange(() => ElDocumentationLijstUI);
            }
        }

        private Bestelbon _elDocumentationSelectedItem;

        public Bestelbon ElDocumentationSelectedItem
        {
            get { return _elDocumentationSelectedItem; }
            set
            {
                _elDocumentationSelectedItem = value;
                NotifyOfPropertyChange(() => ElDocumentationSelectedItem);
            }
        }

        #endregion


        public ImportElDocBestelbonsViewModel(IEventAggregator eventAggregator, IWindowManager windowManager)
        {
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;

            this.ElProjectsList = new ObservableCollection<ElProjectBestelbonInfo>();
            this.ElDocumentationLijstUI = new ObservableCollection<Bestelbon>();

            LoadElDocumentation();
        }

        public void SetSelectedItem(object item)
        {
            if (item.GetType() == typeof(Bestelbon))
            {
                _eventAggregator.PublishOnUIThreadAsync(new ConvertElBestelbonDocuRequestEvent((Bestelbon)item));
            }
        }
        private void LoadElDocumentation()
        {
            ElProjectsList.Clear();
            string FilePath = Properties.Settings.Default.ElbestelijstenPath;
            if (Directory.Exists(FilePath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(FilePath);
                ElDocumentationLijstUI.Clear();
                System.IO.FileInfo[] allfiles = directoryInfo.GetFiles();
                var files = new List<System.IO.FileInfo>();
                foreach (var fileinfo in allfiles)
                {
                    files.Add(fileinfo);
                }
                foreach (var file in files)
                {
                    if (file.Extension.Contains(".eld"))
                    {
                        ElProjectBestelbonInfo ElBestInfo = new ElProjectBestelbonInfo();
                        if (file.Extension.Contains(".eld"))
                            ElBestInfo.Name = Path.GetFileNameWithoutExtension(file.Name);

                        using (var stream = System.IO.File.OpenRead(file.FullName))
                        {
                            try
                            {
                                var serializer = new XmlSerializer(typeof(ObservableCollection<Bestelbon>));
                                ElBestInfo.ElBestelbons = serializer.Deserialize(stream) as ObservableCollection<Bestelbon>;
                            }
                            catch (Exception)
                            {
                                throw new NotImplementedException($"El Documentationfile {Path.GetFileName(file.FullName)} not LOADED !");
                            }
                        }
                        ElProjectsList.Add(ElBestInfo);
                    }

                }
            }
        }

        public void GenerateBestelbon()
        {
            _eventAggregator.PublishOnUIThreadAsync(new SelectedElDocuBestelbonChangedEvent(ElDocumentationSelectedItem));
        }

        #region EVENT HANDLERS


        #endregion
    }
}
