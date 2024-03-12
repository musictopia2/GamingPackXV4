namespace DealCardGame.Blazor;
public partial class OpponentPropertiesComponent
{
    [Parameter]
    [EditorRequired]
    public CommandContainer? Command { get; set; }
    [Parameter]
    [EditorRequired]
    public EnumColor Color { get; set; }
    [Parameter]
    [EditorRequired]
    public DealCardGamePlayerItem? PlayerChosen { get; set; }
    [Parameter]
    public EventCallback<PropertyCardModel> OnCardChosen { get; set; }

    private HandObservable<DealCardGameCardInformation>? _hand;

    protected override void OnInitialized()
    {
        _hand = GetChosenPlayerHand();
    }

    private HandObservable<DealCardGameCardInformation> GetChosenPlayerHand()
    {
        HandObservable<DealCardGameCardInformation> output = new(Command!);
        output.AutoSelect = EnumHandAutoType.None;
        //you can only choose one.  so must raise event every step of the way.
        output.ObjectClickedAsync = ChoseCardAsync;
        output.Text = $"{PlayerChosen!.NickName} Properties";
        var list = PlayerChosen.SetData.GetCards(Color);
        output.HandList = list.ToRegularDeckDict();
        output.IsEnabled = true;
        return output;
    }
    private async Task ChoseCardAsync(DealCardGameCardInformation card, int deck)
    {

        //not sure if you want to show your information or not (?)
        PropertyCardModel output = new(PlayerChosen!.Id, card);
        await OnCardChosen.InvokeAsync(output);
    }
}