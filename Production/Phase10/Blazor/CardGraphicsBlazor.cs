namespace Phase10.Blazor;
public class CardGraphicsBlazor : BaseColorCardsImageBlazor<Phase10CardInformation>
{
    protected override string BackColor => cc1.Aqua;
    protected override string BackFontColor => cc1.Purple;
    protected override string BackText => "Phase 10";
    protected override bool CanStartDrawing()
    {
        if (DeckObject!.Color == EnumColorTypes.ZOther || DeckObject!.Color == EnumColorTypes.None)
        {
            return false;
        }
        return DeckObject!.CardCategory != EnumCardCategory.Blank;
    }
    protected override void DrawImage()
    {
        DrawValueCard(DefaultRectangle, DeckObject!.Display);
    }
}