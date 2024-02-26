namespace MonopolyCardGame.Core.Data;
public class PrivateModel : IMappable
{
    public BasicList<CalculatorModel> Calculations { get; set; } = [];
    public BasicList<OrganizeModel> TempSets { get; set; } = [];
}