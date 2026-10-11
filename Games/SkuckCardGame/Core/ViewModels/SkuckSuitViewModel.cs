namespace SkuckCardGame.Core.ViewModels;
[InstanceGame]
public partial class SkuckSuitViewModel
{
    public readonly SkuckCardGameVMData Model;
    private readonly SkuckCardGameGameContainer _gameContainer;
    private readonly ITrumpProcesses _processes;
    public SkuckSuitViewModel(CommandContainer commandContainer,
        SkuckCardGameVMData model,
        SkuckCardGameGameContainer gameContainer,
        ITrumpProcesses processes
        )
    {
        Model = model;
        _gameContainer = gameContainer;
        _processes = processes;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public bool CanTrump => Model.TrumpSuit != EnumSuitList.None;
    [Command(EnumCommandCategory.Plain)]
    public async Task TrumpAsync()
    {
        if (_gameContainer.BasicData!.MultiPlayer == true)
        {
            await _gameContainer.Network!.SendAllAsync("trump", Model.TrumpSuit);
        }
        await _processes!.TrumpChosenAsync();
    }
}