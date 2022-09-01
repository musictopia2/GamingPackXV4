namespace SkuckCardGame.Core.ViewModels;
[InstanceGame]
public partial class SkuckChoosePlayViewModel : ScreenViewModel, IBlankGameVM
{
    private readonly SkuckCardGameGameContainer _gameContainer;
    private readonly IPlayChoiceProcesses _processes;
    public SkuckChoosePlayViewModel(
        CommandContainer commandContainer,
        SkuckCardGameGameContainer gameContainer,
        IPlayChoiceProcesses processes,
        IEventAggregator aggregator
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _gameContainer = gameContainer;
        _processes = processes;
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public CommandContainer CommandContainer { get; set; }
    [Command(EnumCommandCategory.Plain)]
    public async Task FirstPlayAsync(EnumChoiceOption choice)
    {
        if (choice == EnumChoiceOption.None)
        {
            throw new CustomBasicException("Not Supported");
        }
        if (choice == EnumChoiceOption.Play)
        {
            if (_gameContainer.BasicData!.MultiPlayer == true)
            {
                await _gameContainer.Network!.SendAllAsync("choosetoplay");
            }
            await _processes!.ChooseToPlayAsync();
            return;
        }
        if (choice == EnumChoiceOption.Pass)
        {
            if (_gameContainer.BasicData!.MultiPlayer == true)
            {
                await _gameContainer.Network!.SendAllAsync("choosetopass");
            }
            await _processes!.ChooseToPassAsync();
            return;
        }
    }
}