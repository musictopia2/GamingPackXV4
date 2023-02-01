namespace TileRummy.Blazor;
public class TileRummyImageBlazor : BaseDeckGraphics<TileInfo>
{
    protected override SizeF DefaultSize => new(60, 40);
    protected override bool NeedsToDrawBacks => true;
    protected override bool CanStartDrawing()
    {
        if (DeckObject!.IsUnknown == true)
        {
            return true;
        }
        return DeckObject.Color != EnumColorType.None;
    }
    protected override string DrawFillColor
    {
        get
        {
            if (DeckObject!.WhatDraw == EnumDrawType.FromSet)
            {
                return cc1.Purple.ToWebColor();
            }
            return base.DrawFillColor;
        }
    }
    protected override void DrawBacks() { } //this means drawing nothing.
    protected override void DrawImage()
    {
        if (DeckObject!.IsJoker)
        {
            RectangleF rect = new(16, 6, 28, 28);
            MainGroup!.DrawSmiley(rect, "", DeckObject.Color.Color, cc1.Black, 2);
            return;
        }
        Text text = new();
        MainGroup!.Children.Add(text);
        text.CenterText();
        text.Content = DeckObject.Number.ToString();
        text.Font_Size = 35;
        text.Y = "50%";
        text.X = "45%";
        text.PopulateStrokesToStyles(strokeWidth: 1);
        text.Fill = DeckObject.Color.WebColor;
    }
}