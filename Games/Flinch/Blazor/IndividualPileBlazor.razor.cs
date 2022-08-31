namespace Flinch.Blazor;
public partial class IndividualPileBlazor
{
    [CascadingParameter]
    public PublicPilesViewModel? MainPiles { get; set; }
    [CascadingParameter]
    public int TargetHeight { get; set; }
    [Parameter]
    public BasicPileInfo<FlinchCardInformation>? IndividualPile { get; set; }
    private string RealHeight => TargetHeight.HeightString();
    private FlinchCardInformation? _card;
    protected override void OnParametersSet()
    {
        _card = IndividualPile!.ObjectList.Last();
        base.OnParametersSet();
    }
}