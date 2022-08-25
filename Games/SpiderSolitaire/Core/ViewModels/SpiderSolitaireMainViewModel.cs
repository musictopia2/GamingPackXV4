
namespace SpiderSolitaire.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class SpiderSolitaireMainViewModel : SolitaireMainViewModel<SpiderSolitaireSaveInfo>
{
    public SpiderSolitaireMainViewModel(IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        ISystemError error
        )
        : base(aggregator, command, resolver)
    {
        _error = error;
        CreateCommands(command);
    }
    partial void CreateCommands(CommandContainer container);
    private SpiderSolitaireMainGameClass? _mainGame;
    private readonly ISystemError _error;
    protected override SolitaireGameClass<SpiderSolitaireSaveInfo> GetGame(IGamePackageResolver resolver)
    {
        _mainGame = resolver.ReplaceObject<SpiderSolitaireMainGameClass>();
        return _mainGame;
    }
    [Command(EnumCommandCategory.Plain)]
    public async Task EndGameAsync()
    {
        await _mainGame!.SendGameOverAsync(_error);
    }
}