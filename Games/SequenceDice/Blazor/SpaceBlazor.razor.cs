namespace SequenceDice.Blazor;
public partial class SpaceBlazor : GraphicsCommand
{
    private SpaceInfoCP? _space;
    protected override void OnInitialized()
    {
        if (CommandParameter != null)
        {
            _space = (SpaceInfoCP)CommandParameter;
        }

        base.OnInitialized();
    }
    private string GetFillRegular => _space!.WasRecent ? "Yellow" : "White";
    private string GetFillDice => _space!.WasRecent ? cs1.Yellow : cs1.White;
    private SimpleDice GetDiceInfo()
    {
        SimpleDice output = new();
        output.Populate(_space!.Number / 2);
        output.FillColor = GetFillDice;
        return output;
    }
    private static PointF GetDiceLocation(int index) => index == 1 ? new(2, 2) : new(26, 26);
    private static SizeF GetDiceSize => new(21, 21); //try this.
    private double FontSize => _space!.Number == 12 ? 30 : 47;
}