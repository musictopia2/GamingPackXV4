namespace CribbagePatience.Core.Data;
public class CribbageCombos
{
    public string Description { get; set; } = "";
    public int NumberNeeded { get; set; }
    public int CardsToUse { get; set; }
    public int NumberInStraight { get; set; }
    public bool IsFlush { get; set; }
    public int NumberForKind { get; set; }
    public bool IsFullHouse { get; set; }
    public int Points { get; set; }
    public bool DoublePairNeeded { get; set; }
    public EnumScoreGroup Group { get; set; } = EnumScoreGroup.NoGroup;
    public EnumJackType JackStatus { get; set; } = EnumJackType.None;
}