namespace HuseHearts.Core.ViewModels;
[InstanceGame]
public partial class MoonViewModel : ScreenViewModel, IBlankGameVM
{
    private readonly HuseHeartsMainGameClass _mainGame;
    public MoonViewModel(CommandContainer commandContainer, HuseHeartsMainGameClass mainGame, IEventAggregator aggregator) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _mainGame = mainGame;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public CommandContainer CommandContainer { get; set; }
    [Command(EnumCommandCategory.Plain)]
    public async Task MoonAsync(EnumMoonOptions option)
    {
        switch (option)
        {
            case EnumMoonOptions.GiveSelfMinus:
                if (_mainGame.BasicData!.MultiPlayer == true)
                {
                    await _mainGame.Network!.SendAllAsync("takepointsaway");
                }
                await _mainGame!.GiveSelfMinusPointsAsync();
                break;
            case EnumMoonOptions.GiveEverybodyPlus:
                if (_mainGame.BasicData!.MultiPlayer == true)
                {
                    await _mainGame.Network!.SendAllAsync("givepointseverybodyelse");
                }
                await _mainGame!.GiveEverybodyElsePointsAsync();
                break;
            default:
                throw new CustomBasicException("Not Supported");
        }
    }
}