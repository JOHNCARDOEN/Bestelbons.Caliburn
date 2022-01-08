using Caliburn.Micro;

namespace WPF_Bestelbons.ViewModels
{
    public class ConfigurationViewModel : Conductor<object>
    {

        public  ConfigurationDirectoriesViewModel ConfigurationDirectoriesVM  { get; set; }
        public ConfigurationProjectIDViewModel ConfigurationProjectIDVM { get; set; }
        public ConfigurationProjectsViewModel ConfigurationProjectsVM { get; set; }


        public ConfigurationViewModel()
        {
            ConfigurationDirectoriesVM = IoC.Get<ConfigurationDirectoriesViewModel>();
            ConfigurationProjectIDVM = IoC.Get<ConfigurationProjectIDViewModel>();
            ConfigurationProjectsVM = IoC.Get<ConfigurationProjectsViewModel>();

        }

        #region COMMANDS
        public void Directories( )
        {
            ActivateItemAsync(ConfigurationDirectoriesVM);
        }

        public void ProjectFileID()
        {
            ActivateItemAsync(ConfigurationProjectIDVM);
        }

        public void Projects()
        {
            ActivateItemAsync(ConfigurationProjectsVM);
        }
        #endregion
        public void CloseButton()
        {
            TryCloseAsync();
        }
    }


}
