using Caliburn.Micro;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WPF_Bestelbons.Events;
using WPF_Bestelbons.Models;
using WPF_Bestelbons.Views;

namespace WPF_Bestelbons.ViewModels
{
    public class BestelbonsViewModel : Screen, IHandle<LoadedBestelbonsEvent>, IHandle<BestelbonAlteredEvent>, IHandle<BestelbonAddedEvent>, IHandle<UserChangedEvent>,
                                               IHandle<InitialUserLoadedEvent>, IHandle<BestelbonDeliveredChangedEvent>, IHandle<DetermineVolgnumberBestelbonEvent>, IHandle<ClearSearchEvent>,
                                               IHandle<BestelbonSendEvent>, IHandle<CloseBestelbonViewEvent>, IHandle<UserListChangedEvent>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;
        private DSNSmtpClient _mailclient;

        Bestelbon BestelbonToAdd { get; set; }
        List<Bestelbon> projectlist { get; set; }
        List<Bestelbon> nonprojectlist { get; set; }
        public string Bestelbonstring { get; set; }
        public int MaxVolgnummer { get; set; }

        public string ProjectDirectory { get; set; }

        public string OwnMACAdress { get; set; }
        private BindableCollection<Bestelbon> SelectedList;
        public ObservableCollection<User> UserList { get; set; }

        BindableCollection<Bestelbon> nonprojectfiles { get; set; }
        BindableCollection<Bestelbon> projectfiles { get; set; }
        BindableCollection<Bestelbon> oddprojectfiles { get; set; }

        #region CANEXECUTE

        public bool CanApprove
        {
            get { return true; }
           // get { return BestelbonsLijstSelectedItem != null; }
            //get { return BestelbonsLijstSelectedItem != null && CurrentUser.CanApprove && OwnMACAdress == CurrentUser.MACAdress; }
        }

        #endregion

        #region BINDABLE FIELDS

        private bool _approvalView;

        public bool ApprovalView
        {
            get { return _approvalView; }
            set
            {
                _approvalView = value;
                NotifyOfPropertyChange(() => ApprovalView);
            }
        }


        private bool _searchBestelbonRegels;

        public bool SearchBestelbonRegels
        {
            get { return _searchBestelbonRegels; }
            set
            {
                _searchBestelbonRegels = value;
                NotifyOfPropertyChange(() => SearchBestelbonRegels);
            }
        }


        private bool _partialDeliveredView;

        public bool PartialDeliveredView
        {
            get { return _partialDeliveredView; }
            set
            {
                _partialDeliveredView = value;
                NotifyOfPropertyChange(() => PartialDeliveredView);
            }
        }


        private bool _expiredTimeView;

        public bool ExpiredTimeView
        {
            get { return _expiredTimeView; }
            set
            {
                _expiredTimeView = value;
                NotifyOfPropertyChange(() => ExpiredTimeView);
            }
        }

        private bool _userView;

