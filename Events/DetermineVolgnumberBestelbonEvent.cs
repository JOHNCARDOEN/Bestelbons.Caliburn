namespace WPF_Bestelbons.Events
{

    public class DetermineVolgnumberBestelbonEvent
    {
        public string Bestelbonstring { get; set; }
        public DetermineVolgnumberBestelbonEvent(string message)
        {
            Bestelbonstring = message;
        }
    }
}
