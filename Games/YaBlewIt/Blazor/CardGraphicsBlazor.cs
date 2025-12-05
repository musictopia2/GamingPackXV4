namespace YaBlewIt.Blazor;
public class CardGraphicsBlazor : BaseDeckGraphics<YaBlewItCardInformation>
{
    private static YaBlewItPlayerItem? _selfPlayer; //this can be shared.
    protected override void BeforeFilling()
    {
        if (_selfPlayer is null)
        {
            YaBlewItGameContainer temp = aa1.Resolver!.Resolve<YaBlewItGameContainer>();
            _selfPlayer = temp.PlayerList!.GetSelf();
        }
        if (DeckObject!.IsUnknown)
        {
            FillColor = cc1.DarkGray;
        }
        else if (DeckObject.CardCategory == EnumCardCategory.Fire)
        {
            FillColor = cc1.Red; //a person can't select this one anyways.  besides never shows up in hand.
        }
        else if (DeckObject.CardColor == _selfPlayer.CursedGem)
        {
            FillColor = cc1.LightGray;
        }
        else
        {
            FillColor = cc1.White;
        }
        base.BeforeFilling();
    }
    protected override SizeF DefaultSize => new(55, 72); //this is default but can change to anything you want.
    protected override bool NeedsToDrawBacks => true;
    protected override bool CanStartDrawing()
    {
        return DeckObject!.Deck > 0;
    }
    protected override void DrawBacks()
    {
        //any code necessary for drawing backs of cards.
    }
    protected override void DrawImage()
    {
        //code necessary to draw the card.
        if (DeckObject!.CardCategory == EnumCardCategory.Gem)
        {
            DrawGems();
        }
        if (DeckObject.CardCategory == EnumCardCategory.Faulty)
        {
            Draw2Words("Faulty", "Detonator");
        }
        if (DeckObject.CardCategory == EnumCardCategory.Jumper)
        {
            Draw2Words("Claim", "Jumper");
        }
        if (DeckObject.CardCategory == EnumCardCategory.Safe)
        {
            DrawSafe();
        }
        if (DeckObject.CardCategory == EnumCardCategory.Fire)
        {
            Draw2Words("Fire In", "The Hole");
        }
    }
    private void Draw2Words(string firstWord, string secondWord)
    {
        RectangleF bounds = new(1, 2, 50, 30);
        DrawText(bounds, firstWord, 15);
        bounds = new(1, 35, 50, 30);
        float fontSize;
        if (secondWord == "Detonator")
        {
            fontSize = 10;
        }
        else if (secondWord == "Jumper")
        {
            fontSize = 13;
        }
        else if (secondWord == "The Hole")
        {
            fontSize = 11;
        }
        else
        {
            fontSize = 15;
        }
        DrawText(bounds, secondWord, fontSize);
    }
    private void DrawSafe()
    {
        RectangleF bounds = new(1, 2, 50, 68);
        DrawText(bounds, "Safe", 22); //well seel
    }
    private void DrawText(RectangleF rectangle, string content, double fontSize)
    {
        Text text = new();
        if (DeckObject!.CardCategory == EnumCardCategory.Fire)
        {
            text.Fill = cc1.White.ToWebColor;
        }
        else
        {
            text.Fill = cc1.Black.ToWebColor;
        }
        text.CenterText(MainGroup!, rectangle);
        text.Content = content;
        text.Font_Size = fontSize;
    }
    private void DrawNumbers(string content, EnumColors color)
    {
        Text text = new();
        if (color == EnumColors.Wild)
        {
            text.Fill = cc1.Black.ToWebColor;
        }
        else
        {
            text.Fill = color.WebColor;
            text.PopulateStrokesToStyles();
        }
        RectangleF bounds = new(0, 44, 55, 25);
        //figure out rectangle
        text.CenterText(MainGroup!, bounds);
        text.Content = content;
        text.Font_Size = 25; //can adjust as needed
    }
    private void DrawGems()
    {
        RectangleF bounds;
        if (DeckObject!.SecondNumber == 0 && DeckObject.CardColor != EnumColors.Wild)
        {
            //the easiest one.
            bounds = new(10, 5, 35, 35);
            DrawTriangle(bounds);
            DrawNumbers(DeckObject.FirstNumber.ToString(), DeckObject.CardColor);
            return;
        }
        if (DeckObject.CardColor != EnumColors.Wild)
        {
            //this means there is 2 of them.
            bounds = new(18, 5, 16, 16);
            DrawTriangle(bounds);
            bounds = new(18, 24, 16, 16);
            DrawTriangle(bounds);
            DrawNumbers($"{DeckObject.FirstNumber}/{DeckObject.SecondNumber}", DeckObject.CardColor);
            return;
        }
        DrawWilds();
    }
    private void DrawWilds()
    {
        RectangleF bounds;
        bounds = new(10, 5, 35, 35);
        DrawTriangle(bounds);
        //needs 6 small rectangles.
        BasicList<RectangleF> firsts = new()
        {
            new(13, 35, 5, 5),
            new(18, 35, 5, 5),
            new(23, 35, 5, 5),
            new(28, 35, 5, 5),
            new(33, 35, 5, 5),
            new(38, 35, 5, 5)
        };
        BasicList<EnumColors> seconds = EnumColors.ColorList;
        if (firsts.Count != seconds.Count)
        {
            return; //just won't do.
        }
        for (int i = 0; i < firsts.Count; i++)
        {
            bounds = firsts[i];
            EnumColors color = seconds[i];
            Rect rects = new();
            rects.PopulateRectangle(bounds);
            rects.Fill = color.WebColor;
            MainGroup!.Children.Add(rects);
        }
        bounds = new(8, 18, 40, 20);
        Text text = new();
        text.CenterText(MainGroup!, bounds);
        text.Fill = cc1.Black.ToWebColor;
        text.Content = "W";
        text.Font_Size = 12;
        DrawNumbers($"{DeckObject!.FirstNumber}/{DeckObject.SecondNumber}", DeckObject.CardColor);
    }
    private void DrawTriangle(RectangleF bounds)
    {
        string color;
        if (DeckObject!.CardColor == EnumColors.Wild)
        {
            color = cc1.White;
        }
        else
        {
            color = DeckObject.CardColor.Color;
        }
        Polygon poly = bounds.PopulateTrianglePolygon(color);
        poly.PopulateStrokesToStyles(strokeWidth: 2);
        MainGroup!.Children.Add(poly);
    }
}