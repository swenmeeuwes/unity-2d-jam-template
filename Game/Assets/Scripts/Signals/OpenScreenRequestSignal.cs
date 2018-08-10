public class OpenScreenRequestSignal
{
    public ScreenType Type { get; set; }
    public bool ForceOpen { get; set; } // Used for opening multiple instances of a screen


    public OpenScreenRequestSignal()
    {
        ForceOpen = false;
    }
}
