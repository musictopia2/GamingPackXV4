namespace DealCardGame.Blazor;
public partial class DisplayStolenSetComponent
{
    [Parameter]
    [EditorRequired]
    public DealCardGamePlayerItem? PlayerToDisplay { get; set; }
    [Parameter]
    [EditorRequired]
    public CommandContainer? Command { get; set; }
    [Parameter]
    [EditorRequired]
    public EnumColor Color { get; set; }
    protected override void OnInitialized()
    {
        _hand = GetHand();
        base.OnInitialized();
    }
    private HandObservable<DealCardGameCardInformation>? _hand;
    private HandObservable<DealCardGameCardInformation>? GetHand()
    {
        if (PlayerToDisplay is null)
        {
            return null;
        }
        if (Command is null)
        {
            return null;
        }
        HandObservable<DealCardGameCardInformation> output = new(Command);
        output.AutoSelect = EnumHandAutoType.ShowObjectOnly;
        if (PlayerToDisplay.PlayerCategory == EnumPlayerCategory.Self)
        {
            output.Text = "Your Set Being Stolen";
        }
        else
        {
            output.Text = "Your Set Being Received";   
        }
        output.IsEnabled = true; //just so not gray.
        var list = PlayerToDisplay.SetData.GetCards(Color);
        output.HandList = list;
        return output;
    }
}