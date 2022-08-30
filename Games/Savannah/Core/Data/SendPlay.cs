namespace Savannah.Core.Data;
public class SendPlay
{
    public int Deck { get; set; }
    public int Pile { get; set; }
    public EnumSelectType WhichType { get; set; }
    public int Player { get; set; } //if discard2, then needs to return which player you are taking from
}