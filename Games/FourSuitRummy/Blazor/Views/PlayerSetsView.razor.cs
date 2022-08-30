namespace FourSuitRummy.Blazor.Views;
public partial class PlayerSetsView
{
    [Parameter]
    public FourSuitRummyPlayerItem? PlayerUsed { get; set; }
    [CascadingParameter]
    public FourSuitRummyGameContainer? GameContainer { get; set; }
}