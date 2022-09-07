namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicPlayerClasses;
public partial class PlayerRummyHand<D> : PlayerSingleHand<D>, IPlayerRummyHand<D>, IHandle<UpdateCountEventModel>
    where D : IDeckObject, new()
{
    private readonly IEventAggregator _aggregator;
    public PlayerRummyHand()
    {
        _aggregator = Resolver!.Resolve<IEventAggregator>();
    }
    public DeckRegularDict<D> AdditionalCards { get; set; } = new DeckRegularDict<D>();
    public override int ObjectCount => MainHandList.Count + _tempCards;
    private int _tempCards;
    public void Handle(UpdateCountEventModel message)
    {
        _tempCards = message.ObjectCount;
    }
    public void DoInit()
    {
        _aggregator.Clear<UpdateCountEventModel>();
        Subscribe();
    }
    private partial void Subscribe();
    private partial void Unsubscribe();
    public void Close()
    {
        Unsubscribe();
    }
}