namespace Rook.Core.ViewModels;
[InstanceGame]
public partial class RookColorViewModel : ScreenViewModel, IBlankGameVM
{
    public readonly RookVMData Model;
    private readonly ITrumpProcesses _processes;
    public RookColorViewModel(CommandContainer commandContainer,
        RookVMData model,
        ITrumpProcesses processes,
        IEventAggregator aggregator
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        Model = model;
        _processes = processes;
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public CommandContainer CommandContainer { get; set; }
    public bool CanTrump => Model.ColorChosen != EnumColorTypes.None;
    [Command(EnumCommandCategory.Plain)]
    public async Task TrumpAsync()
    {
        await _processes.ProcessTrumpAsync();
    }
}