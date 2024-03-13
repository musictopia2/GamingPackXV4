namespace DealCardGame.Blazor;
public partial class DisplayTradeComponent
{
    [Parameter]
    [EditorRequired]
    public DealCardGamePlayerItem? PlayerToDisplay { get; set; }
    private string DisplayName => PlayerToDisplay!.PlayerCategory == EnumPlayerCategory.Self ? "Play Just Say No To Prevent This Trade" : "Play Just Say No To Continue Trading";
    private DealCardGameGameContainer? _gameContainer;

    private DealCardGameCardInformation? _yourCard;
    private DealCardGameCardInformation? _opponentCard;
    private DealCardGamePlayerItem? _mainPlayer;
    private DealCardGamePlayerItem? _tradePlayer;



    //private DealCardGameCardInformation? _displayCard;
    protected override void OnInitialized()
    {
        _gameContainer = aa1.Resolver!.Resolve<DealCardGameGameContainer>();
        _tradePlayer = _gameContainer.PlayerList!.Single(x => x.Id == _gameContainer.SaveRoot.PlayerUsedAgainst);
        _mainPlayer = _gameContainer.PlayerList!.GetWhoPlayer();
        _yourCard = _gameContainer.DeckList.GetSpecificItem(_gameContainer.SaveRoot.YourTrade);
        _opponentCard = _gameContainer.DeckList.GetSpecificItem(_gameContainer.SaveRoot.OpponentTrade);

        //_displayCard = _gameContainer.DeckList.GetSpecificItem(_gameContainer.SaveRoot.CardStolen);
        base.OnInitialized();
    }
}