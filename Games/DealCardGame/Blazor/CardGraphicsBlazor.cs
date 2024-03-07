namespace DealCardGame.Blazor;
public class CardGraphicsBlazor : BaseDeckGraphics<DealCardGameCardInformation>
{
    protected override SizeF DefaultSize => new (55, 72); //this is default but can change to anything you want.
    protected override bool NeedsToDrawBacks => true;
    protected override bool CanStartDrawing()
    {
        return true;
    }
    protected override void DrawBacks()
    {
        //any code necessary for drawing backs of cards.
    }
    protected override void DrawImage()
    {
        //code necessary to draw the card.
        if (DeckObject!.CardType == EnumCardType.Money)
        {
            DrawMoney();
            return;
        }
        if (DeckObject!.CardType == EnumCardType.PropertyRegular)
        {
            DrawRegularProperty();
            return;
        }
    }
    private void DrawRegularProperty()
    {
        RectangleF bounds = GetFillRectangle;
        Rect rect = new();
        rect.PopulateRectangle(bounds);
        rect.Fill = DeckObject!.MainColor.WebColor;
        MainGroup!.Children.Add(rect);
    }
    private static RectangleF GetFillRectangle => new(5, 5, 40, 57);
    private void DrawMoney()
    {
        RectangleF bounds = GetFillRectangle;
        Rect rect = new();
        rect.PopulateRectangle(bounds);
        if (DeckObject!.ClaimedValue == 10)
        {
            rect.Fill = cc1.DarkOrange.ToWebColor();
        }
        if (DeckObject.ClaimedValue == 5)
        {
            rect.Fill = cc1.DarkOrchid.ToWebColor();
        }
        if (DeckObject.ClaimedValue == 4)
        {
            rect.Fill = cc1.DeepSkyBlue.ToWebColor();
        }
        if (DeckObject.ClaimedValue == 3)
        {
            rect.Fill = cc1.LightGreen.ToWebColor();
        }
        if (DeckObject.ClaimedValue == 2)
        {
            rect.Fill = cc1.NavajoWhite.ToWebColor();
        }
        if (DeckObject.ClaimedValue == 1)
        {
            rect.Fill = cc1.LemonChiffon.ToWebColor();
        }
        MainGroup!.Children.Add(rect);
        Text text = new();
        text.Content = DeckObject!.ClaimedValue.ToString();
        text.Font_Size = 32;
        text.Fill = cc1.Black.ToWebColor();
        text.CenterText(MainGroup!, bounds);
        text.PopulateStrokesToStyles(cc1.White.ToWebColor(), 1);
    }
}