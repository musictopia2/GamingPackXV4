namespace Racko.Blazor;
public class CardGraphicsBlazor : BaseDeckGraphics<RackoCardInformation>
{
    protected override SizeF DefaultSize => new(200, 35);
    protected override bool NeedsToDrawBacks => true;
    protected override bool CanStartDrawing()
    {
        if (DeckObject!.IsUnknown)
        {
            return true;
        }
        return DeckObject.Value > 0;
    }
    protected override void BeforeFilling()
    {
        if (DeckObject!.IsUnknown)
        {
            FillColor = cc1.Red;
        }
        else
        {
            FillColor = cc1.White;
        }
        base.BeforeFilling();
    }
    protected override void DrawBacks()
    {
        RectangleF rect = new(2, 2, 200, 30);
        Text text = new();
        text.CenterText(MainGroup!, rect);
        text.Content = "Racko";
        text.Font_Size = 30;
    }
    private int _maxs;
    protected override void OnInitialized()
    {
        RackoDeckCount temps = aa1.Resolver!.Resolve<RackoDeckCount>();
        _maxs = temps.GetDeckCount();
        base.OnInitialized();
    }
    protected override void DrawImage()
    {
        var maxSize = 40;
        double percs = (DeckObject!.Value) / (double)_maxs;
        var maxLeft = DefaultSize.Width - maxSize;
        var lefts = maxLeft * percs;
        lefts += 5;
        float fontSize = 30;
        Text text = new();
        text.X = lefts.ToString();
        text.Height = "100%";
        text.Y = "50%";
        text.Dominant_Baseline = "middle";
        text.Text_Anchor = "middle";
        MainGroup!.Children.Add(text);
        text.Font_Size = fontSize;
        text.Fill = cc1.Red.ToWebColor();
        text.Content = DeckObject!.Value.ToString();
    }
}