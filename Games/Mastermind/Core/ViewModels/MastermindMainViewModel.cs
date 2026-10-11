namespace Mastermind.Core.ViewModels;
[InstanceGame]
public partial class MastermindMainViewModel(CommandContainer commandContainer,
        IGamePackageResolver resolver,
         LevelClass level,
        BasicData basicData,
        IEventAggregator aggregator) : IBasicEnableProcess,
    IMainGameVM, IHandleAsync<OpenMainEventModel>, IHandleAsync<CloseMainEventModel>
{
    void IMainGameVM.Initialize()
    {
        Subscribe();
        LevelChosen = level.LevelChosen;
        _global = resolver.ReplaceObject<GlobalClass>();
        CustomColorClass colorClass = new(_global);
        _mainGame = resolver.ReplaceObject<MastermindMainGameClass>();
        MainBoard = resolver.Resolve<GameBoardViewModel>();
        Color1 = new(commandContainer, colorClass);
        Color1.AutoSelectCategory = EnumAutoSelectCategory.AutoEvent;
        Color1.IsEnabled = true;
        Color1.ItemClickedAsync = Color1_ItemClickedAsync;
        CreateCommands(commandContainer);
    }
    private partial void Subscribe();
    private partial void Unsubscribe();
    partial void CreateCommands(CommandContainer command);
    private MastermindMainGameClass _mainGame = null!;
    private GlobalClass _global = null!;
    public int LevelChosen { get; set; }
    public SimpleEnumPickerVM<EnumColorPossibilities> Color1 = null!;
    public bool CanGiveUp
    {
        get
        {
            if (_global.Solution == null)
            {
                return false;
            }
            return _global.Solution.Count > 0;
        }
    }
    [Command(EnumCommandCategory.Plain)]
    public async Task GiveUpAsync()
    {
        await _mainGame.GiveUpAsync();
    }
    public bool CanAccept => MainBoard.DidFillInGuess();
    [Command(EnumCommandCategory.Plain)]
    public async Task AcceptAsync()
    {
        await MainBoard.SubmitGuessAsync();
    }
    private Task Color1_ItemClickedAsync(EnumColorPossibilities piece)
    {
        MainBoard.SelectedColorForCurrentGuess(piece);
        return Task.CompletedTask;
    }
    public GameBoardViewModel MainBoard { get; set; } = null!;
    public bool CanEnableBasics()
    {
        return true; //because maybe you can't enable it.
    }
    async Task IHandleAsync<OpenMainEventModel>.HandleAsync(OpenMainEventModel message)
    {
        basicData.GameDataLoading = false;
        await _mainGame.NewGameAsync(MainBoard);
        Color1.LoadEntireList();
        //commandContainer.UpdateAll();
    }
    async Task IHandleAsync<CloseMainEventModel>.HandleAsync(CloseMainEventModel message)
    {
        Unsubscribe();
    }
}