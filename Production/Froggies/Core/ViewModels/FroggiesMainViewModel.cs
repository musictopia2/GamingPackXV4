namespace Froggies.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
[UseStateChanged]
public partial class FroggiesMainViewModel(CommandContainer commandContainer,
        IGamePackageResolver resolver,
         LevelClass level,
        BasicData basicData,
        IEventAggregator aggregator) : IBasicEnableProcess,
    IMainGameVM, IHandleAsync<OpenMainEventModel>, IHandleAsync<CloseMainEventModel>
{
    private partial void Subscribe();
    private partial void Unsubscribe();
    public FroggiesMainGameClass MainGame = null!;
    [StateProperty]
    public partial int DrawVersion { get; set; } = 1;
    [LabelColumn]
    public partial int StartingFrogs { get; set; }
    [LabelColumn]
    public partial int NumberOfFrogs { get; set; }
    [LabelColumn]
    public partial int MovesLeft { get; set; }
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
    partial void CreateCommands(CommandContainer command);
    public Action? StateHasChanged { get; set; }
    public bool CanEnableBasics()
    {
        return true; //because maybe you can't enable it.
    }
    void IMainGameVM.Initialize()
    {
        Subscribe();
        StartingFrogs = level.NumberOfFrogs;
        NumberOfFrogs = level.NumberOfFrogs;
        MainGame = resolver.ReplaceObject<FroggiesMainGameClass>();
        CreateCommands(commandContainer);
    }
    async Task IHandleAsync<OpenMainEventModel>.HandleAsync(OpenMainEventModel message)
    {
        basicData.GameDataLoading = false;
        await MainGame.NewGameAsync(this); //this may have to be before (?)
        StateHasChanged?.Invoke();
    }

    async Task IHandleAsync<CloseMainEventModel>.HandleAsync(CloseMainEventModel message)
    {
        Unsubscribe();
    }
}