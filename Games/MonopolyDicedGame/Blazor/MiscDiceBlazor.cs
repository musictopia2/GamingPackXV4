namespace MonopolyDicedGame.Blazor;
public class MiscDiceBlazor : ComponentBase
{
    [Parameter]
    public EnumMiscType Category { get; set; }
    [Parameter]
    public string TargetHeight { get; set; } = "4vh";
    [Parameter]
    public int NumberOfHouses { get; set; }
    private static Rect StartRect()
    {
        Rect output = new();
        output.RX = "0";
        output.RY = "0";
        output.Width = "50";
        output.Height = "50";
        return output;
    }
    private void CreateGrapics(SVG container)
    {
        Rect rect = StartRect();
        rect.Fill = cc1.White.ToWebColor();
        container.Children.Add(rect);
        if (Category == EnumMiscType.Police)
        {
            container.DrawPolice();
            return;
        }
        if (Category == EnumMiscType.Free)
        {
            container.DrawOutOfJailFree();
            return;
        }
        if (Category == EnumMiscType.RegularHouse)
        {
            container.DrawHouse(NumberOfHouses);
            return;
        }
        if (Category == EnumMiscType.BrokenHouse)
        {
            container.DrawBrokenHouse();
            return;
        }
        if (Category == EnumMiscType.Hotel)
        {
            container.DrawHotel();
            return;
        }
        if (Category == EnumMiscType.Go)
        {
            container.DrawGoDice();
            return;
        }
        throw new CustomBasicException("Unable to draw");
    }
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        SvgRenderClass render = new();
        SVG svg = new();
        svg.Width = TargetHeight;
        svg.Height = TargetHeight;
        svg.ViewBox = "0 0 50 50";
        CreateGrapics(svg);
        render.RenderSvgTree(svg, builder);
        base.BuildRenderTree(builder);
    }
}