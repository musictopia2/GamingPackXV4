namespace Pinochle2Player.Blazor;
public partial class TwoPlayerTrickBlazor
{
    [Parameter]
    public BasicTrickAreaObservable<EnumSuitList, Pinochle2PlayerCardInformation>? DataContext { get; set; }

    [CascadingParameter]
    public int TargetHeight { get; set; } = 15;
    private string RealHeight => $"{TargetHeight}vh";
}