namespace LottoDominos.Core.ViewModels;
[InstanceGame]
[EventAggregatorFromMemberClass(nameof(Aggregator))]
public partial class LottoDominosMainViewModel(CommandContainer commandContainer,
        LottoDominosMainGameClass mainGame,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        LottoDominosVMData data,
        LottoDominosScreenState main
        ) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData, resolver, aggregator), IHandleAsync<ChangeGameStatusEventModel>
{
    private partial void Subscribe();
    private partial void Unsubscribe();
    public LottoDominosScreenState MainView => main;
    protected override void AdditionalInitialize()
    {
        Subscribe();
        base.AdditionalInitialize();
    }
    public LottoDominosVMData VMData { get; set; } = data;
    public PlayerCollection<LottoDominosPlayerItem> GetPlayers => mainGame.SaveRoot.PlayerList;
    public ChooseNumberViewModel? ChooseNumber { get; private set; }
    protected override async Task OpenAsync()
    {
        if (mainGame.SaveRoot.GameStatus == EnumStatus.ChooseNumber)
        {
            ChooseNumber = Resolver.Resolve<ChooseNumberViewModel>();
            MainView.MainPlay = false;
            return;
        }
        if (mainGame.SaveRoot.GameStatus == EnumStatus.NormalPlay)
        {
            ChooseNumber = null;
            MainView.MainPlay = true;
            return;
        }
        throw new CustomBasicException("Rethink because no status found");
    }
    protected override Task CloseAsync()
    {
        ChooseNumber = null;
        MainView.MainPlay = false;
        Unsubscribe();
        return base.CloseAsync();
    }
    
    async Task IHandleAsync<ChangeGameStatusEventModel>.HandleAsync(ChangeGameStatusEventModel message)
    {
        if (mainGame.SaveRoot.GameStatus != EnumStatus.NormalPlay)
        {
            throw new CustomBasicException("Only normal play is supported to change status.  Otherwise, rethink");
        }
        if (ChooseNumber == null)
        {
            return;
        }
        ChooseNumber = null;
        MainView.MainPlay = true;
    }
}