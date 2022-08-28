namespace Risk.Blazor;
public class RiskCardsBlazor : BaseDeckGraphics<RiskCardInfo>
{
    protected override SizeF DefaultSize => new(55, 72);
    protected override bool NeedsToDrawBacks => false;
    protected override bool CanStartDrawing()
    {
        return true;
    }
    protected override void DrawBacks()
    {
        //nothing here.
    }
    protected override void DrawImage()
    {
        if (DeckObject!.Army == EnumArmyType.Wild)
        {
            DrawAllThree();
            return;
        }
        if (DeckObject.Army == EnumArmyType.Artillery)
        {
            DrawArtillery();
            return;
        }
        if (DeckObject.Army == EnumArmyType.Cavalry)
        {
            DrawCavalry();
            return;
        }
        if (DeckObject.Army == EnumArmyType.Infantry)
        {
            DrawInfantry();
            return;
        }
    }
    private void DrawAllThree()
    {
        DrawInfantry(new PointF(2, 2));
        DrawCavalry(new PointF(2, 24));
        DrawArtillery(new PointF(4, 46)); //each will be 22 high no matter what.
    }
    private void DrawCavalry()
    {
        DrawCavalry(new(2, 24));
    }
    private void DrawInfantry()
    {
        DrawInfantry(new(2, 24));
    }
    private void DrawArtillery()
    {
        DrawArtillery(new(4, 20));
    }
    private static RectangleF GetRectangle(PointF location) => new(location.X, location.Y, 51, 22);
    private void DrawCavalry(PointF location)
    {
        //the height will be 22
        //width needs to be 51
        var bounds = GetRectangle(location);
        DrawPiece(bounds, "cavalry.svg");
    }
    private void DrawInfantry(PointF location)
    {
        var bounds = GetRectangle(location);
        DrawPiece(bounds, "infantry.svg");
    }
    private void DrawArtillery(PointF location)
    {
        RectangleF bounds = new(location.X, location.Y, 42, 22);
        DrawPiece(bounds, "artilary.svg");
    }
    private void DrawPiece(RectangleF bounds, string fileName)
    {
        Image image = new();
        image.PopulateFullExternalImage(this, fileName);
        image.PopulateImagePositionings(bounds);
        MainGroup!.Children.Add(image);
    }
}