namespace Payday.Core.Data;
[UseScoreboard]
public partial class PaydayPlayerItem : PlayerBoardGame<EnumColorChoice>
{
    public override bool DidChooseColor => Color.IsNull == false && Color != EnumColorChoice.None;
    public override void Clear()
    {
        Color = EnumColorChoice.None;
    }
    [ScoreColumn]
    public partial decimal Loans { get; set; }
    [ScoreColumn]
    public partial decimal MoneyHas { get; set; }
    [ScoreColumn]
    public partial int CurrentMonth { get; set; }
    [ScoreColumn]
    public partial int DayNumber { get; set; }
    [ScoreColumn]
    public partial int ChoseNumber { get; set; }
    public decimal NetIncome()
    {
        return MoneyHas - Loans;
    }
    public DeckRegularDict<CardInformation> Hand { get; set; } = [];
}