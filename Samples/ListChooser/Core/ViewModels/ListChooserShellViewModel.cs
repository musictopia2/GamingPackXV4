namespace ListChooser.Core.ViewModels;
public class ListChooserShellViewModel(IGamePackageResolver main,
    CommandContainer command,
    ISaveSinglePlayerClass saves,
    IEventAggregator aggregator
        ) : SinglePlayerShellViewModel(main, command, saves, aggregator)
{
    protected override bool AlwaysNewGame => false;
    protected override bool AutoStartNewGame => false;
}