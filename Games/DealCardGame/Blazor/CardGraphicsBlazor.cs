namespace DealCardGame.Blazor;
public class CardGraphicsBlazor : BaseDeckGraphics<DealCardGameCardInformation>
{
    protected override SizeF DefaultSize => new(55, 72); //this is default but can change to anything you want.
    protected override bool NeedsToDrawBacks => true;
    protected override bool CanStartDrawing()
    {
        return true;
    }
    protected override void DrawBacks()
    {
        //any code necessary for drawing backs of cards.
    }
    protected override void DrawImage()
    {
        //code necessary to draw the card.
        if (DeckObject!.CardType == EnumCardType.Money)
        {
            DrawMoney();
            return;
        }
        if (DeckObject!.CardType == EnumCardType.PropertyRegular)
        {
            DrawRegularProperty();
            return;
        }
        if (DeckObject!.CardType == EnumCardType.PropertyWild && DeckObject.AnyColor == false)
        {
            DrawWildProperty2Choices();
            return;
        }
        if (DeckObject.CardType == EnumCardType.PropertyWild)
        {
            DrawWildPropertyAnyChoice();
            return;
        }
        if (DeckObject.CardType == EnumCardType.ActionRent && DeckObject.AnyColor == false)
        {
            DrawRentRegular();
            return;
        }
        if (DeckObject.CardType == EnumCardType.ActionRent)
        {
            DrawRentWild();
            return;
        }
        if (DeckObject.CardType == EnumCardType.ActionStandard)
        {
            DrawActionStandard();
            return;
        }
    }
    private void DrawSimpleClaimValue()
    {
        //RectangleF bounds = new(5, 5, 30, 30);
        Text text = new();
        text.X = "5";
        text.Y = "19";
        MainGroup!.Children.Add(text);
        //text.CenterText(MainGroup!, bounds);
        text.Fill = cc1.Black.ToWebColor();
        text.Font_Size = 21;
        text.PopulateStrokesToStyles(cc1.White.ToWebColor(), 1);
        text.Content = DeckObject!.ClaimedValue.ToString();
    }
    private void DrawActionStandard()
    {
        DrawClaimBackground();
        RectangleF bounds = new(6, 18, 38, 38);
        Circle circle = new();
        circle.PopulateCircle(bounds, cc1.White);
        circle.PopulateStrokesToStyles(cc1.Black.ToWebColor(), 1);
        MainGroup!.Children.Add(circle);
        if (DeckObject!.SecondActionText == "")
        {
            AddBoldText(bounds, DeckObject.FirstActionText, 8);
            DrawSimpleClaimValue();
            return;
        }
        bounds = new(6, 19, 38, 19);
        AddBoldText(bounds, DeckObject.FirstActionText, 6);
        bounds = new(6, 28, 38, 19);
        AddBoldText(bounds, DeckObject.SecondActionText, 6);
        DrawSimpleClaimValue();
    }
    private void AddBoldText(RectangleF bounds,  string content, int fontSize)
    {
        Text text = new();
        content = content.ToUpper();
        text.CenterText(MainGroup!, bounds);
        text.Content = content;
        text.Font_Size = fontSize;
        text.Font_Weight = "bold";
    }
    private void DrawRentWild()
    {
        DrawClaimBackground();
        var list = EnumColor.ColorList;
        RectangleF main;
        list = list.Take(5).ToBasicList();
        int x = 5;
        foreach (var item in list)
        {
            main = new(x, 5, 8, 12);
            PopulateStandardRectangle(main, item);
            x += 8;
        }
        list = EnumColor.ColorList;
        list = list.Skip(5).ToBasicList();
        x = 5;
        foreach (var item in list)
        {
            main = new(x, 51, 8, 12);
            PopulateStandardRectangle(main, item);
            x += 8;
        }
        DrawRentCenter();
    }
    private void DrawRentRegular()
    {
        DrawClaimBackground();
        RectangleF bounds = new(5, 5, 40, 12);
        PopulateStandardRectangle(bounds, DeckObject!.FirstColorChoice);
        bounds = new(5, 51, 40, 12);
        PopulateStandardRectangle(bounds, DeckObject.SecondColorChoice);
        DrawRentCenter();
    }
    private void DrawRentCenter()
    {
        //this is where i draw the text and the circles.
        RectangleF bounds = new(10, 19, 30, 30);
        Circle circle = new();
        circle.PopulateCircle(bounds, cc1.White);
        circle.PopulateStrokesToStyles(cc1.Black.ToWebColor(), 1);
        MainGroup!.Children.Add(circle);
        AddBoldText(bounds, "Rent", 8);
        DrawSimpleClaimValue();
    }
    private void DrawRegularProperty()
    {
        RectangleF bounds = GetFillRectangle;
        Rect rect = new();
        rect.PopulateRectangle(bounds);
        rect.Fill = DeckObject!.MainColor.WebColor;
        MainGroup!.Children.Add(rect);
        bounds = new(6, 18, 38, 38);
        Circle circle = new();
        circle.PopulateCircle(bounds, cc1.White);
        circle.PopulateStrokesToStyles(cc1.Black.ToWebColor(), 1);
        MainGroup!.Children.Add(circle);
        AddBoldText(bounds, "Property", 7);
        DrawSimpleClaimValue();
    }
    private void DrawWildPropertyAnyChoice()
    {
        RectangleF body = new(5, 35, 40, 25);
        DrawWildText(body);
        var list = EnumColor.ColorList;
        RectangleF main;
        int x = 5;
        foreach (var item in list)
        {
            main = new(x, 5, 4, 25);
            PopulateStandardRectangle(main, item);
            x += 4;
        }
    }
    //if changing the main color means the image is different and causes issues, then rethink.
    private void DrawWildProperty2Choices()
    {
        RectangleF top = new(5, 5, 40, 15);
        RectangleF body = new(5, 23, 40, 21);
        RectangleF footer = new(5, 47, 40, 15);
        PopulateStandardRectangle(top, DeckObject!.FirstColorChoice);
        DrawWildText(body);
        PopulateStandardRectangle(footer, DeckObject.SecondColorChoice);
        DrawSimpleClaimValue();
    }
    private void DrawWildText(RectangleF bounds)
    {
        PopulateStandardRectangle(bounds, DeckObject!.MainColor);
        Text text = new();
        text.Content = "W";
        text.Font_Size = 21;
        text.Fill = cc1.Black.ToWebColor();
        text.CenterText(MainGroup!, bounds);
        text.PopulateStrokesToStyles(cc1.White.ToWebColor(), 1);
    }
    private void PopulateStandardRectangle(RectangleF bounds, EnumColor color)
    {
        string toUse;
        if (color == EnumColor.None)
        {
            toUse = cc1.LightGray.ToWebColor();
        }
        else
        {
            toUse = color.WebColor;
        }
        Rect rect = new();
        rect.PopulateRectangle(bounds);
        rect.Fill = toUse;
        rect.PopulateStrokesToStyles();
        MainGroup!.Children.Add(rect);
    }
    private static RectangleF GetFillRectangle => new(5, 5, 40, 57);
    private void DrawClaimBackground()
    {
        RectangleF bounds = GetFillRectangle;
        Rect rect = new();
        rect.PopulateRectangle(bounds);
        if (DeckObject!.ClaimedValue == 10)
        {
            rect.Fill = cc1.DarkOrange.ToWebColor();
        }
        if (DeckObject.ClaimedValue == 5)
        {
            rect.Fill = cc1.DarkOrchid.ToWebColor();
        }
        if (DeckObject.ClaimedValue == 4)
        {
            rect.Fill = cc1.DeepSkyBlue.ToWebColor();
        }
        if (DeckObject.ClaimedValue == 3)
        {
            rect.Fill = cc1.LightGreen.ToWebColor();
        }
        if (DeckObject.ClaimedValue == 2)
        {
            rect.Fill = cc1.NavajoWhite.ToWebColor();
        }
        if (DeckObject.ClaimedValue == 1)
        {
            rect.Fill = cc1.LemonChiffon.ToWebColor();
        }
        MainGroup!.Children.Add(rect);
    }
    private void DrawMoney()
    {
        DrawClaimBackground();
        RectangleF bounds = GetFillRectangle;
        Text text = new();
        text.Content = DeckObject!.ClaimedValue.ToString();
        text.Font_Size = 32;
        text.Fill = cc1.Black.ToWebColor();
        text.CenterText(MainGroup!, bounds);
        text.PopulateStrokesToStyles(cc1.White.ToWebColor(), 1);
    }
}