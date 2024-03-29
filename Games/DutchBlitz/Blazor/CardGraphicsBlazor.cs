namespace DutchBlitz.Blazor;
public class CardGraphicsBlazor : BaseColorCardsImageBlazor<DutchBlitzCardInformation>
{
    protected override string BackColor => cc1.Aqua;
    protected override string BackFontColor => cc1.DarkGreen;
    protected override string BackText => "Dutch Blitz";
    protected override bool CanStartDrawing()
    {
        if (DeckObject!.IsUnknown)
        {
            return true;
        }
        return DeckObject!.Display != "" && DeckObject!.Color != EnumColorTypes.None;
    }
    protected override void DrawImage()
    {
        DrawValueCard(DefaultRectangle, DeckObject!.Display);
    }
}