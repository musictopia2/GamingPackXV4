namespace TeeItUp.Blazor;
public class CardGraphicsBlazor : BaseDeckGraphics<TeeItUpCardInformation>
{
    protected override SizeF DefaultSize => new(76, 105);
    protected override bool NeedsToDrawBacks => true;
    protected override void BeforeFilling()
    {
        if (DeckObject!.IsUnknown)
        {
            FillColor = cc1.Green;
        }
        else
        {
            FillColor = cc1.White;
        }
        base.BeforeFilling();
    }
    protected override bool CanStartDrawing()
    {
        return true;
    }
    private void DrawText(string content, RectangleF bounds, string color, float fontSize, bool hasBorders)
    {
        Text text = new();
        text.Content = content;
        text.CenterText(MainGroup!, bounds);
        text.Fill = color.ToWebColor();
        text.Font_Size = fontSize;
        if (hasBorders)
        {
            text.PopulateStrokesToStyles();
        }
    }
    protected override void DrawBacks()
    {
        Image image = new();
        RectangleF bounds = new(2, 15, 70, 70);
        image.PopulateFullExternalImage(this, "GolfBall.svg");
        image.PopulateImagePositionings(bounds);
        MainGroup!.Children.Add(image);
    }
    protected override void DrawImage()
    {
        if (DeckObject == null)
        {
            return;
        }
        var topRect = new RectangleF(13, 13, 50, 50);
        DrawCircle(topRect);
        if (DeckObject.IsMulligan == false)
        {
            var firstFonts = topRect.Height * 0.8f;
            DrawText(DeckObject.Points.ToString(), topRect, cc1.White, firstFonts, true);
        }
        var fullBottom = new RectangleF(4, 67, 68, 35);
        var firstBottom = new RectangleF(4, 67, 68, 17);
        var secondBottom = new RectangleF(4, 84, 68, 17);
        var thisList = GetTextList();
        var fontSize = fullBottom.Height * 0.41f;
        string color;
        if (DeckObject.IsMulligan)
        {
            color = cc1.Red;
        }
        else
        {
            color = cc1.Green;
        }

        if (thisList.Count == 1)
        {
            DrawText(thisList.Single(), fullBottom, color, fontSize, false);
        }
        else if (thisList.Count == 2)
        {
            DrawText(thisList.First(), firstBottom, color, fontSize, false);
            DrawText(thisList.Last(), secondBottom, color, fontSize, false);
        }
    }
    private void DrawCircle(RectangleF bounds)
    {
        if (MainGroup == null)
        {
            return;
        }
        Circle circle = new();
        circle.PopulateCircle(bounds, cc1.Green);
        MainGroup.Children.Add(circle);
    }
    private static BasicList<string> GetSingleList(string firstText)
    {
        return new() { firstText };
    }
    private static BasicList<string> GetPairList(string firstText, string secondText)
    {
        return new() { firstText, secondText };
    }
    private BasicList<string> GetTextList()
    {
        if (DeckObject == null)
        {
            return new();
        }
        if (DeckObject.IsMulligan == true)
        {
            return GetSingleList("Mulligan");
        }
        if (DeckObject.Points == -5)
        {
            return GetPairList("Hole", "In One");
        }
        if (DeckObject.Points == -3)
        {
            return new() { "Albatross" };
        }
        if (DeckObject.Points == -2)
        {
            return new() { "Eagle" };
        }
        if (DeckObject.Points == -1)
        {
            return GetSingleList("Birdie");
        }
        if (DeckObject.Points == 0)
        {
            return GetSingleList("Par");
        }
        if (DeckObject.Points == 1)
        {
            return GetSingleList("Bogey");
        }
        if (DeckObject.Points == 2)
        {
            return GetPairList("Double", "Bogey");
        }
        if (DeckObject.Points == 3)
        {
            return GetPairList("Triple", "Bogey");
        }
        if (DeckObject.Points == 4)
        {
            return GetPairList("Out Of", "Bounds");
        }
        if (DeckObject.Points == 5)
        {
            return GetPairList("Water", "Hazard");
        }
        if (DeckObject.Points == 6)
        {
            return GetPairList("Sand", "Trap");
        }
        if (DeckObject.Points == 7)
        {
            return GetSingleList("In Rough");
        }
        if (DeckObject.Points == 8)
        {
            return GetPairList("Lost", "Ball");
        }
        if (DeckObject.Points == 9)
        {
            return GetSingleList("In Ravine");
        }
        return new(); //since runtime messages don't show up anyways (?)
    }
}