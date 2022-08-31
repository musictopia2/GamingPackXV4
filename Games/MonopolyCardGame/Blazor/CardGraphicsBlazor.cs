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
            FillColor = cc.Aqua;
        }
        else
        {
            FillColor = cc.White;
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
                    DrawRailRoadText();
                    DrawRailroadImage();
                    break;
                }
            case 17:
                {
                    DrawUtilityText();
                    DrawElectricImage();
                    break;
                }
            case 18:
                {
                    DrawUtilityText();
                    DrawWaterworksImage();
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
        DrawPiece($"{DeckObject.CardValue}.svg", new(0, 0, 55, 72));
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
        DrawText("Mr.", firstRect, cc.Black, fontSize);
        DrawText("Monopoly", secondRect, cc.Black, fontSize);
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
        var thisRect = new RectangleF(2, 30, 45, 30);
        DrawPiece("RailRoad.png", thisRect);
    }
    private void DrawElectricImage()
    {
        var thisRect = new RectangleF(7, 25, 40, 40);
        DrawPiece("Electric.png", thisRect);
    }
    private void DrawWaterworksImage()
    {
        var thisRect = new RectangleF(7, 25, 40, 40);
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