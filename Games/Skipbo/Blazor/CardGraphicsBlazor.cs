namespace Skipbo.Blazor;
public class CardGraphicsBlazor : BaseColorCardsImageBlazor<SkipboCardInformation>
{
    protected override string BackColor => cc1.Red;
    protected override string BackFontColor => cc1.BlanchedAlmond;
    protected override string BackText => "Skip Bo";
    protected override bool CanStartDrawing()
    {
        return DeckObject!.Display != "" && DeckObject.Color != EnumColorTypes.None;
    }
    protected override void DrawImage()
    {
        DrawValueCard(DefaultRectangle, DeckObject!.Display);
    }
}