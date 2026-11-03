namespace YahtzeeHandsDown.Core.Data;
[SingletonGame]
public class YahtzeeHandsDownSaveInfo : BasicSavedCardClass<YahtzeeHandsDownPlayerItem, YahtzeeHandsDownCardInformation>, IMappable, ISaveInfo
{
    public int ExtraTurns { get; set; }
    public BasicList<int> Combos { get; set; } = [];
    public BasicList<int> ChanceList { get; set; } = [];
    public int FirstPlayerWentOut { get; set; }
}