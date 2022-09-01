namespace HuseHearts.Core.Logic;
[SingletonGame]
public class HuseHeartsDelegates
{
    public Action<DeckRegularDict<HuseHeartsCardInformation>>? SetDummyList { get; set; }
    public Func<DeckRegularDict<HuseHeartsCardInformation>>? GetDummyList { get; set; }
}