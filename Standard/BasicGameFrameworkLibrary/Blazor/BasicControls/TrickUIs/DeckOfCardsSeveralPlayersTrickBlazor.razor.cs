namespace BasicGameFrameworkLibrary.Blazor.BasicControls.TrickUIs;
public partial class DeckOfCardsSeveralPlayersTrickBlazor<P, B, T>
    where P : IPlayerTrick<EnumSuitList, T>, new()
    where B : BasicTrickAreaObservable<EnumSuitList, T>, IMultiplayerTrick<EnumSuitList, T, P>
    where T : class, IRegularCard, ITrickCard<EnumSuitList>, new()
{
    [Parameter]
    public B? DataContext { get; set; }

    [CascadingParameter]
    public int TargetHeight { get; set; } = 15;
    [Parameter]
    public bool ExtraLongSecondColumn { get; set; } = false;
    private string RealHeight => $"{TargetHeight}vh";
}