namespace FillOrBust.Blazor;
public class CardGraphicsBlazor : BaseDeckGraphics<FillOrBustCardInformation>
{
    protected override SizeF DefaultSize => new(107, 135);
    protected override bool NeedsToDrawBacks => true;
    protected override bool CanStartDrawing()
    {
        return DeckObject!.Status != EnumCardStatusList.Unknown;
    }
    protected override void BeforeFilling()
    {
        if (DeckObject!.IsUnknown)
        {
            FillColor = cc1.Red;
        }
        else if (DeckObject!.Value == 2500)
        {
            FillColor = cc1.Maroon;
        }
        else if (DeckObject.Status == EnumCardStatusList.MustBust)
        {
            FillColor = cc1.Red;
        }
        else if (DeckObject!.Value > 0)
        {
            FillColor = cc1.Aqua;
        }
        else if (DeckObject.Status == EnumCardStatusList.DoubleTrouble)
        {
            FillColor = cc1.Aqua;
        }
        else
        {
            FillColor = cc1.Maroon;
        }
    }
    protected override void DrawBacks() { }
    protected override void DrawImage()
    {
        if (DeckObject == null)
        {
            return;
        }
        if (DeckObject.Value == 0 && DeckObject.Status == EnumCardStatusList.None)
        {
            return;
        }
        if (DeckObject.Value == 2500)
        {
            DrawRevenge();
            return;
        }
        if (DeckObject.Value == 1000)
        {
            DrawFill();
            return;
        }
        if (DeckObject.Status == EnumCardStatusList.MustBust)
        {
            DrawMustBust();
            return;
        }
        if (DeckObject.Status == EnumCardStatusList.DoubleTrouble)
        {
            DrawDoubleTrouble();
            return;
        }
        if (DeckObject.Status == EnumCardStatusList.NoDice)
        {
            DrawNoDice();
            return;
        }
        if (DeckObject.Value > 0)
        {
            DrawBonus();
        }
    }
    private void DrawText(string content, float fontSize, string fillColor, RectangleF rect, bool hasBorders = true)
    {
        Text text = new();
        text.Content = content;
        text.CenterText(MainGroup!, rect);
        text.Fill = fillColor.ToWebColor();
        text.Font_Size = fontSize;
        if (hasBorders)
        {
            text.PopulateStrokesToStyles(); //i think.
        }
        else
        {
            text.Font_Weight = "bold";
        }
    }
    private void DrawRectangle(RectangleF rect, string fillcolor, bool hasBorders = false)
    {
        Rect output = new();
        output.PopulateRectangle(rect);
        output.Fill = fillcolor.ToWebColor();
        if (hasBorders)
        {
            output.PopulateStrokesToStyles(strokeWidth: 5, color: cc1.Aqua.ToWebColor());
        }
        MainGroup!.Children.Add(output);
    }
    private void DrawBonus()
    {
        var firstRect = new RectangleF(4, 4, 95, 25);
        var secondRect = new RectangleF(4, 100, 95, 25);
        DrawRectangle(firstRect, cc1.Red);
        DrawRectangle(secondRect, cc1.Red);
        var firstText = new RectangleF(0, 35, 107, 35);
        var secondText = new RectangleF(0, 70, 107, 35);
        var fontSize = 33;
        DrawText("Bonus", fontSize, cc1.Maroon, firstText);
        DrawText(DeckObject!.Value.ToString(), fontSize, cc1.Maroon, secondText);
    }
    private void DrawFill()
    {
        var firstText = new RectangleF(0, 20, 107, 40);

        var firstFont = 40;
        var secondFont = 35;
        var tempRect = new RectangleF(15, 65, 77, 35);
        DrawRectangle(tempRect, cc1.Red);
        DrawText("Fill", firstFont, cc1.Red, firstText);
        DrawText("1000", secondFont, cc1.Aqua, tempRect);
    }
    private void DrawDoubleTrouble()
    {
        RectangleF firstRect;
        RectangleF lastRect;
        RectangleF firstText;
        RectangleF middleText;
        RectangleF lastText;
        firstRect = new RectangleF(4, 5, 95, 20);
        lastRect = new RectangleF(4, 107, 95, 20);
        firstText = new RectangleF(0, 22, 107, 30);
        middleText = new RectangleF(0, 52, 107, 30);
        lastText = new RectangleF(0, 82, 107, 30);
        string firstColor = cc1.LightPink;
        string middleColor = cc1.Maroon;
        var fontSize = 25;
        DrawRectangle(firstRect, cc1.Red);
        DrawRectangle(lastRect, cc1.Red);
        DrawText("Double", fontSize, firstColor, firstText);
        DrawText("Trouble", fontSize, middleColor, middleText);
        DrawText("Double", fontSize, firstColor, lastText);
    }
    private void DrawMustBust()
    {
        RectangleF firstRect = new(7, 10, 90, 25);
        RectangleF lastRect = new(7, 97, 90, 25);
        DrawRectangle(firstRect, cc1.Maroon, true);
        DrawRectangle(lastRect, cc1.Maroon, true);
        var fontSize = 30;
        var textRect = new RectangleF(0, 35, 107, 37);
        DrawText("Must", fontSize, cc1.Aqua, textRect);
        textRect = new RectangleF(0, 63, 107, 37); //i think.
        DrawText("Bust!", fontSize, cc1.Aqua, textRect);
    }
    private void DrawRevenge()
    {
        RectangleF tempRect = new(-3, 5, 109, 27);
        var fontSize = 18;
        DrawText("Vengeance", fontSize, cc1.Aqua, tempRect, false);
        tempRect = new RectangleF(-1, 27, 109, 27);
        DrawText("2500", fontSize, cc1.Aqua, tempRect, false);
        tempRect = new RectangleF(4, 55, 95, 70);
        DrawRectangle(tempRect, cc1.Red);
        Image image = new();
        image.PopulateFullExternalImage("revenge.png");
        image.PopulateImagePositionings(tempRect);
        MainGroup!.Children.Add(image);
    }
    private void DrawNoDice()
    {
        var textRect = new RectangleF(0, 10, 107, 44);
        var fontSize = 27;
        DrawText("No Dice", fontSize, cc1.Aqua, textRect);
        var lastRect = new RectangleF(23, 59, 60, 60);
        Circle circle = new();
        circle.PopulateCircle(lastRect, "");
        circle.PopulateStrokesToStyles(strokeWidth: 7, color: cc1.Red.ToWebColor());
        MainGroup!.Children.Add(circle);
        PointF point = new(lastRect.Left + (lastRect.Size.Width / 4), lastRect.Top + (lastRect.Size.Height / 4));
        SizeF size = new(34, 34);
        RectangleF fins = new(point, size);
        MainGroup.DrawDice(fins, 6);
        Line line = new();
        PointF firstPoint = new(lastRect.Left + 10, lastRect.Top + 10);
        PointF lastPoint = new(lastRect.Right - 10, lastRect.Bottom - 10);
        line.PopulateLine(firstPoint, lastPoint);
        line.PopulateStrokesToStyles(strokeWidth: 7, color: cc1.Red.ToWebColor(), opacity: .5);
        MainGroup.Children.Add(line);
    }
}