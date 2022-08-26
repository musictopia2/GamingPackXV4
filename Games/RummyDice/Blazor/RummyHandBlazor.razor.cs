namespace RummyDice.Blazor;
public partial class RummyHandBlazor
{
    [Parameter]
    public RummyDiceHandVM? DataContext { get; set; }
    [Parameter]
    public string TargetHeight { get; set; } = ""; //for now, do here.
    private string TempSetName => $"Temp Set {DataContext!.Index}";
    private async Task OnClickAsync(RummyDiceInfo dice)
    {
        if (DataContext!.DiceCommand!.CanExecute(dice) == false)
        {
            return;
        }
        await DataContext.DiceCommand.ExecuteAsync(dice);
    }
}