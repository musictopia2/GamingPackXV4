namespace MonopolyDicedGame.Blazor;
public static class GraphicsExtensions
{
    public static void DrawPropertyValue(this IParentGraphic container, string color, string value)
    {
        Circle circle = new();
        circle.CX = "25";
        circle.CY = "25";
        circle.R = "21";
        circle.Fill = color; //has to already have the proper color i think
        container.Children.Add(circle);
        container.DrawText(new(), value, 0, "", true);
    }
    public static void DrawTrainDice(this IParentGraphic container, object assembly)
    {
        container.DrawImageDice(assembly, "blacktrain.svg");
        container.DrawText(new(), "200", 0, "", true);
        //do other things on top of this.
    }
    public static void DrawWaterDice(this IParentGraphic container, object assembly)
    {
        container.DrawUtilitiesDice(assembly, "blackwaterworks.svg");
    }
    public static void DrawElectricDice(this IParentGraphic container, object assembly)
    {
        container.DrawUtilitiesDice(assembly, "blackelectric.svg");
    }
    private static void DrawUtilitiesDice(this IParentGraphic container, object assembly, string name)
    {
        RectangleF bounds = new(10, 2, 30, 30);
        container.DrawImageDice(assembly, name, bounds);
        bounds = new(2, 32, 46, 19);
        container.DrawText(bounds, "100", 18, cc1.Black.ToWebColor(), false);
    }
    public static void DrawChanceDice(this IParentGraphic container, object assembly)
    {
        container.DrawImageDice(assembly, "chance.svg");
    }
    private static void DrawImageDice(this IParentGraphic container, object assembly, string name, RectangleF bounds = new())
    {
        if (bounds.Width == 0)
        {
            bounds = new(2, 2, 46, 46);
        }
        Image image = new();
        image.PopulateFullExternalImage(assembly, name);
        //RectangleF bounds = new(2, 2, 46, 46);
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
            fontSize = 22; //experiment.
        }
        if (color == "")
        {
            color = cc1.White.ToWebColor();
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
    public static void DrawBrokenHouse(this IParentGraphic container, object assembly)
    {
        container.DrawImageDice(assembly, "brokenhouse.svg");
    }
    public static void DrawHouse(this IParentGraphic container, object assembly)
    {
        container.DrawImageDice(assembly, "house.svg");
    }
    public static void DrawHotel(this IParentGraphic container, object assembly)
    {
        container.DrawImageDice(assembly, "hotel.svg");
    }
    public static void DrawPolice(this IParentGraphic container, object assembly)
    {
        container.DrawImageDice(assembly, "police.svg");
    }
    public static void DrawOutOfJailFree(this IParentGraphic container, object assembly)
    {
        RectangleF bounds = new(10, 2, 30, 30);
        container.DrawImageDice(assembly, "free.svg", bounds);
        bounds = new(2, 32, 46, 19);
        container.DrawText(bounds, "Free", 18, cc1.Black.ToWebColor(), false);
    }
}