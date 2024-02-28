namespace MonopolyDicedGame.Blazor;
public class BasicDiceBlazor : GraphicsCommand
{
    [Parameter]
    public BasicDiceModel? Dice { get; set; }
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
        if (Dice is null)
        {
            return;
        }
        Rect rect = StartRect();
        rect.Fill = cc1.White.ToWebColor();
        container.Children.Add(rect);
        if (Dice.IsSelected)
        {
            rect = StartRect();
            rect.Fill = cc1.Red.ToWebColor();
            rect.Fill_Opacity = ".1";
            container.Children.Add(rect);
        }
        if (Dice.WhatCard == EnumBasicType.Property)
        {
            container.DrawPropertyValue(Dice.GetColor(), Dice.GetRegularValue().ToString());
            return;
        }
        if (Dice.WhatCard == EnumBasicType.Chance)
        {
            container.DrawChanceDice(this);
            return;
        }
        if (Dice.WhatCard == EnumBasicType.Utility)
        {
            if (Dice.Index == 9)
            {
                container.DrawWaterDice(this);
                return;
            }
            if (Dice.Index == 10)
            {
                container.DrawElectricDice(this);
                return;
            }
            throw new CustomBasicException("Utilities is only 9 or 10");
        }
        if (Dice.WhatCard == EnumBasicType.Railroad)
        {
            container.DrawTrainDice(this);
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