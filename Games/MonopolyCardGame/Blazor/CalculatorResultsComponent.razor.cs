namespace MonopolyCardGame.Blazor;
public partial class CalculatorResultsComponent
{
    [CascadingParameter]
    private MonopolyCardGameVMData? Model { get; set; } //this for sure requires the model.  so i can get what i need from it.
    [Parameter]
    public bool IsTesting { get; set; }
    private BasicList<CalculatorModel> _list = [];
    private MonopolyCardGameGameContainer? _container;
    protected override void OnInitialized()
    {
        if (IsTesting)
        {
            Model!.Calculator1.GenerateTestCalculatorResults();
        }
        _list = Model!.Calculator1.GetTotalCalculations;
        _container = aa1.Resolver!.Resolve<MonopolyCardGameGameContainer>();
        //means will generate a list of items for the calculator.
        //if there are none, then do nothing.
        //try to do without frame.

    }
    private BasicList<MonopolyCardGameCardInformation> GetCards(CalculatorModel calculator)
    {
        BasicList<MonopolyCardGameCardInformation> output = calculator.GetCards(_container!.DeckList);
        foreach (var item in output)
        {
            item.PlainCategory = EnumPlainCategory.Calculations;
        }
        return output;
        //return calculator.GetCards(_container!.DeckList);
    }
    private decimal GetTotal(CalculatorModel calculator) => calculator.GetMoneyEarned(_container!.DeckList);
}