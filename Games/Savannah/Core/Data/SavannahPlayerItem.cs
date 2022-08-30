namespace Savannah.Core.Data;
[UseScoreboard]
public partial class SavannahPlayerItem : PlayerSingleHand<RegularSimpleCard>
{
    [ScoreColumn]
    public int DiscardLeft => DiscardList.Count; //try this way.
    [ScoreColumn]
    public int ReserveLeft => ReserveList.Count;
    [ScoreColumn]
    public string NextReserve
    {
        get
        {
            if (ReserveList.Count == 0)
            {
                return "None";
            }
            var card = ReserveList.Last();
            if (card.Value == EnumRegularCardValueList.Joker)
            {
                return "Joker";
            }
            if (card.Value == EnumRegularCardValueList.Queen)
            {
                return "Queen";
            }
            if (card.Value == EnumRegularCardValueList.King)
            {
                return "King";
            }
            if (card.Value == EnumRegularCardValueList.Jack)
            {
                return "Jack";
            }
            if (card.Value == EnumRegularCardValueList.LowAce || card.Value == EnumRegularCardValueList.HighAce)
            {
                return "Ace";
            }
            return card.Value.Value.ToString();
        }
    }
    public int WhenToStackDiscards { get; set; }
    public DeckRegularDict<RegularSimpleCard> ReserveList { get; set; } = new();
    public DeckRegularDict<RegularSimpleCard> DiscardList { get; set; } = new();

    [JsonIgnore]
    public SelfDiscardCP? SelfDiscard { get; set; }
}