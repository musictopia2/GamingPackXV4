namespace Spades2Player.Core.ViewModels;
[InstanceGame]
public partial class SpadesBeginningViewModel : ScreenViewModel, IBlankGameVM
{
    private readonly Spades2PlayerMainGameClass _mainGame;
    public SpadesBeginningViewModel(CommandContainer commandContainer, Spades2PlayerMainGameClass mainGame, Spades2PlayerVMData model, IEventAggregator aggregator) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _mainGame = mainGame;
        Model = model;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public CommandContainer CommandContainer { get; set; }
    public Spades2PlayerVMData Model { get; }
    [Command(EnumCommandCategory.Plain)]
    public async Task TakeCardAsync()
    {
        if (_mainGame.BasicData!.MultiPlayer == true)
        {
            await _mainGame.Network!.SendAllAsync("acceptcard");
        }
        await _mainGame.AcceptCardAsync();
    }
}