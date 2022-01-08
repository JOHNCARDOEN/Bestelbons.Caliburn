using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using WPF_Bestelbons.Events;

namespace WPF_Bestelbons.ViewModels
{
    public class LoadViewModel : Screen , IHandle<StopTimerEvent>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IWindowManager _windowManager;

           DispatcherTimer timer = new DispatcherTimer();

        #region BINDABLE FIELDS
        private BindableCollection<string> _loadMessages;

        public BindableCollection<string> LoadMessages
        {
            get { return _loadMessages; }
            set
            {
                _loadMessages = value;
                NotifyOfPropertyChange(() => LoadMessages);
            }
        }

        private int _progressValue;

        public int ProgressValue
        {
            get { return _progressValue; }
            set
            {
                _progressValue = value;
                NotifyOfPropertyChange(() => ProgressValue);
            }
        }


        #endregion

        public LoadViewModel(IWindowManager windowsmanager, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _windowManager = windowsmanager;
            _eventAggregator.Subscribe(this);
            this.LoadMessages = new BindableCollection<string>();    
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Random random = new Random();
            int RandomNumber = random.Next(1, 5);
            ProgressValue += RandomNumber;
            if (ProgressValue > 100) ProgressValue = 0;
        }

        public Task HandleAsync(StopTimerEvent message, CancellationToken cancellationToken)

        {
            timer.Stop();
            return Task.CompletedTask;
        }
    }
}
