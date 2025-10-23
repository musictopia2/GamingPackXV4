namespace Rook.Blazor;
public class CardGraphicsBlazor : BaseColorCardsImageBlazor<RookCardInformation>
{
    protected override string BackColor => cc1.Aqua;
    protected override string BackFontColor => cc1.DarkOrange;
    protected override string BackText => "Rook";
    protected override bool CanStartDrawing()
    {
        return DeckObject!.Display != "" || DeckObject.IsUnknown;
    }
    protected override void DrawImage()
    {
        if (DeckObject!.IsBird == false)
        {
            DrawValueCard(DefaultRectangle, DeckObject!.Display);
        }
        else
        {
            //rethink the bird.  its a black bird (the background would be white.
            Image image = new();
            image.PopulateFullExternalImage("RookBird.svg");
            RectangleF bounds = new(5, 5, 40, 62);
            image.PopulateImagePositionings(bounds);
            MainGroup!.Children.Add(image);
        }
    }
}