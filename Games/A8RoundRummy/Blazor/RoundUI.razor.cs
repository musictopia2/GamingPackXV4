namespace A8RoundRummy.Blazor;
public partial class RoundUI
{
    [Parameter]
    public StateList<RoundClass>? Rounds { get; set; }
}