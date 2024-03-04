namespace MonopolyCardGame.Core.Data;
public class PrivateModel : IMappable
{
    public Dictionary<int, DetectiveInfo> PersonalNotebook = [];
    public BasicList<PrivatePlayer> ComputerData { get; set; } = []; //so the computer can make smarter decisions.
    
    //public BasicList<CalculatorModel> Calculations { get; set; } = [];
    //public BasicList<OrganizeModel> TempSets { get; set; } = [];
}