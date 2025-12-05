namespace MonopolyDicedGame.Blazor;
public static class GraphicsExtensions
{
    extension (IParentContainer container)
    {
        public void DrawPropertyValue(string color, string value)
        {
            Circle circle = new();
            circle.CX = "25";
            circle.CY = "25";
            circle.R = "21";
            circle.Fill = color; //has to already have the proper color i think
            container.Children.Add(circle);
            container.DrawText(new(), value, 0, "", true);
        }
        public void DrawTrainBoard(string value)
        {
            RectangleF bounds = new(10, 22, 28, 28);
            container.DrawImageDice("whitetrain.svg", bounds);
            bounds = new(0, -2, 50, 30);
            container.DrawText(bounds, value, 20, "", true); //not sure if that needs to be changed (can be done if necessary)
        }
        public void DrawTrainDice()
        {
            container.DrawImageDice("blacktrain.svg");
            container.DrawText(new(), "200", 0, "", true);
            //do other things on top of this.
        }

        public void DrawWaterDice()
        {
            container.DrawUtilitiesDice("blackwaterworks.svg");
        }
        public void DrawElectricDice()
        {
            container.DrawUtilitiesDice("blackelectric.svg");
        }
        public void DrawWaterBoard(string value)
        {
            container.DrawUtilitiesBoard("whitewaterworks.svg", value);
        }
        public void DrawElectricBoard(string value)
        {
            container.DrawUtilitiesBoard("whiteelectric.svg", value);
        }
        private void DrawUtilitiesBoard(string name, string value)
        {
            RectangleF bounds = new(10, 22, 28, 28);
            container.DrawImageDice(name, bounds);
            bounds = new(0, -2, 50, 30);
            container.DrawText(bounds, value, 20, "", true);
        }
        private void DrawUtilitiesDice(string name)
        {
            RectangleF bounds = new(10, 2, 30, 30);
            container.DrawImageDice(name, bounds);
            bounds = new(2, 32, 46, 19);
            container.DrawText(bounds, "100", 18, cc1.Black.ToWebColor, false);
        }
        public void DrawGoDice()
        {
            container.DrawImageDice("go.svg");
        }
        public void DrawChanceDice()
        {
            container.DrawImageDice("chance.svg");
        }
        private void DrawImageDice(string name, RectangleF bounds = new())
        {
            if (bounds.Width == 0)
            {
                bounds = new(2, 2, 46, 46);
            }
            Image image = new();
            image.PopulateBasicExternalImage(name);
            //RectangleF bounds = new(2, 2, 46, 46);
            image.PopulateImagePositionings(bounds);
            container.Children.Add(image);
        }
        private void DrawText(RectangleF bounds, string value, float fontSize, string color, bool needsStrokes)
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
                color = cc1.White.ToWebColor;
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
        public void DrawBrokenHouse()
        {
            container.DrawImageDice("brokenhouse.svg");
        }
        public void DrawHouse(int howMany)
        {
            container.DrawImageDice("house.svg");
            if (howMany > 0)
            {
                container.DrawText(new(), howMany.ToString(), 40, "", true);
            }
        }
        public void DrawHotel()
        {
            container.DrawImageDice("hotel.svg");
        }
        public void DrawPolice()
        {
            container.DrawImageDice("police.svg");
        }
        public void DrawOutOfJailFree()
        {
            RectangleF bounds = new(10, 2, 30, 30);
            container.DrawImageDice("free.svg", bounds);
            bounds = new(2, 32, 46, 19);
            container.DrawText(bounds, "Free", 18, cc1.Black.ToWebColor, false);
        }
    }
    
}