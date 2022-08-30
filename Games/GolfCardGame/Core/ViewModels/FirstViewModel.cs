namespace GolfCardGame.Core.ViewModels;
[InstanceGame]
public partial class FirstViewModel : ScreenViewModel, IBlankGameVM
{
    private readonly GolfCardGameVMData _model;
    private readonly GolfCardGameGameContainer _gameContainer;
    private readonly IBeginningProcesses _processes;
    private readonly IToast _toast;
    public string Instructions { get; set; } = "";
    public FirstViewModel(
        CommandContainer commandContainer,
        GolfCardGameVMData model,
        GolfCardGameGameContainer gameContainer,
        IBeginningProcesses processes,
        IEventAggregator aggregator,
        IToast toast
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _model = model;
        _gameContainer = gameContainer;
        _processes = processes;
        _toast = toast;
        Instructions = "Choose the 2 cards to put into your hand";
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public CommandContainer CommandContainer { get; set; }

    [Command(EnumCommandCategory.Plain)]
    public async Task ChooseFirstCardsAsync()
    {
        if (_model.Beginnings1.CanContinue == false)
        {
            _toast.ShowUserErrorToast("Sorry, must select 2 and only 2 cards to put into your hand");
            return;
        }
        _model.Beginnings1.GetSelectInfo(out DeckRegularDict<RegularSimpleCard> selectList, out DeckRegularDict<RegularSimpleCard> unselectList);
        if (_gameContainer.BasicData!.MultiPlayer == true)
        {
            SendBeginning thisB = new();
            thisB.SelectList = selectList;
            thisB.UnsSelectList = unselectList;
            thisB.Player = _gameContainer.PlayerList!.Where(items => items.PlayerCategory == EnumPlayerCategory.Self).Single().Id;
            await _gameContainer.Network!.SendAllAsync("selectbeginning", thisB);
        }
        int player = _gameContainer.PlayerList!.Single(items => items.PlayerCategory == EnumPlayerCategory.Self).Id;
        await _processes.SelectBeginningAsync(player, selectList, unselectList);
    }
}