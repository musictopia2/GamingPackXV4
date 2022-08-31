namespace YahtzeeHandsDown.Core.Data;
[SingletonGame]
public class YahtzeeHandsDownSaveInfo : BasicSavedCardClass<YahtzeeHandsDownPlayerItem, YahtzeeHandsDownCardInformation>, IMappable, ISaveInfo
{
    public int ExtraTurns { get; set; }
    public BasicList<int> Combos { get; set; } = new();
    public BasicList<int> ChanceList { get; set; } = new();
    public int FirstPlayerWentOut { get; set; }
}