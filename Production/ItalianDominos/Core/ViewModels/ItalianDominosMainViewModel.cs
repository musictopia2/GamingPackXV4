namespace ItalianDominos.Core.ViewModels;
[InstanceGame]
public partial class ItalianDominosMainViewModel(
    CommandContainer commandContainer,
    ItalianDominosMainGameClass mainGame,
    ItalianDominosVMData viewModel,
    BasicData basicData,
    IGamePackageResolver resolver,
    IEventAggregator aggregator,
    IToast toast)
    : DominoGamesVM<SimpleDominoInfo>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator)
        
{
    public ItalianDominosVMData VMData { get; set; } = viewModel;
    protected override void AdditionalInitialize()
    {
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    public HandObservable<SimpleDominoInfo> PlayerHand => VMData.PlayerHand1;
    public DominosBoneYardClass<SimpleDominoInfo> BoneYard => VMData.BoneYard;
    public PlayerCollection<ItalianDominosPlayerItem> GetPlayerList => mainGame.SaveRoot.PlayerList;
    protected override bool CanEnableBoneYard()
    {
        return !mainGame.SingleInfo!.DrewYet;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task PlayAsync()
    {
        int deck = VMData.PlayerHand1.ObjectSelected();
        if (deck == 0)
        {
            toast.ShowUserErrorToast("You must choose one domino to play");
            return;
        }
        await mainGame.PlayDominoAsync(deck);
    }
}