        public bool UserView
        {
            get { return _userView; }
            set
            {
                _userView = value;
                NotifyOfPropertyChange(() => UserView);
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


        private BindableCollection<Bestelbon> _bestelbonsLijst;
        public BindableCollection<Bestelbon> BestelbonsLijst
        {
            get { return _bestelbonsLijst; }
            set
            {
                _bestelbonsLijst = value;
                NotifyOfPropertyChange(() => BestelbonsLijst);
            }
        }

        private BindableCollection<Bestelbon> _bestelbonsLijstUI;

        public BindableCollection<Bestelbon> BestelbonsLijstUI
        {
            get { return _bestelbonsLijstUI; }
            set
            {
                _bestelbonsLijstUI = value;
                NotifyOfPropertyChange(() => BestelbonsLijstUI);
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

        private Bestelbon _bestelbonsLijstSelectedItem;

        public Bestelbon BestelbonsLijstSelectedItem
        {
            get { return _bestelbonsLijstSelectedItem; }
            set
            {
                _bestelbonsLijstSelectedItem = value;
                NotifyOfPropertyChange(() => BestelbonsLijstSelectedItem);
                NotifyOfPropertyChange(() => CanApprove);

                if (BestelbonsLijstSelectedItem != null)
                {
                    _eventAggregator.PublishOnUIThreadAsync(message: new SelectedBestelbonChangedEvent(BestelbonsLijstSelectedItem));
                }
            }
        }

        #endregion

        public BestelbonsViewModel(IEventAggregator eventAggregator, IWindowManager windowsmanager, DSNSmtpClient mailclient)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _mailclient = mailclient;
            BestelbonsLijst = new BindableCollection<Bestelbon>();
            SelectedList = new BindableCollection<Bestelbon>();
            BestelbonsLijstUI = new BindableCollection<Bestelbon>();
            BestelbonToAdd = new Bestelbon();
            projectlist = new List<Bestelbon>();
            nonprojectlist = new List<Bestelbon>();
            this.UserList = new ObservableCollection<User>();
            this.Search = "";
            this.nonprojectfiles = new BindableCollection<Bestelbon>();
            this.projectfiles = new BindableCollection<Bestelbon>();
            this.oddprojectfiles = new BindableCollection<Bestelbon>();

            OwnMACAdress = "";
            ManagementClass managClass = new ManagementClass("win32_processor");
            ManagementObjectCollection managCollec = managClass.GetInstances();

            foreach (ManagementObject managObj in managCollec)
            {
                OwnMACAdress = managObj.Properties["processorID"].Value.ToString();
                break;
            }
            NotifyOfPropertyChange(() => CanApprove);
        }

        public void Select()
        {
            string SelectionLower = string.Empty;
            if (!string.IsNullOrEmpty(Search))
            {
                SelectedList.Clear();
                SelectionLower = Search.ToLower();
                string[] substrings = SelectionLower.Split('+');

                foreach (var bestelbon in BestelbonsLijst)
                {
                    bool BonSelected = (!SearchBestelbonRegels && substrings.All(subs => bestelbon.Name.Contains(subs, StringComparison.OrdinalIgnoreCase)) || SearchBestelbonRegels && CheckBestelbonRegels(bestelbon));// || bestelbon.Name.Contains(""));
                    bool UserViewOK = (!UserView || (UserView && bestelbon.Creator == CurrentUser.LastName));
                    bool ExpiredViewOK = (!ExpiredTimeView || (ExpiredTimeView && bestelbon.DeliveryTimeExpired));
                    bool PartialDeliveredOK = (!PartialDeliveredView || (PartialDeliveredView && (bestelbon.PercentageDelivered != 100)));

                    if (BonSelected && UserViewOK && ExpiredViewOK && PartialDeliveredOK)
                    {
                        bestelbon.CalculateDeliveryTimeExpired();
                        SelectedList.Add(bestelbon);
                    }
                }
                var OrderedSelectedList = SelectedList.OrderBy(b => b.DeliveryTime);
                BestelbonsLijstUI.Clear();
                foreach (var item in OrderedSelectedList)
                {
                    BestelbonsLijstUI.Add(item);
                }
            }

            if (string.IsNullOrEmpty(Search))
            {
                BestelbonsLijstUI.Clear();
                foreach (var bestelbon in BestelbonsLijst)
                {
                    if ((!UserView || (UserView && bestelbon.Creator == CurrentUser.LastName)) &&
                        (!ExpiredTimeView || (ExpiredTimeView && bestelbon.DeliveryTimeExpired)) &&
                        (!PartialDeliveredView || (PartialDeliveredView && (bestelbon.PercentageDelivered != 100))) &&
                        (!ApprovalView || (ApprovalView && bestelbon.AskForApproval))) BestelbonsLijstUI.Add(bestelbon);

                }
            }
            if (SearchBestelbonRegels) _eventAggregator.PublishOnUIThreadAsync(new SearchBestelbonsChangedEvent(SelectionLower));
        }

        public bool CheckBestelbonRegels(Bestelbon bestelbon)
        {
            bool Found = false;
            string SelectionLower = Search.ToLower();
            string[] substrings = SelectionLower.Split('+');

            for (int i = 0; i < bestelbon.Bestelbonregels.Count; i++)
            {
                if (substrings.All(subs => bestelbon.Bestelbonregels[i].Bestelregel.Contains(subs, StringComparison.OrdinalIgnoreCase)))
                {
                    Found = true;
                    break;
                }
            }
            return Found;
        }

        public void SearchBestelbons()
        {
            if (!SearchBestelbonRegels)
            {
                SearchBestelbonRegels = true;
                ExpiredTimeView = false;
                PartialDeliveredView = false;
                ApprovalView = false;
                Select();
            }
            else
            {
                SearchBestelbonRegels = false;
                Search = string.Empty;
                Select();
            }

        }

        public void ExipredSelect()
        {
            if (!ExpiredTimeView)
            {
                ExpiredTimeView = true;
                PartialDeliveredView = false;
                Select();
            }
            else
            {
                ExpiredTimeView = false;
                Select();
            }
        }

        public void ApprovalFilter()
        {
            if (!ApprovalView)
            {
                ApprovalView = true;
                _eventAggregator.PublishOnUIThreadAsync(new ReloadAllFilesEvent());
            }
            else
            {
                ApprovalView = false;
                Select();
            }
        }

        public void ClearFilter()
        {
            Search = "";
            Select();
        }

        public void UserFilter()
        {
            if (!UserView)
            {
                UserView = true;
                Select();

            }
            else
            {
                UserView = false;
                Select();

            }
        }

        public void PartialDeliveredFilter()
        {
            string t = ToString();
            if (!PartialDeliveredView)
            {
                PartialDeliveredView = true;
                ExpiredTimeView = false;
                Select();
            }

            else
            {
                PartialDeliveredView = false;
                Select();
            }
        }

        public int CheckVolgNumber()
        {
            MaxVolgnummer = 0;
            Search = Bestelbonstring;
            if (!string.IsNullOrEmpty(Search))
            {
                ExpiredTimeView = false;
                PartialDeliveredView = false;

                Select();
                foreach (var bestelbon in BestelbonsLijstUI)
                {
                    string[] substrings = bestelbon.Name.Split('-');
                    int volgnummer;
                    if (Int32.TryParse(substrings[2], out volgnummer))
                    {

                        if (volgnummer > MaxVolgnummer) MaxVolgnummer = volgnummer;
                    }
                }
                _eventAggregator.PublishOnUIThreadAsync(new SuggestedNewVolgnummerBestelbonEvent(MaxVolgnummer + 1));
                return MaxVolgnummer + 1;
            }
            return 0;

        }

        public async void Approve()
        {
            BestelbonsLijstSelectedItem.AskForApproval = false;
            BestelbonsLijstSelectedItem.Approved = true;

            try
            {

                using (var writer = new System.IO.StreamWriter(Properties.Settings.Default.BestelbonsPath + "\\" + BestelbonsLijstSelectedItem.Name + ".abb"))
                {
                    var serializer = new XmlSerializer(typeof(Bestelbon));
                    serializer.Serialize(writer, BestelbonsLijstSelectedItem);
                    writer.Flush();
                }
                _eventAggregator.PublishOnUIThreadAsync(message: new SelectedBestelbonChangedEvent(BestelbonsLijstSelectedItem));

                var mailadress = string.Empty;
                foreach (var user in UserList)
                {
                    if (BestelbonsLijstSelectedItem.Creator == user.LastName) mailadress = user.Email;
                }
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(CurrentUser.FirstName, CurrentUser.Email));
                message.To.Add(new MailboxAddress(BestelbonsLijstSelectedItem.Creator, mailadress));
                message.Subject = $"Bestelbon  {BestelbonsLijstSelectedItem.Name} is goedgekeurd";

                var builder = new BodyBuilder();
                MimeEntity image;

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WPF_Bestelbons.Resources.LOGO_275.png"))
                {
                    image = builder.LinkedResources.Add("LOGO_275.png", stream);
                    image.ContentId = MimeUtils.GenerateMessageId();
                }

                string emailbody = $"Beste, " +
                                   $"\r\n\r\n Bestelbon  {BestelbonsLijstSelectedItem.Name} is goedgekeurd" +
                                   $"\r\n\r\n\r\n\r\nMet vriendelijke groeten" +
                                   $"\r\n {CurrentUser.LastName}  { CurrentUser.FirstName}";

                string footer = string.Format(@"

                   <p><br></p>
                   <left><img src=""cid:{0}""></left>
                   <p>Industrielaan 19,zone C2 - B-8810 Lichtervelde<br>
                   Tel. +32 (0)51/72.24.46   &nbsp;&nbsp;&nbsp;&nbsp;&nbsp      <a href=""http://www.astratec.be""> www.astratec.be</a> "
                     , image.ContentId);

                builder.HtmlBody = emailbody + footer;
                message.Body = builder.ToMessageBody();

                try
                {

                    await _mailclient.ConnectAsync("smtp.office365.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    await _mailclient.AuthenticateAsync(CurrentUser.Email, CurrentUser.EmailAuthentication);

                    // Check DNS capabilities
                    var supportsDsn = _mailclient.Capabilities.HasFlag(SmtpCapabilities.Dsn);

                    await _mailclient.SendAsync(message);
                    await _mailclient.DisconnectAsync(true);

                }
                catch (Exception ex)
                {
                    var dialogViewModel = IoC.Get<DialogViewModel>();
                    dialogViewModel.Capiton = "Attachment Error";
                    dialogViewModel.Message = ex.Message.ToString();
                    var result = _windowManager.ShowDialogAsync(dialogViewModel);
                }

            }
            catch (Exception e)
            {
                var dialogViewModel = IoC.Get<DialogViewModel>();
                dialogViewModel.Capiton = "File Open";
                dialogViewModel.Message = e.ToString();
                var result = _windowManager.ShowDialogAsync(dialogViewModel);
            }

        }
        public void CheckCanApprove()
        {
            string d = "test";
        }
        public void Close()
        {
            _eventAggregator.PublishOnUIThreadAsync(message: new CloseActiveItemEvent());
        }

        #region EVENTHANDLERS

        public Task HandleAsync(BestelbonAddedEvent message, CancellationToken cancellationToken)
        {
            // isolate project and nonprojectbestelbons in separate lists
            // add new bestelbon in one of the lists
            // sort nonproject list in ascending order
            // sort project list in decending order
            // clear Bestelbonslijst
            // add all the project bestelbons form projectbestelbons first and next all the nonproject bestlbons


            BestelbonToAdd = message.Bestelbon.DeepCopy();    // VIA Helper ObjectExtensions

            //projectlist.Clear();
            //nonprojectlist.Clear();
            //foreach (var bestelbon in BestelbonsLijst)
            //{
            //    if (!string.IsNullOrEmpty(bestelbon.Name))
            //    {
            //        if (Char.IsLetter(bestelbon.Name[0])) nonprojectlist.Add(bestelbon);
            //        else projectlist.Add(bestelbon);
            //    }
            //}

            //if (Char.IsLetter(message.Bestelbon.Name[0])) nonprojectlist.Add(BestelbonToAdd);
            //else projectlist.Add(BestelbonToAdd);

            //var sortednonproject = nonprojectlist.OrderBy(x => x.Name).ToList();
            //var sortedproject = projectlist.OrderByDescending(x => x.Name).ToList();

            //BestelbonsLijst.Clear();

            //foreach (var bestelbon in sortedproject)
            //{
            //    BestelbonsLijst.Add(bestelbon);
            //}
            //foreach (var bestelbon in sortednonproject)
            //{
            //    BestelbonsLijst.Add(bestelbon);
            //}

            ////
            //nonprojectfiles.Clear();
            //projectfiles.Clear();
            //oddprojectfiles.Clear();

            BestelbonsLijst.Add(BestelbonToAdd);

            foreach (var bestbon in BestelbonsLijst)
            {
                try
                {
                    if (!Char.IsLetter(bestbon.Name[0]))
                    {
                        if (bestbon.Name.Substring(0, 2) == "20") projectfiles.Add(bestbon);
                        else oddprojectfiles.Add(bestbon);

                    }
                    else nonprojectfiles.Add(bestbon);
                }
                catch (Exception)
                {

                }
            }

            var query = projectfiles.OrderByDescending(x => x.Name);
            //projectfiles.Clear();
            //foreach (var item in query)
            //{
            //    projectfiles.Add(item);
            //}

            BestelbonsLijst.Clear();

            foreach (var bestelbon in query)
            {
                BestelbonsLijst.Add(bestelbon);
            }

            foreach (var bestelbon in oddprojectfiles)
            {
                BestelbonsLijst.Add(bestelbon);
            }

            foreach (var bestelbon in nonprojectfiles)
            {
                BestelbonsLijst.Add(bestelbon);
            }



            //Select();
            return Task.CompletedTask;
            //}

        }

        public Task HandleAsync(LoadedBestelbonsEvent message, CancellationToken cancellationToken)
        {
            BestelbonsLijst = message.Bestelbonlist;
            foreach (Bestelbon bestelbon in BestelbonsLijst)
            {
                foreach (var bestelbonregel in bestelbon.Bestelbonregels)
                {
                    bestelbonregel.PropertyChanged += bestelbon.BestelbonregelChanged;

                }
                bestelbon.CalculateDeliveryTimeExpired();
            }
            BestelbonsLijstUI.Clear();
            //foreach (var bestelbon in BestelbonsLijst)
            //{
            //    BestelbonsLijstUI.Add(bestelbon);
            //}

           // BestelbonsLijstSelectedItem = BestelbonsLijst[0];
            Select();
            return Task.CompletedTask;
        }

        public Task HandleAsync(BestelbonAlteredEvent message, CancellationToken cancellationToken)
        {
            Bestelbon Alteredbestelbon;
            Alteredbestelbon = message.Bestelbon;
            return Task.CompletedTask;
        }

        public Task HandleAsync(UserChangedEvent message, CancellationToken cancellationToken)
        {
            CurrentUser = message.user;
            NotifyOfPropertyChange(() => CanApprove);
            Select();
            return Task.CompletedTask;
        }

        public Task HandleAsync(InitialUserLoadedEvent message, CancellationToken cancellationToken)
        {
            CurrentUser = message.user;
            NotifyOfPropertyChange(() => CanApprove);
            return Task.CompletedTask;

        }

        public Task HandleAsync(BestelbonDeliveredChangedEvent message, CancellationToken cancellationToken)
        {
            string BestelbonName = message.Bestelbon.Name;
            int PercentageDelivered = message.Bestelbon.PercentageDelivered;
            bool AskForApproval = message.Bestelbon.AskForApproval;
            bool Approved = message.Bestelbon.Approved;
            bool BestelbonSend = message.Bestelbon.BestelbonSend;

            for (int i = 0; i < BestelbonsLijst.Count; i++)
            {
                if (BestelbonsLijst[i].Name == BestelbonName)
                {
                    BestelbonsLijst[i].PercentageDelivered = PercentageDelivered;
                    BestelbonsLijst[i].AskForApproval = AskForApproval;
                    BestelbonsLijst[i].Approved = Approved;
                    BestelbonsLijst[i].BestelbonSend = BestelbonSend;

                    break;
                }


            }
            return Task.CompletedTask;
        }

        public Task HandleAsync(BestelbonSendEvent message, CancellationToken cancellationToken)
        {
            string BestelbonName = message.Bestelbon.Name;

            for (int i = 0; i < BestelbonsLijst.Count; i++)
            {
                if (BestelbonsLijst[i].Name == BestelbonName)
                {
                    BestelbonsLijst[i].BestelbonSend = message.Bestelbon.BestelbonSend;//= true;
                    BestelbonsLijst[i].Approved = message.Bestelbon.Approved;//= false;
                    BestelbonsLijst[i].AskForApproval = message.Bestelbon.AskForApproval;//false;
                    break;
                }
            }
            return Task.CompletedTask;
        }

        public Task HandleAsync(DetermineVolgnumberBestelbonEvent message, CancellationToken cancellationToken)
        {
            Bestelbonstring = message.Bestelbonstring;
            CheckVolgNumber();
            return Task.CompletedTask;
        }

        public Task HandleAsync(CloseBestelbonViewEvent message, CancellationToken cancellationToken)
        {
            Close();
            return Task.CompletedTask;
        }
        public Task HandleAsync(UserListChangedEvent message, CancellationToken cancellationToken)
        {
            UserList = message.UserList;
            return Task.CompletedTask;

        }

        public Task HandleAsync(ClearSearchEvent message, CancellationToken cancellationToken)
        {
            ClearFilter();
            return Task.CompletedTask;
        }

        #endregion

    }

    public static class StringExtensions
    {
        public static bool Contains(this string str, string substring,
                                    StringComparison comp)
        {
            if (substring == null)
                throw new ArgumentNullException("substring",
                                             "substring cannot be null.");
            else if (!Enum.IsDefined(typeof(StringComparison), comp))
                throw new ArgumentException("comp is not a member of StringComparison",
                                         "comp");
            int i = str.IndexOf(substring, comp);
            return str.IndexOf(substring, comp) >= 0;
        }
    }
}
