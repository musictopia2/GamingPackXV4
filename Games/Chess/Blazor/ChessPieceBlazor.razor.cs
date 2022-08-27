namespace Chess.Blazor;
public partial class ChessPieceBlazor
{
    [Parameter]
    public EnumColorChoice Color { get; set; }

    [Parameter]
    public int LongestSize { get; set; }

    [Parameter]
    public PointF Location { get; set; }

    [Parameter]
    public EnumPieceType PieceCategory { get; set; }
    private string GetMainStyle()
    {
        return $"fill:{Color.WebColor}; stroke:#000000; stroke-linecap:butt;";
    }
    private string GetFill()
    {
        return $"fill: { Color.WebColor};";
    }
}