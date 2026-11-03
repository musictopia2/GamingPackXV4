namespace SinisterSix.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class SinisterSixVMData : IBasicDiceGamesData<EightSidedDice>
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    [LabelColumn]
    public partial int MaxRolls { get; set; }
    [LabelColumn]
    public partial int RollNumber { get; set; }
    private readonly IGamePackageResolver _resolver;
    private readonly CommandContainer _command;
    public DiceCup<EightSidedDice>? Cup { get; set; }
    public SinisterSixVMData(IGamePackageResolver resolver, CommandContainer command)
    {
        _resolver = resolver;
        _command = command;
    }
    public void LoadCup(ISavedDiceList<EightSidedDice> saveRoot, bool autoResume)
    {
        if (Cup != null && autoResume)
        {
            return;
        }
        Cup = new DiceCup<EightSidedDice>(saveRoot.DiceList, _resolver, _command);
        if (autoResume == true)
        {
            Cup.CanShowDice = true;
        }
        Cup.HowManyDice = 6;
        Cup.Visible = true;
        Cup.ShowHold = false;
    }
}