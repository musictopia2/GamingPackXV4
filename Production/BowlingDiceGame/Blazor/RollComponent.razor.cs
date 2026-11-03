namespace BowlingDiceGame.Blazor;
public partial class RollComponent
{
    private string GetStyle()
    {
        if (CanProcess == false)
        {
            return "display: none;";
        }
        return "fill: red;";
    }
}