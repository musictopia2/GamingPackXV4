﻿namespace ThreeLetterFun.Blazor;
public class TileCardBlazor : BaseDeckGraphics<TileInformation>
{
    protected override SizeF DefaultSize => new(19, 30);
    protected override bool NeedsToDrawBacks => false;
    protected override bool CanStartDrawing()
    {
        return true;
    }
    protected override void DrawBacks()
    {
        //don't raise exceptions since they don't show up on desktops properly anyways.
    }
    public TileCardBlazor()
    {
        BorderWidth = 1; //try to make it alot smaller for this game.
        RoundedRadius = 2;
    }
    protected override void BeforeFilling()
    {
        if (DeckObject!.IsMoved)
        {
            FillColor = cc.Yellow;
        }
        else
        {
            FillColor = cc.White;
        }
    }
    protected override void DrawImage()
    {
        Text text = new();
        text.CenterText();
        string letter = DeckObject!.Letter.ToString().ToUpper();
        text.Content = letter;
        string color = letter.GetColorOfLetter();
        text.Fill = color.ToWebColor();
        text.Font_Size = 18;
        MainGroup!.Children.Add(text);
    }
}