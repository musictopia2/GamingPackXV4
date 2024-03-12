namespace DealCardGame.Blazor;
public partial class StealingComponent
{
    private StealingViewModel? DataContext { get; set; }
    protected override void OnInitialized()
    {
        DataContext = aa1.Resolver!.Resolve<StealingViewModel>();
        DataContext.AddAction(StateHasChanged);
    }
    public BasicGameCommand StealCommand => DataContext!.StealCommand!;
    public BasicGameCommand CancelCommand => DataContext!.CancelCommand!;
    private DealCardGameCardInformation? _chosenCard;
    private void OpponentPropertyChosen(PropertyCardModel property)
    {
        //DataContext!.StealInfo.PlayerId = property.PlayerId;
        DataContext!.StealInfo.CardChosen = property.Card.Deck;
        _chosenCard = property.Card;
    }
}