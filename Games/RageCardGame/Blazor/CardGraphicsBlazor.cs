namespace RageCardGame.Blazor;
public class CardGraphicsBlazor : BaseDarkCardsBlazor<RageCardGameCardInformation>
{
    protected override SizeF DefaultSize => new(55, 72); //this is default but can change to anything you want.
    protected override bool NeedsToDrawBacks => true;
    protected override bool CanStartDrawing()
    {
        return DeckObject!.IsUnknown || DeckObject.SpecialType != EnumSpecialType.Blank;
    }
    protected override void DrawBacks()
    {
        var fontSize = DefaultSize.Height * .25f;
        DrawText("Rage", fontSize);
    }
    private void DrawText(string content, float fontSize)
    {
        string textColor = cc1.White;
        Text text = new();
        text.Content = content;
        RectangleF bounds = new(0, -3, DefaultSize.Width, DefaultSize.Height);
        text.CenterText(MainGroup!, bounds);
        text.Fill = textColor.ToWebColor();
        text.Font_Size = fontSize;
        text.PopulateStrokesToStyles();
    }
    private void DrawText(string content, float fontSize, string textColor, RectangleF bounds)
    {
        Text text = new();
        text.Content = content;
        text.Fill = textColor.ToWebColor();
        text.Font_Size = fontSize;
        text.CenterText(MainGroup!, bounds);
    }
    private void DrawNumberCard()
    {
        var fontSize = DefaultSize.Height * .5f;
        DrawText(DeckObject!.Value.ToString(), fontSize);
    }
    private void DrawSpecialCard()
    {
        var firstText = DeckObject!.SpecialType switch
        {
            EnumSpecialType.Out => "X",
            EnumSpecialType.Change => "!",
            EnumSpecialType.Wild => "W",
            EnumSpecialType.Bonus => "+5",
            EnumSpecialType.Mad => "-5",
            _ => "U"
        };
        var firstRect = new RectangleF(2, 2, 45, 20);
        float firstFontSize = firstRect.Height * .8f;
        string textColor = cc1.White;
        DrawText(firstText, firstFontSize, textColor, firstRect);
        var secondRect = new RectangleF(2, 22, 45, 20);
        float secondFontSize = secondRect.Height * .5f;
        textColor = cc1.Black;
        DrawWhiteRectangle(secondRect);
        DrawText(DeckObject!.SpecialType.ToString(), secondFontSize, textColor, secondRect);
        var thirdRect = new RectangleF(2, 42, 45, 28);
        float thirdFontSize = thirdRect.Height * .6f;
        textColor = cc1.Red;
        DrawWhiteRectangle(thirdRect);
        DrawText("Rage", thirdFontSize, textColor, thirdRect);
    }
    private void DrawWhiteRectangle(RectangleF bounds)
    {
        Rect rect = new();
        rect.PopulateRectangle(bounds);
        rect.Fill = cc1.White.ToWebColor();
        MainGroup!.Children.Add(rect);
    }
    protected override void DrawImage()
    {
        if (DeckObject!.SpecialType == EnumSpecialType.None)
        {
            DrawNumberCard();
        }
        else
        {
            DrawSpecialCard();
        }
        if (DeckObject!.IsSelected || DeckObject.Drew)
        {
            DrawHighlighters();
        }
    }
    protected override bool IsLightColored => DeckObject!.Color == EnumColor.Yellow;
    protected override void BeforeFilling()
    {
        FillColor = PrivateColor();
    }
    private string PrivateColor()
    {
        if (DeckObject!.IsUnknown)
        {
            return cc1.Red;
        }
        if (DeckObject.SpecialType == EnumSpecialType.None)
        {
            return DeckObject!.Color.Color;
        }
        return cc1.Black;
    }
}