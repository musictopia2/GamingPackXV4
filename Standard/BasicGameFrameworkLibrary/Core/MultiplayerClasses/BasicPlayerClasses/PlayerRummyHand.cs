namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicPlayerClasses;
public partial class PlayerRummyHand<D> : PlayerSingleHand<D>, IPlayerRummyHand<D>, IHandle<UpdateCountEventModel>
    where D : IDeckObject, new()
{
    private readonly IEventAggregator _aggregator;
    public PlayerRummyHand()
    {
        _aggregator = Resolver!.Resolve<IEventAggregator>();
    }
    public DeckRegularDict<D> AdditionalCards { get; set; } = new DeckRegularDict<D>(); //taking a risk.  hopefully it pays off.
    public override int ObjectCount => MainHandList.Count + _tempCards; //hopefully this simple.
    private int _tempCards;
    public void Handle(UpdateCountEventModel message)
    {
        _tempCards = message.ObjectCount;
    }
    public void DoInit()
    {
        _aggregator.Clear<UpdateCountEventModel>(); //try to make it clear only updatecount.  because clearing everything means cannot show next round or even game.
        Subscribe();
    }
    private partial void Subscribe();
    private partial void Unsubscribe();
    public void Close()
    {
        Unsubscribe();
    }
}