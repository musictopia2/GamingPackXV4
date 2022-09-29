namespace Risk.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class RiskVMData : ISimpleBoardGamesData, IBasicEnableProcess
{
    private readonly CommandContainer _command;
    private readonly IGamePackageResolver _resolver;
    public RiskVMData(CommandContainer command, IGamePackageResolver resolver)
    {
        _command = command;
        _resolver = resolver;
        AttackPicker = new(command, resolver);
        AttackPicker.IndexMethod = ListViewPicker.EnumIndexMethod.OneBased;
        AttackPicker.ItemSelectedAsync = AttackPicker_ItemSelectedAsync;
        NumberPicker = new(command, resolver);
        NumberPicker.ChangedNumberValueAsync = MovePicker_ChangedNumberValueAsync;
        PlayerHand1 = new(_command);
        PlayerHand1.Text = "Your Risk Cards";
        PlayerHand1.Maximum = 5;
        PlayerHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        PlayerHand1.SendEnableProcesses(this, () =>
        {
            if (_saveRoot is null)
            {
                return false;
            }
            return _saveRoot.Stage == EnumStageList.Begin;
        });
    }
    private Task MovePicker_ChangedNumberValueAsync(int chosen)
    {
        ArmiesChosen = chosen;
        _command.UpdateAll();
        return Task.CompletedTask;
    }
    private Task AttackPicker_ItemSelectedAsync(int selectedIndex, string selectedText)
    {
        if (_saveRoot is null)
        {
            throw new CustomBasicException("Must have save root in order to set the number of armies for attacking");
        }
        _saveRoot.ArmiesInBattle = selectedIndex;
        return Task.CompletedTask;
    }
    public HandObservable<RiskCardInfo> PlayerHand1 { get; set; }
    public NumberPicker NumberPicker { get; set; }
    public int ArmiesChosen { get; set; }
    [LabelColumn]
    public int ArmiesToPlace { get; set; }
    [LabelColumn]
    public int BonusReenforcements { get; set; }
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public string Instructions { get; set; } = "";
    public DiceCup<AttackDice>? AttackCup { get; set; }
    public DiceCup<SimpleDice>? DefenseCup { get; set; }
    public bool RollingProgress { get; set; }
    public ListViewPicker AttackPicker;
    private RiskSaveInfo? _saveRoot;
    public void LoadCups(RiskSaveInfo saveRoot, bool autoResume)
    {
        if (AttackCup is not null && DefenseCup is not null && autoResume)
        {
            return;
        }
        _saveRoot = saveRoot;
        AttackCup = new(_resolver, _command);
        DefenseCup = new(_resolver, _command);
        AttackCup.CommandActionString = "AttackCup";
        DefenseCup.CommandActionString = "DefenseCup";
        if (autoResume == false)
        {
            AttackCup.HowManyDice = 0;
            DefenseCup.HowManyDice = 0;
        }
    }
    public bool CanEnableBasics()
    {
        return true; //i think.  hopefully this simple.
    }
}