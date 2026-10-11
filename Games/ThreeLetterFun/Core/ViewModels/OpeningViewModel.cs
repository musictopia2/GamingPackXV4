namespace ThreeLetterFun.Core.ViewModels;
[InstanceGame]
public partial class OpeningViewModel(IEventAggregator aggregator, BasicData data) : IPromptVM, IHandleAsync<OpenPromptEventModel>,
    IHandleAsync<NextScreenEventModel>, IStateChanged
{
    public event Action? StateChanged;
    private partial void Subscribe();
    private partial void Unsubscribe();
    public static EnumTestCategory TestMode { get; set; } = EnumTestCategory.None;
    public FirstOptionViewModel? FirstOptionVM { get; set; }
    public AdvancedOptionsViewModel? AdvancedOptionsVM { get; set; }
    public CardsPlayerViewModel? CardsPlayerVM { get; set; }
    private void OpenFirst()
    {
        FirstOptionVM = Resolver!.Resolve<FirstOptionViewModel>();
        AdvancedOptionsVM = null;
        CardsPlayerVM = null;
        StateChanged?.Invoke();
    }
    private void OpenAdvanced()
    {
        AdvancedOptionsVM = Resolver!.Resolve<AdvancedOptionsViewModel>();
        FirstOptionVM = null;
        CardsPlayerVM = null;
        StateChanged?.Invoke();
    }
    private void OpenCardsPerPlayer()
    {
        CardsPlayerVM = Resolver!.Resolve<CardsPlayerViewModel>();
        FirstOptionVM = null;
        AdvancedOptionsVM = null;
        StateChanged?.Invoke();
    }
    async Task IHandleAsync<OpenPromptEventModel>.HandleAsync(OpenPromptEventModel message)
    {
        if (TestMode != EnumTestCategory.None && data.GamePackageMode == EnumGamePackageMode.Production)
        {
            throw new CustomBasicException("Cannot have a test mode for screens because its in production");
        }
        if (TestMode== EnumTestCategory.FirstOption || TestMode == EnumTestCategory.None)
        {
            OpenFirst();
            return;
        }
        if (TestMode == EnumTestCategory.Advanced)
        {
            OpenAdvanced();
            return;
        }
        if (TestMode == EnumTestCategory.CardsPlayer)
        {
            OpenCardsPerPlayer();
            return;
        }
    }
    void IPromptVM.Initialize()
    {
        Subscribe();
        //not sure yet.,

    }
    async Task IHandleAsync<NextScreenEventModel>.HandleAsync(NextScreenEventModel message)
    {
        if (message.Screen == EnumNextScreen.Advanced)
        {
            OpenAdvanced();
            return;
        }
        if (message.Screen == EnumNextScreen.Cards)
        {
            OpenCardsPerPlayer();
            return;
        }
        if (message.Screen == EnumNextScreen.Finished)
        {
            Unsubscribe();
            await aggregator.PublishAsync(new ClosePromptEventModel());
            return;
        }
        throw new CustomBasicException("Next screen not supported");       
    }
}