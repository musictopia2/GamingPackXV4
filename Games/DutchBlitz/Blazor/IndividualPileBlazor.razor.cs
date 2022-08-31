namespace DutchBlitz.Blazor;
public partial class IndividualPileBlazor
{
    [CascadingParameter]
    public PublicViewModel? MainPiles { get; set; }
    [CascadingParameter]
    public int TargetHeight { get; set; }
    [Parameter]
    public BasicPileInfo<DutchBlitzCardInformation>? IndividualPile { get; set; }
    private string RealHeight => TargetHeight.HeightString();
    private DutchBlitzCardInformation? _card;
    protected override void OnParametersSet()
    {
        _card = IndividualPile!.ObjectList.Last();
        base.OnParametersSet();
    }
}