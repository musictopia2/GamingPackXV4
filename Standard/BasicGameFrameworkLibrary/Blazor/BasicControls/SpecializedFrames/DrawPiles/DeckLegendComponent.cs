namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SpecializedFrames.DrawPiles;
internal sealed class DeckLegendComponent : ComponentBase
{
    [Parameter]
    [EditorRequired]
    public int Count { get; set; }

    [Parameter]
    public EnumSize? SizeCategory { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<CompactLabel>(0);
        builder.AddAttribute(
            1, nameof(CompactLabel.SizeCategory), SizeCategory);

        builder.AddAttribute(
            2, nameof(CompactLabel.ChildContent),
            (RenderFragment)(content =>
            {
                content.AddContent(0, "Deck ");
                content.OpenElement(1, "span");
                content.AddAttribute(2, "class", "deck-count");
                content.AddContent(3, Count);
                content.CloseElement();
            }));

        builder.CloseComponent();
    }
}