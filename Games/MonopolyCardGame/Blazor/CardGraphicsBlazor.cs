namespace MonopolyCardGame.Blazor;
public class CardGraphicsBlazor : BaseDeckGraphics<MonopolyCardGameCardInformation>
{
    protected override SizeF DefaultSize => new(55, 72);
    protected override bool NeedsToDrawBacks => true;
    protected override bool CanStartDrawing()
    {
        return DeckObject!.IsUnknown == true || DeckObject.CardValue > 0;
    }
    protected override void BeforeFilling()
    {
        if (DeckObject!.IsUnknown)
        {
            FillColor = cc1.Aqua;
        }
        else
        {
            FillColor = cc1.White;
        }
        base.BeforeFilling();
    }
    protected override void DrawBacks() { } //try with no drawings because of rotated text.
    protected override void DrawImage()
    {
        if (DeckObject == null)
        {
            return;
        }
        switch (DeckObject.CardValue)
        {
            case int _ when DeckObject.CardValue < 7:
                {
                    DrawTokenText();
                    DrawTokenImage();
                    break;
                }
            case 7:
                {
                    DrawMRMonopolyText();
                    DrawMrMonopolyImage();
                    break;
                }
            case object _ when DeckObject.CardValue < 12:
                {
                    DrawHouseText();
                    DrawHouseImage();
                    break;
                }
            case 12:
                {
                    DrawHotel();
                    break;
                }
            case object _ when DeckObject.CardValue < 17:
                {
                    if (DeckObject.PlainCategory == EnumPlainCategory.None)
                    {
                        DrawRailRoadText();
                    }
                    DrawRailroadImage();
                    break;
                }
            case 17:
                {
                    if (DeckObject.PlainCategory == EnumPlainCategory.None)
                    {
                        DrawUtilityText();
                        DrawElectricImage();
                    }
                    else if (DeckObject.PlainCategory == EnumPlainCategory.Chooser)
                    {
                        DrawWaterworksImage(); //i think should lean towrds waterworks.
                    }
                    else
                    {
                        DrawElectricImage();
                    }
                    break;
                }
            case 18:
                {
                    if (DeckObject.PlainCategory == EnumPlainCategory.None)
                    {
                        DrawUtilityText();
                    }
                    DrawWaterworksImage();
                    //if (DeckObject.Plain == false)
                    //{
                    //    DrawUtilityText();
                    //}

                    break;
                }
            case int _ when DeckObject.CardValue < 41:
                {
                    DrawProperties();
                    break;
                }
            case 41:
                {
                    DrawChance();
                    break;
                }
            case 42:
                {
                    DrawGo();
                    break;
                }
            default:
                {
                    //has to ignore because no runtime error messages anyways.
                    break;
                }
        }
    }
    private void DrawProperties()
    {
        if (DeckObject == null)
        {
            return;
        }
        if (DeckObject.PlainCategory == EnumPlainCategory.None)
        {
            DrawPiece($"{DeckObject.CardValue}.svg", new(0, 0, 55, 72));
            return;
        }
        DrawPiece($"group{DeckObject.Group}.svg", new(0, 0, 55, 72));
        //string fillColor;
        //Rect thisRect = new();
        //thisRect.X = "8";
        //thisRect.Y = "8";
        //thisRect.Width = "35";
        //thisRect.Height = "46";
        ////hopefully this is fine for now.  i probably have tools but can't remember what they are for now.
        //if (DeckObject.Group == 1)
        //{
        //    fillColor = "#800080";
        //}
        //else if (DeckObject.Group == 2)
        //{
        //    fillColor = "#00FFFF";
        //}
        //else if (DeckObject.Group == 3)
        //{
        //    fillColor = "#FF00FF";
        //}
        //else if (DeckObject.Group == 4)
        //{
        //    fillColor = "#FF8C00";
        //}
        //else if (DeckObject.Group == 5)
        //{
        //    fillColor = "#FF0000";
        //}
        //else if (DeckObject.Group == 6)
        //{
        //    fillColor = "#FFFF00";
        //}
        //else if (DeckObject.Group == 7)
        //{
        //    fillColor = "#008000";
        //}
        //else if (DeckObject.Group == 8)
        //{
        //    fillColor = "#00008B";
        //}
        //else
        //{
        //    throw new CustomBasicException("Wrong group");
        //}
        //fillColor = $"FF{fillColor}";
        ////thisRect.Fill = cc1.Pink.ToWebColor();
        //thisRect.Fill = fillColor;
        //MainGroup!.Children.Add(thisRect);
    }
    private void DrawTokenText()
    {
        var thisRect = new RectangleF(0, 0, 55, 72); //iffy
        DrawPiece("Token.svg", thisRect); //hopefully will work (?)
    }
    private void DrawChance()
    {
        var thisRect = new RectangleF(0, 0, 55, 72);
        DrawPiece("41.svg", thisRect); //iffy.
    }
    private void DrawGo()
    {
        var thisRect = new RectangleF(0, 0, 55, 72);
        DrawPiece("42.svg", thisRect);
    }
    private void DrawMRMonopolyText()
    {
        var fontSize = 9;
        var firstRect = new RectangleF(0, 5, 55, 15);
        var secondRect = new RectangleF(-2, 15, 55, 15);
        DrawText("Mr.", firstRect, cc1.Black, fontSize);
        DrawText("Monopoly", secondRect, cc1.Black, fontSize);
    }
    private void DrawHouseText()
    {
        if (DeckObject == null)
        {
            return;
        }
        DrawPiece($"{DeckObject.CardValue}.svg", new(0, 0, 55, 72));
    }
    private void DrawHotel()
    {
        DrawPiece($"{DeckObject!.CardValue}.svg", new(0, 0, 55, 72));
    }
    private void DrawRailRoadText()
    {
        if (DeckObject == null)
        {
            return;
        }
        DrawPiece($"{DeckObject!.CardValue}.svg", new(0, 0, 55, 72));
    }
    private void DrawUtilityText()
    {
        DrawPiece("Utility.svg", new(0, 0, 55, 72));
    }
    private void DrawTokenImage()
    {
        if (DeckObject == null)
        {
            return;
        }
        string imageName = "";
        if (DeckObject.CardValue == 1)
        {
            imageName = "Dog.png";
        }
        else if (DeckObject.CardValue == 2)
        {
            imageName = "Horse.png";
        }
        else if (DeckObject.CardValue == 3)
        {
            imageName = "Iron.png";
        }
        else if (DeckObject.CardValue == 4)
        {
            imageName = "Car.png";
        }
        else if (DeckObject.CardValue == 5)
        {
            imageName = "Ship.png";
        }
        else if (DeckObject.CardValue == 6)
        {
            imageName = "Hat.png";
        }
        var bounds = new RectangleF(6, 25, 40, 40);
        DrawPiece(imageName, bounds);
    }
    private void DrawMrMonopolyImage()
    {
        var thisRect = new RectangleF(13, 30, 25, 35);
        DrawPiece("MrMonopoly.png", thisRect);
    }
    private void DrawHouseImage()
    {
        var thisRect = new RectangleF(0, 35, 50, 30);
        DrawPiece("House.png", thisRect);
    }
    private void DrawRailroadImage()
    {
        RectangleF thisRect;
        if (DeckObject!.PlainCategory == EnumPlainCategory.None)
        {
            thisRect = new(2, 30, 45, 30);
        }
        else
        {
            thisRect = new(2, 10, 45, 30); //not sure.
        }
        DrawPiece("RailRoad.png", thisRect);
    }
    private void DrawElectricImage()
    {
        RectangleF thisRect;
        if (DeckObject!.PlainCategory == EnumPlainCategory.None)
        {
            thisRect = new (7, 25, 40, 40);
        }
        else
        {
            thisRect = new(7, 15, 40, 40);
        }
        DrawPiece("Electric.png", thisRect);
    }
    private void DrawWaterworksImage()
    {
        RectangleF thisRect;
        if (DeckObject!.PlainCategory == EnumPlainCategory.None)
        {
            thisRect = new(7, 25, 40, 40);
        }
        else
        {
            thisRect = new(2, 15, 40, 40);
        }
        DrawPiece("Waterworks.png", thisRect);
    }
    private void DrawPiece(string fileName, RectangleF bounds)
    {
        Image image = new();
        image.PopulateFullExternalImage(this, fileName);
        image.PopulateImagePositionings(bounds);
        MainGroup!.Children.Add(image);
    }
    private void DrawText(string content, RectangleF bounds, string color, float fontSize, bool hasBorders = false)
    {
        Text text = new();
        text.Content = content;
        text.CenterText(MainGroup!, bounds);
        text.Fill = color.ToWebColor();
        text.Font_Size = fontSize;
        text.Font_Weight = "bold";
        if (hasBorders)
        {
            text.PopulateStrokesToStyles();
        }
    }
}