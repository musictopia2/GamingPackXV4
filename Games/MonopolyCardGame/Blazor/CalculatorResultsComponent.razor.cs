namespace MonopolyCardGame.Blazor;
public partial class CalculatorResultsComponent
{
    [Parameter]
    [EditorRequired]
    public BasicList<CalculatorModel> Calculations { get; set; } = [];
    private MonopolyCardGameGameContainer? _container;
    protected override void OnInitialized()
    {
        _container = aa1.Resolver!.Resolve<MonopolyCardGameGameContainer>();
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