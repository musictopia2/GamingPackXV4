namespace MilkRun.Blazor;
public class CardGraphicsBlazor : BaseDeckGraphics<MilkRunCardInformation>
{
    protected override SizeF DefaultSize => new(55, 72);
    protected override bool NeedsToDrawBacks => true;
    protected override bool CanStartDrawing()
    {
        if (DeckObject!.IsUnknown)
        {
            return true;
        }
        return DeckObject.CardCategory != EnumCardCategory.None && DeckObject.MilkCategory != EnumMilkType.None;
    }
    private void DrawText(string content, RectangleF bounds, string color, float fontSize)
    {
        Text text = new();
        text.Content = content;
        text.Fill = color.ToWebColor();
        text.CenterText(MainGroup!, bounds);
        text.Font_Size = fontSize;
        text.PopulateStrokesToStyles();
    }
    private void DrawRectangle(RectangleF bounds, string color)
    {
        Rect rect = new();
        rect.PopulateRectangle(bounds);
        rect.Fill = color.ToWebColor();
        MainGroup!.Children.Add(rect);
    }
    protected override void DrawBacks()
    {
        RectangleF firstRect;
        firstRect = new RectangleF(4, 4, 42, 28);
        var secondRect = new RectangleF(4, 35, 42, 28);
        DrawRectangle(firstRect, cc1.DeepPink);
        DrawRectangle(secondRect, cc1.Chocolate);
        BasicList<RectangleF> thisList = new();
        float fontSize = DefaultSize.Height / 3.2f;
        string color = cc1.Aqua;
        thisList.Add(firstRect);
        thisList.Add(secondRect);
        int x = 0;
        string thisText;
        foreach (var thisRect in thisList)
        {
            x += 1;
            switch (x)
            {
                case 1:
                    {
                        thisText = "Milk";
                        break;
                    }
                case 2:
                    {
                        thisText = "Run";
                        break;
                    }
                default:
                    {
                        return;
                    }
            }
            DrawText(thisText, thisRect, color, fontSize);
        }
    }
    private void DrawPiece()
    {
        if (DeckObject == null || MainGroup == null)
        {
            return;
        }
        var bounds = new RectangleF(2, 2, DefaultSize.Width - 6, DefaultSize.Height / 2.1f);
        string fileName;
        if (DeckObject.MilkCategory == EnumMilkType.Chocolate)
        {
            fileName = "chocolate.svg";
        }
        else if (DeckObject.MilkCategory == EnumMilkType.Strawberry)
        {
            fileName = "strawberry.svg";
        }
        else
        {
            return;
        }
        Image image = new();
        image.PopulateFullExternalImage(fileName);
        image.PopulateImagePositionings(bounds);
        MainGroup.Children.Add(image);
    }
    private void DrawOval(RectangleF bounds, string color)
    {
        Ellipse ellipse = new();
        ellipse.PopulateEllipse(bounds);
        ellipse.Fill = color.ToWebColor();
        MainGroup!.Children.Add(ellipse);
    }
    protected override void DrawImage()
    {
        if (DeckObject == null || MainGroup == null)
        {
            return;
        }
        DrawPiece();
        string thisText = "";
        var circleRect = new RectangleF(3, 33, 45, 30);
        var secondRect = new RectangleF(3, 35, 45, 30);
        if (DeckObject.CardCategory != EnumCardCategory.Joker)
        {
            if (DeckObject.CardCategory == EnumCardCategory.Go)
            {
                thisText = "Go";
            }
            else if (DeckObject.CardCategory == EnumCardCategory.Stop)
            {
                thisText = "Stop";
            }
            else
            {
                thisText = DeckObject.Points.ToString();
            }
        }
        switch (DeckObject.CardCategory)
        {
            case EnumCardCategory.Joker:
                var newRect = new RectangleF(13, 34, 29, 29);
                if (DeckObject.MilkCategory == EnumMilkType.Chocolate)
                {
                    MainGroup.DrawSmiley(newRect, cc1.Chocolate, cc1.Black, cc1.Black, 2);
                }
                else if (DeckObject.MilkCategory == EnumMilkType.Strawberry)
                {
                    MainGroup.DrawSmiley(newRect, cc1.DeepPink, cc1.Black, cc1.Black, 2);
                }
                return;
            case EnumCardCategory.Go:
                DrawOval(circleRect, cc1.Lime);
                break;
            case EnumCardCategory.Stop:
                DrawOval(circleRect, cc1.Red);
                break;
            default:
                break;
        }
        if (thisText == "")
        {
            return;
        }
        var fontSize = secondRect.Height * 1f;
        if (thisText == "Stop")
        {
            fontSize = secondRect.Height * .65f; //otherwise parts get cut off.
        }
        DrawText(thisText, secondRect, cc1.White, fontSize);
    }
}