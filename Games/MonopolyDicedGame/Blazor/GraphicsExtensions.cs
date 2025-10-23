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
    public static void DrawTrainBoard(this IParentGraphic container, string value)
    {
        RectangleF bounds = new(10, 22, 28, 28);
        container.DrawImageDice("whitetrain.svg", bounds);
        bounds = new(0, -2, 50, 30);
        container.DrawText(bounds, value, 20, "", true); //not sure if that needs to be changed (can be done if necessary)
    }
    public static void DrawTrainDice(this IParentGraphic container)
    {
        container.DrawImageDice("blacktrain.svg");
        container.DrawText(new(), "200", 0, "", true);
        //do other things on top of this.
    }

    public static void DrawWaterDice(this IParentGraphic container)
    {
        container.DrawUtilitiesDice("blackwaterworks.svg");
    }
    public static void DrawElectricDice(this IParentGraphic container)
    {
        container.DrawUtilitiesDice("blackelectric.svg");
    }
    public static void DrawWaterBoard(this IParentGraphic container, string value)
    {
        container.DrawUtilitiesBoard("whitewaterworks.svg", value);
    }
    public static void DrawElectricBoard(this IParentContainer container, string value)
    {
        container.DrawUtilitiesBoard("whiteelectric.svg", value);
    }
    private static void DrawUtilitiesBoard(this IParentGraphic container, string name, string value)
    {
        RectangleF bounds = new(10, 22, 28, 28);
        container.DrawImageDice(name, bounds);
        bounds = new(0, -2, 50, 30);
        container.DrawText(bounds, value, 20, "", true);
    }
    private static void DrawUtilitiesDice(this IParentGraphic container, string name)
    {
        RectangleF bounds = new(10, 2, 30, 30);
        container.DrawImageDice(name, bounds);
        bounds = new(2, 32, 46, 19);
        container.DrawText(bounds, "100", 18, cc1.Black.ToWebColor(), false);
    }
    public static void DrawGoDice(this IParentGraphic container)
    {
        container.DrawImageDice("go.svg");
    }
    public static void DrawChanceDice(this IParentGraphic container)
    {
        container.DrawImageDice("chance.svg");
    }
    private static void DrawImageDice(this IParentGraphic container, string name, RectangleF bounds = new())
    {
        if (bounds.Width == 0)
        {
            bounds = new(2, 2, 46, 46);
        }
        Image image = new();
        image.PopulateFullExternalImage(name);
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
    public static void DrawBrokenHouse(this IParentGraphic container)
    {
        container.DrawImageDice("brokenhouse.svg");
    }
    public static void DrawHouse(this IParentGraphic container, int howMany)
    {
        container.DrawImageDice("house.svg");
        if (howMany > 0)
        {
            container.DrawText(new(), howMany.ToString(), 40, "", true);
        }
    }
    public static void DrawHotel(this IParentGraphic container)
    {
        container.DrawImageDice("hotel.svg");
    }
    public static void DrawPolice(this IParentGraphic container)
    {
        container.DrawImageDice("police.svg");
    }
    public static void DrawOutOfJailFree(this IParentGraphic container)
    {
        RectangleF bounds = new(10, 2, 30, 30);
        container.DrawImageDice("free.svg", bounds);
        bounds = new(2, 32, 46, 19);
        container.DrawText(bounds, "Free", 18, cc1.Black.ToWebColor(), false);
    }
}