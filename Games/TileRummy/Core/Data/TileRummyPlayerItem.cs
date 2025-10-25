namespace TileRummy.Core.Data;
[UseScoreboard]
public partial class TileRummyPlayerItem : PlayerSingleHand<TileInfo>, IHandle<UpdateCountEventModel>
{
    private IEventAggregator? _aggregator;
    public DeckRegularDict<TileInfo> AdditionalTileList { get; set; } = new();
    public bool InitCompleted { get; set; }
    [ScoreColumn]
    public int Score { get; set; }
    public override int ObjectCount => MainHandList.Count + _tempCards;
    private int _tempCards;
    public void Handle(UpdateCountEventModel message)
    {
        _tempCards = message.ObjectCount;
    }
    private partial void Subscribe();
    private partial void Unsubscribe();
    public void DoInit(IEventAggregator aggregator)
    {
        _aggregator = aggregator;
        _aggregator.ClearSingle<UpdateCountEventModel>();
        //this will subscribe as needed.
        Subscribe();
    }
    public void Close()
    {
        Unsubscribe();
    }
}