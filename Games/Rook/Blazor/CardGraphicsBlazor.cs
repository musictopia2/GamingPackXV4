namespace Rook.Blazor;
public class CardGraphicsBlazor : BaseColorCardsImageBlazor<RookCardInformation>
{
    protected override string BackColor => cc.Aqua;
    protected override string BackFontColor => cc.DarkOrange;
    protected override string BackText => "Rook";
    protected override bool CanStartDrawing()
    {
        return DeckObject!.Display != "" || DeckObject.IsUnknown;
    }
    protected override void DrawImage()
    {
        DrawValueCard(DefaultRectangle, DeckObject!.Display);
    }
}