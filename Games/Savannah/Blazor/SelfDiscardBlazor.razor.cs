namespace Savannah.Blazor;
public partial class SelfDiscardBlazor
{
    [Parameter] public RenderFragment<RegularSimpleCard>? ChildContent { get; set; }
    [Parameter] public SelfDiscardCP? DiscardPile { get; set; }
    [CascadingParameter]
    public int TargetHeight { get; set; }
    private static double Divider => 5;
    private bool IsDisabled => !DiscardPile!.IsEnabled;
    private string GetColorStyle()
    {
        if (IsDisabled == false)
        {
            return "";
        }
        return $"color:{cc1.LightGray.ToWebColor()}; border-color: {cc1.LightGray.ToWebColor()}";
    }
    private string GetContainerStyle()
    {
        return "overflow-x: auto; width: 100%; margin-right: 10px;";
    }
    private string GetCardContainerStyle()
    {
        //if (DiscardPile!.HandList.Count == 0)
        //{
        //    return "position: relative; width: auto; height: auto;";
        //}

        var card = new RegularSimpleCard();
        double cardWidth = card.DefaultSize.Width;
        double cardHeight = card.DefaultSize.Height;
        double containerWidth;

        containerWidth = (cardWidth / Divider) * 6 + cardWidth * 2  / Divider;


        return $"position: relative; width: {containerWidth}px; height: {TargetHeight}vh; overflow: hidden;";
    }
   
    private string GetCardStyle(int index, RegularSimpleCard card)
    {
        double cardWidth = card.DefaultSize.Width;
        double cardHeight = card.DefaultSize.Height;
        double leftOffset;

        if (index < DiscardPile!.Maximum)
        {
            // Slight overlap
            leftOffset = index * (cardWidth / Divider);
        }
        else
        {
            // Stack directly on top of the 4th card
            leftOffset = (DiscardPile.Maximum - 1) * (cardWidth / Divider); // same as index 3
        }

        return $"position: absolute; width: {cardWidth}px; height: {cardHeight}px; " +
               $"left: {leftOffset}px; top: 0px;";
    }

    
    private async Task ClickAsync()
    {
        if (DiscardPile!.HandList.Count == 0)
        {
            await DiscardPile.DiscardEmptyAsync();
        }
    }

    protected override void OnParametersSet()
    {
        // This will no longer be needed as we’re moving to CSS for layout.
    }
}