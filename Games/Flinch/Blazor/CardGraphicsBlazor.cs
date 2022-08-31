namespace Flinch.Blazor;
public class CardGraphicsBlazor : BaseColorCardsImageBlazor<FlinchCardInformation>
{
    protected override string BackColor => cc.Blue;
    protected override string BackFontColor => cc.Red;
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