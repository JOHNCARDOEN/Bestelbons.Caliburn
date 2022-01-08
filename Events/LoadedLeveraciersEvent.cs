using Caliburn.Micro;
using WPF_Bestelbons.Models;

namespace WPF_Bestelbons.Events
{
    public class LoadedLeveraciersEvent
    {
        public BindableCollection<Leverancier> Leveranciers { get; set; }
        public LoadedLeveraciersEvent(BindableCollection<Leverancier> message)
        {
            Leveranciers = message;
        }




    }
}
