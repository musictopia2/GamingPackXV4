namespace DealCardGame.Blazor;
public class RentOptionGraphicsComponent : ComponentBase
{
    [CascadingParameter]
    public BasePieceGraphics? MainGraphics { get; set; }

    [Parameter]
    public BasicPickerData<EnumRentCategory>? RentCategory { get; set; }

    protected override void OnInitialized()
    {
        MainGraphics!.OriginalSize = new SizeF(500, 100);
        MainGraphics.NeedsHighlighting = true;
        //double the width than height.
        base.OnInitialized();
    }
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (RentCategory is null)
        {
            return;
        }
        EnumRentCategory category = RentCategory.EnumValue;
        if (category == EnumRentCategory.NeedChoice || category == EnumRentCategory.NA)
        {
            return;
        }
        ISvg svg = MainGraphics!.GetMainSvg(false);
        SvgRenderClass render = new();
        //try to do text.
        //for now, whatever the option is would be used.
        //may create a control that can be used for text enum options (for future).
        RectangleF bounds = new(2, 2, 496, 98);
        Text text = new();
        text.CenterText(svg, bounds);

        text.Font_Size = 75;
        text.Content = category.ToString().TextWithSpaces();
        text.Fill = cc1.Aqua.ToWebColor();
        text.PopulateStrokesToStyles("black", 3);
        render.RenderSvgTree(svg, builder);
        base.BuildRenderTree(builder);
    }
}
