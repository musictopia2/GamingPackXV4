namespace ItalianDominos.Core.ViewModels;
[InstanceGame]
public partial class ItalianDominosMainViewModel : DominoGamesVM<SimpleDominoInfo>
{
    private readonly ItalianDominosMainGameClass _mainGame; //if we don't need, delete.
    private readonly IToast _toast;
    public ItalianDominosVMData VMData { get; set; }
    public ItalianDominosMainViewModel(CommandContainer commandContainer,
            ItalianDominosMainGameClass mainGame,
            ItalianDominosVMData viewModel,
            BasicData basicData,
            TestOptions test,
            IGamePackageResolver resolver,
            IEventAggregator aggregator,
            IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator)
    {
        _mainGame = mainGame;
        VMData = viewModel;
        _toast = toast;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public HandObservable<SimpleDominoInfo> PlayerHand => VMData.PlayerHand1;
    public DominosBoneYardClass<SimpleDominoInfo> BoneYard => VMData.BoneYard;
    public PlayerCollection<ItalianDominosPlayerItem> GetPlayerList => _mainGame.SaveRoot.PlayerList;
    protected override bool CanEnableBoneYard()
    {
        return !_mainGame.SingleInfo!.DrewYet;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task PlayAsync()
    {
        int deck = VMData.PlayerHand1.ObjectSelected();
        if (deck == 0)
        {
            _toast.ShowUserErrorToast("You must choose one domino to play");
            return;
        }
        await _mainGame.PlayDominoAsync(deck);
    }
}