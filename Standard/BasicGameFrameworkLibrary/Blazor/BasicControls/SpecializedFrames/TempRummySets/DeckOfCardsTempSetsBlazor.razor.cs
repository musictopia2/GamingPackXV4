namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SpecializedFrames.TempRummySets;
public partial class DeckOfCardsTempSetsBlazor<RU>
    where RU : class, IRummmyObject<EnumSuitList, EnumRegularColorList>, IRegularCard, new()
{
    [Parameter]
    public double HeightPercent { get; set; } = 0;
    [Parameter]
    public TempSetsObservable<EnumSuitList, EnumRegularColorList, RU>? TempPiles { get; set; }
}