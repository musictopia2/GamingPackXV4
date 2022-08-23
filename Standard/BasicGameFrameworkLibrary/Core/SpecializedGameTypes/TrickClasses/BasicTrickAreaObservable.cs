namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.TrickClasses;
public abstract partial class BasicTrickAreaObservable<S, T> : SimpleControlObservable
    where S : IFastEnumSimple
    where T : class, ITrickCard<S>, new()
{
    private readonly IEventAggregator _aggregator;
    public ControlCommand? CardSingleClickCommand { get; set; }
    [Command(EnumCommandCategory.Control)]
    protected async Task PrivateCardClickedAsync(T card)
    {
        if (card == null)
        {
            throw new CustomBasicException("Card can't be Nothing");
        }
        if (CardList.IndexOf(card) == -1)
        {
            throw new CustomBasicException("Can't be -1 for index");
        }
        await ProcessCardClickAsync(card);
    }
    protected abstract Task ProcessCardClickAsync(T thisCard);
    public int MaxPlayers { get; set; }
    protected async Task AnimateWinAsync()
    {
        if (WinCard == null)
        {
            throw new CustomBasicException("Can't animate win because no card sent");
        }
        await _aggregator.PublishAsync(new AnimateTrickEventModel()); //hopefully this simple this time.
    }
    protected override void EnableChange()
    {
        CardSingleClickCommand!.ReportCanExecuteChange();
    }
    public BasicTrickAreaObservable(CommandContainer container, IEventAggregator aggregator) : base(container) //this still needs the ieventaggregator for trick animations.
    {
        _aggregator = aggregator;
        CreateCommands();
    }
    partial void CreateCommands();
    public DeckRegularDict<T> CardList = new();
    public T? WinCard;
    protected override void PrivateEnableAlways() { }
    public void TradeCard(int index, T newCard)
    {
        PointF oldLocation;
        oldLocation = CardList[index].Location;
        newCard.Location = oldLocation;
        newCard.IsSelected = false;
        newCard.Drew = false;
        var oldCard = CardList[index];
        CardList.ReplaceItem(oldCard, newCard); // hopefully that simple.
    }
    public bool Visible { get; set; }
    public void HideCards()
    {
        CardList.ForEach(x => x.Visible = false);
    }
}