namespace ClueBoardGame.Blazor;
public class WeaponBlazor : ComponentBase
{
    [Parameter]
    public WeaponInfo? Weapon { get; set; }
    [CascadingParameter]
    public BasePieceGraphics? MainGraphics { get; set; }
    protected override void OnInitialized()
    {
        MainGraphics!.OriginalSize = Weapon!.DefaultSize;
        base.OnInitialized();
    }
    private string GetFileName => $"{Weapon!.Weapon}.svg";
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        ISvg svg = MainGraphics!.GetMainSvg(false);
        SvgRenderClass render = new ();
        Image image = new ();
        image.PopulateFullExternalImage(this, GetFileName);
        svg.Children.Add(image);
        render.RenderSvgTree(svg, 0, builder);
        base.BuildRenderTree(builder);
    }
}