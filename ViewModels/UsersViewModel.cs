using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Caliburn.Micro;
using WPF_Bestelbons.Events;
using WPF_Bestelbons.Models;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Screen = Caliburn.Micro.Screen;

namespace WPF_Bestelbons.ViewModels
{
    //public class UsersViewModel : Screen , IHandle<LoadUsersMessage> , IHandle<UserListMessage>
    public class UsersViewModel : Screen, IHandle<UserListChangedEvent>
    {


        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;

        public int index;

        #region CANEXECUTE

        public bool CanDelete
        {
            get { return (UsersSelectedItem != null); }
        }

        public bool CanCaptureMAC
        {
            get { return (UsersSelectedItem != null); }
        }
        #endregion


        #region BINDABLE FIELDS

        private string _firstName;

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                NotifyOfPropertyChange(() => FirstName);
                if (index >= 0)
                    UsersList[index].FirstName = _firstName;
            }
        }

        private string _lastName;

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                NotifyOfPropertyChange(() => LastName);
                if (index >= 0)
                    UsersList[index].LastName = _lastName;
            }
        }

        private string _email;

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                NotifyOfPropertyChange(() => Email);
                if (index >= 0)
                    UsersList[index].Email = _email;
            }
        }

        private string _tel;

        public string Tel
        {
            get { return _tel; }
            set
            {
                _tel = value;
                NotifyOfPropertyChange(() => Tel);
                if (index >= 0)
                    UsersList[index].Tel = _tel;
            }
        }

        private string _handtekening;

        public string Handtekening
        {
            get { return _handtekening; }
            set
            {
                _handtekening = value;
                NotifyOfPropertyChange(() => Handtekening);
                if (index >= 0)
                    UsersList[index].Handtekening = _handtekening;
            }
        }

        private bool _canApprove;

        public bool CanApprove
        {
            get { return _canApprove; }
            set
            {
                _canApprove = value;
                NotifyOfPropertyChange(() => CanApprove);
                if (index >= 0)
                    UsersList[index].CanApprove = _canApprove;
            }
        }

        private string _macAdress;

        public string MACAdress
        {
            get { return _macAdress; }
            set
            {
                _macAdress = value;
                if (MACAdress != null) MACAdressSecret = "******" + MACAdress.Substring(MACAdress.Length - 4);
                else MACAdressSecret = string.Empty;


                NotifyOfPropertyChange(() => MACAdress);
                if (index >= 0)
                    UsersList[index].MACAdress = _macAdress;
                NotifyOfPropertyChange(() => MACAdressSecret);
            }
        }

        private bool _macAdressOK;

        public bool MACAdressOK
        {
            get { return _macAdressOK; }
            set
            {
                _macAdressOK = value;
                NotifyOfPropertyChange(() => MACAdressOK);
                if (index >= 0)
                    UsersList[index].MACAdressOK = _macAdressOK;

            }
        }

        private string __macAdressSecret;

        public string MACAdressSecret
        {
            get { return __macAdressSecret; }
            set
            {
                __macAdressSecret = value; 
                NotifyOfPropertyChange(() => MACAdressSecret);
            }
        }



        private User _usersSelectedItem;

        public User UsersSelectedItem
        {
            get { return _usersSelectedItem; }
            set
            {
                _usersSelectedItem = value;
                //NotifyOfPropertyChange(() => UsersSelectedItem);
                if (_usersSelectedItem != null)
                {
                    User userindex = UsersList.Where(user => user.LastName == _usersSelectedItem.LastName).FirstOrDefault();
                    index = UsersList.IndexOf(userindex);
                    LastName = _usersSelectedItem.LastName;
                    FirstName = _usersSelectedItem.FirstName;
                    Email = _usersSelectedItem.Email;
                    Tel = _usersSelectedItem.Tel;
                    Handtekening = _usersSelectedItem.Handtekening;
                    CanApprove = _usersSelectedItem.CanApprove;
                    MACAdressOK = _usersSelectedItem.MACAdressOK;
                    MACAdress = _usersSelectedItem.MACAdress;
                }
                else
                {
                    index = -1;
                }
                NotifyOfPropertyChange(() => CanDelete);
                NotifyOfPropertyChange(() => CanCaptureMAC);
            }
        }

        private BindableCollection<User> _usersList;

        public BindableCollection<User> UsersList
        {
            get { return _usersList; }
            set
            {
                _usersList = value;
                NotifyOfPropertyChange(() => UsersList);
            }
        }

        private int _selectedIndex;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                NotifyOfPropertyChange(() => SelectedIndex);
            }
        }

        #endregion


        public UsersViewModel(IEventAggregator eventAggregator, IWindowManager windowsmanager)
        {
            _eventAggregator = eventAggregator;
            _windowManager = windowsmanager;
            _eventAggregator.Subscribe(this);
            UsersSelectedItem = null;
            this.UsersList = new BindableCollection<User>();

        }



        public void Add()
        {
            User NewUser = new User();
            NewUser.FirstName = FirstName;
            NewUser.LastName = LastName;
            NewUser.Email = Email;
            NewUser.Tel = Tel;
            NewUser.Handtekening = Handtekening;
            UsersList.Add(NewUser);
            Save();
        }

        public void Clear()
        {
            UsersSelectedItem = null;
            SelectedIndex = -1;
            FirstName = "";
            LastName = "";
            Email = "";
            Tel = "";
            Handtekening = "";
            CanApprove = false;
        }

        public void Delete()
        {
            UsersList.RemoveAt(index);
            FirstName = "";
            LastName = "";
            Email = "";
            Tel = "";
            Handtekening = "";
            CanApprove = false;
            }

        public void Save()
        {

            var FilePath = Properties.Settings.Default.UserListPath;
            var serializer = new XmlSerializer(typeof(BindableCollection<User>));
            using (var writer = new System.IO.StreamWriter(FilePath))
            {
                serializer.Serialize(writer, UsersList);
                writer.Flush();
            }
        }


        public void Close()
        {
            TryCloseAsync(false);
        }

        public void Signature()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Signatures (*.png)|*.png|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = "C\\:";
            if (openFileDialog.ShowDialog() == true)
            {
                Handtekening = openFileDialog.FileName;
            }
        }

        public void CaptureMAC()
        {
            string MACaddr = "";
            ManagementClass managClass = new ManagementClass("win32_processor");
            ManagementObjectCollection managCollec = managClass.GetInstances();

            foreach (ManagementObject managObj in managCollec)
            {
                 MACaddr = managObj.Properties["processorID"].Value.ToString();
                break;
            }
            UsersSelectedItem.MACAdress = MACaddr;
            MACAdress = MACaddr;
            if (MACaddr.Length == 16) UsersSelectedItem.MACAdressOK = true;
            Save();
        }

        public Task HandleAsync(UserListChangedEvent message, CancellationToken cancellationToken)
        {
            UsersList = message.UserList;
            return Task.CompletedTask;
        }
    }
}
