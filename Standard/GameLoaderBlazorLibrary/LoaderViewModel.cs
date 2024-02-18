namespace GameLoaderBlazorLibrary;
public abstract class LoaderViewModel : ILoaderVM
{
    public Action? StateChanged { get; set; }
    public string GameName { get; set; } = "";
    public static bool IsSinglePlayerOnly { get; set; } //this means if set to true, then some action can happen to the loader processes.
    public RenderFragment? GameRendered { get; private set; }
    public abstract string Title { get; }
    protected IStartUp Starts;
    protected EnumGamePackageMode Mode;
    public BasicList<string> GameList { get; protected set; } = [];
    public LoaderViewModel(IStartUp starts)
    {
        Starts = starts;
        GenerateGameList();
        Mode = EnumGamePackageMode.Production;
        GlobalStartUp.StartBootStrap = () =>
        {
            IGameBootstrapper _ = ChooseGame();
        };
    }
    protected abstract void GenerateGameList();
    protected abstract IGameBootstrapper ChooseGame();
    protected abstract Type GetGameType();
    public void ChoseGame(string game)
    {
        if (StateChanged is null)
        {
            throw new CustomBasicException("Must send in statehaschanged");
        }
        GameName = game;
        GameRendered = (builder) =>
        {
            Type type = GetGameType();
            builder.OpenComponent(0, type);
            builder.CloseComponent();
        };
        StateChanged.Invoke();
    }
}