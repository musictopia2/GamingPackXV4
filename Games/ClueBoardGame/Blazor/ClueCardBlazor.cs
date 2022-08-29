namespace ClueBoardGame.Blazor;
public class ClueCardBlazor : BaseDeckGraphics<CardInfo>
{
    protected override SizeF DefaultSize => new(55, 72);
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
        image.PopulateFullExternalImage(this, fileName);
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
            EnumCardValues.Wrench => ("Wrench", possibleFontSize),
            EnumCardValues.Revolver => ("Revolver", possibleFontSize),
            EnumCardValues.LeadPipe => ("Lead Pipe", 9),
            EnumCardValues.Kitchen => ("Kitchen", possibleFontSize),
            EnumCardValues.BallRoom => ("Ball", possibleFontSize),
            EnumCardValues.Conservatory => ("Convervatory", 7),
            EnumCardValues.BilliardRoom => ("Billiard", possibleFontSize),
            EnumCardValues.Library => ("Library", possibleFontSize),
            EnumCardValues.Study => ("Study", possibleFontSize),
            EnumCardValues.Hall => ("Hall", possibleFontSize),
            EnumCardValues.Lounge => ("Lounge", possibleFontSize),
            EnumCardValues.DiningRoom => ("Dining", possibleFontSize),
            _ => ("", possibleFontSize),
        };
    }
}
