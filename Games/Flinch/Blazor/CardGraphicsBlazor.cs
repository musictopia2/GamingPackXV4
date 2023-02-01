namespace Flinch.Blazor;
public class CardGraphicsBlazor : BaseColorCardsImageBlazor<FlinchCardInformation>
{
    protected override string BackColor => cc1.Blue;
    protected override string BackFontColor => cc1.Red;
    protected override string BackText => "Flinch";
    protected override bool CanStartDrawing()
    {
        return DeckObject!.Display != "";
    }
    protected override void DrawImage()
    {
        DrawValueCard(DefaultRectangle, DeckObject!.Display);
    }
}