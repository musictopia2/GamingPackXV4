using System.Globalization;

namespace BasicGameFrameworkLibrary.Blazor.GameGraphics.Base;
public partial class BasePieceGraphics : GraphicsCommand
{
    //this will set all the values that is the base.
    //this means a person would use this then the template will be graphics item
    [Parameter]
    public bool Animating { get; set; } = false; //maybe if animating, must render period now.  try that idea next.
    
    [Parameter]
    public bool ForceRender { get; set; }
    public string FillColor { get; set; } = cs1.Aqua; //defaults to aqua but child can set to something else.
    [Parameter]
    public RenderFragment? ChildContent { get; set; } //only one is required here.

    //won't worry about pickers for now.  has to change later though.

    [CascadingParameter]
    public IPiecePicker? CustomPicker { get; set; }
    //[CascadingParameter]
    //public NumberChooserBlazor? NumberPicker { get; set; }

    [Parameter]
    public string HeightPixels { get; set; } = "";



    [Parameter]
    public double? HeightPercent { get; set; }

    //may already be provided

    //[Parameter]
    //public int? CompactHeight { get; set; }

    //[Parameter]
    //public int? TabletHeight { get; set; }

    //[Parameter]
    //public int? DesktopHeight { get; set; }


    [Parameter]
    public float LongestSize { get; set; } //this means ui can't update it.
    public bool NeedsHighlighting { get; set; } //no longer needs to be observable object.  this way it can't anyways.
    public bool HighlightTransparent { get; set; } //the child can set it when necessary.
    [Parameter]
    public PointF Location { get; set; } //iffy.  well see when we do animations.
    public SizeF OriginalSize { get; set; }
    public RectangleF GetRectangle => new(0, 0, OriginalSize.Width, OriginalSize.Height); //try this way since this should already handle the locations.
    [Parameter]
    public bool IsSelected { get; set; }
    public bool UsedSelected = false; // the checker piece will not use that part.  if that changes, can do.
    public float BorderWidth { get; set; } = 4;
    
    public float Scale()
    {
        if (LongestSize == 0)
        {
            return 1;
        }
        if (OriginalSize.Width <= OriginalSize.Height)
        {
            return LongestSize / OriginalSize.Height;
        }
        return LongestSize / OriginalSize.Width;
    }


    private bool GetIsSelected()
    {
        if (CommandParameter is ListPieceModel piece)
        {
            return piece.IsSelected;
        }

        return IsSelected;
    }

    private string GetHighlightColor()
    {
        if (NeedsHighlighting == false)
        {
            return cs1.Transparent.ToWebColor; //this means won't even do
        }
        if (GetIsSelected())
        {
            return cs1.Lime.ToWebColor; //will be lime
        }
        if (CustomCanDo.Invoke() == false)
        {
            return cs1.LightGray.ToWebColor;
        }
        if (HighlightTransparent)
        {
            return cs1.Transparent.ToWebColor;
        }
        return FillColor.ToWebColor;
    }
    private int? GetHeight()
    {
        if (HeightPercent is null)
        {
            return null;
        }
        return ll1.GetHeight(HeightPercent.Value);
        ////Use your existing layout classification here.
        //return ll1.LayoutMode switch
        //{
        //    EnumGameLayoutMode.Compact => CompactHeight,
        //    EnumGameLayoutMode.Tablet => TabletHeight,
        //    EnumGameLayoutMode.Desktop => DesktopHeight,
        //    _ => null
        //};
    }

    private static string SvgNumber(float value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    private void PopulateCustomViewBox(SVG svg)
    {
        float offset = -BorderWidth / 2;
        float width = OriginalSize.Width + BorderWidth;
        float height = OriginalSize.Height + BorderWidth;

        svg.ViewBox =
            $"{SvgNumber(offset)} {SvgNumber(offset)} " +
            $"{SvgNumber(width)} {SvgNumber(height)}";
    }

    public ISvg GetMainSvg(bool? controlhighlight = null)
    {
        SVG output = new();
        int? targetHeight = GetHeight();

        if (targetHeight is int height)
        {
            if (height <= 0)
            {
                throw new InvalidOperationException(
                    "The target piece height must be greater than zero.");
            }

            if (controlhighlight is not false)
            {
                NeedsHighlighting = true;
            }

            float viewBoxWidth = OriginalSize.Width + BorderWidth;
            float viewBoxHeight = OriginalSize.Height + BorderWidth;
            float width = height * viewBoxWidth / viewBoxHeight;

            output.Height = $"{height}px";
            output.Width = $"{SvgNumber(width)}px";

            PopulateCustomViewBox(output);
            output.Style = "margin: 2px;";
        }
        else if (string.IsNullOrWhiteSpace(HeightPixels) == false)
        {
            if (controlhighlight is not false)
            {
                NeedsHighlighting = true;
            }

            float viewBoxWidth = OriginalSize.Width + BorderWidth;
            float viewBoxHeight = OriginalSize.Height + BorderWidth;
            float aspectRatio = viewBoxWidth / viewBoxHeight;

            output.Height = HeightPixels;
            output.Width =
                $"calc({HeightPixels} * {SvgNumber(aspectRatio)})";

            PopulateCustomViewBox(output);
            output.Style = "margin: 2px;";
        }
        else if (CustomPicker is not null)
        {
            //Keep the existing picker behavior until its redesign.
            NeedsHighlighting = CustomPicker.CanHighlight;
            output.Height = CustomPicker.TextHeight;
            output.Style =
                "margin-right: .2vw; margin-bottom: .2vw; margin-left: .2vw;";

            output.ViewBox =
                $"0 0 {CustomPicker.TextWidth} {CustomPicker.ViewBoxHeight}";
        }
        else
        {
            //Keep the existing board and animation sizing.
            float scale = Scale();

            output.X = SvgNumber(Location.X);
            output.Y = SvgNumber(Location.Y);
            output.Width = SvgNumber(scale * OriginalSize.Width);
            output.Height = SvgNumber(scale * OriginalSize.Height);

            PopulateCustomViewBox(output);
        }

        Rect rect = new()
        {
            Width = "100%",
            Height = "100%",
            Fill = GetHighlightColor()
        };

        output.Children.Add(rect);
        CreateClick(output);

        return output;
    }
}