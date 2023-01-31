namespace BasicGameFrameworkLibrary.Blazor.GameGraphics.GamePieces;
public enum EnumCheckerPieceCategory
{
    SinglePiece = 1,
    CrownedPiece = 2,
    OnlyPiece = 3,
    FlatPiece = 4
}
internal record CheckerRecord(string Color, EnumCheckerPieceCategory PieceCategory, bool HasImage, bool IsSelected, bool Enabled);
public class CheckerPiece : ComponentBase
{
    CheckerRecord? _previousRecord;
    protected override bool ShouldRender()
    {
        if (MainGraphics!.Animating)
        {
            return true; //because you are doing animations.
        }
        var current = GetRecord();
        return current.Equals(_previousRecord) == false;
    }
    protected override void OnAfterRender(bool firstRender)
    {
        _previousRecord = GetRecord();
        base.OnAfterRender(firstRender);
    }
    private CheckerRecord GetRecord()
    {
        return new CheckerRecord(MainColor, PieceCategory, HasImage, MainGraphics!.IsSelected, MainGraphics.CustomCanDo.Invoke());
    }
    [CascadingParameter]
    public BasePieceGraphics? MainGraphics { get; set; }
    [Parameter]
    public string MainColor { get; set; } = cs1.Transparent;
    [Parameter]
    public EnumCheckerPieceCategory PieceCategory { get; set; } = EnumCheckerPieceCategory.OnlyPiece; //only game of checkers has to do with single/crowned.
    [Parameter]
    public bool HasImage { get; set; } = true;
    [Parameter]
    public string BlankColor { get; set; } = cs1.White;
    protected override void OnInitialized()
    {
        MainGraphics!.OriginalSize = new SizeF(300, 300); //decided to use 300 by 300 this time.
        MainGraphics.BorderWidth = 10;
        MainGraphics.HighlightTransparent = true;
        base.OnInitialized();
    }
    private void PopulateBlank(ISvg svg)
    {
        Circle circle = new()
        {
            CX = "150",
            CY = "150",
            R = "150",
            Fill = BlankColor.ToWebColor()
        };
        svg.Children.Add(circle);
    }
    private void BuildDefs(ISvg svg)
    {
        Defs defs = new();
        LinearGradient linear = new();
        linear.ID = "gradient_0";
        linear.GradientUnits = "userSpaceOnUse";
        linear.X1 = "0";
        if (PieceCategory == EnumCheckerPieceCategory.FlatPiece)
        {
            linear.Y1 = "299";
        }
        else
        {
            linear.Y1 = "300";
        }
        linear.X2 = "300";
        linear.Y2 = linear.Y1;
        defs.Children.Add(linear);
        svg.Children.Add(defs);
        Stop stop = new()
        {
            Offset = "0",
            Stop_Color = "rgb(255,255,255)",
            Stop_Opacity = ".39215687"
        };
        linear.Children.Add(stop);
        stop = new();
        stop.Offset = "1";
        stop.Stop_Color = "rgb(0,0,0)";
        stop.Stop_Opacity = ".58823532";
        linear.Children.Add(stop);
    }
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        ISvg svg = MainGraphics!.GetMainSvg(false);
        SvgRenderClass render = new();
        if (HasImage == false)
        {
            PopulateBlank(svg);
            render.RenderSvgTree(svg, builder);
            return;
        }
        BuildDefs(svg);
        if (PieceCategory == EnumCheckerPieceCategory.FlatPiece)
        {
            PopulateFlatPiece(svg);
            render.RenderSvgTree(svg, builder);
            return;
        }
        PopulateCrownOrRegular(svg);
        render.RenderSvgTree(svg, builder);
        base.BuildRenderTree(builder);
    }
    private void PopulateCrownOrRegular(ISvg svg)
    {
        Ellipse ellipse;
        ellipse = new()
        {
            Fill = MainColor.ToWebColor(),
            CX = "150",
            CY = "178.5",
            RX = "150",
            RY = "121"
        };
        svg.Children.Add(ellipse);
        if (PieceCategory != EnumCheckerPieceCategory.OnlyPiece)
        {
            ellipse = new Ellipse();
            PopulateEllipseIncludingStroke(ellipse, svg, GetGradientID, 150, 178.5, 150, 121);
            ellipse = new Ellipse();
            PopulateEllipseIncludingStroke(ellipse, svg, MainColor.ToWebColor(), 150, 151.5, 150, 121);
        }
        if (PieceCategory == EnumCheckerPieceCategory.OnlyPiece)
        {
            ellipse = new Ellipse();
            PopulateEllipseIncludingStroke(ellipse, svg, GetGradientID, 150, 150, 150, 150);
            ellipse = new Ellipse();
            PopulateEllipseIncludingStroke(ellipse, svg, MainColor.ToWebColor(), 150, 135, 150, 135);
        }
        if (PieceCategory == EnumCheckerPieceCategory.CrownedPiece)
        {
            ellipse = new Ellipse();
            PopulateEllipseIncludingStroke(ellipse, svg, GetGradientID, 150, 148.5, 150, 121);
            ellipse = new Ellipse();
            PopulateEllipseIncludingStroke(ellipse, svg, MainColor.ToWebColor(), 150, 121.5, 150, 121);
        }
    }
    private static string GetGradientID => "url(#gradient_0)";
    private void PopulateFlatPiece(ISvg svg)
    {
        var ellipse = new Ellipse
        {
            Fill = MainColor.ToWebColor(),
            CX = "150",
            CY = "252.25",
            RX = "150",
            RY = "46"
        };
        svg.Children.Add(ellipse);
        ellipse = new();
        PopulateEllipseIncludingStroke(ellipse, svg, GetGradientID, 150, 252.25, 150, 46);
        Rect rect = new();
        rect.Fill = MainColor.ToWebColor();
        rect.Y = "158.75";
        rect.Width = "300";
        rect.Height = "93.5";
        svg.Children.Add(rect);
        rect = new();
        rect.Fill = GetGradientID;
        rect.Y = "158.75";
        rect.Width = "300";
        rect.Height = "93.5";
        svg.Children.Add(rect);
        Path path = new();
        path.Fill = "none";
        path.PopulateStrokesToStyles(strokeWidth: (int)MainGraphics!.BorderWidth);
        path.D = "M0 158.75 L0 252.25";
        svg.Children.Add(path);
        path = new();
        path.Fill = "none";
        path.PopulateStrokesToStyles(strokeWidth: (int)MainGraphics!.BorderWidth);
        path.D = "M300 158.75 L300 252.25";
        svg.Children.Add(path);
        ellipse = new();
        PopulateEllipseIncludingStroke(ellipse, svg, MainColor.ToWebColor(), 150, 158.75, 150, 46);
    }
    private void PopulateEllipseIncludingStroke(Ellipse ellipse, ISvg svg, string color, double cx, double cy, double rx, double ry)
    {
        ellipse.PopulateStrokesToStyles(strokeWidth: (int)MainGraphics!.BorderWidth);
        ellipse.CX = cx.ToString();
        ellipse.CY = cy.ToString();
        ellipse.Fill = color;
        ellipse.RX = rx.ToString();
        ellipse.RY = ry.ToString();
        svg.Children.Add(ellipse);
    }
}
