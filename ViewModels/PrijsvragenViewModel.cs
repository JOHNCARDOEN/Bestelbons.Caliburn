using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WPF_Bestelbons.Events;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons.ViewModels
{
    public class PrijsvragenViewModel : Screen, IHandle<LoadedPrijsvragenListEvent>, IHandle<UserChangedEvent>, IHandle<InitialUserLoadedEvent>, IHandle<PrijsvraagAddedEvent>,
                                                IHandle<PrijsvraagRemovedEvent>, IHandle<DetermineVolgnumberPrijsvraagEvent>
    {
        private readonly IEventAggregator _eventAggregator;

        Prijsvraag PrijsvraagToAdd { get; set; }
        List<Prijsvraag> projectlist { get; set; }
        List<Prijsvraag> nonprojectlist { get; set; }
        public int MaxVolgnummer { get; set; }
        public string Prijsvraagstring { get; set; }

        private BindableCollection<Prijsvraag> SelectedList;
        private BindableCollection<Prijsvraag> NextNumberList;

        #region BINDABLE FIELDS

        private bool _searchPrijsvraagRegels;

        public bool SearchPrijsvraagRegels
        {
            get { return _searchPrijsvraagRegels; }
            set
            {
                _searchPrijsvraagRegels = value;
                NotifyOfPropertyChange(() => SearchPrijsvraagRegels);
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

        private BindableCollection<Prijsvraag> _prijsvragenLijst;
        public BindableCollection<Prijsvraag> PrijsvragenLijst
        {
            get { return _prijsvragenLijst; }
            set
            {
                _prijsvragenLijst = value;
                NotifyOfPropertyChange(() => PrijsvragenLijst);
            }
        }

        private BindableCollection<Prijsvraag> _prijsvragenLijstUI;

        public BindableCollection<Prijsvraag> PrijsvragenLijstUI
        {
            get { return _prijsvragenLijstUI; }
            set
            {
                _prijsvragenLijstUI = value;
                NotifyOfPropertyChange(() => PrijsvragenLijstUI);
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

        private Prijsvraag _bestelbonsLijstSelectedItem;

        public Prijsvraag PrijsvragenLijstSelectedItem
        {
            get { return _bestelbonsLijstSelectedItem; }
            set
            {
                _bestelbonsLijstSelectedItem = value;
                NotifyOfPropertyChange(() => PrijsvragenLijstSelectedItem);
                if (PrijsvragenLijstSelectedItem != null)
                {
                    _eventAggregator.PublishOnUIThreadAsync(message: new SelectedPrijsvraagChangedEvent(PrijsvragenLijstSelectedItem));
                }
            }
        }

        #endregion
        public PrijsvragenViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            PrijsvragenLijst = new BindableCollection<Prijsvraag>();
            SelectedList = new BindableCollection<Prijsvraag>();
            NextNumberList = new BindableCollection<Prijsvraag>();
            PrijsvragenLijstUI = new BindableCollection<Prijsvraag>();
            projectlist = new List<Prijsvraag>();
            nonprojectlist = new List<Prijsvraag>();
            this.Search = "";


        }

        public void Select()
        {
            SelectedList.Clear();
            string SelectionLower = Search.ToLower();
            string[] substrings = SelectionLower.Split('+');

            foreach (var prijsvraag in PrijsvragenLijst)
            {

                bool BonSelected = (!SearchPrijsvraagRegels && substrings.All(subs => prijsvraag.Name.Contains(subs, StringComparison.OrdinalIgnoreCase)) || SearchPrijsvraagRegels && CheckPrijsvraagRegels(prijsvraag));
                bool UserViewOK = (!UserView || (UserView && prijsvraag.Creator == CurrentUser.LastName));

                if (BonSelected && UserViewOK)
                {
                    SelectedList.Add(prijsvraag);
                }
            }
            PrijsvragenLijstUI = SelectedList;
            if (string.IsNullOrEmpty(Search))
            {
                PrijsvragenLijstUI.Clear();
                foreach (var prijsvraag in PrijsvragenLijst)
                {
                    if (!UserView || (UserView && prijsvraag.Creator == CurrentUser.LastName)) PrijsvragenLijstUI.Add(prijsvraag);

                }
            }
            if (SearchPrijsvraagRegels) _eventAggregator.PublishOnUIThreadAsync(new SearchPrijsvragenChangedEvent(SelectionLower));
        }


        public bool CheckPrijsvraagRegels(Prijsvraag prijsvraag)
        {
            bool Found = false;
            string SelectionLower = Search.ToLower();
            string[] substrings = SelectionLower.Split('+');

            for (int i = 0; i < prijsvraag.Prijsvraagregels.Count; i++)
            {
                if (substrings.All(subs => prijsvraag.Prijsvraagregels[i].Prijsregel.Contains(subs, StringComparison.OrdinalIgnoreCase)))
                {
                    Found = true;
                    break;
                }
            }
            return Found;
        }

        public void SearchPrijsvragen()
        {
            if (!SearchPrijsvraagRegels)
            {
                SearchPrijsvraagRegels = true;
                Select();
            }
            else
            {
                SearchPrijsvraagRegels = false;
                Search = string.Empty;
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


        public int CheckVolgNumber()
        {
            MaxVolgnummer = 0;
            if (!string.IsNullOrEmpty(Prijsvraagstring))
            {
                NextNumberList.Clear();
                string SelectionLower = Prijsvraagstring.ToLower();
                string[] substrings = SelectionLower.Split('+');

                foreach (var prijsvraag in PrijsvragenLijst)
                {
                    if (substrings.All(subs => prijsvraag.Name.Contains(subs, StringComparison.OrdinalIgnoreCase))) NextNumberList.Add(prijsvraag);             
                }
                foreach (var prijsvraag in NextNumberList)
                {
                    substrings = prijsvraag.Name.Split('-');
                    int volgnummer;
                    if (Int32.TryParse(substrings[2], out volgnummer))
                    {

                        if (volgnummer > MaxVolgnummer) MaxVolgnummer = volgnummer;
                    }
                }
                _eventAggregator.PublishOnUIThreadAsync(new SuggestedNewVolgnummerPrijsvraagEvent(MaxVolgnummer + 1));
                return MaxVolgnummer + 1;

            }
            return 0;
        }


        public void Close()
        {
            _eventAggregator.PublishOnUIThreadAsync(message: new CloseActiveItemEvent());
        }

        #region EVENTHANDLERS
        public Task HandleAsync(LoadedPrijsvragenListEvent message, CancellationToken cancellationToken)
        {
            PrijsvragenLijst = message.PrijsvragenList;
            foreach (Prijsvraag prijsvraag in PrijsvragenLijst)
            {
                foreach (var prijsvraagregel in prijsvraag.Prijsvraagregels)
                {
                    prijsvraagregel.PropertyChanged += prijsvraag.PrijsvraagregelChanged;

                }
            }
            PrijsvragenLijstUI.Clear();
            foreach (var prijsvraag in PrijsvragenLijst)
            {
                PrijsvragenLijstUI.Add(prijsvraag);
            }

            return Task.CompletedTask;
        }

        public Task HandleAsync(UserChangedEvent message, CancellationToken cancellationToken)
        {
            CurrentUser = message.user;
            Select();
            return Task.CompletedTask;
        }

        public Task HandleAsync(InitialUserLoadedEvent message, CancellationToken cancellationToken)
        {
            CurrentUser = message.user;
            return Task.CompletedTask;
        }

        public Task HandleAsync(PrijsvraagAddedEvent message, CancellationToken cancellationToken)
        {
            // isolate project and nonprojectprijsvragen in separate lists
            // add new prijsvraag in one of the lists
            // sort nonproject list in ascending order
            // sort project list in decending order
            // clear Bestelbonslijst
            // add all the project prijsvragen from projectprijsvragen first and next all the nonproject prijsvragen

            PrijsvraagToAdd = message.Prijsaanvraag.DeepCopy();    // VIA Helper ObjectExtensions


            projectlist.Clear();
            nonprojectlist.Clear();
            foreach (var prijsvraag in PrijsvragenLijst)
            {
                if (Char.IsLetter(prijsvraag.Name[0])) nonprojectlist.Add(prijsvraag);
                else projectlist.Add(prijsvraag);
            }

            if (Char.IsLetter(message.Prijsaanvraag.Name[0])) nonprojectlist.Add(PrijsvraagToAdd);
            else projectlist.Add(PrijsvraagToAdd);

            var sortednonproject = nonprojectlist.OrderBy(x => x.Name).ToList();
            var sortedproject = projectlist.OrderByDescending(x => x.Name).ToList();

            PrijsvragenLijst.Clear();

            foreach (var prijsvraag in sortedproject)
            {
                PrijsvragenLijst.Add(prijsvraag);
            }
            foreach (var prijsvraag in sortednonproject)
            {
                PrijsvragenLijst.Add(prijsvraag);
            }

            Select();
            return Task.CompletedTask;

        }

        public Task HandleAsync(PrijsvraagRemovedEvent message, CancellationToken cancellationToken)
        {
            PrijsvragenLijst.Remove(message.Prijsaanvraag);
            return Task.CompletedTask;

        }

        public Task HandleAsync(DetermineVolgnumberPrijsvraagEvent message, CancellationToken cancellationToken)
        {
            Prijsvraagstring = message.Prijsvraagstring;
            CheckVolgNumber();
            return Task.CompletedTask;
        }
        #endregion
    }
}
