namespace BowlingDiceGame.Blazor;
public partial class BowlingDiceListBlazor
{
    [Parameter]
    public BasicList<SingleDiceInfo>? DiceList { get; set; }
}