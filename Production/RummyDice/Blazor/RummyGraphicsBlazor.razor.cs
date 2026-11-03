namespace RummyDice.Blazor;
public partial class RummyGraphicsBlazor
{
    [Parameter]
    public RummyDiceInfo? DiceInfo { get; set; }
    [Parameter]
    public string TargetHeight { get; set; } = "";
    [Parameter]
    public EventCallback<RummyDiceInfo> OnClick { get; set; }
    private string GetFillColor()
    {
        return DiceInfo!.Color.WebColor;
    }
    private string WhiteString()
    {
        return $"<text x='50%' y='55%' font-family='Lato' font-size='70' stroke-width='2px' stroke='black' fill='white' dominant-baseline='middle' text-anchor='middle' >{DiceInfo!.Display}</text>";
    }
    private static string GetBorderColor()
    {
        return cc1.White.ToWebColor;
    }
}