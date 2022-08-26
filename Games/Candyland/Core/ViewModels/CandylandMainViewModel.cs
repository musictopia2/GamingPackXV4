namespace Candyland.Core.ViewModels;
[InstanceGame]
public class CandylandMainViewModel : BasicMultiplayerMainVM
{
    private readonly CandylandMainGameClass _mainGame; //if we don't need, delete.
    public CandylandVMData VMData { get; set; }
    public CandylandMainViewModel(CommandContainer commandContainer,
        CandylandMainGameClass mainGame,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        CandylandVMData data
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        _mainGame = mainGame;
        VMData = data;
    }
    public BasicList<CandylandPlayerItem> GetPlayerList
    {
        get
        {
            BasicList<CandylandPlayerItem> output = _mainGame.PlayerList.ToBasicList();
            output.RemoveSpecificItem(_mainGame.SingleInfo!);
            output.Add(_mainGame.SingleInfo!);
            return output;
        }
    }
    public CandylandPlayerItem CurrentPlayer => _mainGame.SingleInfo!;
    public CandylandCardData CurrentCard => _mainGame.SaveRoot.CurrentCard!;
}