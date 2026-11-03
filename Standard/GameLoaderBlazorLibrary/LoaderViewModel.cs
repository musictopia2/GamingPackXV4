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
    private readonly Dictionary<string, bool> _compactSupport = new(StringComparer.Ordinal);

    // Call from GenerateGameList so the catalog uses the game's existing metadata.
    protected void RegisterGame(string gameName, bool supportsCompact)
    {
        _compactSupport[gameName] = supportsCompact;
        if (GameList.Contains(gameName) == false)
        {
            GameList.Add(gameName);
        }
    }

    // A name-only entry remains available on larger devices. Until registered,
    // it is hidden on phones because its compact support has not been supplied.
    public bool SupportsCompactLayout(string game)
    {
        return _compactSupport.TryGetValue(game, out bool supported) && supported;
    }
    public LoaderViewModel(IStartUp starts)
    {
        Starts = starts;
        GenerateGameList();
        Mode = EnumGamePackageMode.Production;
        GlobalStartUp.StartBootStrap = async () =>
        {
            IGameBootstrapper bootstrapper = ChooseGame();
            await bootstrapper.InitializationTask;
            
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