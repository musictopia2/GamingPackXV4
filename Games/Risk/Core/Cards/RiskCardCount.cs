namespace Risk.Core.Cards;
public class RiskCardCount : IDeckCount //cannot do as singleton.  has to do manually
{
    int IDeckCount.GetDeckCount() => 44; //its okay to reshuffle.  makes the autoresume size smaller.
}