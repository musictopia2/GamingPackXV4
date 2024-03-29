﻿namespace SinglePlayerMiscGames.Core.ViewModels;
public class SinglePlayerMiscGamesShellViewModel : SinglePlayerShellViewModel
{
    protected override bool AlwaysNewGame => true; //most games allow new game always.
    public SinglePlayerMiscGamesShellViewModel(IGamePackageResolver mainContainer,
        CommandContainer container,
        IGameInfo GameData,
        ISaveSinglePlayerClass saves,
        IEventAggregator aggregator
        ) : base(mainContainer, container, GameData, saves, aggregator)
    {
    }
    protected override IMainScreen GetMainViewModel()
    {
        var model = MainContainer.Resolve<SinglePlayerMiscGamesMainViewModel>();
        return model;
    }
}