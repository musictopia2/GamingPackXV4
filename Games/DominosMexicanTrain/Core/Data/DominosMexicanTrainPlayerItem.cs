namespace DominosMexicanTrain.Core.Data;
[UseScoreboard]
public partial class DominosMexicanTrainPlayerItem : PlayerSingleHand<MexicanDomino>, IHandle<UpdateCountEventModel>
{
    public DeckRegularDict<MexicanDomino> LongestTrainList { get; set; } = new();
    [ScoreColumn]
    public int PreviousScore { get; set; }
    [ScoreColumn] //does not hurt making as scorecolumn
    public int TotalScore { get; set; }
    [ScoreColumn]
    public int PreviousLeft { get; set; }
    private int _tempCards;
    public override int ObjectCount => base.ObjectCount + _tempCards;
    public void Handle(UpdateCountEventModel message)
    {
        _tempCards = message.ObjectCount;
    }
    private IEventAggregator? _aggregator;
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