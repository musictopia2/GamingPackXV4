namespace Blackjack.Core.ViewModels;
public class BlackjackShellViewModel : SinglePlayerShellViewModel
{
    protected override bool AlwaysNewGame => false; //most games allow new game always.
    public BlackjackShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo GameData,
        ISaveSinglePlayerClass saves,
        IEventAggregator aggregator
        ) : base(mainContainer, container, GameData, saves, aggregator)
    {
    }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<BlackjackMainViewModel>();
        return model;
    }
}