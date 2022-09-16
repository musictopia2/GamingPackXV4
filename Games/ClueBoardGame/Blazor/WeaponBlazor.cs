namespace ClueBoardGame.Blazor;
public class WeaponBlazor : ComponentBase
{
    [Parameter]
    public WeaponInfo? Weapon { get; set; }
    [CascadingParameter]
    public BasePieceGraphics? MainGraphics { get; set; }
    //private PointF? _previousLocation;
    //protected override bool ShouldRender()
    //{
    //    if (_previousLocation is null)
    //    {
    //        return false;
    //    }
    //    if (MainGraphics!.Location.X == _previousLocation.Value.X && MainGraphics!.Location.Y == _previousLocation.Value.Y)
    //    {
    //        return false;
    //    }
    //    return true;
    //}
    protected override void OnInitialized()
    {
        MainGraphics!.OriginalSize = Weapon!.DefaultSize;
        base.OnInitialized();
    }
    private string GetFileName => $"{Weapon!.Weapon}.svg";
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        //_previousLocation = MainGraphics!.Location;
        ISvg svg = MainGraphics!.GetMainSvg(false);
        SvgRenderClass render = new ();
        Image image = new ();
        image.AutoIncrementElement(svg); //try this way (?)
        image.PopulateFullExternalImage(this, GetFileName);
        svg.Children.Add(image);
        render.RenderSvgTree(svg, builder);
        base.BuildRenderTree(builder);
    }
}