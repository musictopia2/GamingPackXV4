namespace Froggies.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class FroggiesMainViewModel : ScreenViewModel, IBasicEnableProcess, IBlankGameVM, IAggregatorContainer
{
    private readonly BasicData _basicData;
    public readonly FroggiesMainGameClass MainGame;
    public int DrawVersion { get; set; } = 1;
    [LabelColumn]
    public int StartingFrogs { get; set; }
    [LabelColumn]
    public int NumberOfFrogs { get; set; }
    [LabelColumn]
    public int MovesLeft { get; set; }
    [Command(EnumCommandCategory.Plain)]
    public async Task MakeMoveAsync(LilyPadModel lily)
    {
        if (lily == null)
        {
            throw new CustomBasicException("Lily cannot be null. Rethink");
        }
        await MainGame.ProcessLilyClickAsync(lily, this);
    }

    [Command(EnumCommandCategory.Plain)]
    public async Task RedoAsync()
    {
        await MainGame.RedoAsync();
        DrawVersion++;
        MovesLeft = MainGame.MovesLeft();
    }
    public FroggiesMainViewModel(IEventAggregator aggregator,
        CommandContainer commandContainer,
        IGamePackageResolver resolver,
        LevelClass level,
        BasicData basicData) : base(aggregator)
    {
        CommandContainer = commandContainer;
        StartingFrogs = level.NumberOfFrogs;
        NumberOfFrogs = level.NumberOfFrogs;
        CommandContainer = commandContainer;
        _basicData = basicData;
        MainGame = resolver.ReplaceObject<FroggiesMainGameClass>(); //hopefully this works.  means you have to really rethink.
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public CommandContainer CommandContainer { get; set; }
    IEventAggregator IAggregatorContainer.Aggregator => Aggregator;
    public Action? StateHasChanged { get; set; }
    public bool CanEnableBasics()
    {
        return true; //because maybe you can't enable it.
    }
    protected override async Task ActivateAsync()
    {
        _basicData.GameDataLoading = false;
        await base.ActivateAsync();
        await MainGame.NewGameAsync(this); //this may have to be before (?)
        StateHasChanged?.Invoke();
    }
}