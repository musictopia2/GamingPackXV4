namespace A8RoundRummy.Blazor;
public class CardGraphicsBlazor : BaseDeckGraphics<A8RoundRummyCardInformation>
{
    protected override SizeF DefaultSize => new(55, 72); //this is default but can change to anything you want.
    protected override bool NeedsToDrawBacks => true;
    protected override bool CanStartDrawing()
    {
        return true;
    }
    protected override void DrawBacks()
    {
        var firstRect = new RectangleF(0, 1, 54, 23);
        var secondRect = new RectangleF(0, 22, 54, 23);
        var thirdRect = new RectangleF(0, 43, 54, 23);
        var fontSize = firstRect.Height * .5;
        DrawText(firstRect, "8", cc1.Red, fontSize);
        DrawText(secondRect, "Round", cc1.Red, fontSize);
        DrawText(thirdRect, "Rummy", cc1.Red, fontSize);
    }
    private void DrawText(RectangleF rectangle, string content, string color, double fontSize)
    {
        Text text = new();
        text.Fill = color.ToWebColor();
        text.CenterText(MainGroup!, rectangle);
        text.Content = content;
        text.Font_Size = fontSize;
    }
    protected override void BeforeFilling()
    {
        if (DeckObject!.IsUnknown)
        {
            FillColor = cc1.Blue;
        }
        else
        {
            FillColor = cc1.White;
        }
        base.BeforeFilling();
    }
    protected override void DrawImage()
    {
        switch (DeckObject!.CardType)
        {
            case EnumCardType.Regular:
                DrawRegular();
                break;
            case EnumCardType.Wild:
                DrawWild();
                break;
            case EnumCardType.Reverse:
                DrawReverse();
                break;
            default:
                break;
        }
    }
    private void DrawRegular()
    {
        string color = DeckObject!.Color.ToColor();
        var shapeRect = new RectangleF(13, 5, 30, 30);
        if (DeckObject!.Shape == EnumCardShape.Circle)
        {
            Circle circle = new();
            circle.PopulateCircle(shapeRect, color);
            circle.PopulateStrokesToStyles(strokeWidth: 2);
            MainGroup!.Children.Add(circle);
        }
        else if (DeckObject.Shape == EnumCardShape.Square)
        {
            Rect rect = new();
            rect.PopulateRectangle(shapeRect);
            rect.Fill = color.ToWebColor();
            rect.PopulateStrokesToStyles(strokeWidth: 2);
            MainGroup!.Children.Add(rect);
        }
        else
        {
            Polygon poly = shapeRect.PopulateTrianglePolygon(color);
            poly.PopulateStrokesToStyles(strokeWidth: 2);
            MainGroup!.Children.Add(poly);
        }
        var textRect = new RectangleF(0, 35, 55, 35);
        var fontSize = textRect.Height;
        Text text = new();
        text.CenterText(MainGroup, textRect);
        text.Font_Size = fontSize;
        text.Fill = color.ToWebColor();
        text.PopulateStrokesToStyles();
        text.Content = DeckObject.Value.ToString();
    }
    private void DrawWild()
    {
        var smileyRect = new RectangleF(8, 5, 35, 35);
        var textRect = new RectangleF(0, 40, 55, 30);
        var fontSize = textRect.Height * 0.5f;
        MainGroup!.DrawSmiley(smileyRect, "", cc1.Black, cc1.Black, 2);
        DrawText(textRect, "WILD", cc1.Black, fontSize);
    }
    private void DrawReverse()
    {
        var firstRect = new RectangleF(0, 1, 54, 23);
        var secondRect = new RectangleF(0, 25, 54, 23);
        var thirdRect = new RectangleF(0, 49, 54, 23);
        var fontSize = firstRect.Height * .45;
        DrawText(firstRect, "Reverse", cc1.Green, fontSize);
        DrawText(secondRect, "And New", cc1.Green, fontSize);
        DrawText(thirdRect, "Hand", cc1.Green, fontSize);
    }
}