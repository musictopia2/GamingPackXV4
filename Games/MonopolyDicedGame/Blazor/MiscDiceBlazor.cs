namespace MonopolyDicedGame.Blazor;
public class MiscDiceBlazor : ComponentBase
{
    [Parameter]
    public EnumMiscType Category { get; set; }
    [Parameter]
    public string TargetHeight { get; set; } = "4vh";

    private static Rect StartRect()
    {
        Rect output = new();
        output.RX = "2";
        output.RY = "2";
        output.Width = "50";
        output.Height = "50";
        return output;
    }
    private void CreateGrapics(ISvg container)
    {
        Rect rect = StartRect();
        rect.Fill = cc1.White.ToWebColor();
        container.Children.Add(rect);

        if (Category == EnumMiscType.Police)
        {
            container.DrawPolice(this);
            return;
        }
        if (Category == EnumMiscType.Free)
        {
            container.DrawOutOfJailFree(this);
            return;
        }
        if (Category == EnumMiscType.RegularHouse)
        {
            container.DrawHouse(this);
            return;
        }
        if (Category == EnumMiscType.BrokenHouse)
        {
            container.DrawBrokenHouse(this);
            return;
        }
        if (Category == EnumMiscType.Hotel)
        {
            container.DrawHotel(this);
            return;
        }
        if (Category == EnumMiscType.Go)
        {
            container.DrawGoDice(this);
            return;
        }
        throw new CustomBasicException("Unable to draw");
    }
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        SvgRenderClass render = new();
        ISvg svg = new SVG();
        svg.Width = TargetHeight;
        svg.Height = TargetHeight;
        svg.ViewBox = "0 0 50 50";
        CreateGrapics(svg);
        render.RenderSvgTree(svg, builder);
        base.BuildRenderTree(builder);
    }
}