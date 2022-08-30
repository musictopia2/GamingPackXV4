namespace MonasteryCardGame.Core.Data;
public class SetInfo
{
    public DeckRegularDict<MonasteryCardInfo> SetCards { get; set; } = new();
    public EnumMonasterySets SetType { get; set; }
    public bool HasLaid { get; set; }
    public bool DidSucceed { get; set; }
    public int HowMany { get; set; }
}