namespace LottoDominos.Core.ViewModels;
[InstanceGame]
public class MainBoardViewModel : ScreenViewModel, IBlankGameVM
{
    public MainBoardViewModel(CommandContainer commandContainer, LottoDominosMainGameClass mainGame, IEventAggregator aggregator) : base(aggregator)
    {
        if (mainGame.SaveRoot.GameStatus != Data.EnumStatus.NormalPlay)
        {
            throw new CustomBasicException("Can't load the board view model when the status is not even normal play.  Rethink");
        }
        CommandContainer = commandContainer;
    }
    public CommandContainer CommandContainer { get; set; }
}