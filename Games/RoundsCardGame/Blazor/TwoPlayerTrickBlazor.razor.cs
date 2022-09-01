namespace RoundsCardGame.Blazor;
public partial class TwoPlayerTrickBlazor
{
    [Parameter]
    public BasicTrickAreaObservable<EnumSuitList, RoundsCardGameCardInformation>? DataContext { get; set; }

    [CascadingParameter]
    public int TargetHeight { get; set; } = 15;
    private string RealHeight => $"{TargetHeight}vh";
}