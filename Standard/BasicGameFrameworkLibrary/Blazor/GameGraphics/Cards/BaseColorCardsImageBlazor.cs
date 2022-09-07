namespace BasicGameFrameworkLibrary.Blazor.GameGraphics.Cards;
public abstract class BaseColorCardsImageBlazor<C> : BaseDarkCardsBlazor<C>
   where C : class, IColorCard, new()
{
    protected override bool IsLightColored => DeckObject!.Color == EnumColorTypes.Yellow;
    protected RectangleF DefaultRectangle => new(0, 0, DefaultSize.Width, DefaultSize.Height);
    protected abstract string BackColor { get; }
    protected abstract string BackFontColor { get; }
    protected abstract string BackText { get; }
    protected override SizeF DefaultSize => new(55, 72);
    protected override bool NeedsToDrawBacks => true;
    protected string GetFillColor()
    {
        if (DeckObject!.IsUnknown)
        {
            return BackColor;
        }
        if (DeckObject!.Color == EnumColorTypes.ZOther || DeckObject!.Color == EnumColorTypes.None)
        {
            return cs.White;
        }
        return PrivateColor();
    }
    protected virtual string PrivateColor()
    {
        return DeckObject!.Color.Color;
    }
    protected override void BeforeFilling()
    {
        FillColor = GetFillColor();
    }
    protected override void DrawBacks()
    {
        if (BackText == "")
        {
            return;
        }
        var list = BackText.Split(" ").ToBasicList();
        if (list.Count > 2)
        {
            return;
        }
        BasicList<RectangleF> rectangles = new();
        if (list.Count == 1)
        {
            rectangles.Add(new RectangleF(0, 0, DefaultSize.Width, DefaultSize.Height));
        }
        else
        {
            RectangleF firstRect;
            firstRect = new RectangleF(0, 0, DefaultSize.Width, DefaultSize.Height / 2.1f);
            var secondRect = new RectangleF(0, DefaultSize.Height / 2, DefaultSize.Width, DefaultSize.Height / 2.1f);
            rectangles.Add(firstRect);
            rectangles.Add(secondRect);
        }
        if (rectangles.Count != list.Count)
        {
            return;
        }
        int x = 0;
        foreach (var thisRect in rectangles)
        {
            float fontSize;
            var thisText = list[x];
            if (thisText.Length >= 5)
            {
                fontSize = DefaultSize.Height / 5f;
            }
            else
            {
                fontSize = DefaultSize.Height / 3.6f;
            }
            Text tt = new();
            tt.CenterText(MainGroup!, thisRect);
            tt.Content = thisText;
            tt.Font_Size = fontSize;
            tt.Fill = BackFontColor.ToWebColor();
            if (thisText.Length >= 5)
            {
                tt.PopulateStrokesToStyles(strokeWidth: .5f);
            }
            else
            {
                tt.PopulateStrokesToStyles();
            }
            x++;
        }
    }
    protected void DrawValueCard(RectangleF rectangle, string valueNeeded)
    {
        var fontSize = rectangle.Height * .45;
        Text text = new();
        text.CenterText(MainGroup!, rectangle);
        text.Font_Size = fontSize;
        text.Content = valueNeeded;
        text.Fill = cs.White.ToWebColor();
        text.PopulateStrokesToStyles(strokeWidth: 1.4f);
    }
}