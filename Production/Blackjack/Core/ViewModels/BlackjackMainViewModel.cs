namespace Blackjack.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
[UseStateChanged]
public partial class BlackjackMainViewModel(CommandContainer commandContainer,

    IEventAggregator aggregator,
    IGamePackageResolver resolver,
        StatsClass stats,
        BasicData basicData
        ) : IBasicEnableProcess,
    IMainGameVM, IHandleAsync<OpenMainEventModel>, IHandleAsync<CloseMainEventModel>
{
    private BlackjackMainGameClass _mainGame = null!;
    public DeckObservablePile<BlackjackCardInfo> DeckPile { get; set; } = null!;
    private partial void Subscribe();
    private partial void Unsubscribe();
    void IMainGameVM.Initialize()
    {
        Subscribe();
        commandContainer.ExecutingChanged = CommandContainer_ExecutingChanged;
        DeckPile = resolver.ReplaceObject<DeckObservablePile<BlackjackCardInfo>>();
        DeckPile.DeckClickedAsync = DeckPile_DeckClickedAsync;
        HumanStack = new(commandContainer);
        ComputerStack = new(commandContainer);
        HumanStack.ProcessLabel(false);
        HumanStack.CardSelectedAsync = HumanStack_CardSelectedAsync;
        ComputerStack.ProcessLabel(true);
        ComputerStack.AlwaysDisabled = true;
        HumanStack.SendFunction(() => NeedsAceChoice == false && SelectedYet == false);
        Wins = stats.Wins;
        Losses = stats.Losses;
        Draws = stats.Draws;
        DeckPile.NeverAutoDisable = true;
        DeckPile.IsEnabled = false;
        DeckPile.SendEnableProcesses(this, () =>
        {
            return false; //false this time.
        });
        _mainGame = resolver.ReplaceObject<BlackjackMainGameClass>(); //hopefully this works.  means you have to really rethink.
        CreateCommands(commandContainer);
    }
    async Task IHandleAsync<OpenMainEventModel>.HandleAsync(OpenMainEventModel message)
    {
        basicData.GameDataLoading = false; //because no autoresume.
        await _mainGame.NewGameAsync(DeckPile, this);
        //commandContainer.ManualReport(); //just had to run the code to manually report.
        //commandContainer.UpdateAll();
    }
    async Task IHandleAsync<CloseMainEventModel>.HandleAsync(CloseMainEventModel message)
    {
        Unsubscribe();
        commandContainer.ExecutingChanged = null;
    }
    partial void CreateCommands(CommandContainer container);
    

    public PlayerStack? ComputerStack; //decided to make it more clear now.
    public PlayerStack? HumanStack;
    [StateProperty]
    public partial bool NeedsAceChoice { get; set; }
    partial void OnNeedsAceChoiceChanged(bool value)
    {
        Reprocess();
    }

    [StateProperty]
    public partial bool SelectedYet { get; set; }

    partial void OnSelectedYetChanged(bool value)
    {
        Reprocess();
    }

    private void Reprocess()
    {
        if (NeedsAceChoice == false && SelectedYet == true)
        {
            CanHitOrStay = true;
        }
        else
        {
            CanHitOrStay = false;
        }
    }
    public bool CanHitOrStay { get; set; }
    [LabelColumn]
    public partial int HumanPoints { get; set; }
    [LabelColumn]
    public partial int ComputerPoints { get; set; }
    //private int _draws;
    [LabelColumn]
    public partial int Draws { get; set; }

    partial void OnDrawsChanged(int value)
    {
        stats.Draws = value;
    }



    [LabelColumn]
    public partial int Wins { get; set; }
    partial void OnWinsChanged(int value)
    {
        stats.Wins = value;
    }

    [LabelColumn]
    public partial int Losses { get; set; }
    partial void OnLossesChanged(int value)
    {
        stats.Losses = value;
    }

    public bool CanAce => NeedsAceChoice;
    [Command(EnumCommandCategory.Game)]
    public async Task AceAsync(EnumAceChoice choice)
    {
        await _mainGame.HumanAceAsync(choice);
    }
    public bool CanHit => CanHitOrStay;
    [Command(EnumCommandCategory.Game)]
    public async Task HitAsync()
    {
        await _mainGame.HumanHitAsync();
    }
    public bool CanStay => CanHitOrStay;
    [Command(EnumCommandCategory.Game)]
    public async Task StayAsync()
    {
        await _mainGame.HumanStayAsync();
    }

    private async Task DeckPile_DeckClickedAsync()
    {
        //if we click on deck, will do code for this.
        await Task.CompletedTask;
    }
    private async void CommandContainer_ExecutingChanged()
    {
        if (commandContainer.IsExecuting)
        {
            return;
        }
        //code to run when its not busy.

        if (_mainGame.GameGoing)
        {
            await _mainGame.SaveStateAsync();
        }
    }
    private async Task HumanStack_CardSelectedAsync(bool HasChoice)
    {
        await _mainGame!.HumanSelectAsync(HasChoice);
    }
    public bool CanEnableBasics()
    {
        return true;
    }
    

    
}