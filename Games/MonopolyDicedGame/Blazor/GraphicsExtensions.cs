namespace MonopolyDicedGame.Blazor;
public static class GraphicsExtensions
{
    public static void DrawPropertyValue(this IParentGraphic container, string color, string value)
    {
        Circle circle = new();
        circle.CX = "50";
        circle.CY = "50";
        circle.R = "50";
        circle.Fill = color; //has to already have the proper color i think
        container.Children.Add(circle);
        container.DrawText(new(), value, 0, "", true);
    }
    public static void DrawTrainDice(this IParentGraphic container, Assembly assembly)
    {
        container.DrawImageDice(assembly, "blackelectric.svg");
        container.DrawText(new(), "200", 0, "", true);
        //do other things on top of this.
    }
    public static void DrawWaterDice(this IParentGraphic container, Assembly assembly)
    {
        container.DrawUtilitiesDice(assembly, "blackwaterworks.svg");
    }
    public static void DrawElectricDice(this IParentGraphic container, Assembly assembly)
    {
        container.DrawUtilitiesDice(assembly, "blackelectric.svg");
    }
    private static void DrawUtilitiesDice(this IParentGraphic container, Assembly assembly, string name)
    {
        container.DrawImageDice(assembly, name);
        RectangleF bounds = new(2, 25, 46, 19);
        container.DrawText(bounds, "100", 13, cc1.Black.ToWebColor(), false);
    }
    public static void DrawChanceDice(this IParentGraphic container, Assembly assembly)
    {
        container.DrawImageDice(assembly, "chance.svg");
    }
    private static void DrawImageDice(this IParentGraphic container, Assembly assembly, string name)
    {
        Image image = new();
        image.PopulateFullExternalImage(assembly, name);
        RectangleF bounds = new(2, 2, 46, 46);
        image.PopulateImagePositionings(bounds);
        container.Children.Add(image);
    }
    private static void DrawText(this IParentGraphic container, RectangleF bounds, string value, float fontSize, string color, bool needsStrokes)
    {
        Text text = new();
        if (bounds.Width == 0)
        {
            bounds = new(2, 2, 46, 46);
        }
        if (fontSize == 0)
        {
            fontSize = 15; //experiment.
        }
        if (color == "")
        {
            color = cc1.Black.ToWebColor();
        }
        text.Content = value;
        text.CenterText(container!, bounds);
        text.Font_Size = fontSize;
        text.Fill = color;
        if (needsStrokes)
        {
            text.PopulateStrokesToStyles();
        }
    }
    public static void DrawBrokenHouse(this IParentGraphic container, Assembly assembly)
    {
        container.DrawImageDice(assembly, "brokenhouse.svg");
    }
    public static void DrawHouse(this IParentGraphic container, Assembly assembly)
    {
        container.DrawImageDice(assembly, "house.svg");
    }
    public static void DrawHotel(this IParentGraphic container, Assembly assembly)
    {
        container.DrawImageDice(assembly, "hotel.svg");
    }
    public static void DrawPolice(this IParentGraphic container, Assembly assembly)
    {
        container.DrawImageDice(assembly, "police.svg");
    }
    public static void DrawOutOfJailFree(this IParentGraphic container, Assembly assembly)
    {
        container.DrawImageDice(assembly, "free.svg");
    }
}