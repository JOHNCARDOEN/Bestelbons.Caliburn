using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPF_Bestelbons.Events;

namespace WPF_Bestelbons.ViewModels
{
    public class InnerShellViewModel : Conductor<Screen>, IHandle<ModeChangedEvent>
    {
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;

        public BestelbonOpmaakViewModel BestelbonOpmaakVM { get; set; }
        public PrijsvraagOpmaakViewModel PrijsaanvraagOpmaakVM { get; set; }



        #region BINDABLE FIELDS
        private bool _modeBestelbon;

        public bool ModeBestelbon
        {
            get { return _modeBestelbon; }
            set
            {
                _modeBestelbon = value;

                if (value)
                {
                    ActivateItemAsync(BestelbonOpmaakVM);
                }
                else
                {
                    ActivateItemAsync(PrijsaanvraagOpmaakVM);
                }
                NotifyOfPropertyChange(() => ModeBestelbon);
            }
        }
        #endregion

        public InnerShellViewModel(IEventAggregator eventAggregator, IWindowManager windowmanager)
        {
            _windowManager = windowmanager;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            BestelbonOpmaakVM = IoC.Get<BestelbonOpmaakViewModel>();
            PrijsaanvraagOpmaakVM = IoC.Get<PrijsvraagOpmaakViewModel>();


           ModeBestelbon = true;

        }

        #region EVENTHANDLERS
        public Task HandleAsync(ModeChangedEvent message, CancellationToken cancellationToken)
        {
            ModeBestelbon = message.ModeBestelbon;
            return Task.CompletedTask;
        }
        #endregion
    }
}
