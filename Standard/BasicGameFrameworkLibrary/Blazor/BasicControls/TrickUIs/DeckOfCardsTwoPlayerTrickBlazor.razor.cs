namespace BasicGameFrameworkLibrary.Blazor.BasicControls.TrickUIs;
public partial class DeckOfCardsTwoPlayerTrickBlazor<T>
    where T : class, IRegularCard, ITrickCard<EnumSuitList>, new()
{
    [Parameter]
    public BasicTrickAreaObservable<EnumSuitList, T>? DataContext { get; set; }
    [CascadingParameter]
    public int TargetHeight { get; set; } = 15;
    private string RealHeight => $"{TargetHeight}vh";
}