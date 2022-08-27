namespace Checkers.Core.ViewModels;
[InstanceGame]
public partial class CheckersMainViewModel : BasicMultiplayerMainVM
{
    private readonly CheckersMainGameClass _mainGame; //if we don't need, delete.
    public CheckersVMData VMData { get; set; }
    private readonly BasicData _basicData;
    public CheckersMainViewModel(CommandContainer commandContainer,
        CheckersMainGameClass mainGame,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        CheckersVMData data
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        _mainGame = mainGame;
        VMData = data;
        _basicData = basicData;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public override bool CanEndTurn()
    {
        return _mainGame.SaveRoot.GameStatus == EnumGameStatus.PossibleTie;
    }
    public bool CanTie
    {
        get
        {
            if (_mainGame.SaveRoot.SpaceHighlighted > 0)
            {
                return false;
            }
            return _mainGame.SaveRoot.ForcedToMove == false;
        }
    }
    [Command(EnumCommandCategory.Game)]
    public async Task TieAsync()
    {
        if (_basicData.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync("possibletie");
        }
        CommandContainer.ManuelFinish = true;
        await _mainGame.ProcessTieAsync();
    }
}