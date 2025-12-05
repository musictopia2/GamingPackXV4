namespace FiveCrowns.Blazor;
public class CardGraphicsBlazor : BaseDeckGraphics<FiveCrownsCardInformation>
{
    private readonly float _suitSize = 32.4f;
    protected override SizeF DefaultSize { get; } = new SizeF(165, 216);
    protected override bool NeedsToDrawBacks => true;
    protected override bool CanStartDrawing()
    {
        if (DeckObject == null)
        {
            return false; //we don't even have one.
        }
        if (DeckObject!.IsUnknown)
        {
            return true;
        }
        if (DeckObject.CardType == EnumCardTypeList.Joker)
        {
            return true;
        }
        return DeckObject!.Suit != EnumSuitList.None;
    }
    protected override void DrawBacks()
    {
        Image image = new();
        image.PopulateBasicExternalImage("deckofcardback.svg");
        image.Width = "140";
        image.Height = "191";
        image.X = "10";
        image.Y = "10";
        MainGroup!.Children.Add(image);
    }
    protected override void DrawImage()
    {
        var rect_Center = new RectangleF(40, 27, 80, 162);
        if (DeckObject!.CardValue == EnumCardValueList.Joker)
        {
            DrawJoker(); //this part is the same.
            return;
        }
        DrawCardAndStartingSuit();
        if (DeckObject.CardValue > EnumCardValueList.Ten)
        {
            DrawFaceCards();
        }
        else
        {
            DrawCenterSuits(rect_Center);
        }
    }
    private static BasicList<PointF> GetCenterPoints(EnumCardValueList value)
    {
        BasicList<PointF> arr_Current = new();
        if (value == EnumCardValueList.Three)
        {
            arr_Current.Add(new PointF(2, 1));
            arr_Current.Add(new PointF(2, 4));
            arr_Current.Add(new PointF(2, 7));
        }
        else if (value == EnumCardValueList.Four)
        {
            arr_Current.Add(new PointF(1, 1));
            arr_Current.Add(new PointF(1, 7));
            arr_Current.Add(new PointF(3, 1));
            arr_Current.Add(new PointF(3, 7));
        }
        else if (value == EnumCardValueList.Five)
        {
            arr_Current.Add(new PointF(1, 1));
            arr_Current.Add(new PointF(1, 7));
            arr_Current.Add(new PointF(3, 1));
            arr_Current.Add(new PointF(3, 7));
            arr_Current.Add(new PointF(2, 4));
        }
        else if (value == EnumCardValueList.Six)
        {
            arr_Current.Add(new PointF(1, 1));
            arr_Current.Add(new PointF(1, 4));
            arr_Current.Add(new PointF(1, 7));
            arr_Current.Add(new PointF(3, 1));
            arr_Current.Add(new PointF(3, 4));
            arr_Current.Add(new PointF(3, 7));
        }
        else if (value == EnumCardValueList.Seven)
        {
            arr_Current.Add(new PointF(1, 1));
            arr_Current.Add(new PointF(1, 4));
            arr_Current.Add(new PointF(1, 7));
            arr_Current.Add(new PointF(3, 1));
            arr_Current.Add(new PointF(3, 4));
            arr_Current.Add(new PointF(3, 7));
            arr_Current.Add(new PointF(2, 3));
        }
        else if (value == EnumCardValueList.Eight)
        {
            arr_Current.Add(new PointF(1, 1));
            arr_Current.Add(new PointF(1, 3));
            arr_Current.Add(new PointF(1, 5));
            arr_Current.Add(new PointF(1, 7));
            arr_Current.Add(new PointF(3, 1));
            arr_Current.Add(new PointF(3, 3));
            arr_Current.Add(new PointF(3, 5));
            arr_Current.Add(new PointF(3, 7));
        }
        else if (value == EnumCardValueList.Nine)
        {
            arr_Current.Add(new PointF(1, 1));
            arr_Current.Add(new PointF(1, 3));
            arr_Current.Add(new PointF(1, 5));
            arr_Current.Add(new PointF(1, 7));
            arr_Current.Add(new PointF(3, 1));
            arr_Current.Add(new PointF(3, 3));
            arr_Current.Add(new PointF(3, 5));
            arr_Current.Add(new PointF(3, 7));
            arr_Current.Add(new PointF(2, 4));
        }
        else if (value == EnumCardValueList.Ten)
        {
            arr_Current.Add(new PointF(1, 1));
            arr_Current.Add(new PointF(1, 3));
            arr_Current.Add(new PointF(1, 5));
            arr_Current.Add(new PointF(1, 7));
            arr_Current.Add(new PointF(3, 1));
            arr_Current.Add(new PointF(3, 3));
            arr_Current.Add(new PointF(3, 5));
            arr_Current.Add(new PointF(3, 7));
            arr_Current.Add(new PointF(2, 2));
            arr_Current.Add(new PointF(2, 6));
        }
        return arr_Current;
    }
    private void DrawCenterSuits(RectangleF rect_Center)
    {
        Hashtable arr_Rectangles = new(); // hashtable is supported with the .net standard 2.0.  collection is not
        int int_Row;
        int int_Col;
        int int_Count;
        RectangleF rect_Temp;
        int int_Height;
        PointF pt_Temp;
        int_Height = (int)rect_Center.Height / 5;
        for (int_Row = 0; int_Row <= 6; int_Row++)
        {
            for (int_Col = 0; int_Col <= 2; int_Col++)
            {
                rect_Temp = new RectangleF(rect_Center.Left + ((rect_Center.Width / 3) * int_Col), rect_Center.Top + ((rect_Center.Height / 7) * int_Row) - (int_Height - (rect_Center.Height / 7)), _suitSize, _suitSize);
                arr_Rectangles.Add(new PointF(int_Col + 1, int_Row + 1), rect_Temp);
            }
        }
        BasicList<PointF> arr_Current = GetCenterPoints(DeckObject!.CardValue);
        var loopTo = arr_Current.Count;
        for (int_Count = 1; int_Count <= loopTo; int_Count++)
        {
            pt_Temp = arr_Current[int_Count - 1]; // because its 0 based
            rect_Temp = (RectangleF)arr_Rectangles[pt_Temp]!;
            if (pt_Temp.Y > 4)
            {
                DrawSuit(rect_Temp, true);
            }
            else
            {
                DrawSuit(rect_Temp, false);
            }
        }
    }
    private void DrawSuit(RectangleF rect_Suit, bool bln_Flip)
    {
        G currentGroup;
        if (bln_Flip == false)
        {
            currentGroup = MainGroup!;
        }
        else
        {
            currentGroup = new G(); //rotations
            currentGroup.Rotate180Degrees(rect_Suit);
            MainGroup!.Children.Add(currentGroup);
        }
        DrawCustomSuit(currentGroup, GetColor, rect_Suit);
    }
    private void DrawFaceCards()
    {
        string color = GetColor;
        RectangleF rect = new(40, 60, 75, 75);
        if (color != cc1.Yellow)
        {
            MainGroup!.DrawRoyalSuits(rect, color);
        }
        else
        {
            MainGroup!.DrawRoyalSuits(rect, color, cc1.Black, 20);
        }
    }
    private void DrawCardAndStartingSuit()
    {
        Text text = new();
        string value = GetTextOfCard();
        text.Content = value;
        text.PopulateTextFont();
        double fontSize;
        string x;
        if (value == "10")
        {
            fontSize = 36;
            x = "0";
        }
        else
        {
            fontSize = 43.2;
            x = "5";
        }

        text.Font_Size = fontSize;
        text.X = x;
        text.Y = "43";
        string color = GetColor;
        text.Fill = color.ToWebColor; //try this.
        if (text.Fill == cc1.Yellow.ToWebColor)
        {
            text.PopulateStrokesToStyles(strokeWidth: 2); //try this way.
        }
        MainGroup!.Children.Add(text);
        DrawCustomSuit(MainGroup, color, 3);
        G other = new();
        other.Transform = "rotate(180) translate(-163, -214)";
        MainGroup.Children.Add(other);
        text = new();
        text.Content = value;
        text.PopulateTextFont();
        text.Font_Size = fontSize;
        text.X = x;
        text.Y = "43";
        text.Fill = color.ToWebColor;
        if (text.Fill == cc1.Yellow.ToWebColor)
        {
            text.PopulateStrokesToStyles(strokeWidth: 2);
        }
        other.Children.Add(text);
        DrawCustomSuit(other, color, 5);
    }
    private void DrawCustomSuit(G group, string color, int starts)
    {
        RectangleF bounds = new(starts, 46, _suitSize, _suitSize);
        DrawCustomSuit(group, color, bounds);
    }
    private void DrawCustomSuit(G group, string color, RectangleF bounds)
    {
        if (DeckObject!.Suit == EnumSuitList.Clubs)
        {
            group.DrawCardSuit(BasicGameFrameworkLibrary.Core.RegularDeckOfCards.EnumSuitList.Clubs, bounds, color);
        }
        else if (DeckObject.Suit == EnumSuitList.Hearts)
        {
            group.DrawCardSuit(BasicGameFrameworkLibrary.Core.RegularDeckOfCards.EnumSuitList.Hearts, bounds, color);
        }
        else if (DeckObject.Suit == EnumSuitList.Diamonds)
        {
            group.DrawCardSuit(BasicGameFrameworkLibrary.Core.RegularDeckOfCards.EnumSuitList.Diamonds, bounds, color);
        }
        else if (DeckObject.Suit == EnumSuitList.Spades)
        {
            group.DrawCardSuit(BasicGameFrameworkLibrary.Core.RegularDeckOfCards.EnumSuitList.Spades, bounds, color);
        }
        else if (DeckObject.Suit == EnumSuitList.Stars)
        {
            group.DrawStar(bounds, color, cc1.Black, 3); //i think
        }
    }
    private string GetColor
    {
        get
        {
            return DeckObject!.ColorSuit.Color;
        }
    }
    private EnumCardValueList ValueToDisplay
    {
        get
        {
            return DeckObject!.CardValue;
        }
    }
    private string GetTextOfCard()
    {
        EnumCardValueList tempValue = ValueToDisplay;
        if (tempValue == EnumCardValueList.Jack)
        {
            return "J";
        }
        if (tempValue == EnumCardValueList.Queen)
        {
            return "Q";
        }
        if (tempValue == EnumCardValueList.King)
        {
            return "K";
        }
        return tempValue.Value.ToString(); //try this way (?)
    }

    private void DrawJoker()
    {
        if (MainGroup == null)
        {
            return;
        }
        RectangleF firstRect = new(0, 5, 160, 55);
        var fontSize = firstRect.Height;
        Text text = new();
        text.Fill = cc1.Black.ToWebColor;
        text.Content = "Joker";
        text.Font_Size = fontSize;
        text.CenterText(MainGroup, firstRect);
        RectangleF secondRect = new(30, 75, 100, 100);
        MainGroup.DrawSmiley(secondRect, "", cc1.Black, cc1.Black, 2);
    }
}