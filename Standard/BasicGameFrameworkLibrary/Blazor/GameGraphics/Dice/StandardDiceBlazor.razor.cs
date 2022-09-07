namespace BasicGameFrameworkLibrary.Blazor.GameGraphics.Dice;
internal record DiceRecord(int Value, string FillColor, string DotColor, bool Selected, bool Visible, float X, float Y);
public partial class StandardDiceBlazor : GraphicsCommand
{
    private DiceRecord? _previous;
    protected override void OnAfterRender(bool firstRender)
    {
        if (Dice == null)
        {
            return;
        }
        _previous = GetRecord;
        base.OnAfterRender(firstRender);
    }
    protected override bool ShouldRender()
    {
        if (_previous == null || Dice == null)
        {
            return true;
        }
        return _previous!.Equals(GetRecord) == false;
    }
    private bool ReallySelected => Dice!.Hold || Dice.IsSelected;
    private DiceRecord GetRecord => new(Dice!.Value, Dice.FillColor, Dice.DotColor, ReallySelected, Dice.Visible, X, Y);
    [Parameter]
    public float X { get; set; }
    [Parameter]
    public float Y { get; set; }
    [Parameter]
    public string TargetHeight { get; set; } = "";
    [Parameter]
    public IStandardDice? Dice { get; set; }
    private static float DiceRadius => 10;
    private string WhiteString()
    {
        return $"<text x='50%' y='55%' font-family='tahoma' font-size='80' stroke='black' fill='white' dominant-baseline='middle' text-anchor='middle' >{Dice!.Value}</text>";
    }
    private string RectString()
    {
        return $"<rect fill={GetRealColor(Dice!.FillColor)} rx=6 ry=6 x=3 y=3 width=94 height=94 stroke='black' stroke-width= '6px' />";
    }
    private static string GetRealColor(string colorUsed)
    {
        if (colorUsed.Length == 0)
        {
            throw new CustomBasicException("Had no color.  Rethink");
        }
        if (colorUsed.Length != 9)
        {
            throw new CustomBasicException("Color In Wrong Format");
        }
        if (colorUsed.StartsWith("#FF") == false)
        {
            throw new CustomBasicException("Colors must start with FF so no transparency");
        }
        string output = $"#{colorUsed.Substring(3, 6)}";
        return output;
    }
}