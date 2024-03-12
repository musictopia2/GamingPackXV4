namespace DealCardGame.Blazor;
public partial class TradingComponent
{
    private TradingViewModel? DataContext { get; set; }
    protected override void OnInitialized()
    {
        DataContext = aa1.Resolver!.Resolve<TradingViewModel>();
        _hand = GetYourCards();
        DataContext.AddAction(StateHasChanged);
    }
    private HandObservable<DealCardGameCardInformation>? _hand;
    public BasicGameCommand TradeCommand => DataContext!.TradeCommand!;
    public BasicGameCommand CancelCommand => DataContext!.CancelCommand!;
    private DealCardGameCardInformation? _tradeChosenCard;
    private DealCardGameCardInformation? _yourChosenCard;
    private void OpponentPropertyChosen(PropertyCardModel property)
    {
        //DataContext!.TradeInfo.PlayerId = property.PlayerId;
        DataContext!.TradeInfo.OpponentCard = property.Card.Deck;
        //DataContext!.StealInfo.PlayerId = property.PlayerId;
        //DataContext.StealInfo.CardChosen = property.Card.Deck;
        _tradeChosenCard = property.Card;
    }
    private string YourText => $"{DataContext!.GetYourPlayer.NickName} chosen";
    private string TradeText => $"{DataContext!.GetChosenPlayer.NickName} chosen";
    private DealCardGameCardInformation YourCard()
    {
        if (_yourChosenCard is not null)
        {
            return _yourChosenCard;
        }
        return GetBlankCard();
    }
    private DealCardGameCardInformation TradeCard()
    {
        if (_tradeChosenCard is not null)
        {
            return _tradeChosenCard;
        }
        return GetBlankCard();
    }
    private static DealCardGameCardInformation GetBlankCard()
    {
        DealCardGameCardInformation output = new();
        output.IsUnknown = true;
        output.Deck = 1;
        return output;
    }
    private HandObservable<DealCardGameCardInformation> GetYourCards()
    {
        HandObservable<DealCardGameCardInformation> output = new(DataContext!.GetCommandContainer);
        output.Text = "Your Properties that has no completed sets";
        output.AutoSelect = EnumHandAutoType.None;
        output.ObjectClickedAsync = YourCardChosenAsync;
        var player = DataContext.GetYourPlayer;
        output.IsEnabled = true;
        foreach (var item in player.SetData)
        {
            if (item.HasRequiredSet() == false)
            {
                output.HandList.AddRange(item.Cards);
            }
        }
        return output;
    }
    private Task YourCardChosenAsync(DealCardGameCardInformation card, int index)
    {
        _yourChosenCard = card; //hopefully this simple.
        DataContext!.TradeInfo.YourCard = card.Deck;
        var property = DataContext.GetYourPlayer.GetPropertyFromCard(card.Deck);
        DataContext.TradeInfo.YourColor = property!.Color;
        //StateHasChanged(); //i think.
        return Task.CompletedTask;
    }
}