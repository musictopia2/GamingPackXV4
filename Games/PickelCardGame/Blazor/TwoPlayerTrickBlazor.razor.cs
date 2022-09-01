namespace PickelCardGame.Blazor;
public partial class TwoPlayerTrickBlazor
{
    [Parameter]
    public BasicTrickAreaObservable<EnumSuitList, PickelCardGameCardInformation>? DataContext { get; set; }

    [CascadingParameter]
    public int TargetHeight { get; set; } = 15;
    private string RealHeight => $"{TargetHeight}vh";
}