namespace Rummy500.Core.Data;
public class SendNewSet
{
    public BasicList<int> DeckList { get; set; } = new();
    public EnumWhatSets SetType { get; set; }
    public bool UseSecond { get; set; }
}