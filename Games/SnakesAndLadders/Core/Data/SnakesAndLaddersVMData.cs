namespace SnakesAndLadders.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class SnakesAndLaddersVMData : IViewModelData, ICup<SimpleDice>, IBasicEnableProcess
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    private readonly CommandContainer _command;
    private readonly IGamePackageResolver _resolver;
    public DiceCup<SimpleDice>? Cup { get; set; }
    public SnakesAndLaddersVMData(CommandContainer command, IGamePackageResolver resolver)
    {
        _command = command;
        _resolver = resolver;
    }
    public void LoadCup(SnakesAndLaddersSaveInfo saveRoot, bool autoResume)
    {
        Cup = new DiceCup<SimpleDice>(saveRoot.DiceList, _resolver, _command);
        Cup.SendEnableProcesses(this, () =>
        {
            return false; //because you can't click the dice.
        });
        Cup.HowManyDice = 1;
        if (autoResume == true && saveRoot.HasRolled == true)
        {
            Cup.CanShowDice = true;
            Cup.Visible = true;
        }
    }
    bool IBasicEnableProcess.CanEnableBasics()
    {
        return false;
    }
}