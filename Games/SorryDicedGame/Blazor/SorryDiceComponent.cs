namespace SorryDicedGame.Blazor;
public class SorryDiceComponent : GraphicsCommand
{
    [Parameter]
    public SorryDiceModel? Dice { get; set; }
    [Parameter]
    public string TargetHeight { get; set; } = "4vh";
    private static Rect StartRect()
    {
        Rect output = new();
        output.RX = "2";
        output.RY = "2";
        output.Width = "40";
        output.Height = "40";
        return output;
    }
    private void CreateGrapics(ISvg container)
    {
        if (Dice is null)
        {
            return;
        }
        Rect rect = StartRect();
        if (Dice.IsEnabled)
        {
            rect.Fill = cc1.Black.ToWebColor;
        }
        else
        {
            rect.Fill = cc1.DarkGray.ToWebColor;
        }
        container.Children.Add(rect);
        if (Dice.IsSelected)
        {
            rect = StartRect();
            rect.Fill = cc1.Red.ToWebColor;
            rect.Fill_Opacity = ".5";
            container.Children.Add(rect);
        }
        if (Dice.Category == EnumDiceCategory.Color)
        {
            var otherRect = new RectangleF(3, 3, 34, 34);
            container!.DrawPawnPiece(otherRect, Dice.Color.Color);
            return;
        }
        if (Dice.Category == EnumDiceCategory.Wild)
        {
            RectangleF wild = new(2, 2, 16, 16);
            container.DrawPawnPiece(wild, cc1.Blue);
            wild = new(2, 20, 16, 16);
            container.DrawPawnPiece(wild, cc1.Yellow);
            wild = new(20, 2, 16, 16);
            container.DrawPawnPiece(wild, cc1.Green);
            wild = new(20, 20, 16, 16);
            container.DrawPawnPiece(wild, cc1.Red);
            return;
        }
        if (Dice.Category == EnumDiceCategory.Slide)
        {
            Rect slide = new();
            RectangleF fins = new(2, 25, 36, 10);
            slide.PopulateRectangle(fins);
            slide.Fill = cc1.Red.ToWebColor;
            container.Children.Add(slide);
            fins = new(2, 2, 36, 20);
            Text text = new();
            text.Font_Size = 15;
            text.Content = "Slide";
            text.CenterText(container, fins);
            if (Dice.IsEnabled)
            {
                text.Fill = cc1.White.ToWebColor;
            }
            else
            {
                text.Fill = cc1.Black.ToWebColor;
            }
            return;
        }
        if (Dice.Category == EnumDiceCategory.Sorry)
        {
            Text text = new();
            RectangleF sorryRect = new(2, 2, 35, 42);
            text.Font_Size = 40;
            text.Content = "S";
            text.CenterText(container, sorryRect);
            text.Fill = cc1.Black.ToWebColor;
            text.PopulateStrokesToStyles(cc1.White.ToWebColor, 1);
            return;
        }
    }
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        SvgRenderClass render = new();
        ISvg svg = new SVG();
        svg.Width = TargetHeight;
        svg.Height = TargetHeight;
        svg.ViewBox = "0 0 40 40";
        CreateGrapics(svg);
        CreateClick(svg);
        render.RenderSvgTree(svg, builder);
        base.BuildRenderTree(builder);
    }
}