namespace MonopolyCardGame.Core.TempHandClasses;
public class TempHand : HandObservable<MonopolyCardGameCardInformation>
{
    public event SetClickedEventHandler? SetClickedAsync;
    public delegate Task SetClickedEventHandler(TempHand setUsed);
    public TempHand(CommandContainer command, IGamePackageResolver resolver) : base(command)
    {
        _resolver = resolver;
        PrepSort();
    }
    public bool DidClickObject { get; set; } = false; //sometimes this is needed for mobile.
    public void RemoveObject(int deck)
    {
        HandList.RemoveObjectByDeck(deck);
    }
    protected override Task ProcessObjectClickedAsync(MonopolyCardGameCardInformation thisObject, int index)
    {
        DidClickObject = true; //this is needed too.  so if other gets raised, will be ignored because already handled.
        thisObject.IsSelected = !thisObject.IsSelected; //try here.  hopefully works well.
        return Task.CompletedTask;
    }
    protected override async Task PrivateBoardSingleClickedAsync()
    {
        if (SetClickedAsync == null)
        {
            return;
        }
        await SetClickedAsync.Invoke(this);
    }
    private ISortObjects<MonopolyCardGameCardInformation>? _thisSort;
    private readonly IGamePackageResolver _resolver;
    private void PrepSort()
    {
        bool rets;
        rets = _resolver.RegistrationExist<ISortObjects<MonopolyCardGameCardInformation>>();
        if (rets == true)
        {
            _thisSort = _resolver.Resolve<ISortObjects<MonopolyCardGameCardInformation>>();
        }
    }
    public void AddCards(IDeckDict<MonopolyCardGameCardInformation> thisList)
    {
        HandList.AddRange(thisList);
        SortCards();
        HandList.UnselectAllObjects();
    }
    private void SortCards()
    {
        if (_thisSort != null)
        {
            HandList.Sort(_thisSort);
        }
        else
        {
            HandList.Sort();
        }
    }


}
