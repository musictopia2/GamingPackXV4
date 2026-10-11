namespace SnakesAndLadders.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class SnakesAndLaddersVMData(CommandContainer command, IGamePackageResolver resolver) : IViewModelData, ICup<SimpleDice>, IBasicEnableProcess
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    public DiceCup<SimpleDice>? Cup { get; set; }
    public void LoadCup(SnakesAndLaddersSaveInfo saveRoot, bool autoResume)
    {
        Cup = new DiceCup<SimpleDice>(saveRoot.DiceList, resolver, command);
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