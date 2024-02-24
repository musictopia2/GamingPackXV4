namespace MonopolyCardGame.Blazor;
public partial class CalculatorResultsComponent
{
    [CascadingParameter]
    private MonopolyCardGameVMData? Model { get; set; } //this for sure requires the model.  so i can get what i need from it.
    [Parameter]
    public bool IsTesting { get; set; }
    protected override void OnInitialized()
    {
        if (IsTesting)
        {
            Model!.Calculator1.GenerateTestCalculatorResults();
        }
        //means will generate a list of items for the calculator.
        //if there are none, then do nothing.
        //try to do without frame.

    }
}