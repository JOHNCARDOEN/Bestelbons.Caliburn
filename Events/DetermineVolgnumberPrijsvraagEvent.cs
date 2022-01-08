namespace WPF_Bestelbons.Events
{

    public class DetermineVolgnumberPrijsvraagEvent
    {
        public string Prijsvraagstring { get; set; }
        public DetermineVolgnumberPrijsvraagEvent(string message)
        {
            Prijsvraagstring = message;
        }
    }
}
