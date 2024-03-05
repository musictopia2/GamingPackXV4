﻿
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
        rect.Fill = cc1.Black.ToWebColor();
        container.Children.Add(rect);
        if (Dice.Category == EnumDiceCategory.Color)
        {
            //this means will simply be the pawn piece.
            var otherRect = new RectangleF(3, 3, 34, 34);

            //string yourColor = cc1.Blue.ToWebColor();
            //string otherColor = Dice.Color.Color;
            //string nextColor = Dice.Color.WebColor;
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
            //has to figure out the slide now.
            //RectangleF slide = new(2, 10, 36, 20);
            Rect slide = new();
            RectangleF fins = new(2, 25, 36, 10);
            slide.PopulateRectangle(fins);
            slide.Fill = cc1.Red.ToWebColor();

            
            container.Children.Add(slide);
            fins = new(2, 2, 36, 20);

            Text text = new();
            text.Font_Size = 15;
            text.Content = "Slide";
            text.CenterText(container, fins);
            text.Fill = cc1.White.ToWebColor();
            return;
        }
        if (Dice.Category == EnumDiceCategory.Sorry)
        {
            Text text = new();
            RectangleF sorryRect = new(2, 2, 35, 42);
            text.Font_Size = 40;
            text.Content = "S";
            text.CenterText(container, sorryRect);
            text.Fill = cc1.Black.ToWebColor();
            text.PopulateStrokesToStyles(cc1.White.ToWebColor(), 1);
            return;
        }

        //for now, nothing else.
    }
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        SvgRenderClass render = new();
        ISvg svg = new SVG();
        svg.Width = TargetHeight;
        svg.Height = TargetHeight;
        svg.ViewBox = "0 0 40 40";
        CreateGrapics(svg);
        render.RenderSvgTree(svg, builder);
        base.BuildRenderTree(builder);
    }
}