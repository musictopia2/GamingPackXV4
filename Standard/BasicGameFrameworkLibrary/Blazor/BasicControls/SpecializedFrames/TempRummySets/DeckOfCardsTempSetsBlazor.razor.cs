namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SpecializedFrames.TempRummySets;
public partial class DeckOfCardsTempSetsBlazor<RU>
    where RU : class, IRummmyObject<EnumSuitList, EnumRegularColorList>, IRegularCard, new()
{
    [Parameter]
    public string TargetContainerSize { get; set; } = "";
    [Parameter]
    public TempSetsObservable<EnumSuitList, EnumRegularColorList, RU>? TempPiles { get; set; }
}