namespace BasicGameFrameworkLibrary.Blazor.BasicControls.GameBoards;
public partial class GameSpaceWithoutBorders
{
    //this is like the borderedspace but no borders.
    //used for games like checkers/chess
    //but could be used for any game where we need a space but somehow no borders.
    //this is even
    //used for rollem dice game as well.
    public enum EnumShapeCategory
    {
        Rectangle,
        Oval //decided for oval instead of circle to be more flexible.  if you want circle, then make the width and height the same.
    }
    [Parameter]
    public EnumShapeCategory ShapeCategory { get; set; } = EnumShapeCategory.Rectangle; //default to rectangle if none is used.
    [Parameter]
    public SizeF SpaceSize { get; set; } //this is for the size of the space that needs the border.
    [Parameter]
    public EventCallback SpaceClicked { get; set; }
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    [Parameter]
    public string FillColor { get; set; } = cs1.Transparent;
    private async Task ProcessClickAsync()
    {
        if (SpaceClicked.HasDelegate == false)
        {
            return;
        }
        await SpaceClicked.InvokeAsync(null);
    }
    [Parameter]
    public PointF SpaceLocation { get; set; }
    private string ViewBox()
    {
        return $"0 0 {SpaceSize.Width} {SpaceSize.Height}";
    }
    private float RadiusX => SpaceSize.Width / 2;
    private float RadiusY => SpaceSize.Height / 2;
}