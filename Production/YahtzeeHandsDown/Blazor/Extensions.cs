namespace YahtzeeHandsDown.Blazor;
public static class Extensions
{
    extension (IParentGraphic graphic)
    {
        public void DrawNormalRectangle(RectangleF rectangle, string color, float borderSize = 0)
        {
            Rect output = new();
            graphic.Children.Add(output);
            output.PopulateRectangle(rectangle);
            output.Fill = color.ToWebColor;
            if (borderSize > 0)
            {
                output.PopulateStrokesToStyles(strokeWidth: borderSize);
            }
        }
        public void DrawRoundedRectangle(RectangleF rectangle, string color, float radius, float borderSize)
        {
            Rect output = new();
            graphic.Children.Add(output);
            output.PopulateRectangle(rectangle);
            output.Fill = color.ToWebColor;
            output.RX = radius.ToString();
            output.RY = radius.ToString();
            output.PopulateStrokesToStyles(strokeWidth: borderSize);
        }
        public void DrawCustomText(RectangleF rectangle, string content, float fontSize, string color, bool bold = false)
        {
            Text text = new();
            text.CenterText(graphic, rectangle);
            text.Fill = color.ToWebColor;
            text.Font_Size = fontSize;
            text.Content = content;
            if (bold)
            {
                text.Font_Weight = "bold";
            }
        }
    }
    
}