namespace DealCardGame.Blazor;
public partial class DisplayStolenPropertyComponent
{
    [Parameter]
    [EditorRequired]
    public DealCardGamePlayerItem? PlayerToDisplay { get; set; }
    private string DisplayName => PlayerToDisplay!.PlayerCategory == EnumPlayerCategory.Self ? "Your Property Being Stolen" : "Play Just Say No To Continue To Steal This";
    private DealCardGameGameContainer? _gameContainer;
    private DealCardGameCardInformation? _displayCard;
    protected override void OnInitialized()
    {
        _gameContainer = aa1.Resolver!.Resolve<DealCardGameGameContainer>();
        _displayCard = _gameContainer.DeckList.GetSpecificItem(_gameContainer.SaveRoot.CardStolen);
        base.OnInitialized();
    }
}