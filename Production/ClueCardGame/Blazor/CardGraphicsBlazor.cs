namespace ClueCardGame.Blazor;
public class CardGraphicsBlazor : BaseDeckGraphics<ClueCardGameCardInformation>
{
    protected override SizeF DefaultSize => new (55, 72); //this is default but can change to anything you want.
    protected override bool NeedsToDrawBacks => false;
    protected override bool CanStartDrawing()
    {
        return DeckObject!.CardValue != EnumCardValues.None;
    }
    protected override void DrawBacks() { } //no drawing of backs.
    protected override void DrawImage()
    {
        int tempValue = (int)DeckObject!.CardValue;
        string fileName = $"card{tempValue}.png";
        Image image = new();
        image.PopulateBasicExternalImage(fileName);
        image.PopulateImagePositionings(5, 5, 45, 45);
        MainGroup!.Children.Add(image);
        Text text = new();
        text.CenterText(MainGroup, 0, 45, 55, 27);
        var (display, fontSize) = GetTextData();
        text.Content = display;
        text.Font_Size = fontSize;
    }
    private (string display, int fontSize) GetTextData()
    {
        int possibleFontSize = 11;
        return DeckObject!.CardValue switch
        {
            EnumCardValues.Peacock => ("Peacock", possibleFontSize),
            EnumCardValues.Green => ("Mr. Green", 9),
            EnumCardValues.Plum => ("Prof. Plum", 9),
            EnumCardValues.Scarlet => ("Scarlet", possibleFontSize),
            EnumCardValues.White => ("White", possibleFontSize),
            EnumCardValues.Colonel => ("Mustard", possibleFontSize),
            EnumCardValues.Candlestick => ("Candlestick", 8),
            EnumCardValues.Knife => ("Knife", possibleFontSize),
            EnumCardValues.Rope => ("Rope", possibleFontSize),
            EnumCardValues.LeadPipe => ("Lead Pipe", 9),
            EnumCardValues.BallRoom => ("Ball", possibleFontSize),
            EnumCardValues.Library => ("Library", possibleFontSize),
            EnumCardValues.Study => ("Study", possibleFontSize),
            EnumCardValues.Hall => ("Hall", possibleFontSize),
            EnumCardValues.DiningRoom => ("Dining", possibleFontSize),
            _ => ("", possibleFontSize),
        };
    }
}