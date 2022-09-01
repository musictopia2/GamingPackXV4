namespace GalaxyCardGame.Blazor;
public partial class TwoPlayerTrickBlazor
{
    [Parameter]
    public BasicTrickAreaObservable<EnumSuitList, GalaxyCardGameCardInformation>? DataContext { get; set; }

    [CascadingParameter]
    public int TargetHeight { get; set; } = 15;
    private string RealHeight => $"{TargetHeight}vh";
}