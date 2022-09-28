namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.RummyClasses;
public class RummyHandObservable<S, C, R> : HandObservable<R>
    where S : IFastEnumSimple
    where C : IFastEnumColorSimple
    where R : IDeckObject, IRummmyObject<S, C>, new()
{
    public Func<RummyHandObservable<S, C, R>, Task>? SetClickedAsync { get; set; }
    public bool DidClickObject { get; set; } = false;
    public void RemoveObject(int deck)
    {
        HandList.RemoveObjectByDeck(deck);
    }
    protected override Task ProcessObjectClickedAsync(R thisObject, int index)
    {
        DidClickObject = true;
        thisObject.IsSelected = !thisObject.IsSelected;
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
    private ISortObjects<R>? _thisSort;
    private readonly IGamePackageResolver _resolver;
    public void AddCards(IDeckDict<R> thisList)
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
    private void PrepSort()
    {
        bool rets;
        rets = _resolver.RegistrationExist<ISortObjects<R>>();
        if (rets == true)
        {
            _thisSort = _resolver.Resolve<ISortObjects<R>>();
        }
    }
    public RummyHandObservable(CommandContainer container, IGamePackageResolver resolver) : base(container)
    {
        _resolver = resolver;
        PrepSort();
    }
}