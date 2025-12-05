namespace MonopolyDicedGame.Blazor;
public class BasicDiceBlazor : GraphicsCommand
{
    [Parameter]
    public BasicDiceModel? Dice { get; set; }
    [Parameter]
    public string TargetHeight { get; set; } = "4vh";
    [Parameter]
    public bool OnBoardNotOwned { get; set; } //if on board and not owned, will be different.
    private static Rect StartRect()
    {
        Rect output = new();
        output.RX = "2";
        output.RY = "2";
        output.Width = "50";
        output.Height = "50";
        return output;
    }
    private void CreateGrapics(SVG container)
    {
        if (Dice is null)
        {
            return;
        }
        if (OnBoardNotOwned == false)
        {
            Rect rect = StartRect();
            rect.Fill = cc1.White.ToWebColor;
            container.Children.Add(rect);
            if (Dice.IsSelected)
            {
                rect = StartRect();
                rect.Fill = cc1.Red.ToWebColor;
                rect.Fill_Opacity = ".1";
                container.Children.Add(rect);
            }
            if (Dice.WhatDice == EnumBasicType.Property)
            {
                container.DrawPropertyValue(Dice.GetColor(), Dice.GetRegularValue().ToString());
                return;
            }
            if (Dice.WhatDice == EnumBasicType.Chance)
            {
                container.DrawChanceDice();
                return;
            }
            if (Dice.WhatDice == EnumBasicType.Utility)
            {
                if (Dice.Index == 9)
                {
                    container.DrawWaterDice();
                    return;
                }
                if (Dice.Index == 10)
                {
                    container.DrawElectricDice();
                    return;
                }
                throw new CustomBasicException("Utilities is only 9 or 10");
            }
            if (Dice.WhatDice == EnumBasicType.Railroad)
            {
                container.DrawTrainDice();
                return;
            }
            throw new CustomBasicException("Unable to draw");
        }
        //this means be a little different.
        if (Dice.WhatDice == EnumBasicType.Railroad)
        {
            container.DrawTrainBoard(Dice.GetMonopolyValue().ToString());
            return;
        }
        if (Dice.WhatDice == EnumBasicType.Utility)
        {
            if (Dice.Index == 9)
            {
                container.DrawWaterBoard(Dice.GetMonopolyValue().ToString());
                return;
            }
            if (Dice.Index == 10)
            {
                container.DrawElectricBoard(Dice.GetMonopolyValue().ToString());
                return;
            }
        }
        throw new CustomBasicException("Unable to draw");
        //rethink the others.
    }
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        SvgRenderClass render = new();
        SVG svg = new ();
        svg.Width = TargetHeight;
        svg.Height = TargetHeight;
        svg.ViewBox = "0 0 50 50";
        CreateGrapics(svg);
        render.RenderSvgTree(svg, builder);
        base.BuildRenderTree(builder);
    }
}