using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using WPF_Bestelbons.Events;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons.ViewModels
{
    public class SelectUserViewModel : Screen , IHandle<UserListChangedEvent>
    {
        private readonly IEventAggregator _eventAggregator;

        public BindableCollection<User> UserList { get; set; }

        private string _capiton;

        public string Capiton
        {
            get { return _capiton; }
            set
            {
                _capiton = value;
                NotifyOfPropertyChange(() => Capiton);
            }
        }

        private User __selectedItem;

        public User SelectedItem
        {
            get { return __selectedItem; }
            set
            {
                __selectedItem = value; 
                NotifyOfPropertyChange(() => SelectedItem);
                _eventAggregator.PublishOnUIThreadAsync(message: new UserChangedEvent(SelectedItem));
                Properties.Settings.Default.CurrentUser = SelectedItem.FirstName;
                // User scope settings gebruikt omdat we anders niet kunnen saven !
                Properties.Settings.Default.Save();
            }
        }

        public SelectUserViewModel(IEventAggregator eventaggregator)
        {
            this.UserList = new BindableCollection<User>();
            _eventAggregator = eventaggregator;
            _eventAggregator.Subscribe(this);
        }

        public void OK()
        {
            TryCloseAsync();
        }

        public void CloseButton()
        {
            TryCloseAsync(false);
        }

        public Task HandleAsync (UserListChangedEvent message, CancellationToken cancellationToken)
        {
            UserList = message.UserList;
            return Task.CompletedTask;
        }
    }
}
