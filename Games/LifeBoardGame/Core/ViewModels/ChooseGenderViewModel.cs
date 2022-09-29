namespace LifeBoardGame.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class ChooseGenderViewModel : ScreenViewModel, IBlankGameVM
{
    private readonly LifeBoardGameVMData _model;
    private readonly IGenderProcesses _processes;
    private readonly LifeBoardGameGameContainer _gameContainer;
    [LabelColumn]
    public string Turn { get; set; } = "";
    [LabelColumn]
    public string Instructions { get; set; } = "";
    public LifeBoardGamePlayerItem GetPlayer => _gameContainer.SingleInfo!;
    public SimpleEnumPickerVM<EnumGender> GetGenderPicker => _model.GenderChooser;
    public ChooseGenderViewModel(CommandContainer commandContainer, LifeBoardGameVMData model, IGenderProcesses processes, LifeBoardGameGameContainer gameContainer, IEventAggregator aggregator) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _model = model;
        _processes = processes;
        _gameContainer = gameContainer;
        _gameContainer.SelectGenderAsync = _processes.ChoseGenderAsync;
        _processes.SetInstructions = (x => Instructions = x);
        _processes.SetTurn = (x => Turn = x); //has to set delegates before init obviously.
        _model.GenderChooser.ItemClickedAsync = GenderChooser_ItemClickedAsync;
    }
    private Task GenderChooser_ItemClickedAsync(EnumGender piece)
    {
        return _processes.ChoseGenderAsync(piece);
    }
    public CommandContainer CommandContainer { get; set; }
}