namespace Sorry.Blazor;
public class CardGraphicsBlazor : BaseDeckGraphics<CardInfo>
{
    [Parameter]
    public SorryGameContainer? Container { get; set; }
    public CardGraphicsBlazor()
    {
        RoundedRadius = 15;
    }
    protected override SizeF DefaultSize => new(82, 172);
    protected override bool NeedsToDrawBacks => false;
    protected override bool CanStartDrawing()
    {
        return Container!.SaveRoot.DidDraw;
    }
    protected override void DrawBacks()
    {

    }
    protected override void DrawImage()
    {
        string realText;
        float fontSize;
        if (DeckObject!.Value == 13)
        {
            realText = "S";
        }
        else
        {
            realText = DeckObject.Value.ToString();
        }
        if (DeckObject.Value < 10 || realText == "S")
        {
            fontSize = 140f;
        }
        else
        {
            fontSize = 70f;
        }
        Text text = new()
        {
            Font_Size = fontSize
        };
        text.CenterText();
        text.Content = realText;
        MainGroup!.Children.Add(text);
    }
}