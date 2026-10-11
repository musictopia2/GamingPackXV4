namespace SkuckCardGame.Core.ViewModels;
[InstanceGame]
public partial class SkuckChoosePlayViewModel
{
    private readonly SkuckCardGameGameContainer _gameContainer;
    private readonly IPlayChoiceProcesses _processes;
    public SkuckChoosePlayViewModel(
        CommandContainer commandContainer,
        SkuckCardGameGameContainer gameContainer,
        IPlayChoiceProcesses processes
        ) 
    {
        _gameContainer = gameContainer;
        _processes = processes;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
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