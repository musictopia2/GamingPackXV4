namespace MonopolyCardGame.Blazor;
public partial class RailRoadChooserComponent
{
    [Parameter]
    public MonopolyCardGameCardInformation? RailRoad { get; set; }
    [Parameter]
    public int HowMany { get; set; }
    [Parameter]
    public EventCallback<int> OnChose { get; set; }
    private string GetText => $"{HowMany} Railroads";
    private void ChooseClick()
    {
        OnChose.InvokeAsync(HowMany);
    }
}